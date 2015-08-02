using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QF_AcquireItem : QuestFragment
{
    private string targetItemMarker;


    public QF_AcquireItem(string itemMarker)
        : base()
    {
        targetItemMarker = itemMarker;
    }



    public override bool AttemptToComplete(QuestFragmentCompletionData data)
    {
        if (questAction != data.questAction)
            return false;

        QFCD_AcquireItem specificData = data as QFCD_AcquireItem;

        if (specificData == null)
            return false;


        if (specificData.itemCue.hasCueMarker(targetItemMarker))
        {
            Complete();
            return true;
        }
        return false;
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
        questAction = QuestAction.ACQUIRE_ITEM;
    }

    public override void setFragmentDescription()
    {
        description = "Acquire a " + targetItemMarker;
    }

}

public class QFCD_AcquireItem : QuestFragmentCompletionData
{
    public ItemCue itemCue;

    public QFCD_AcquireItem(ItemCue _itemCue) 
        : base()
    {
        itemCue = _itemCue;
    }

    protected override void SetQuestAction()
    {
        questAction = QuestAction.ACQUIRE_ITEM;
    }
}