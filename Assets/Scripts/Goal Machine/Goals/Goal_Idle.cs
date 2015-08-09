using UnityEngine;
using System.Collections.Generic;


public abstract class Goal_Idle : Goal_Atomic
{
    public Goal_Idle(CognitiveAgent owner)
        : base(owner) { }

    public override void Activate()
    {
        //Debug.Log("Activating the Goal_Idle");
        Owner.NavAgent.avoidancePriority = 50;
        AddToThoughtBubble("Idle");
        CurrentStatus = GoalStatus.Active;

        //Owner.lblThought.text = ("(Idling...)");
        Owner._animationController.SetAnimation( CharacterState.Idle, Owner.AnimationSpeed );

        base.Activate();
    }

    public override void Terminate()
    {
        Owner.NavAgent.avoidancePriority = Owner.CharDetails.AgentID + 5;
        base.Terminate();
    }

    public override GoalStatus Process()
    {
        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        return CurrentStatus;
    }
}

public class Goal_Idle_Idle : Goal_Idle
{
    public Goal_Idle_Idle(CognitiveAgent owner)
        : base(owner) { }

    public override void ApplyStateModVector()
    {
    }

 
}

public class Goal_Idle_Blacksmith : Goal_Idle
{
    public Goal_Idle_Blacksmith(CognitiveAgent owner)
        : base(owner) { }

    public override void ApplyStateModVector()
    {
        Owner.StartModification(Task.Blacksmith);
    }

   
}

public class Goal_Idle_StableWork : Goal_Idle
{
    public Goal_Idle_StableWork(CognitiveAgent owner)
        : base(owner) { }

    public override void ApplyStateModVector()
    {
        Owner.StartModification(Task.StableWorker);
    }

   
}

public class Goal_Idle_Fish : Goal_Idle
{
    public Goal_Idle_Fish(CognitiveAgent owner)
        : base(owner) { }

    public override void ApplyStateModVector()
    {
        Owner.StartModification(Task.Fisher);
    }

   
}

public class Goal_Idle_Shopkeep : Goal_Idle
{
    public Goal_Idle_Shopkeep(CognitiveAgent owner)
        : base(owner) { }

    public override void ApplyStateModVector()
    {
        Owner.StartModification(Task.Shopkeeper);
    }

   
}

public class Goal_Idle_WoodCut : Goal_Idle
{
    public Goal_Idle_WoodCut(CognitiveAgent owner)
        : base(owner) { }

    public override void ApplyStateModVector()
    {
        Owner.StartModification(Task.Woodcutter);
    }

  
}

public class Goal_Idle_LumberJack : Goal_Idle
{
    public Goal_Idle_LumberJack(CognitiveAgent owner)
        : base(owner) { }

    public override void ApplyStateModVector()
    {
        Owner.StartModification(Task.Lumberjack);
    }

}


public class Goal_Idle_Eat : Goal_Idle
{
    public Goal_Idle_Eat(CognitiveAgent owner)
        : base(owner) { }

    public override void ApplyStateModVector()
    {
        Owner.StartModification(Task.Eat);
    }

   
}

public class Goal_Idle_Sleep : Goal_Idle
{
    public Goal_Idle_Sleep(CognitiveAgent owner)
        : base(owner) { }

    public override void ApplyStateModVector()
    {
        Owner.StartModification(Task.Sleep);
    }

}

public class Goal_Idle_Quest : Goal_Idle
{
    public Goal_Idle_Quest(CognitiveAgent owner)
        : base(owner) { }

    public override void ApplyStateModVector()
    {
    }

}
