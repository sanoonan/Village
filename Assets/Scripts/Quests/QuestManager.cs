using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum QuestAction
{
    TALK_TO_NPC,
    ACQUIRE_ITEM,
    GIVE_ITEM_TO_NPC,
    KILL_NPC,
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;   

    public Dictionary<System.Guid, QuestChain> activeQuests;


    void Awake()
    {
        Instance = this;
        activeQuests = new Dictionary<System.Guid, QuestChain>();
    }

    void Start()
    {
    }

    void Update()
    {
    }

    void LateUpdate()
    {
    }

    public void AddActiveQuest(QuestChain questChain)
    {
        System.Guid uniqueId = System.Guid.NewGuid();
        activeQuests.Add(uniqueId, questChain);

        QuestUIController.Instance.AddQuest(uniqueId);

        if (questChain.AttemptImmediateCompletion())
        {
            if (questChain.IsComplete())
                CompleteQuest(uniqueId);

            QuestUIController.Instance.UpdateAllPages();
        }
    }

    public QuestChain GetQuestById(System.Guid questId)
    {
        QuestChain questChain = null;
        activeQuests.TryGetValue(questId, out questChain);
        return questChain;
    }

    public void AttemptToComplete(QuestFragmentCompletionData data)
    {
        int numActiveQuests = activeQuests.Count;

        List<KeyValuePair<System.Guid, QuestChain>> activeQuestPairs = activeQuests.ToList();

        bool anyQuestCompleted = false;

        for (int i = 0; i < numActiveQuests; i++)
        {
            if(activeQuestPairs[i].Value.AttemptToComplete(data)) 
            {
                anyQuestCompleted = true;

                if (activeQuestPairs[i].Value.IsComplete())
                    CompleteQuest(activeQuestPairs[i].Key);
            }                
        }
        if(anyQuestCompleted)
            QuestUIController.Instance.UpdateAllPages();

        RemoveImpossibleQuests();
    }

    private void RemoveImpossibleQuests()
    {
        int numActiveQuests = activeQuests.Count;
        List<KeyValuePair<System.Guid, QuestChain>> activeQuestPairs = activeQuests.ToList();

        bool anyQuestImpossible = false;
        for ( int i = 0; i < numActiveQuests; i++ )
        {
            if ( !activeQuestPairs[i].Value.IsPossible() )
            {
                anyQuestImpossible = true;
                FailQuest( activeQuestPairs[i].Key );
            }
        }

        if ( anyQuestImpossible )
            QuestUIController.Instance.UpdateAllPages();
    }

    private void CompleteQuest( System.Guid questId )
    {
        activeQuests.Remove(questId);
    }

    private void FailQuest( System.Guid questId )
    {
        activeQuests.Remove( questId );
    }


  

   
}