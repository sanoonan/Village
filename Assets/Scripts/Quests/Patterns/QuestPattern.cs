using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class QuestPattern
{
    protected int _questGiverId;

    protected string _description = "";

    private List<QuestPoint> _questPoints;
    private int _numPoints;

    private int _numCompletedPoints;

    public QuestPattern(int id)
    {
        _questGiverId = id;
        _numPoints = 0;
        _numCompletedPoints = 0;
        _questPoints = new List<QuestPoint>();
    }

    public QuestPattern(string agentName)
    {
        _questGiverId = AgentManager.Instance.GetAgentIdByName(agentName);
        _numPoints = 0;
        _numCompletedPoints = 0;
        _questPoints = new List<QuestPoint>();
    }

    public List<QuestPoint> GetActivePoints()
    {
        List<QuestPoint> points = new List<QuestPoint>();

        for (int i = 0; i < _numPoints; i++)
            if (_questPoints[i].IsActive())
                points.Add( _questPoints[i] );

        return points;
    }

    public void AddQuestPoint(QuestPoint point )
    {
        _questPoints.Add( point );
        _numPoints++;
    }

    public void AddActiveQuestPoint( QuestPoint point )
    {
        AddQuestPoint( point );
        _questPoints[_numPoints - 1].Activate();
    }

    public void AddLinearQuestPoint(QuestPoint point)
    {
        AddQuestPoint( point );

        for (int i = 0; i < _numPoints - 1; i++)
            _questPoints[_numPoints - 1].AddEnabler(i);
    }



    public void AddQuestPointWithEnabler( QuestPoint point, QuestPoint enabler )
    {
        AddQuestPoint( point );

        int enablerId = _questPoints.IndexOf( enabler );

        AddEnablerToQuestPointByOrdering( _numPoints - 1, enablerId );
    }

    public void AddEnablerToQuestPoint(QuestPoint point, QuestPoint enabler)
    {
        int pointId = _questPoints.IndexOf( point );
        int enablerId = _questPoints.IndexOf( enabler );

        AddEnablerToQuestPointByOrdering( pointId, enablerId );
    }

    public void AddEnablerToQuestPointByOrdering( int pointId, int enablerId )
    {
        _questPoints[pointId].AddEnabler( enablerId );
    }



    public bool AttemptToComplete(QuestPointCompletionData data)
    {
        bool questCompleted = false;

        for (int i = 0; i < _numPoints; i++)
        {
            QuestPoint currFragment = _questPoints[i];
            if(currFragment.IsActive())
            {
                if (currFragment.AttemptToComplete(data))
                {
                    _numCompletedPoints++;
                    questCompleted = true;

                    for (int j = 0; j < _numPoints; j++)
                    {
                        if (i != j)
                            _questPoints[j].AttemptToActivate(i);
                    }
                }
            }
        }

        return questCompleted;
    }

    public bool AttemptImmediateCompletion()
    {
        bool anyQuestCompleted = false;

        for (int i = 0; i < _numPoints; i++)
        {
            QuestPoint currFragment = _questPoints[i];
            if (currFragment.IsActive())
            {
                if (currFragment.AttemptImmediateCompletion())
                {
                    _numCompletedPoints++;
                    anyQuestCompleted = true;

                    for (int j = 0; j < _numPoints; j++)
                    {
                        if (i != j)
                            _questPoints[j].AttemptToActivate(i);
                    }
                }
            }
        }

        return anyQuestCompleted;
    }

    public bool IsPossible()
    {
        for ( int i = 0; i < _numPoints; i++ )
        {
            if ( !_questPoints[i].IsComplete() )
            {
                if ( !_questPoints[i].IsPossible() )
                    return false;
            }
        }
        return true;
    }

    public bool IsComplete()
    {
        if (_numCompletedPoints == _numPoints)
            return true;

        return false;
    }

    public string GetDescription()
    {
        if (_description == "")
            SetPatternDescription();

        return _description;
    }

    public abstract void SetPatternDescription();

    public void SetPatternDescription( string description )
    {
        _description = description;
    }

    public int GetQuestGiverId()
    {
        return _questGiverId;
    }
}