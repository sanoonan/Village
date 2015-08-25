using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class QuestPoint
{
    protected bool _active;
    protected bool _completed;

    private List<int> _enablers;

    protected string _description;

    protected QuestAction _questAction;

    public QuestPoint()
    {
        _active = false;
        _completed = false;

        _enablers = new List<int>();

        SetQuestAction();
    }

    protected abstract void SetQuestAction();

    public abstract bool AttemptImmediateCompletion();
    public abstract bool IsPossible();

    public void Activate()
    {
        _active = true;
    }

    public void Complete()
    {
        _completed = true;
        _active = false;
    }


    #region GETTERS
    public bool IsActive()
    {
        return _active;
    }
    public bool IsComplete()
    {
        return _completed;
    }
    #endregion

    public bool HasEnablers()
    {
        if ( _enablers.Count > 0 )
            return true;

        return false;
    }

    public void AttemptToActivate(int enablerId)
    {
        if ( (_active) || (_completed) || ( !HasEnablers() ))
            return;

        if ( _enablers.Contains( enablerId ) )
        {
            _enablers.Remove( enablerId );
            if ( _enablers.Count <= 0 )
            {
                if ( !HasEnablers() )
                {
                    Activate();
                }
            }
        }
    }

    public void AddEnabler(int fragmentId)
    {
        if ( !_enablers.Contains( fragmentId ) )
        {
            _enablers.Add( fragmentId );
        }
    }

    public abstract bool AttemptToComplete(QuestPointCompletionData data);



    public string GetFragmentDescription()
    {
        if((_description == "")||(_description==null))
            SetFragmentDescription();

        return _description;
    }

    public abstract void SetFragmentDescription();
    
}




public abstract class QuestPointCompletionData
{
    public QuestAction questAction;

    public QuestPointCompletionData()
    {
        SetQuestAction();
    }

    protected abstract void SetQuestAction();
}