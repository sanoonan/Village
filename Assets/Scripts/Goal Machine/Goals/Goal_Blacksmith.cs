using UnityEngine;
using System.Collections.Generic;

public class Goal_Blacksmith : Goal_Composite
{
    public Goal_Blacksmith(CognitiveAgent owner)
        : base(owner) { }

    //Gather Materials
    //Get Hammer
    //Gather two bushels of charcoal
    //Go to Smithy

    public override void Activate()
    {
        //Debug.Log("Activating the Goal_Blacksmith");
        Owner.ThoughtManager.SetDialog("(BLACKSMITH | )");
        CurrentStatus = GoalStatus.Active;
        RemoveAllSubgoals();

        //We can decompose working at the Smith's into smaller actions.
        //First we check to see if we need more materials, and gather them if so.
        //Then we check to see where the hammer is, if it needs repaired etc., and get a working hammer.
        //We then travel to the smith's to do our work. 

        //Since the goals are as a stack, we have to add them in reverse order.

        AddSubgoal(new Goal_Idle_Blacksmith(Owner));
        AddSubgoal(new Goal_MoveToPosition(Owner, LocationManager.Instance.GetLocationMemory("Work_Blacksmith")));
        AddSubgoal(new Goal_CollectItemFromMemory(Owner, "Hammers", true));
        //AddSubgoal(new Goal_CollectItem(Owner, "Item_SmithingHammer", true));
        //AddSubgoal(new Goal_GatherSmithingMaterials(Owner));
        //AddSubgoal(new Goal_PatrolArea(Owner, "NorthernStreets"));
        //AddSubgoal(new Goal_MoveToPosition(Owner, LocationManager.Instance.GetLocationMemory("Work_Blacksmith")));
        //AddSubgoal(new Goal_MoveToPosition(Owner, LocationManager.Instance.GetLocationMemory("Work_Shop")));
    }

    public override void Terminate()
    {
        Owner.Inventory.UnequipItem();
        base.Terminate();
    }

    public override GoalStatus Process()
    {
        //Debug.Log("Processing the Goal_Blacksmith");

        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        if (subGoals.Count > 0)
            CurrentStatus = ProcessSubgoals();
        else
        {
            //We can carry out the "Blacksmith" specific process.
            //Or should we just make it another sub-goal that keeps going at the end...?
        }

        return CurrentStatus;
    }
}

public class Goal_GatherSmithingMaterials : Goal_Composite
{
    int frames;

    public Goal_GatherSmithingMaterials(CognitiveAgent owner)
        : base(owner) { }

    public override void Activate()
    {
        //Debug.Log("Activating the Goal_GatherSmithingMaterials");
        AddToThoughtBubble("Gathering Materials");

        CurrentStatus = GoalStatus.Active;
        RemoveAllSubgoals();
        frames = 0;

        AddSubgoal(new Goal_MoveToPosition(Owner, LocationManager.Instance.GetLocationMemory("Mine_Gems")));
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