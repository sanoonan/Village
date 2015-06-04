using UnityEngine;
using System.Collections.Generic;

//Niall adding stuff to ruin tiarnáns stuff
public class Goal_Fishing : Goal_Composite
{
    public Goal_Fishing(CognitiveAgent owner)
        : base(owner) { }

    //Get FishingRod (probably hammer for now)
    //Go to Docks (There is a fishing target but prob move him to the docks for now

    public override void Activate()
    {
        //Debug.Log("Activating the Goal_Blacksmith");
        Owner.ThoughtManager.SetDialog("(FISHER | )");
        CurrentStatus = GoalStatus.Active;
        RemoveAllSubgoals();

        //We can decompose working at the Smith's into smaller actions.
        //First check for Fishing rod, PROBABLY A HAMMER FOR NOW
        //We then travel to the Dock to do our work. 

        //Since the goals are as a stack, we have to add them in reverse order.

        AddSubgoal(new Goal_Idle_Fish(Owner));
        AddSubgoal(new Goal_MoveToPosition(Owner, LocationManager.Instance.GetLocationMemory("Work_Fishing")));
        AddSubgoal(new Goal_CollectItemFromMemory(Owner, "Rod", true)); 
    }

    public override void Terminate()
    {
        base.Terminate(); 
        Owner.Inventory.UnequipItem();        
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