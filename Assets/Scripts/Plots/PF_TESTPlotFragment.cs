using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PF_TESTPlotFragment : PlotFragment
{
    private struct PlotDetails
    {
        public int questGiverNpc;

        public PlotDetails( int questGiver )
        {
            questGiverNpc = questGiver;
        }
    }

    private PlotDetails _plotDetails;

    public PF_TESTPlotFragment()
        : base()
    {
        _label = "TEST. Just talk to questgiver";
    }

    protected override void SetAuthorGoals()
    {
        _authorGoals.Add( AuthorGoal.GOOD );
        _authorGoals.Add( AuthorGoal.DEATH );
        _authorGoals.Add( AuthorGoal.EVIL );
        _authorGoals.Add( AuthorGoal.VIOLENCE );
    }

    protected override void SetQuestPattern()
    {
        _questPattern = new QPa_EmptyQuestPattern( _plotDetails.questGiverNpc );
        _questPattern.AddActiveQuestPoint( new QPo_TalkToNPC( _plotDetails.questGiverNpc ) );
    }



    public override bool AreConstraintsFulfilled()
    {
        int numAgents = AgentManager.Instance.GetAgentCount();
        int randomInt = Random.Range( 0, numAgents );

        _plotDetails = new PlotDetails( randomInt );

        return true;
    }

    public override void PrintPlotDetails()
    {
        Debug.Log( "questGiverNPC - " + AgentManager.Instance.GetAgentNameById( _plotDetails.questGiverNpc ) );
    }
}

