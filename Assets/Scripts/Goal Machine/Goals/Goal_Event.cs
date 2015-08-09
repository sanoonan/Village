using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class Goal_Event : Goal_Composite 
{
    public Goal_Event(CognitiveAgent owner)
        : base(owner)
    { }

    public override void Activate()
    {
        AddToThoughtBubble("Discussing a Recent Event");
        CurrentStatus = GoalStatus.Active;

        MemoryGraphNode eventNode = Owner.MindsEye.GetConceptNode("EVN_FrogAttack");

        if(eventNode != null)
        {
            eventNode = eventNode.Neighbours[0];        //This gives us direct access to the global, shared event node.

            if(eventNode.Neighbours.Count > 0)
            {
                List<string> targetUIDs = new List<string>();
                Owner.MindsEye.GetConceptNeighbours(eventNode.UID, MemoryType.Location, ref targetUIDs);     //Gets the locations the frogs were spotted at.
                MemoryGraphNode largeFrogNode = eventNode.FindMemoryAmongNeighbours("LargeFrogEvent");          //Sees if they've any idea about the sub events.
                MemoryGraphNode smallFrogNode = eventNode.FindMemoryAmongNeighbours("SmallFrogEvent");

                AddSubgoal(new Goal_DisplayDialogue(Owner, "...do you think they'll be back...?"));
                AddSubgoal(new Goal_DisplayDialogue(Owner, "Although I've no idea why..."));
                AddSubgoal(new Goal_DisplayDialogue(Owner, "I was sure we were in trouble, but then they just vanished!"));

                if(smallFrogNode != null)
                {
                    AddSubgoal(new Goal_DisplayDialogue(Owner, "Those guys give me the chills, that's for sure!"));
                    AddSubgoal(new Goal_DisplayDialogue(Owner, string.Format("I even spotted {0} tiny red frogs on the beach!", ((EventNode)smallFrogNode.MemoryNode).eventParams[0].ToString())));
                }

                if(largeFrogNode != null)
                {
                    AddSubgoal(new Goal_DisplayDialogue(Owner, string.Format("They were HUGE! {0} of them towered over the town!", ((EventNode)largeFrogNode.MemoryNode).eventParams[0].ToString())));
                }

                AddSubgoal(new Goal_DisplayDialogue(Owner, string.Format("They suddenly appeared at the {0}.", SplitCamelCase(targetUIDs[0]))));

            }
            else
            {
                AddSubgoal(new Goal_DisplayDialogue(Owner, "...but I didn't really see anything happen."));
                AddSubgoal(new Goal_DisplayDialogue(Owner, "I could feel the ground rumble..."));
            }
        }
        else
        {
            AddSubgoal(new Goal_DisplayDialogue(Owner, "Missed it completely. Just don't ask me how."));
            AddSubgoal(new Goal_DisplayDialogue(Owner, "Truth is, I've no idea about it."));
        }

        AddSubgoal(new Goal_DisplayDialogue(Owner, "So you want to talk about the frogs?"));
    }

    private string SplitCamelCase(string s)
    {
        var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

        return r.Replace(s, " ");
    }

    public override void Reactivate()
    {
        AddToThoughtBubble("Discussing a Recent Event");
        AddSubgoal(new Goal_DisplayDialogue(Owner, "Huh, so where were we?"));

        base.Reactivate();
    }

    public override void Terminate()
    {
        Owner.DialogManager.SetDialog("...");
        base.Terminate();
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

public class Goal_DisplayDialogue : Goal
{
    string dialogue;
    float dialogueTimer;

    public Goal_DisplayDialogue(CognitiveAgent Owner, string dialogue, float dialogueTimer = 3.0f)
        : base(Owner)
    {
        this.dialogue = dialogue;
        this.dialogueTimer = dialogueTimer;
    }

    public override void Activate()
    {
        Owner.IsAlert = false;
        Owner.DialogManager.SetDialog(dialogue);
        dialogueTimer = 3.0f;
        CurrentStatus = GoalStatus.Active;

        Owner._animationController.SetAnimation( CharacterState.Talking, Owner.AnimationSpeed );
    }

    public override void Reactivate()
    {
        Activate();
    }

    public override void Terminate()
    {
        Owner.DialogManager.SetDialog("...");
        Owner.IsAlert = true;
    }

    public override GoalStatus Process()
    {
        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        dialogueTimer -= Time.smoothDeltaTime;

        if (dialogueTimer <= 0.0f)
            CurrentStatus = GoalStatus.Completed;

        return CurrentStatus;
    }
}
