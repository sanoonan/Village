using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QC_KillNpcAndReportToQuestGiver : QuestChain
{
    private int _victemId;

    public QC_KillNpcAndReportToQuestGiver( int questGiverId, int victemId )
        : base( questGiverId )
    {
        _victemId = victemId;

        Setup();
    }

    public QC_KillNpcAndReportToQuestGiver( string questGiverName, string victemName )
        : base( questGiverName )
    {
        _victemId = AgentManager.Instance.GetAgentIdByName( victemName );

        Setup();
    }

    private void Setup()
    {
        AddActiveQuestFragment( new QF_KillNPC( _victemId ) );
        AddLinearQuestFragment( new QF_TalkToNPC( _questGiverId ) );
    }


    public override void SetChainDescription()
    {
        description = "Kill " + AgentManager.Instance.GetAgentNameById( _victemId ) + " and report back to " + AgentManager.Instance.GetAgentNameById( _questGiverId );
    }
}