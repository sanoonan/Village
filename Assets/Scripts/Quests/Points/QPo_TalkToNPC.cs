using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QPo_TalkToNPC : QuestPoint
{
    public int targetAgentId;


    public QPo_TalkToNPC(int agentId) 
        : base()
    {
        targetAgentId = agentId;
    }

    public QPo_TalkToNPC(string agentName)
        : base()
    {
        targetAgentId = AgentManager.Instance.GetAgentIdByName(agentName);
    }

    public override bool AttemptToComplete(QuestPointCompletionData data)  
    {
        if (_questAction != data.questAction)
            return false;

        QPoCD_TalkToNPC specificData = data as QPoCD_TalkToNPC;

        if (specificData == null)
            return false;

        if (targetAgentId == specificData.agentId)
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

    public override bool IsPossible()
    {
        CharacterDetails targetAgentDetails = AgentManager.Instance.GetAgent( targetAgentId );

        if ( targetAgentDetails.IsAlive() )
            return true;

        return false;
    }

    protected override void SetQuestAction()
    {
        _questAction = QuestAction.TALK_TO_NPC;
    }

    public override void SetFragmentDescription()
    {
        string agentName = AgentManager.Instance.GetAgentNameById(targetAgentId);

        _description = "Talk to " + agentName;
    }
}

public class QPoCD_TalkToNPC : QuestPointCompletionData
{
    public int agentId;

    public QPoCD_TalkToNPC(int id)
        : base()
    {
        agentId = id;
    }

    protected override void SetQuestAction()
    {
        questAction = QuestAction.TALK_TO_NPC;
    }
}