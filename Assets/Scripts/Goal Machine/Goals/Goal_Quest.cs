using UnityEngine;
using System.Collections;

public class Goal_Quest : Goal_Composite
{
    string cueUID;

    public Goal_Quest(CognitiveAgent owner, string cueUID)
        : base(owner)
    {
        this.cueUID = cueUID;
    }

    public override void Activate()
    {
        Owner.ThoughtManager.SetDialog(string.Format("(QUEST: {0} | )", cueUID));

        CurrentStatus = GoalStatus.Active;
        RemoveAllSubgoals();

        AddSubgoal(new Goal_Idle_Quest(Owner));
        AddSubgoal(new Goal_MoveToPosition(Owner, LocationManager.Instance.GetLocationMemory("QuestBoard")));

        Owner.EnableQuest(new QuestInfo() { QuestUID = cueUID, ExpiryTimer = -1.0f, CashReward = 150 });
    }

    public override void Reactivate()
    {
        if (Owner.MindsEye.GetConceptLocation(cueUID))
            CurrentStatus = GoalStatus.Completed;
        
        base.Reactivate();
    }

    public override GoalStatus Process()
    {
        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        if (CurrentStatus == GoalStatus.Completed)
            return CurrentStatus;

        if (subGoals.Count > 0)
            CurrentStatus = ProcessSubgoals();

        return CurrentStatus;
    }

    public override bool HandleMessage(Telegram message)
    {
        if (base.HandleMessage(message))
            return true;

        if (message.TelegramType == TelegramType.QuestTrade)
        {
            RemoveAllSubgoals();

            CharacterDetails traderDetails = AgentManager.Instance.GetAgent(message.Sender);
            AgentInventory traderInventory = (traderDetails._isPlayer ? traderDetails.PlayerAgent.Inventory : traderDetails._cognitiveAgent.Inventory);

            //Get equipped item from trader's inventory.
            //See if cues of equipped item match the cue needed to satisfy the quest.
            //If so, remove from one inventory and pass to the other.
            ItemCue traderItem = traderInventory.GetEquippedItem();
            if (traderItem != null)
            {
                for (int i = 0; i < traderItem.CueMarkers.Length; i++)
                {
                    if(traderItem.CueMarkers[i] == Owner.QuestInfo.QuestUID)
                    {
                        Owner.Inventory.PickupItem(traderDetails.PlayerAgent.Inventory.LoseEquippedItem(), true);
                        Owner.CompleteQuest();
                        CurrentStatus = GoalStatus.Completed;
                        return true;
                    }
                }
            }
        }

        return false;   //The message is unhandled by this goal.
    }
}
