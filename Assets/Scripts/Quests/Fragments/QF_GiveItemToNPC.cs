using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QF_GiveItemToNPC : QuestFragment
{
    private int targetAgentId;
    private string targetItemMarker;


    public QF_GiveItemToNPC(int agentId, string itemMarker)
        : base()
    {
        targetAgentId = agentId;
        targetItemMarker = itemMarker;
    }

    public QF_GiveItemToNPC(string agentName, string itemMarker)
        : base()
    {
        targetAgentId = AgentManager.Instance.GetAgentIdByName(agentName);
        targetItemMarker = itemMarker;
    }

    public override bool AttemptToComplete(QuestFragmentCompletionData data)
    {
        if (questAction != data.questAction)
            return false;

        QFCD_GiveItemToNPC specificData = data as QFCD_GiveItemToNPC;

        if (specificData == null)
            return false;

        if ((targetAgentId == specificData.agentId)&&(specificData.itemCue.hasCueMarker(targetItemMarker)))
        {
            Complete();
            return true;
        }
        return false;
    }

    public override bool AttemptImmediateCompletion()
    {
        return false;
    }

    protected override void SetQuestAction()
    {
        questAction = QuestAction.GIVE_ITEM_TO_NPC;
    }

    public override void setFragmentDescription()
    {
        string agentName = AgentManager.Instance.GetAgentName(targetAgentId);

        description = "Give a " + targetItemMarker + "to " + agentName;
    }

}

public class QFCD_GiveItemToNPC : QuestFragmentCompletionData
{
    public int agentId;
    public ItemCue itemCue;

    public QFCD_GiveItemToNPC(int _agentId, ItemCue _itemCue)
        : base()
    {
        agentId = _agentId;
        itemCue = _itemCue;
    }

    protected override void SetQuestAction()
    {
        questAction = QuestAction.GIVE_ITEM_TO_NPC;
    }
}