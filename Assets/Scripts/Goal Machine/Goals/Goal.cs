using UnityEngine;
using System.Collections.Generic;

//Work in bi-directional nodes, so that we can have a node map inference. Also, make nodes shared, so an NPC only stores... references to single nodes?
//e.g. if two NPCs have "Pub" nodes, then they are actually nodes referencing the Pub memory...? (How could this work? Is this... worth it?)
//It really depends on the amount of duplicated data. Could be useful for shared memories.
//Create different node types? People Nodes, Item Nodes, Event (Shared) Nodes. Cue Nodes (Single direction entry point nodes)
//See if it is worth while doing that, or if it provides no benefit just just general nodes.

public enum GoalStatus
{
    Inactive,   //The goal is waiting to be activated.
    Active,     //The goal has been activated and will be processed each update step.
    Completed,  //The goal has been completed and will be removed on the next update.
    Failed      //The goal has failed and will either replan or be removed on the next update.
};

public abstract class Goal
{
    protected CognitiveAgent Owner;
    protected GoalStatus CurrentStatus;
    protected int GoalType;

    

    public Goal(CognitiveAgent Owner)
    {
        this.Owner = Owner;
    }

    //Contains initialisation logic and representse the planning phase of the goal.
    //This can be called any number of times to replan if the situation demands.
    public abstract void Activate();

    //This is executed each update step, and returns an enum to indicate the status of the goal.
    public abstract GoalStatus Process();

    //Undertakes any necessary tidying up before a goal is exited and is called just before a goal is destroyed.
    public virtual void Terminate() { }

    public virtual void Reactivate() { }

    //Handles any messages going to the goal and / or sub-goals.
    public virtual bool HandleMessage(Telegram message)
    {
        return false;   //The message is unhandled.
    }

    public virtual void AddSubgoal(Goal g)
    {
        //To suit the design, we must allow AddSubgoal to be called on both goal types.
        //However only composite actions can have sub-goals, so we must catch any faulty logic which tries to add a subgoal to an atomic, i.e. single action, goal.
        throw new System.NotImplementedException("AddSubgoal is unhandled. It is possible this is an atomic (not composite) goal.");
    }

    protected void AddToThoughtBubble(string subgoalString)
    {
        if (Owner.ThoughtManager.GetDialogue().IndexOf('|') >= 0)
            Owner.ThoughtManager.SetDialog(Owner.ThoughtManager.GetDialogue().Substring(0, Owner.ThoughtManager.GetDialogue().IndexOf('|')) + "| " + subgoalString + ")");
        else
            Owner.ThoughtManager.SetDialog(subgoalString);
    }

    public bool IsActive()
    {
        return CurrentStatus == GoalStatus.Active;
    }

    public bool IsInactive()
    {
        return CurrentStatus == GoalStatus.Inactive;
    }

    public bool IsCompleted()
    {
        return CurrentStatus == GoalStatus.Completed;
    }

    public bool HasFailed()
    {
        return CurrentStatus == GoalStatus.Failed;
    }

    

}


public abstract class Goal_Atomic : Goal
{


    public Goal_Atomic(CognitiveAgent owner)
        : base(owner) { }

    public override void Terminate()
    {
        base.Terminate();
        Owner.stopModification();
    }

    public abstract void applyStateModVector();

    
}





public abstract class Goal_Composite : Goal
{
    protected Stack<Goal> subGoals;

    public Goal_Composite(CognitiveAgent owner)
        : base(owner) 
    {
        subGoals = new Stack<Goal>();
    }

    public override void AddSubgoal(Goal g)
    {
        subGoals.Push(g);       //Add to the front of the stack, making it the new current sub-goal.
    }

    protected GoalStatus ProcessSubgoals()
    {
        GoalStatus statusOfSubGoal = GoalStatus.Active;
        while (subGoals.Count > 0 && (subGoals.Peek().IsCompleted() || subGoals.Peek().HasFailed()))
        {
            if(subGoals.Peek().IsCompleted())
                statusOfSubGoal = GoalStatus.Completed;
            else
                statusOfSubGoal = GoalStatus.Failed;

            subGoals.Pop().Terminate();
            if(subGoals.Count > 0)
                subGoals.Peek().Reactivate();   //This allows for special commands that might need executed after a sub-goal takes main control again.
        }

        if (subGoals.Count > 0)
        {
            statusOfSubGoal = subGoals.Peek().Process();

            if (statusOfSubGoal == GoalStatus.Completed && subGoals.Count > 1)
                return GoalStatus.Active;
        }

        return statusOfSubGoal;
    }

    public override void Reactivate()
    {
        if (subGoals.Count > 0)
            subGoals.Peek().Reactivate();
    }

    protected void RemoveAllSubgoals()
    {
        int subgoalCount = subGoals.Count;
        for (int i = 0; i < subgoalCount; i++)
            subGoals.Pop().Terminate();             //Remove from the stack and terminate it.

        subGoals.Clear();
    }

    public override bool HandleMessage(Telegram message)
    {
        if (subGoals.Count > 0 && subGoals.Peek().HandleMessage(message))
            return true;    //The message has been handled by a sub-goal.

        return false;   //The message is unhandled by this goal.
    }
}

