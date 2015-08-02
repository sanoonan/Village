using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class QuestChain
{
    protected int questGiverId;

    protected string description = "";

    private List<QuestFragment> questFragments;
    private int numFragments;

    private int numCompletedFragments;

    public QuestChain(int id)
    {
        questGiverId = id;
        numFragments = 0;
        numCompletedFragments = 0;
        questFragments = new List<QuestFragment>();
    }

    public QuestChain(string agentName)
    {
        questGiverId = AgentManager.Instance.GetAgentIdByName(agentName);
        numFragments = 0;
        numCompletedFragments = 0;
        questFragments = new List<QuestFragment>();
    }

    public List<QuestFragment> getActiveFragments()
    {
        List<QuestFragment> fragments = new List<QuestFragment>();

        for (int i = 0; i < numFragments; i++)
            if (questFragments[i].isActive())
                fragments.Add(questFragments[i]);

        return fragments;
    }

    public void AddQuestFragment(QuestFragment fragment)
    {
        questFragments.Add(fragment);
        numFragments++;
    }

    public void AddActiveQuestFragment(QuestFragment fragment)
    {
        AddQuestFragment(fragment);
        questFragments[numFragments - 1].Activate();
    }

    public void AddLinearQuestFragment(QuestFragment fragment)
    {
        AddQuestFragment(fragment);

        for (int i = 0; i < numFragments - 1; i++)
            questFragments[numFragments - 1].AddPrerequisite(i);
    }



    public void AddQuestFragmentWithPrerequisite(QuestFragment fragment, QuestFragment prerequisite)
    {
        AddQuestFragment(fragment);

        int prerequisiteId = questFragments.IndexOf(prerequisite);

        AddPrerequisiteToQuestFragment(numFragments - 1, prerequisiteId);
    }

    public void AddPrerequisiteToQuestFragment(QuestFragment fragment, QuestFragment prerequisite)
    {
        int fragmentId = questFragments.IndexOf(fragment);
        int prerequisiteId = questFragments.IndexOf(prerequisite);

        AddPrerequisiteToQuestFragment(fragmentId, prerequisiteId);
    }

    public void AddPrerequisiteToQuestFragment(int fragmentId, int prerequisiteId)
    {
        questFragments[fragmentId].AddPrerequisite(prerequisiteId);
    }



    public bool AttemptToComplete(QuestFragmentCompletionData data)
    {
        bool questCompleted = false;

        for (int i = 0; i < numFragments; i++)
        {
            QuestFragment currFragment = questFragments[i];
            if(currFragment.isActive())
            {
                if (currFragment.AttemptToComplete(data))
                {
                    numCompletedFragments++;
                    questCompleted = true;

                    for (int j = 0; j < numFragments; j++)
                    {
                        if (i != j)
                            questFragments[j].AttemptToActivate(i);
                    }
                }
            }
        }

        return questCompleted;
    }

    public bool AttemptImmediateCompletion()
    {
        bool anyQuestCompleted = false;

        for (int i = 0; i < numFragments; i++)
        {
            QuestFragment currFragment = questFragments[i];
            if (currFragment.isActive())
            {
                if (currFragment.AttemptImmediateCompletion())
                {
                    numCompletedFragments++;
                    anyQuestCompleted = true;

                    for (int j = 0; j < numFragments; j++)
                    {
                        if (i != j)
                            questFragments[j].AttemptToActivate(i);
                    }
                }
            }
        }

        return anyQuestCompleted;
    }

    public bool IsComplete()
    {
        if (numCompletedFragments == numFragments)
            return true;

        return false;
    }

    public string getDescription()
    {
        if (description == "")
            setChainDescription();

        return description;
    }

    public abstract void setChainDescription();
}