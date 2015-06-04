using UnityEngine;
using System.Collections.Generic;

//Need to put in the code to "catch" walking past a person in your list.
//Keeping an eye on the Owner's list of current active Cues seems the best way to do it!
//Or use a grouped message system!
public class Goal_LookForPerson : Goal_Composite
{
    Transform targetTransform;
    Vector3 targetPosition;

    string targetUID;
    List<string> locationUIDs;

    public Goal_LookForPerson(CognitiveAgent owner, string targetUID)
        : base(owner)
    {
        this.targetUID = targetUID;
        locationUIDs = new List<string>();

        if (!Owner.MindsEye.GetConceptNeighbours(targetUID, MemoryType.Location, ref locationUIDs))
            CurrentStatus = GoalStatus.Failed;
            //throw new UnityException("I can't remember anything! I've no idea what to doooooooooooooooo! - " + Owner.name);
    }

    public override void Activate()
    {
        //Debug.Log("Activating the Goal_LookForPerson - " + locationUIDs.Count);
        AddToThoughtBubble("Looking for " + targetUID);

        CurrentStatus = GoalStatus.Active;
        RemoveAllSubgoals();

        if (locationUIDs.Count > 0)
        {
            string locationUID = locationUIDs[0];
            locationUIDs.RemoveAt(0);
            AddSubgoal(new Goal_PatrolArea(Owner, locationUID));
        }
        else
            CurrentStatus = GoalStatus.Failed;      //There is nowhere left to check;
    }

    //Add code to check to see if they can see the person yet.
    public override GoalStatus Process()
    {
        //Debug.Log("Processing the Goal_GatherSmithingMaterials");

        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        if (subGoals.Count > 0)
        {
            CurrentStatus = ProcessSubgoals();

            if (CurrentStatus == GoalStatus.Completed)
                Activate();
        }

        return CurrentStatus;
    }
}

public class Goal_MeetAndRequestInfo : Goal_Composite
{
    Transform targetTransform;
    Vector3 targetPosition;

    string cueUID;          //The topic around which the agent wants to meet about.
    string targetUID;
    List<string> targetUIDs;

    bool messagedTarget;

    public Goal_MeetAndRequestInfo(CognitiveAgent owner, string cueUID)
        : base(owner)
    {
        //Debug.Log(cueUID);
        this.cueUID = cueUID;
        Owner.DesiredTopic = cueUID;
        
        targetUIDs = new List<string>();

        if (!Owner.MindsEye.GetConceptNeighbours("Villagers", ref targetUIDs))
            CurrentStatus = GoalStatus.Failed;
            //throw new UnityException("I can't remember anything! I've no idea what to doooooooooooooooo! - " + Owner.name);

        //Debug.Log("Activating the Goal_MeetAndRequestInfo - " + targetUIDs.Count);
    }

    public override void Reactivate()
    {
        AddToThoughtBubble("Looking for " + targetUID);
        Owner.DesiredTopic = cueUID;    //In case a sub-goal had him looking for something else, we reset the agent's desired topic to this topic.

        if (Owner.MindsEye.GetConceptLocation(cueUID))
            CurrentStatus = GoalStatus.Completed;

        base.Reactivate();
    }

    public override void Activate()
    {
        //Debug.Log("Activating the Goal_MeetAndRequestInfo - " + targetUIDs.Count);
        AddToThoughtBubble("Looking for " + targetUID);
        messagedTarget = false;
        CurrentStatus = GoalStatus.Active;
        RemoveAllSubgoals();

        if (targetUIDs.Count > 0)
        {
            if (targetUIDs[0] == Owner.CharacterCue.UniqueNodeID)
            {
                targetUIDs.RemoveAt(0);
                Activate();
            }
            else
            {
                targetUID = targetUIDs[0];
                targetUIDs.RemoveAt(0);
                AddSubgoal(new Goal_LookForPerson(Owner, targetUID));
            }
        }
        else
            CurrentStatus = GoalStatus.Failed;      //There is nobody left to meet.
    }

    public override GoalStatus Process()
    {
        if (CurrentStatus == GoalStatus.Inactive || CurrentStatus == GoalStatus.Failed && targetUIDs.Count > 0)
            Activate();

        if (CurrentStatus == GoalStatus.Completed || CurrentStatus == GoalStatus.Failed)
            return CurrentStatus;

        if (subGoals.Count > 0)
        {
            CurrentStatus = ProcessSubgoals();

            if (CurrentStatus == GoalStatus.Completed && messagedTarget == false)
                Activate();
        }

        //Probably want to add a timer here, so if the message isn't answered we time out with a fail.

        if (messagedTarget == false && Owner.MindsEye.ActiveCues.ContainsKey(targetUID))
        {
            //We've found the person we're looking for!
            messagedTarget = true;
            Message.DispatchMessage(Owner.CharDetails.AgentID, AgentManager.Instance.GetAgent(targetUID).AgentID, TelegramType.Greeting);
        }

        return CurrentStatus;
    }

    public override bool HandleMessage(Telegram message)
    {
        if(base.HandleMessage(message))
            return true;

        if (message.TelegramType == TelegramType.Greeting && message.IsResponse)
        {
            CurrentStatus = GoalStatus.Completed;
        }

        return false;   //We allow it to be properly handled further up the chain.        
    }
}

public class Goal_Conversation : Goal_Composite
{
    Vector3 meetingLocation;
    bool isLeader;

    float meetingTimer;
    List<int> participants;

    string targetUID;
    bool IsCasualConversation;

    public Goal_Conversation(CognitiveAgent owner, Vector3 meetingDirection, List<int> participants, bool isLeader = false, string targetUID = null)
        : base(owner)
    {
        this.meetingLocation = Owner.Transform.position + meetingDirection * 0.35f;
//        Debug.Log(isLeader + " : " + Owner.Transform.position + ": " + meetingLocation);
        this.isLeader = isLeader;
        this.participants = participants;
        this.targetUID = targetUID;

        if (targetUID == null)
            IsCasualConversation = true;

        if (isLeader)
            meetingTimer = Random.Range(20.0f, 30.0f);
    }

    //Walk to meeting point
    //Enter taling animation
    //Wait for end of conversation from conversation "leader".
    public override void Reactivate()
    {
        Owner.IsAlert = false;
        AddToThoughtBubble("Talking with " + Owner.name);
        base.Reactivate();
    }

    public override void Activate()
    {
        Owner.IsAlert = false;
        AddToThoughtBubble("Talking with " + Owner.name);
        CurrentStatus = GoalStatus.Active;

        RemoveAllSubgoals();

        AddSubgoal(new Goal_Talk(Owner, participants));
        AddSubgoal(new Goal_MoveToPosition(Owner, meetingLocation));
        AddSubgoal(new Goal_Greeting(Owner, participants));
    }

    public override void Terminate()
    {
        base.Terminate();

        Owner.IsAlert = true;

        for (int i = 0; i < participants.Count; i++)
            if (participants[i] != Owner.CharDetails.AgentID)
                Message.DispatchMessage(Owner.CharDetails.AgentID, participants[i], TelegramType.MeetingComplete);        
    }

    public override GoalStatus Process()
    {
        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        if (subGoals.Count > 0)
            CurrentStatus = ProcessSubgoals();

        if (isLeader)
        {
            meetingTimer -= Time.smoothDeltaTime;
            if (meetingTimer <= 0.0f)
            {
                RemoveAllSubgoals();
                CurrentStatus = GoalStatus.Completed;

                if (isLeader && !IsCasualConversation)
                {
                    Owner.MindsEye.GainMemoriesFromSource(participants, Owner.CharDetails.AgentID, targetUID);      //This allows the owner to gather information about the conversation that just occured.
                    //Do they have a spare item in their inventory they could lend me?
                    CheckForSpareItem(Owner, participants, targetUID);
                }

                //Send a meeting complete message to all participants.
                for (int i = 0; i < participants.Count; i++)
                    if (participants[i] != Owner.CharDetails.AgentID)
                        Message.DispatchMessage(Owner.CharDetails.AgentID, participants[i], TelegramType.MeetingComplete);
            }
        }

        return CurrentStatus;
    }

    private void CheckForSpareItem(CognitiveAgent Owner, List<int> participants, string targetUID)
    {
        for (int i = 0; i < participants.Count; i++)
        {
            if (participants[i] == Owner.CharDetails.AgentID)
                continue;

            AgentInventory traderInventory = AgentManager.Instance.GetAgent(participants[i]).CognitiveAgent.Inventory;

            //Get equipped item from trader's inventory.
            //See if cues of equipped item match the cue needed to satisfy the quest.
            //If so, remove from one inventory and pass to the other.
            ItemCue traderItem = traderInventory.GetSpareItem(targetUID);
            if (traderItem != null)
            {
                Debug.Log("I've got an item handed to me! YAY!");
                Owner.Inventory.PickupItem(traderItem, true);
                Owner.CheckForQuestCompletion(targetUID);      //If the Owner was looking for this item, then they've since found info about it on their own so close the quest as auto-completed.
            }
        }
    }

    public override bool HandleMessage(Telegram message)
    {
        if (base.HandleMessage(message))
            return true;

        if (message.TelegramType == TelegramType.MeetingComplete)
        {
            RemoveAllSubgoals();
            CurrentStatus = GoalStatus.Completed;
            return true;
        }

        return false;   //The message is unhandled by this goal.
    }
}
