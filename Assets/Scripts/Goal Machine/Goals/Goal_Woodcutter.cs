
using UnityEngine;
using System.Collections.Generic;
public class Goal_WoodCutting : Goal_Composite
{
    public Goal_WoodCutting(CognitiveAgent owner)
        : base(owner) { }

    //Get FishingRod (probably hammer for now)
    //Go to Docks (There is a fishing target but prob move him to the docks for now

    public override void Activate()
    {
        //Debug.Log("Activating the Goal_Blacksmith");
        Owner.ThoughtManager.SetDialog("(WOODCUTTER | )");
        CurrentStatus = GoalStatus.Active;
        RemoveAllSubgoals();

        //Lumberjack
        //Makes sure theres wood,
        //Gets axe
        //Cuts

        //Since the goals are as a stack, we have to add them in reverse order.

        AddSubgoal(new Goal_Idle_WoodCut(Owner));
        AddSubgoal(new Goal_MoveToPosition(Owner, LocationManager.Instance.GetLocationMemory("Work_WoodCutting")));
        AddSubgoal(new Goal_CollectItemFromMemory(Owner, "Axe", true));
        AddSubgoal(new Goal_GatherWood(Owner));
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

public class Goal_GatherWood : Goal_Composite
{
    int frames;

    public Goal_GatherWood(CognitiveAgent owner)
        : base(owner) { }

    public override void Activate()
    {
        //Debug.Log("Activating the Goal_GatherSmithingMaterials");
        AddToThoughtBubble("Gathering Wood");

        CurrentStatus = GoalStatus.Active;
        RemoveAllSubgoals();
        frames = 0;

        AddSubgoal(new Goal_MoveToPosition(Owner, LocationManager.Instance.GetLocationMemory("Chop_Wood")));
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