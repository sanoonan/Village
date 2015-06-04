using UnityEngine;
using System.Collections.Generic;

public class Goal_LumberJack : Goal_Composite
{
    public Goal_LumberJack(CognitiveAgent owner)
        : base(owner) { }



    public override void Activate()
    {
        //Debug.Log("Activating the Goal_Blacksmith");
        Owner.ThoughtManager.SetDialog("(LUMBERJACK | )");
        CurrentStatus = GoalStatus.Active;
        RemoveAllSubgoals();

        //Lumberjack
        //Makes sure theres wood,
        //Gets axe
        //Cuts

        //Since the goals are as a stack, we have to add them in reverse order.

        AddSubgoal(new Goal_Idle_LumberJack(Owner));
        AddSubgoal(new Goal_MoveToPosition(Owner, LocationManager.Instance.GetLocationMemory("Work_TreeCutting")));
        AddSubgoal(new Goal_DropWoodOff(Owner));
        AddSubgoal(new Goal_MoveToPosition(Owner, LocationManager.Instance.GetLocationMemory("Work_TreeCutting")));
        AddSubgoal(new Goal_CollectItemFromMemory(Owner, "Axe", true));

    }

    public override GoalStatus Process()
    {
        //Debug.Log("Processing the Goal_Blacksmith");

        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        if (subGoals.Count > 0)
        {
            CurrentStatus = ProcessSubgoals();
        }
        else
        {
            //We can carry out the "Blacksmith" specific process.
            //Or should we just make it another sub-goal that keeps going at the end...?
        }

        return CurrentStatus;
    }
}

public class Goal_DropWoodOff : Goal_Composite
{
    int frames;

    public Goal_DropWoodOff(CognitiveAgent owner)
        : base(owner) { }

    public override void Activate()
    {
        //Debug.Log("Activating the Goal_GatherSmithingMaterials");
        AddToThoughtBubble("Dropping off Wood");

        CurrentStatus = GoalStatus.Active;
        RemoveAllSubgoals();
        frames = 0;

        AddSubgoal(new Goal_MoveToPosition(Owner, LocationManager.Instance.GetLocationMemory("Collect_Wood")));
    }

    public override GoalStatus Process()
    {
        //Debug.Log("Processing the Goal_GatherSmithingMaterials");

        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        if (subGoals.Count > 0)
        {
            CurrentStatus = ProcessSubgoals();

            if (CurrentStatus == GoalStatus.Completed)
                CurrentStatus = GoalStatus.Active;
        }
        else
        {
            frames++;

            if (frames >= 120)
                CurrentStatus = GoalStatus.Completed;
        }

        return CurrentStatus;
    }
}