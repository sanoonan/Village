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
        Owner.Animation[Owner.AnimationKeys["idle"]].speed = Owner.AnimationSpeed;
        Owner.Animation.CrossFade(Owner.AnimationKeys["idle"]);
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

    public override void SetStateModVector()
    {
        stateModVector.setToZero();
    }
}

public class Goal_Idle_Blacksmith : Goal_Idle
{
    public Goal_Idle_Blacksmith(CognitiveAgent owner)
        : base(owner) { }

    public override void SetStateModVector()
    {
        stateModVector.setToZero();
    }
}

public class Goal_Idle_StableWork : Goal_Idle
{
    public Goal_Idle_StableWork(CognitiveAgent owner)
        : base(owner) { }

    public override void SetStateModVector()
    {
        stateModVector.setToZero();
    }
}

public class Goal_Idle_Fish : Goal_Idle
{
    public Goal_Idle_Fish(CognitiveAgent owner)
        : base(owner) { }

    public override void SetStateModVector()
    {
        stateModVector.setToZero();
    }
}

public class Goal_Idle_Shopkeep : Goal_Idle
{
    public Goal_Idle_Shopkeep(CognitiveAgent owner)
        : base(owner) { }

    public override void SetStateModVector()
    {
        stateModVector.setToZero();
    }
}

public class Goal_Idle_WoodCut : Goal_Idle
{
    public Goal_Idle_WoodCut(CognitiveAgent owner)
        : base(owner) { }

    public override void SetStateModVector()
    {
        stateModVector.setToZero();
    }
}

public class Goal_Idle_LumberJack : Goal_Idle
{
    public Goal_Idle_LumberJack(CognitiveAgent owner)
        : base(owner) { }

    public override void SetStateModVector()
    {
        stateModVector.setToZero();
    }
}


public class Goal_Idle_Eat : Goal_Idle
{
    public Goal_Idle_Eat(CognitiveAgent owner)
        : base(owner) { }

    public override void SetStateModVector()
    {
        stateModVector.setToZero();
    }
}

public class Goal_Idle_Sleep : Goal_Idle
{
    public Goal_Idle_Sleep(CognitiveAgent owner)
        : base(owner) { }

    public override void SetStateModVector()
    {
        stateModVector.setToZero();
    }
}

public class Goal_Idle_Quest : Goal_Idle
{
    public Goal_Idle_Quest(CognitiveAgent owner)
        : base(owner) { }

    public override void SetStateModVector()
    {
        stateModVector.setToZero();
    }
}
