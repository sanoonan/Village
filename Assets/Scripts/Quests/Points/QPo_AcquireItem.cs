using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QPo_AcquireItem : QuestPoint
{
    private string targetItemMarker;


    public QPo_AcquireItem(string itemMarker)
        : base()
    {
        targetItemMarker = itemMarker;
    }



    public override bool AttemptToComplete(QuestPointCompletionData data)
    {
        if (_questAction != data.questAction)
            return false;

        QPoCD_AcquireItem specificData = data as QPoCD_AcquireItem;

        if (specificData == null)
            return false;


        if (specificData.itemCue.hasCueMarker(targetItemMarker))
        {
            Complete();
            return true;
        }
        return false;
    }

    public override bool IsPossible()
    {
        return true;
    }

    public override bool AttemptImmediateCompletion()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        PlayerController playerController = playerObject.GetComponent<PlayerController>();
        AgentInventory playerInventory = playerController.Inventory;

        if (playerInventory.CheckIfHasItemWithCueMarker(targetItemMarker))
        {
            Complete();
            return true;
        }
        return false;
    }

    protected override void SetQuestAction()
    {
        _questAction = QuestAction.ACQUIRE_ITEM;
    }

    public override void SetFragmentDescription()
    {
        _description = "Acquire a " + targetItemMarker;
    }

}

public class QPoCD_AcquireItem : QuestPointCompletionData
{
    public ItemCue itemCue;

    public QPoCD_AcquireItem(ItemCue _itemCue) 
        : base()
    {
        itemCue = _itemCue;
    }

    protected override void SetQuestAction()
    {
        questAction = QuestAction.ACQUIRE_ITEM;
    }
}