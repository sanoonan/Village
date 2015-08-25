using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PF_JealousSuitor : PlotFragment
{
    private struct PlotDetails
    {
        public int suitorNpc;
        public int loveInterestNpc;
        public int victemNpc;

        public PlotDetails( int suitor, int loveInterest, int victem )
        {
            suitorNpc = suitor;
            loveInterestNpc = loveInterest;
            victemNpc = victem;
        }
    }

    private PlotDetails _plotDetails;

    public PF_JealousSuitor()
        : base()
    {
    }

    protected override void SetAuthorGoals()
    {
        _authorGoals.Add( AuthorGoal.VIOLENCE );
        _authorGoals.Add( AuthorGoal.EVIL );
        _authorGoals.Add( AuthorGoal.DEATH );
    }

    protected override void SetQuestPattern()
    {
        _questPattern = new QPa_KillNpcAndReportToNpc( _plotDetails.suitorNpc, _plotDetails.victemNpc, _plotDetails.suitorNpc );
    }

    

    public override bool AreConstraintsFulfilled()
    {
        List<PlotDetails> possiblePlotCharacters = new List<PlotDetails>();

        //must find suiter must be aggressive
        List<int> possibleSuitors = AgentManager.Instance.GetAgentIdsWithTraits( Trait.Aggressiveness, 5, true );

        for ( int i = 0; i < possibleSuitors.Count; i++ )
        {
            int currSuitorId = possibleSuitors[i];
            CharacterDetails currSuitor = AgentManager.Instance.GetAgent( currSuitorId );

            //suitor must be sad
            StateVector currSuitorSV = currSuitor.GetStateVector();
            if ( !currSuitorSV.IsStateValueBelowThreshold( State.Mood ) )
                continue;

            //suitor must be attracted to love interest
            RelationshipManager currSuitorRM = currSuitor.GetRelationshipManager();
            List<int> possibleLoveInterests = currSuitorRM.GetAgentIdsWithRelationshipTraits( RelationshipTrait.Attraction, 5.0f, true );
            if ( possibleLoveInterests.Count == 0 )
                continue;

            for ( int j = 0; j < possibleLoveInterests.Count; j++ )
            {
                int currLoveInterestId = possibleLoveInterests[j];
                //love interest must have spouse
                CharacterDetails currLoveInterest = AgentManager.Instance.GetAgent( currLoveInterestId );
                RelationshipManager currLoveInterestRM = currLoveInterest.GetRelationshipManager();
                if ( !currLoveInterestRM.HasSpouse() )
                    continue;

                //suitor must know spouse, and not like them
                int possibleVictem = currLoveInterestRM.GetSpouseId();
                RelationshipVector suitorVictemRelationship = currSuitorRM.GetRelationshipById( possibleVictem );
                if ( suitorVictemRelationship == null )
                    continue;

                float suitorVictemFriendliness = suitorVictemRelationship.GetTraitValue( RelationshipTrait.Friendship );
                if( suitorVictemFriendliness >= 0.0f )
                    continue;


                //fulfills all constraints, add to possibles
                possiblePlotCharacters.Add( new PlotDetails( currSuitorId, currLoveInterestId, possibleVictem ) );
            }
        }
        if ( possiblePlotCharacters.Count == 0 )
            return false;

        int randomInt = Random.Range( 0, possiblePlotCharacters.Count );
        _plotDetails = possiblePlotCharacters[randomInt];

        return true;
    }
}
