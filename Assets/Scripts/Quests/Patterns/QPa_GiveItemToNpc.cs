using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QPa_GiveItemToNpc : QuestPattern
{
    public string _targetItemMarker;
    public int _targetNpcId;

    public QPa_GiveItemToNpc(int questGiverId, string itemMarker, int targetNpcId)
        : base(questGiverId)
    {
        _targetItemMarker = itemMarker;
        _targetNpcId = targetNpcId;

        Setup();
    }

    public QPa_GiveItemToNpc( string questGiverName, string itemMarker, string targetNpcName )
        : base(questGiverName)
    {
        _targetItemMarker = itemMarker;
        _targetNpcId = AgentManager.Instance.GetAgentIdByName( targetNpcName );

        Setup();
    }

    private void Setup()
    {
        AddActiveQuestPoint( new QPo_AcquireItem( _targetItemMarker ) );
        AddLinearQuestPoint( new QPo_GiveItemToNPC( _targetNpcId, _targetItemMarker ) );
    }




    public override void SetPatternDescription()
    {
        _description = "Give a " + _targetItemMarker + " to " + AgentManager.Instance.GetAgentNameById( _targetNpcId );
    }
}