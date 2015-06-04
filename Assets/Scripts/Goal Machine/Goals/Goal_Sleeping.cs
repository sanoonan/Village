using UnityEngine;
using System.Collections.Generic;


public class Goal_Sleeping : Goal_Composite
{
    public Goal_Sleeping(CognitiveAgent owner)
        : base(owner) { }

    public override void Activate()
    {
        //Debug.Log("Activating the Goal_Blacksmith");
        Owner.ThoughtManager.SetDialog("(SLEEPING | )");
        Owner.IsAlert = false;                              //The agent is not alert when sleeping, so can't respond to messages, chat requests etc.
        CurrentStatus = GoalStatus.Active;
        RemoveAllSubgoals();

        AddSubgoal(new Goal_Idle_Sleep(Owner));
        AddSubgoal(new Goal_MoveToPosition(Owner, Owner.BedTarget));        //Goes directly to their bed.
        //AddSubgoal(new Goal_MoveToPosition(Owner, LocationManager.Instance.GetLocationMemory("Bed_"+Owner.name)));    //Uses the memory model.
        
    }

    public override void Terminate()
    {
        Owner.IsAlert = true;       //The agent is now awake, so can respond again.
        base.Terminate();
    }

    public override GoalStatus Process()
    {
        //Debug.Log("Processing the Goal_Blacksmith");

        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        if (subGoals.Count > 0)
            CurrentStatus = ProcessSubgoals();

        return CurrentStatus;
    }
}