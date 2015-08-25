using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class PlotFragment
{
    protected List<AuthorGoal> _authorGoals;
    protected QuestPattern _questPattern;

    public PlotFragment()
    {
        _authorGoals = new List<AuthorGoal>();

        SetAuthorGoals();
    }

    public bool HasAuthorGoal( AuthorGoal goal )
    {
        return _authorGoals.Contains( goal );
    }
    public QuestPattern GetQuest()
    {
        SetQuestPattern();
        return _questPattern;
    }

    protected abstract void SetAuthorGoals();

    protected abstract void SetQuestPattern();

    public abstract bool AreConstraintsFulfilled();
}

