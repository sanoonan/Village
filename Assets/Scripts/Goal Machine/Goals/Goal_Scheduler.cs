using UnityEngine;
using System.Collections.Generic;

//Our main "brain" goal, which can be used to schedule events and catch global messages.
public class Goal_Scheduler : Goal_Composite
{
    public Goal_Scheduler(CognitiveAgent owner, Task initTask)
        : base(owner)
    {
        UpdateTask(initTask);
    }

    public override void Activate()
    {
        CurrentStatus = GoalStatus.Active;
    }

    public void UpdateTask(Task newTask)
    {
        RemoveAllSubgoals();        //We remove all the current sub-goals, so they move to their new task.

        #region Set Task
        switch (newTask)
        {
            case Task.Blacksmith:
                AddSubgoal(new Goal_Blacksmith(Owner));
                break;

            case Task.Fisher:
                AddSubgoal(new Goal_Fishing(Owner));
                break;

            case Task.Lumberjack:
                AddSubgoal(new Goal_LumberJack(Owner));
                break;

            case Task.StableWorker:
                AddSubgoal(new Goal_StableWork(Owner));
                break;

            case Task.Woodcutter:
                AddSubgoal(new Goal_WoodCutting(Owner));
                break;
/*
            case Task.Custom:    //This is for any custom initial goals you only want to apply to a specific person for testing.
                AddSubgoal(new Goal_PatrolArea(Owner, "NorthernStreets", true));
                break;
                */
            case Task.Sleep:
                AddSubgoal(new Goal_Sleeping(Owner));
                break;

            case Task.Eat:
                AddSubgoal(new Goal_Eat(Owner));
                break;

            case Task.Idle:
                AddSubgoal(new Goal_Idle_Idle(Owner));
                break;

            case Task.Shopkeeper:
                AddSubgoal(new Goal_ShopKeeping(Owner));
                break;

            default:
                Debug.Log(string.Format("{0} task is not properly initalised, so agent will idle.", newTask));
                AddSubgoal(new Goal_Idle_Idle(Owner));   //If a task hasn't yet been hooked up, we'll just idle.
                break;
        }
        #endregion    
    }

    public override GoalStatus Process()
    {
        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        if (subGoals.Count > 0)
            CurrentStatus = ProcessSubgoals();
        else
         //  UpdateTask(Task.Idle);
            throw new UnityException("It really shouldn't be possible to have scheduled no goals!");

        return CurrentStatus;
    }

    public override bool HandleMessage(Telegram message)
    {
        if (base.HandleMessage(message))
            return true;

        if (Owner.IsAlert)
        {
            if (message.TelegramType == TelegramType.Greeting)
            {
                if (message.IsResponse)
                {
                    //This is a response message to a greeting message we sent to the sender.
                    Message.DispatchMessage(message.Receiver, message.Sender, TelegramType.MeetingRequest, false, Owner.Transform.position);
                    AddSubgoal(new Goal_Conversation(Owner, (AgentManager.Instance.GetAgent(message.Sender)._cognitiveAgent.Transform.position - Owner.Transform.position), new List<int>() { message.Receiver, message.Sender }, true, Owner.DesiredTopic));
                    return true;
                }
                else
                {
                    //This is a message from the sender, inviting us to a conversation.
                    //We've got a greeting request. We must respond and let the other agent know we are ready to talk.
                    Owner.IsAlert = false;      //We don't want to respond to any other greetings for the moment.
                    Message.DispatchMessage(message.Receiver, message.Sender, TelegramType.Greeting, true);
                    return true;
                }
            }
            else if (message.TelegramType == TelegramType.QuestTrade)
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
                        if (traderItem.CueMarkers[i] == Owner.QuestInfo.QuestUID)
                        {
                            Owner.Inventory.PickupItem(traderDetails.PlayerAgent.Inventory.LoseEquippedItem(), false);
                            Owner.CompleteQuest();
                            return true;
                        }
                    }
                }
            }
            else if (message.TelegramType == TelegramType.DiscussEvent)
            {
                AddSubgoal(new Goal_Event(Owner));
                return true;
            }
        }

        if (message.TelegramType == TelegramType.MeetingRequest)
        {
            //The other agent got our response and so we have no agreed to meet and talk.
            AddSubgoal(new Goal_Conversation(Owner, message.Location.Value - Owner.Transform.position, new List<int>() { message.Receiver, message.Sender }));
            return true;
        }
            

        return false;   //The message is unhandled by this goal.
    }
}
