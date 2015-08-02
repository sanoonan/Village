using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class QuestUIPage
{
    private System.Guid questChainId;
    private string questChainText;
    private List<string> questFragmentsText;
    private int numFragmentTexts;

    public QuestUIPage(System.Guid questId)
    {
        questChainId = questId;
        Update();
    }

    public void Update()
    {
        QuestChain questChain = QuestManager.Instance.GetQuestById(questChainId);
        if (questChain == null)
        {
            Debug.Log("Quest chain does not exist");
            QuestUIController.Instance.RemoveQuestPage(this);
            return;
        }

        questChainText = questChain.getDescription();

        List<QuestFragment> activeQuestFragments = questChain.getActiveFragments();
        numFragmentTexts = activeQuestFragments.Count;

        questFragmentsText = new List<string>();
        for (int i = 0; i < numFragmentTexts; i++)
        {
            questFragmentsText.Add(activeQuestFragments[i].getFragmentDescription());
        }
    }

    public string GetQuestChainText()
    {
        return questChainText;
    }
    public int GetNumQuestFragmentTexts()
    {
        return numFragmentTexts;
    }
    public string GetQuestFragmentByNum(int fragNum)
    {
        return questFragmentsText[fragNum];
    }
}
