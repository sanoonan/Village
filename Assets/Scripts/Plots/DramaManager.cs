using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum AuthorGoal
{
    VIOLENCE = 0,
    DEATH,
    EVIL,
    GOOD,
}

public class DramaManager : MonoBehaviour 
{
    public bool _isTestingQuests = false;
    public bool _isTestingDrama = false;

    public float _updateFrequency = 60.0f;
    private float _timeSinceUpdate = 0.0f;

    private List<PlotFragment> _availablePlotFragments;
    private List<AuthorGoal> _availableAuthorGoals;

    void Awake()
    {
        if( _isTestingQuests )
        {
            TESTSetupPlotFragments();
        }
        else
        {   
            SetupPlotFragments();
        }


        _availableAuthorGoals = ( System.Enum.GetValues( typeof( AuthorGoal ) ) ).OfType<AuthorGoal>().ToList(); ;
    }

    void Update()
    {
        if( _isTestingDrama )
            return;

        if ( _timeSinceUpdate >= _updateFrequency )
        {
            UpdateDrama();
            _timeSinceUpdate = 0.0f;
        }
        else
        {
            _timeSinceUpdate += Time.deltaTime;
        }
    }

    private void SetupPlotFragments()
    {
        _availablePlotFragments = new List<PlotFragment>();

        _availablePlotFragments.Add( new PF_JealousSuitor() );
        _availablePlotFragments.Add( new PF_AFriendlyGift() );
    }

    private void TESTSetupPlotFragments()
    {
        _availablePlotFragments = new List<PlotFragment>();
        
       _availablePlotFragments.Add( new PF_TESTPlotFragment() );
    }

    private void UpdateDrama()
    {
        AuthorGoal currentAuthorGoal = GetRandomFromList<AuthorGoal>( _availableAuthorGoals );

        List<PlotFragment> possiblePlotFragments = GetPlotFragmentsWithAuthorGoal( currentAuthorGoal );
        if ( possiblePlotFragments.Count == 0 )
            return;

        List<PlotFragment> satisfiedPlotFragments = GetPlotFragmentsWithConstraintsFulfilled( possiblePlotFragments );
        if ( satisfiedPlotFragments.Count == 0 )
            return;

        PlotFragment selectedPlotFragment = GetRandomFromList<PlotFragment>( satisfiedPlotFragments );

        QuestPattern selectedQuestPattern = selectedPlotFragment.GetQuest();
        QuestManager.Instance.AddQuestToQuestGiver( selectedQuestPattern );
    }

    private List<PlotFragment> GetPlotFragmentsWithAuthorGoal( AuthorGoal goal )
    {
        List<PlotFragment> suibtablePlotFragments = new List<PlotFragment>();
        for ( int i = 0; i < _availablePlotFragments.Count; i++ )
        {
            if ( _availablePlotFragments[i].HasAuthorGoal( goal ) )
                suibtablePlotFragments.Add( _availablePlotFragments[i] );
        }
        return suibtablePlotFragments;
    }

    private List<PlotFragment> GetPlotFragmentsWithConstraintsFulfilled( List<PlotFragment> plots )
    {
        List<PlotFragment> suitablePlotFragments = new List<PlotFragment>();
        for ( int i = 0; i < plots.Count; i++ )
        {
            if ( plots[i].AreConstraintsFulfilled() )
                suitablePlotFragments.Add( plots[i] );
        }

        return suitablePlotFragments;
    }


    public void UpdateDramaWithDebugMessages()
    {
        Debug.Break();
        TESTPrintAuthorGoals();

        string startingString = "Available plot fragments: ";
        TESTPrintPlotFragmentsLabels( startingString, _availablePlotFragments );

        AuthorGoal currentAuthorGoal = GetRandomFromList<AuthorGoal>( _availableAuthorGoals );

        Debug.Log( "Random selected author goal: " + currentAuthorGoal.ToString());
        Debug.Log( "Checking plot fragments to find matching author goal" );
        
        List<PlotFragment> possiblePlotFragments = GetPlotFragmentsWithAuthorGoal( currentAuthorGoal );
        if ( possiblePlotFragments.Count == 0 )
        {
            Debug.Log( "There are no plot fragments with match author goal" );
            return;
        }

        startingString = "The plot fragments that satisfy the author goal are: ";
        TESTPrintPlotFragmentsLabels( startingString, possiblePlotFragments );
   
        
        List<PlotFragment> satisfiedPlotFragments = GetPlotFragmentsWithConstraintsFulfilled( possiblePlotFragments );
        if ( satisfiedPlotFragments.Count == 0 )
        {
            Debug.Log( "None of the constraints were met" );
            return;
        }

        startingString = "The plot fragments that have the constraints met are: ";
        TESTPrintPlotFragmentsLabels( startingString, satisfiedPlotFragments  );

        PlotFragment selectedPlotFragment = GetRandomFromList<PlotFragment>( satisfiedPlotFragments );

        Debug.Log( "The plot fragment that was selected is : " + selectedPlotFragment._label);

        selectedPlotFragment.PrintPlotDetails();

        QuestPattern selectedQuestPattern = selectedPlotFragment.GetQuest();
        QuestManager.Instance.AddQuestToQuestGiver( selectedQuestPattern );
    }

    private void TESTPrintPlotFragmentsLabels( string startingString, List<PlotFragment> plotFragments )
    {
        string debugString = startingString;
        for( int i=0; i<plotFragments.Count; i++ )
        {
            debugString += plotFragments[i]._label + " , ";
        }
        Debug.Log( debugString );
    }

    private T GetRandomFromList<T>( List<T> list )
    {
        int randomNum = Random.Range( 0, list.Count );
        return list[randomNum];
    }

    private void TESTPrintAuthorGoals()
    {
        string debugString = "Available author goals: ";
        for( int i=0; i<_availableAuthorGoals.Count; i++ )
        {
            debugString += _availableAuthorGoals[i].ToString() + " , ";
        }
        Debug.Log( debugString );
    }
}
