using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QF_KillNPC : QuestFragment
{
    public int targetAgentId;


    public QF_KillNPC( int agentId )
        : base()
    {
        targetAgentId = agentId;
    }

    public QF_KillNPC( string agentName )
        : base()
    {
        targetAgentId = AgentManager.Instance.GetAgentIdByName( agentName );
    }

    public override bool AttemptToComplete( QuestFragmentCompletionData data )
    {
        if ( questAction != data.questAction )
            return false;

        QFCD_KillNPC specificData = data as QFCD_KillNPC;

        if ( specificData == null )
            return false;

        if ( targetAgentId == specificData.agentId )
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
        questAction = QuestAction.KILL_NPC;
    }

    public override void SetFragmentDescription()
    {
        string agentName = AgentManager.Instance.GetAgentNameById( targetAgentId );

        description = "Kill " + agentName;
    }
}

public class QFCD_KillNPC : QuestFragmentCompletionData
{
    public int agentId;

    public QFCD_KillNPC( int id )
        : base()
    {
        agentId = id;
    }

    protected override void SetQuestAction()
    {
        questAction = QuestAction.KILL_NPC;
    }
}