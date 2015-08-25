using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QPo_GiveItemToNPC : QuestPoint
{
    private int targetAgentId;
    private string targetItemMarker;


    public QPo_GiveItemToNPC(int agentId, string itemMarker)
        : base()
    {
        targetAgentId = agentId;
        targetItemMarker = itemMarker;
    }

    public QPo_GiveItemToNPC(string agentName, string itemMarker)
        : base()
    {
        targetAgentId = AgentManager.Instance.GetAgentIdByName(agentName);
        targetItemMarker = itemMarker;
    }

    public override bool AttemptToComplete(QuestPointCompletionData data)
    {
        if (_questAction != data.questAction)
            return false;

        QPoCD_GiveItemToNPC specificData = data as QPoCD_GiveItemToNPC;

        if (specificData == null)
            return false;

        if ((targetAgentId == specificData.agentId)&&(specificData.itemCue.hasCueMarker(targetItemMarker)))
        {
            Complete();
            return true;
        }
        return false;
    }

    public override bool IsPossible()
    {
        CharacterDetails targetAgentDetails = AgentManager.Instance.GetAgent( targetAgentId );

        if ( targetAgentDetails.IsAlive() )
            return true;

        return false;
    }

    public override bool AttemptImmediateCompletion()
    {
        return false;
    }

    protected override void SetQuestAction()
    {
        _questAction = QuestAction.GIVE_ITEM_TO_NPC;
    }

    public override void SetFragmentDescription()
    {
        string agentName = AgentManager.Instance.GetAgentNameById(targetAgentId);

        _description = "Give a " + targetItemMarker + "to " + agentName;
    }

}

public class QPoCD_GiveItemToNPC : QuestPointCompletionData
{
    public int agentId;
    public ItemCue itemCue;

    public QPoCD_GiveItemToNPC(int _agentId, ItemCue _itemCue)
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