using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QPo_KillNPC : QuestPoint
{
    public int targetAgentId;


    public QPo_KillNPC( int agentId )
        : base()
    {
        targetAgentId = agentId;
    }

    public QPo_KillNPC( string agentName )
        : base()
    {
        targetAgentId = AgentManager.Instance.GetAgentIdByName( agentName );
    }

    public override bool AttemptToComplete( QuestPointCompletionData data )
    {
        if ( _questAction != data.questAction )
            return false;

        QPoCD_KillNPC specificData = data as QPoCD_KillNPC;

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
        _questAction = QuestAction.KILL_NPC;
    }

    public override void SetFragmentDescription()
    {
        string agentName = AgentManager.Instance.GetAgentNameById( targetAgentId );

        _description = "Kill " + agentName;
    }
}

public class QPoCD_KillNPC : QuestPointCompletionData
{
    public int agentId;

    public QPoCD_KillNPC( int id )
        : base()
    {
        agentId = id;
    }

    protected override void SetQuestAction()
    {
        questAction = QuestAction.KILL_NPC;
    }
}