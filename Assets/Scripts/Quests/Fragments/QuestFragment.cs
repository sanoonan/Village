using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class QuestFragment
{
    protected bool active;
    protected bool completed;

    private bool prerequisites;
    private List<int> prerequisiteFragments;

    protected string description;

    protected QuestAction questAction;

    public QuestFragment()
    {
        active = false;
        completed = false;

        prerequisites = false;
        prerequisiteFragments = new List<int>();

        SetQuestAction();
    }

    protected abstract void SetQuestAction();

    public abstract bool AttemptImmediateCompletion();

    public void Activate()
    {
        active = true;
    }

    public void Complete()
    {
        completed = true;
        active = false;
    }


    #region GETTERS

    public bool isActive()
    {
        return active;
    }

    public bool isCompleted()
    {
        return completed;
    }

    public bool hasPrerequisites()
    {
        return prerequisites;
    }


    #endregion

    public void AttemptToActivate(int fragmentId)
    {
        if ((active) || (completed) || (!prerequisites))
            return;

        if (prerequisiteFragments.Contains(fragmentId))
        {
            prerequisiteFragments.Remove(fragmentId);
            if (prerequisiteFragments.Count <= 0)
            {
                prerequisites = false;
                Activate();
            }
        }
    }

    public void AddPrerequisite(int fragmentId)
    {
        prerequisites = true;

        if (!prerequisiteFragments.Contains(fragmentId))
            prerequisiteFragments.Add(fragmentId);
    }

    public abstract bool AttemptToComplete(QuestFragmentCompletionData data);



    public string getFragmentDescription()
    {
        if((description == "")||(description==null))
            setFragmentDescription();

        return description;
    }

    public abstract void setFragmentDescription();
    
}




public abstract class QuestFragmentCompletionData
{
    public QuestAction questAction;

    public QuestFragmentCompletionData()
    {
        SetQuestAction();
    }

    protected abstract void SetQuestAction();
}