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

    public Dictionary<System.Guid, QuestPattern> _activeQuests;
    private int _numAvailableQuests = 0;

    void Awake()
    {
        Instance = this;
        _activeQuests = new Dictionary<System.Guid, QuestPattern>();
        _numAvailableQuests = 0;
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

    public void AddQuestToQuestGiver( QuestPattern questPattern )
    {
        QuestGiver questGiver = GetQuestGiverComponentByAgentId( questPattern.GetQuestGiverId() );
        questGiver.SetQuest( questPattern );
        _numAvailableQuests++;
    }

    public void QuestRemovedFromQuestGiver()
    {
        _numAvailableQuests--;
    }

    public void AddActiveQuest(QuestPattern questPattern)
    {
        System.Guid uniqueId = System.Guid.NewGuid();
        _activeQuests.Add(uniqueId, questPattern);

        QuestUIController.Instance.AddQuest(uniqueId);

        if (questPattern.AttemptImmediateCompletion())
        {
            if (questPattern.IsComplete())
                CompleteQuest(uniqueId);

            QuestUIController.Instance.UpdateAllPages();
        }
    }

    public int GetNumActiveQuests()
    {
        return _activeQuests.Count;
    }
    public int GetNumAvailableQuests()
    {
        return _numAvailableQuests;
    }

    public QuestPattern GetQuestById(System.Guid questId)
    {
        QuestPattern questChain = null;
        _activeQuests.TryGetValue(questId, out questChain);
        return questChain;
    }

    public void AttemptToComplete(QuestPointCompletionData data)
    {
        int numActiveQuests = _activeQuests.Count;

        List<KeyValuePair<System.Guid, QuestPattern>> activeQuestPairs = _activeQuests.ToList();

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
        int numActiveQuests = _activeQuests.Count;
        List<KeyValuePair<System.Guid, QuestPattern>> activeQuestPairs = _activeQuests.ToList();

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

        AgentManager.Instance.UpdateQuestGiversPossibility();
    }

    private void CompleteQuest( System.Guid questId )
    {
        QuestPattern questPattern = GetQuestById( questId );
        QuestGiver questGiver = GetQuestGiverComponentByAgentId( questPattern.GetQuestGiverId() );
        questGiver.CompleteWaitingQuest();

        RemoveActiveQuest( questId );
    }

    private void FailQuest( System.Guid questId )
    {
        QuestPattern questPattern = GetQuestById( questId );
        QuestGiver questGiver = GetQuestGiverComponentByAgentId( questPattern.GetQuestGiverId() );
        questGiver.FailWaitingQuest();

        RemoveActiveQuest( questId );
    }

    private void RemoveActiveQuest( System.Guid questId )
    {
        _activeQuests.Remove( questId );
    }

    private QuestGiver GetQuestGiverComponentByAgentId( int id )
    {
        CharacterDetails questGiverDetails = AgentManager.Instance.GetAgent( id );
        QuestGiver questGiver = questGiverDetails.gameObject.GetComponentInChildren<QuestGiver>();

        if ( questGiver == null )
            Debug.LogError( "ERROR: " + questGiverDetails.gameObject.name + " does not have an attached quest giver component" );

        return questGiver;
    }
  
}