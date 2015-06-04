using UnityEngine;
using System.Collections.Generic;

public class Goal_StableWork : Goal_Composite
{
    public Goal_StableWork(CognitiveAgent owner)
        : base(owner) { }

    //Get FishingRod (probably hammer for now)
    //Go to Docks (There is a fishing target but prob move him to the docks for now

    public override void Activate()
    {
        //Debug.Log("Activating the Goal_Blacksmith");
        Owner.ThoughtManager.SetDialog("(STABLE WORKER | )");
        CurrentStatus = GoalStatus.Active;
        RemoveAllSubgoals();

        //StableBoy
        //Get Hay 
        //Gets bucket for water
        //goes to well for water
        //Work

        //Since the goals are as a stack, we have to add them in reverse order.

        AddSubgoal(new Goal_Idle_StableWork(Owner));
        AddSubgoal(new Goal_MoveToPosition(Owner, LocationManager.Instance.GetLocationMemory("Work_Stable")));
        AddSubgoal(new Goal_MoveToPosition(Owner, LocationManager.Instance.GetLocationMemory("Work_Well")));
        AddSubgoal(new Goal_CollectItemFromMemory(Owner, "Bucket", true));  //The Fishing Rod is currently the pitchfork near the Blacksmiths. 
        AddSubgoal(new Goal_MoveToPosition(Owner, LocationManager.Instance.GetLocationMemory("Work_Stable")));
        AddSubgoal(new Goal_CollectItemFromMemory(Owner, "Hay", true));  //The Fishing Rod is currently the pitchfork near the Blacksmiths. 
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

public class Goal_GatherHay : Goal_Composite
{
    int frames;

    public Goal_GatherHay(CognitiveAgent owner)
        : base(owner) { }

    public override void Activate()
    {
        //Debug.Log("Activating the Goal_GatherSmithingMaterials");
        AddToThoughtBubble("Gathering Hay");

        CurrentStatus = GoalStatus.Active;
        RemoveAllSubgoals();
        frames = 0;
        //Not useing yet so not sure on location
        //AddSubgoal(new Goal_MoveToPosition(Owner, LocationManager.Instance.GetLocationMemory("Chop_Wood")));
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
