using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CharacterState
{
    Idle,
    Walking,
    Trotting,
    Running,
    Jumping,
    Greeting,
    Talking,
    Dying,
    Attacking,
}

public enum Task
{
    Blacksmith,
    Fisher,
    Lumberjack,
    Woodcutter,
    StableWorker,
    Sleep,
    Eat,
    Idle,
    Shopkeeper,
};


public struct QuestInfo
{
    public string QuestUID;
    public float ExpiryTimer;
    public int CashReward;
}

public class CognitiveAgent : MonoBehaviour
{
    public bool _useStateVector = true;
    public bool _useRelationships = false;
    public bool _useTraits = false;
    
    [HideInInspector]
    public StateVector stateVector;
    [HideInInspector]
    public RelationshipManager relationshipManager;
    [HideInInspector]
    public TraitsManager traitsManager;



    public bool StaticDemo;
    internal string DesiredTopic;           //The agent is currently thinking about, concerned about, or looking for the "UID" contained in here.
    internal AgentInventory Inventory;      //The agent's current inventory of equipped and un-equipped items.
    internal bool HasQuest;
    public QuestInfo QuestInfo;

    protected Goal_Scheduler CurrentGoal;               //The agent's current top-level goal.
    public Task StartingTask;                    //What should be the initial top-level goal (i.e. starting job)?    

    #region Transform Target Points
    public Transform BedTarget;             //The initial starting position of the Agent, at their bed in the morning.
    #endregion

    #region Agent Components
    internal NavMeshAgent NavAgent;                 //A pointer to the NavMeshAgent component, for moving across the NavMesh.
    internal HeadLookController ViewController;     //A pointer to the HeadLook component, which uses IK to dynamically alter the current animation to look towards objects (i.e. people) of interest.
    internal CharacterDetails CharDetails;          //A pointer to the component with the info about the current character.
    public CharacterCue CharacterCue;               //A pointer to the component which contains info about the current character's cue information.
    #endregion

    #region Cached Components    
    public AnimationController _animationController;
    internal Transform Transform;       //Cached transform
    internal GameObject Self;           //and cached Game Object lookup.
    #endregion

    //Pointers to the agent's thought and dialog speech bubble components.
    public QuestUIManager QuestManager;
    public DialogUIManager ThoughtManager;  
    public DialogUIManager DialogManager;

    public MindsEye MindsEye;   //Pointer to the MindsEye component, which is the memory component in charge with sensing the world and building up relationships and connections with what it observes.

    internal bool IsAlert;                  //Is the Agent currently alert to events happening around them. Can be false if the Agent is busy, i.e. in a conversation or sleeping.
    float greetTimer = 16.0f;               //How frequently the agent should stop and greet people it passes.
    public AudioClip GreetingClip;          //The audio clip to play when a greeting is triggered.

    public TextAsset DailyRoutine;
    Routine routine;

    Task curTask, nextTask;

    #region Animation Variables
    internal float MoveSpeed = 0.0f;
    internal float WalkSpeed = 2.0f;
    internal float TrotSpeed = 4.0f;
    internal float RunSpeed = 6.0f;

    internal Vector3 MoveDirection = Vector3.zero;

    internal float RotateSpeed = 500.0f;
    internal float TrotAfterSeconds = 3.0f;
    internal float WalkTimeStart = 0.0f;

    internal float SpeedSmoothing = 10.0f;

    internal CharacterState _characterState = CharacterState.Idle;
    internal bool Running = false;

   
    internal float AnimationSpeed = 1.0f;                                //How fast the animations should play.
    #endregion

    void Awake()
    {
        //Cache our frequent lookups.
        Transform = GetComponent<Transform>();
        Self = gameObject;

        //Get our basic components.
        NavAgent = GetComponent<NavMeshAgent>();
        ViewController = GetComponent<HeadLookController>();
        CharDetails = GetComponent<CharacterDetails>();
        _animationController = GetComponent<AnimationController>();

        NavAgent.enabled = false;   //We disable the nav agent, so that we can move the Agent to its starting position (Moving with NavAgent enabled causes path finding and we simply want to "place" them at the start.) 
   
        if(!StaticDemo)
            Transform.position = BedTarget.position;    //Place the agent at the start.

        MoveDirection = Transform.TransformDirection(Vector3.forward);  //Get their initial facing direction.



        Inventory = new AgentInventory(this);
		CharacterCue.Inventory = Inventory;
        
        if( _useStateVector )
            stateVector = GetComponentInChildren<StateVector>();
        if( _useRelationships )
            relationshipManager = GetComponent<RelationshipManager>();
        if( _useTraits )
            traitsManager = GetComponent<TraitsManager>();
       
        
    }

    void Start()
    {
        if(!StaticDemo)
            NavAgent.enabled = true;            //We enable the nav agent again, having now positioned the agent.

        DialogManager.SetDialog("...");     //We initialise the dialog and thoughts to create a small initial size for the speech and thought bubbles.
        ThoughtManager.SetDialog("(...)");

       

        IsAlert = true;

        if (!StaticDemo)
        {
            routine = new Routine(DailyRoutine);
            curTask = routine.GetCurrentTask(DaylightScript.GetCurrentTime());

            if ( _useStateVector )
            {
                stateVector.SetupTasks(routine);
            }
        }

        if (!StaticDemo)
            CurrentGoal = new Goal_Scheduler(this, curTask);
        else
            CurrentGoal = new Goal_Scheduler(this, Task.Idle);
    }

    void Update()
    {
        if (!StaticDemo)
        {
            if ( _useStateVector )
            {
                nextTask = stateVector.GetBestTask( curTask );
            }
            else
            {
                nextTask = routine.GetCurrentTask( DaylightScript.GetCurrentTime() );
            }

            if (curTask != nextTask)
            {
                //            Debug.Log(DaylightScript.GetTimeStamp() + " - New Task is: " + curTask.ToString());
                CurrentGoal.UpdateTask(nextTask);
                curTask = nextTask;
            }
  
        }
    }

    void LateUpdate()
    {
        CheckForLineOfSight();          //Check to see if the Agent can see anything of interest in its surroundings.
        CurrentGoal.Process();          //Then process the current top-level goal (and any subgoals it has).
    }

    void CheckForLineOfSight()    //Sees if anything of interest is in the current agent's view cone.
    {
        if (!IsAlert)
            return;

        int closestID = -1;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < AgentManager.Instance.GetAgentCount(); i++)
        {
            if (i == CharDetails.AgentID)
                continue;

            float dist = Vector2.Distance(Transform.position, AgentManager.Instance.GetAgent(i).HeadTarget.position);
            if (dist < closestDistance)
            {
                closestID = i;
                closestDistance = dist;
            }

        }

        if (closestID == -1)
            ViewController.enabled = false;
        else
            ViewController.UpdateTarget(AgentManager.Instance.GetAgent(closestID));

        greetTimer -= Time.smoothDeltaTime;

        if (greetTimer > 0.0f)
            return;

        const float maxView = 10.0f;
        const float viewCone = 90.0f;

        for (int i = 0; i < AgentManager.Instance.GetAgentCount(); i++)
        {
            if (i == CharDetails.AgentID)
                continue;

            Vector3 direction = AgentManager.Instance.GetAgent(i).HeadTarget.position - CharDetails.HeadTarget.position;

            if (MindsEye.MemoryGraph.GetCueOpinion(AgentManager.Instance.GetAgent(i).CharCue) < 0.0f)
                continue;       //We don't greet people we have a negative opinion about.

            if (direction.magnitude <= maxView)
            {
                float angle = Vector3.Angle(MoveDirection, direction);
                if (angle <= viewCone && angle >= -viewCone)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(CharDetails.HeadTarget.position, direction, out hit))
                    {
                        //Debug.Log(hit.transform.name);
                        CharacterDetails hitChar = hit.transform.GetComponent<CharacterDetails>();

                        if (hitChar != null && hitChar.AgentID == i)
                        {
                            //if (hitChar.IsPlayer)
                            //{
                                //Then we don't want to send them a conversation request message, so we'll just greet and move on.
                                CurrentGoal.AddSubgoal(new Goal_Greeting(this, new List<int>() { CharDetails.AgentID, hitChar.AgentID }));

                                //if(!AgentManager.Instance.GetAgent(i).IsPlayer)
                                    MindsEye.MemoryGraph.UpdateCueOpinion(AgentManager.Instance.GetAgent(i).CharCue, -1.0f);

                                greetTimer = 64.0f;
                            //}
                            //else
                            //{
                            //    Message.DispatchMessage(CharDetails.AgentID, hitChar.AgentID, TelegramType.Greeting);
                            //    //CurrentGoal.AddSubgoal(new Goal_Conversation(this, Transform.position, new List<int>() { CharDetails.AgentID, hitChar.AgentID }, true));
                            //    greetTimer = 512.0f;
                            //}                            
                        }
                    }
                }
            }
        }
    }

    public bool HandleMessage(Telegram message)     //Handles messages that are passed to this agent, passing them into the current goal (which can then pass it to any sub-goals, to try and handle it).
    {
        if (CurrentGoal != null && CurrentGoal.HandleMessage(message))
            return true;

        return false;
    }

    internal void EnableQuest(QuestInfo questInfo)
    {
        HasQuest = true;
        QuestInfo = questInfo;        
        QuestManager.EnableQuest(questInfo.QuestUID);
    }

    internal bool CheckForQuestCompletion(string uid)
    {
        if(QuestInfo.QuestUID == uid)
        {
            CompleteQuest();
            return true;
        }

        return false;
    }

    internal void CompleteQuest()
    {
        HasQuest = false;
        QuestManager.DisableQuest();
    }

    internal void CheckForQuestCompletion(string[] p)
    {
        throw new System.NotImplementedException();
    }

    internal void ConsumeEquippedItem()
    {
        throw new System.NotImplementedException();
    }




    public void StartModification(Task task)
    {
        if ( _useStateVector )
            stateVector.StartModification(task);
    }
    public void StopModification()
    {
        if ( _useStateVector )
            stateVector.StopModification();
    }


    public void Die()
    {
        StartCoroutine( DeathCoroutine() );
    }

    private IEnumerator DeathCoroutine()
    {
        if ( ( _characterState == CharacterState.Dying ) || ( !CharDetails.IsAlive() ) )
        {
            yield break;
        }

        _characterState = CharacterState.Dying;
        _animationController.SetAnimation( _characterState );

        while ( _animationController.IsAnimationPlaying( CharacterState.Dying ) )
            yield return null;

        CharDetails.Die();
    }
}