using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PF_AFriendlyGift : PlotFragment
{
    private string[] _possibleGifts = new string[] 
        {
            "hammer",
            "Food",
            "Rod",
        };

    private struct PlotDetails
    {
        public int giverNpc;
        public int receiverNpc;
        public string giftItem;

        public PlotDetails( int giver, int receiver, string gift )
        {
            giverNpc = giver;
            receiverNpc = receiver;
            giftItem = gift;
        }
    }

    private PlotDetails _plotDetails;

    public PF_AFriendlyGift()
        : base()
    {
        _label = "A Friendly Gift";
    }

    protected override void SetAuthorGoals()
    {
        _authorGoals.Add( AuthorGoal.GOOD );
    }

    protected override void SetQuestPattern()
    {
        _questPattern = new QPa_GiveItemToNpc( _plotDetails.giverNpc, _plotDetails.giftItem, _plotDetails.receiverNpc );
        _questPattern.AddLinearQuestPoint( new QPo_TalkToNPC( _plotDetails.giverNpc ) );
    }



    public override bool AreConstraintsFulfilled()
    {
        List<PlotDetails> possiblePlotDetails = new List<PlotDetails>();

        //giver must be nice
        List<int> possibleGivers = AgentManager.Instance.GetAgentIdsWithTraits( Trait.Niceness, 5, true );

        for ( int i = 0; i < possibleGivers.Count; i++ )
        {
            //giver must like receiver
            int currGiverId = possibleGivers[i];
            CharacterDetails currGiver = AgentManager.Instance.GetAgent( currGiverId );
            RelationshipManager currGiverRM = currGiver.GetRelationshipManager();
            List<int> possibleReceivers = currGiverRM.GetAgentIdsWithRelationshipTraits( RelationshipTrait.Friendship, 5.0f, true );
            if ( possibleReceivers.Count == 0 )
                continue;

            for ( int j = 0; j < possibleReceivers.Count; j++ )
            {
                //receiver must be sad
                int currReceiverId = possibleReceivers[j];
                CharacterDetails currReceiver = AgentManager.Instance.GetAgent( currReceiverId );
                StateVector currReceiverSV = currReceiver.GetStateVector();
                if( !currReceiverSV.IsStateValueBelowThreshold( State.Mood ) )
                    continue;

                string randomItem = GetRandomItem();

                //fulfills all constraints, add to possibles
                possiblePlotDetails.Add( new PlotDetails( currGiverId, currReceiverId, randomItem ) );
            }
        }
        if ( possiblePlotDetails.Count == 0 )
            return false;

        int randomInt = Random.Range( 0, possiblePlotDetails.Count );
        _plotDetails = possiblePlotDetails[randomInt];

        return true;
    }

    private string GetRandomItem()
    {
        int numItems = _possibleGifts.Length;
        int randomInt = Random.Range( 0, numItems );
        return _possibleGifts[randomInt];
    }

    public override void PrintPlotDetails()
    {
        Debug.Log( "giver - " + AgentManager.Instance.GetAgentNameById( _plotDetails.giverNpc ) );
        Debug.Log( "receiver - " + AgentManager.Instance.GetAgentNameById( _plotDetails.receiverNpc ) );
        Debug.Log( "gift - " + _plotDetails.giftItem );
    }
}

