using UnityEngine;
using System.Collections.Generic;


public class Goal_Talk : Goal
{
    List<int> participants;

    public Goal_Talk(CognitiveAgent owner, List<int> participants)
        : base(owner)
    {
        this.participants = participants;
    }

    public override void Reactivate()
    {
        Activate();
    }

    public override void Activate()
    {
        AddToThoughtBubble("Talking");
        Owner.DialogManager.SetDialog("*rabble* *rabble*");

        CurrentStatus = GoalStatus.Active;

        Owner.Animation[Owner.AnimationKeys["talk"]].speed = Owner.AnimationSpeed;
        Owner.Animation.CrossFade(Owner.AnimationKeys["talk"]);
    }

    public override GoalStatus Process()
    {
        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        return CurrentStatus;
    }

    public override void Terminate()
    {
        Owner.DialogManager.SetDialog("...");
    }
}


public class Goal_Greeting : Goal
{
    List<int> participants;

    public Goal_Greeting(CognitiveAgent owner, List<int> participants)
        : base(owner)
    {
        this.participants = participants;
    }

    public override void Activate()
    {
        AddToThoughtBubble("Greeting");

        if (participants.Count > 2)
            Owner.DialogManager.SetDialog("Hey, everyone!");
        else
            Owner.DialogManager.SetDialog(string.Format("Hey, {0}.", AgentManager.Instance.GetAgent(participants[1]).name));

        CurrentStatus = GoalStatus.Active;

        Owner.Animation[Owner.AnimationKeys["greet"]].speed = Owner.AnimationSpeed * 0.5f;
        Owner.Animation[Owner.AnimationKeys["greet"]].wrapMode = WrapMode.Once;
        Owner.Animation.CrossFade(Owner.AnimationKeys["greet"]);

        Owner.GetComponent<AudioSource>().PlayOneShot(Owner.GreetingClip);

        if(Owner.useStateVector)
            ModifyRelationships();
    }

    public override GoalStatus Process()
    {
        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        if (Owner.Animation.IsPlaying(Owner.AnimationKeys["greet"]) == false)
            CurrentStatus = GoalStatus.Completed;

        return CurrentStatus;
    }

    public override void Terminate()
    {
        Owner.DialogManager.SetDialog("...");
    }

    private void ModifyRelationships()
    {
        int numParticipants = participants.Count;

        for (int i = 0; i < numParticipants; i++)
            if(participants[i] != Owner.CharDetails.AgentID)
                Owner.relationshipManager.AddNewRelationship(participants[i]);
    }
}

public class Goal_CollectItemFromMemory : Goal_Composite
{
    MemoryGraphNode strongestMem;

    string cueUID;
    bool putInHand;

    bool searchingForInfo;

    public Goal_CollectItemFromMemory(CognitiveAgent owner, string cueUID, bool putInHand = false)
        : base(owner)
    {
        this.cueUID = cueUID;
        this.putInHand = putInHand;
    }

    public override void Activate()
    {
        AddToThoughtBubble("Collecting " + cueUID);

        CurrentStatus = GoalStatus.Active;
        RemoveAllSubgoals();

        if (Owner.MindsEye.GetConceptLocation(cueUID, out strongestMem))
        {
            searchingForInfo = false;
            Owner.CheckForQuestCompletion(cueUID);      //If the Owner was looking for this item, then they've since found info about it on their own so close the quest as auto-completed.
            if (Owner.Inventory.Contains(strongestMem.UID))
            {
                Owner.Inventory.EquipItem(strongestMem.UID);

                if (((ItemCue)strongestMem.MemoryNode.MemoryCue).Durability >= 0.0f)
                    Owner.MindsEye.MemoryGraph.UpdateCueOpinion(strongestMem.MemoryNode, 0.25f);      //Each time we use an item we will get more familiar with it.

                CurrentStatus = GoalStatus.Completed;
            }
            else
            {
                AddSubgoal(new Goal_MoveToPosition(Owner, strongestMem.LastPosition));
            }
        }
        else
        {
            if (searchingForInfo)
            {
                AddSubgoal(new Goal_Quest(Owner, cueUID));
                CurrentStatus = GoalStatus.Active;
            }
            else
            {
                searchingForInfo = true;
                AddSubgoal(new Goal_MeetAndRequestInfo(Owner, cueUID));
            }
        }
    }

    public override void Reactivate()
    {
        AddToThoughtBubble("Collecting " + cueUID);
        base.Reactivate();
    }

    public override void Terminate()
    {
        AddToThoughtBubble("Collecting " + cueUID);
        base.Terminate();
    }

    public override GoalStatus Process()
    {
        //Debug.Log("Processing the Goal_CollectItem");

        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        if (subGoals.Count > 0)
        {
            CurrentStatus = ProcessSubgoals();

            if (CurrentStatus == GoalStatus.Completed)
            {
                if (searchingForInfo)
                {
                    Activate();
                }
                else
                {
                    ItemCue itemCue = (ItemCue)strongestMem.MemoryNode.MemoryCue;

                    if (Owner.MindsEye.ActiveCues.ContainsKey(itemCue.UniqueNodeID) && itemCue.Status != ItemStatus.Owned)  //For the moment, we'll ignore items owned by players. We don't want them to steal them out of their hand!
                    {
                        if(itemCue.Status == ItemStatus.Shop)
                        {
                            //Now I need to try and buy it!
                            AddSubgoal(new Goal_PurchaseItem(Owner, itemCue.UniqueNodeID));
                            searchingForInfo = true;    //This will cause the goal to reset once the shop purchase is finished, to see if it was sucessful.
                            CurrentStatus = GoalStatus.Active;
                            
                        }
                        else if (itemCue.Status == ItemStatus.Free)
                        {
                            Owner.Inventory.PickupItem(itemCue, putInHand);
                            if (itemCue.Durability >= 0.0f)
                                Owner.MindsEye.MemoryGraph.UpdateCueOpinion(strongestMem.MemoryNode, 0.25f);    //Each time we use an item we will get more familiar with it.

                            CurrentStatus = GoalStatus.Completed;
                        }
                        
                    }
                    else
                    {
                        MemoryGraphNode mgn = Owner.MindsEye.MemoryGraph.GetNamedNodeFromGraph(itemCue.UniqueNodeID);
                        mgn.LastLocation = Locations.Unknown;
                        Activate();
                    }
                }
            }
            else if (CurrentStatus == GoalStatus.Failed)
            {
                //Debug.Log("Attempting to meet people have failed, so I'll create a quest and hope someone can help me out!");
                AddSubgoal(new Goal_Quest(Owner, cueUID));
                CurrentStatus = GoalStatus.Active;
            }
        }

        return CurrentStatus;
    }
}