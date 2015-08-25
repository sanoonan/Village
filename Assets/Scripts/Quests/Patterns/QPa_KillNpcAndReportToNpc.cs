using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QPa_KillNpcAndReportToNpc : QuestPattern
{
    private int _victemId;
    private int _reporteeId;

    public QPa_KillNpcAndReportToNpc( int questGiverId, int victemId, int reporteeId )
        : base( questGiverId )
    {
        _victemId = victemId;
        _reporteeId = reporteeId;

        Setup();
    }

    public QPa_KillNpcAndReportToNpc( string questGiverName, string victemName, string reporteeName )
        : base( questGiverName )
    {
        _victemId = AgentManager.Instance.GetAgentIdByName( victemName );
        _reporteeId = AgentManager.Instance.GetAgentIdByName( reporteeName );

        Setup();
    }

    private void Setup()
    {
        AddActiveQuestPoint( new QPo_KillNPC( _victemId ) );
        AddLinearQuestPoint( new QPo_TalkToNPC( _reporteeId ) );
    }


    public override void SetPatternDescription()
    {
        _description = "Kill " + AgentManager.Instance.GetAgentNameById( _victemId ) + " and report back to " + AgentManager.Instance.GetAgentNameById( _reporteeId );
    }
}