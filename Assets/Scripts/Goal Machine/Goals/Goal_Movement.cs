using UnityEngine;
using System.Collections.Generic;

public class Goal_MoveToPosition : Goal_Composite
{
    Vector3 destination;

    public Goal_MoveToPosition(CognitiveAgent owner, Vector3 destination)
        : base(owner)
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(destination, out hit, 20.0f, 1);
        this.destination = hit.position;
    }

    public Goal_MoveToPosition(CognitiveAgent owner, Transform destination)
        : base(owner)
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(destination.position, out hit, 20.0f, 1);
        this.destination = hit.position;
    }

    public override void Reactivate()
    {
        Activate();
        base.Reactivate();
    }

    public override void Activate()
    {
        CurrentStatus = GoalStatus.Active;

        //Make sure the subgoal list is clear, as we need to plan a new path anyway.
        RemoveAllSubgoals();

        NavMeshPath navPath = new NavMeshPath();

        if (Owner.NavAgent.CalculatePath(destination, navPath))
            AddSubgoal(new Goal_FollowPath(Owner, navPath));
        else
        {
            throw new UnityException("Can't find a valid path to target!");
            CurrentStatus = GoalStatus.Failed;
        }
    }

    public override GoalStatus Process()
    {
        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        if (CurrentStatus == GoalStatus.Failed) //If the activation fails, then we can just return a failure.
            return GoalStatus.Failed;

        CurrentStatus = ProcessSubgoals();

        if (CurrentStatus == GoalStatus.Failed) //If, however, a subgoal has failed, then we can first attempt to repath. If that fails, we then return a failure.
            Activate();

        return CurrentStatus;
    }
}

public class Goal_HeadToAreaAndPatrol : Goal_Composite
{
    string targetUID;

    public Goal_HeadToAreaAndPatrol(CognitiveAgent owner, string targetUID)
        : base(owner)
    {
        this.targetUID = targetUID;
    }

    public override void Activate()
    {
        //Debug.Log("Activating the Goal_CollectItem");
        AddToThoughtBubble("Patrol for " + targetUID);

        CurrentStatus = GoalStatus.Active;
        RemoveAllSubgoals();

        AddSubgoal(new Goal_PatrolArea(Owner, targetUID));
        AddSubgoal(new Goal_MoveToPosition(Owner, LocationManager.Instance.GetLocation(targetUID)[0]));
    }

    public override GoalStatus Process()
    {
        //Debug.Log("Processing the Goal_GatherSmithingMaterials");

        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        CurrentStatus = ProcessSubgoals();

        return CurrentStatus;
    }
}

public class Goal_PatrolArea : Goal_Composite
{
    string targetUID;
    List<Transform> patrolTransforms;
    bool loopPatrol;

    int currentPatrolPoint;

    public Goal_PatrolArea(CognitiveAgent owner, string targetUID, bool loopPatrol = false)
        : base(owner)
    {
        this.targetUID = targetUID;
        this.loopPatrol = loopPatrol;

        patrolTransforms = LocationManager.Instance.GetLocation(targetUID);
        currentPatrolPoint = 0;
    }

    public override void Activate()
    {
        //Debug.Log("Activating the Goal_CollectItem");
        AddToThoughtBubble("Patrolling " + targetUID + " " + (currentPatrolPoint + 1) + "/" + patrolTransforms.Count);

        CurrentStatus = GoalStatus.Active;
        RemoveAllSubgoals();

        //Debug.Log(patrolTransforms.Count + " " + currentPatrolPoint + " " + Owner.name);
        AddSubgoal(new Goal_MoveToPosition(Owner, patrolTransforms[currentPatrolPoint]));

        if (loopPatrol)
        {
            //Debug.Log("Looping patrol points - " + Owner.name);
            currentPatrolPoint = (currentPatrolPoint + 1) % patrolTransforms.Count;
        }
        else
        {
            //Debug.Log("Removing patrol point - " + Owner.name);
            patrolTransforms.RemoveAt(0);
        }
    }

    public override GoalStatus Process()
    {
        //Debug.Log("Processing the Goal_GatherSmithingMaterials");

        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        CurrentStatus = ProcessSubgoals();

        if (CurrentStatus == GoalStatus.Completed && patrolTransforms.Count > 0)
            Activate();

        return CurrentStatus;
    }
}

public class Goal_FollowPath : Goal_Composite
{
    NavMeshPath navPath = new NavMeshPath();
    int pathID;

    public Goal_FollowPath(CognitiveAgent owner, NavMeshPath navPath)
        : base(owner)
    {
        this.navPath = navPath;
    }

    bool IsStuck()
    {
        return false;
    }

    public override void Activate()
    {
        //Debug.Log("Activating the Goal_FollowPath");

        CurrentStatus = GoalStatus.Active;
        Vector3 target = navPath.corners[pathID++];
        AddSubgoal(new Goal_TraverseNode(Owner, target, pathID >= navPath.corners.Length));
    }

    public override GoalStatus Process()
    {
        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        CurrentStatus = ProcessSubgoals();

        if (CurrentStatus == GoalStatus.Completed && pathID < navPath.corners.Length)
            Activate();

        return CurrentStatus;
    }
}

public class Goal_TraverseNode : Goal
{
    Vector3 targetLocation;

    Vector3 prevPosition;
    float movementSpeed;

    bool lastEdgeInPath;    //Is this the last node in the path?
    double timeExpected;    //The estimate time the bost should take to traverse this section of the path.
    double startTime;       //This records the time this goal was activated.

    public Goal_TraverseNode(CognitiveAgent owner, Vector3 target, bool last)
        : base(owner)
    {
        this.targetLocation = target;
        this.lastEdgeInPath = last;

        prevPosition = owner.Transform.position;
    }

    bool IsStuck()
    {
        return false;
    }

    public override void Activate()
    {
        //Debug.Log("Activating the Goal_TraverseNode");
        CurrentStatus = GoalStatus.Active;
    }

    public override GoalStatus Process()
    {
        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        if (IsStuck())
            return CurrentStatus = GoalStatus.Failed;

        Vector3 curMove = Owner.Transform.position - prevPosition;
        movementSpeed = curMove.magnitude / Time.deltaTime;
        prevPosition = Owner.Transform.position;

        Vector3 movement = Vector3.zero;
        Vector3 direction = targetLocation - Owner.Transform.position;
        //direction.y = 0.0f;

        if (direction.magnitude < 0.75f)
        {
            //We've arrived!
            //Owner.Transform.position = targetLocation;
            return CurrentStatus = GoalStatus.Completed;
        }

        //Vector3 normalisedDirection = direction.normalized;

        // Target direction relative to the camera
        Vector3 targetDirection = direction.normalized;
        //targetDirection.y = 0.0f;

        // We store speed and direction seperately,
        // so that when the character stands still we still have a valid forward direction
        // moveDirection is always normalized, and we only update it if there is user input.
        if (targetDirection != Vector3.zero)
        {
            // If we are really slow, just snap to the target direction
            if (Owner.MoveSpeed < Owner.WalkSpeed * 0.9f)
            {
                Owner.MoveDirection = targetDirection.normalized;
            }
            // Otherwise smoothly turn towards it
            else
            {
                Owner.MoveDirection = Vector3.RotateTowards(Owner.MoveDirection, targetDirection, Owner.RotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
                Owner.MoveDirection = Owner.MoveDirection.normalized;
            }
        }

        // Smooth the speed based on the current target direction
        float curSmooth = Owner.SpeedSmoothing * Time.deltaTime;

        // Choose target speed
        //* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
        float targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);

        Owner._characterState = CharacterState.Idle;

        // Pick speed modifier
        if (Owner.Running)
        {
            targetSpeed *= Owner.RunSpeed;
            Owner._characterState = CharacterState.Running;
        }
        else if (Time.time - Owner.TrotAfterSeconds > Owner.WalkTimeStart)
        {
            targetSpeed *= Owner.TrotSpeed;
            Owner._characterState = CharacterState.Trotting;
        }
        else
        {
            targetSpeed *= Owner.WalkSpeed;
            Owner._characterState = CharacterState.Walking;
        }

        Owner.MoveSpeed = Mathf.Lerp(Owner.MoveSpeed, targetSpeed, curSmooth);

        // Reset walk time start when we slow down
        if (Owner.MoveSpeed < Owner.WalkSpeed * 0.3f)
            Owner.WalkTimeStart = Time.time;

        // Calculate actual motion
        movement = Owner.MoveDirection * Owner.MoveSpeed;
        movement *= Time.deltaTime;

        // Move the controller
        Owner.NavAgent.Move(movement);

        if ( ( Owner._characterState == CharacterState.Running ) || ( Owner._characterState == CharacterState.Walking ) || ( Owner._characterState == CharacterState.Trotting ) ) 
        {
            Owner._animationController.SetAnimation( Owner._characterState, movementSpeed );
        }

        Owner.Transform.rotation = Quaternion.LookRotation(Owner.MoveDirection);

        return CurrentStatus;
    }
}
