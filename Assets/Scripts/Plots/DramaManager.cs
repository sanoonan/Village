using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AuthorGoal
{
    VIOLENCE = 0,
    DEATH,
    EVIL,
    GOOD,
}

public class DramaManager : MonoBehaviour 
{
    public float _updateFrequency = 60.0f;
    private float _timeSinceUpdate = 0.0f;

    private List<PlotFragment> _availablePlotFragments;
    private List<AuthorGoal> _availableAuthorGoals;

    void Awake()
    {
        SetupPlotFragments();

        _availableAuthorGoals = ( System.Enum.GetValues( typeof( AuthorGoal ) ) ).OfType<AuthorGoal>().ToList(); ;
    }

    void Update()
    {
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

  //      _availablePlotFragments.Add( new PF_TESTPlotFragment() );
    }

    private void UpdateDrama()
    {
        int randomInt = Random.Range( 0, _availableAuthorGoals.Count );
        AuthorGoal currentAuthorGoal = _availableAuthorGoals[randomInt];

        List<PlotFragment> possiblePlotFragments = GetPlotFragmentsWithAuthorGoal( currentAuthorGoal );
        if ( possiblePlotFragments.Count == 0 )
            return;

        PlotFragment selectedPlotFragment = GetPlotFragmentsWithConstraintsFulfilled( possiblePlotFragments );
        if ( selectedPlotFragment == null )
            return;

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

    private PlotFragment GetPlotFragmentsWithConstraintsFulfilled( List<PlotFragment> plots )
    {
        List<PlotFragment> suitablePlotFragments = new List<PlotFragment>();
        for ( int i = 0; i < plots.Count; i++ )
        {
            if ( plots[i].AreConstraintsFulfilled() )
                suitablePlotFragments.Add( plots[i] );
        }
        if ( suitablePlotFragments.Count == 0 )
            return null;

        int randomInt = Random.Range( 0, suitablePlotFragments.Count );
        return suitablePlotFragments[randomInt];
    }
}
