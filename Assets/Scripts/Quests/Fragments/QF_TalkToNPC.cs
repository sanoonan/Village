using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QF_TalkToNPC : QuestFragment
{
    public int targetAgentId;


    public QF_TalkToNPC(int agentId) 
        : base()
    {
        targetAgentId = agentId;
    }

    public QF_TalkToNPC(string agentName)
        : base()
    {
        targetAgentId = AgentManager.Instance.GetAgentIdByName(agentName);
    }

    public override bool AttemptToComplete(QuestFragmentCompletionData data)  
    {
        if (questAction != data.questAction)
            return false;

        QFCD_TalkToNPC specificData = data as QFCD_TalkToNPC;

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

    protected override void SetQuestAction()
    {
        questAction = QuestAction.TALK_TO_NPC;
    }

    public override void setFragmentDescription()
    {
        string agentName = AgentManager.Instance.GetAgentName(targetAgentId);

        description = "Talk to " + agentName;
    }
}

public class QFCD_TalkToNPC : QuestFragmentCompletionData
{
    public int agentId;

    public QFCD_TalkToNPC(int id)
        : base()
    {
        agentId = id;
    }

    protected override void SetQuestAction()
    {
        questAction = QuestAction.TALK_TO_NPC;
    }
}