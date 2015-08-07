using UnityEngine;
using System.Collections;

public class Goal_Eat : Goal_Composite
{
    public Goal_Eat(CognitiveAgent owner)
        : base(owner) { }

    //Eat (Consume item)
    //Sit Down...?
    //Get Food

    public override void Activate()
    {
        Owner.ThoughtManager.SetDialog("(EATING | )");
        CurrentStatus = GoalStatus.Active;
        RemoveAllSubgoals();

        AddSubgoal(new Goal_ConsumeFood(Owner));
        AddSubgoal(new Goal_CollectItemFromMemory(Owner, "Food", true));
    }

    public override GoalStatus Process()
    {
        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        if (subGoals.Count > 0)
            CurrentStatus = ProcessSubgoals();

        return CurrentStatus;
    }
}

public class Goal_ConsumeFood : Goal_Atomic
{
    string targetUID;

    public Goal_ConsumeFood(CognitiveAgent owner)
        : base(owner) { }

    public override void Activate()
    {
        Reactivate();
        CurrentStatus = GoalStatus.Active;
        Owner.Inventory.DropCurrentItem();

        base.Activate();
    }

    public override void Reactivate()
    {
        AddToThoughtBubble("Munching...");

        Owner.Animation[Owner.AnimationKeys["idle"]].speed = Owner.AnimationSpeed;
        Owner.Animation.CrossFade(Owner.AnimationKeys["idle"]);
    }

    public override GoalStatus Process()
    {
        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        return CurrentStatus;
    }

    public override void ApplyStateModVector()
    {
        Owner.StartModification(Task.Eat);
    }
}
