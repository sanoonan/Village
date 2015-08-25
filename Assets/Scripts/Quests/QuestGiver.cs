using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestGiver : MonoBehaviour
{
    private bool _hasQuest = false;
    private bool _isWaitingForQuest = false;

    private QuestPattern _questChain = null;
    private MeshRenderer _star;

    private CharacterDetails _characterDetails;

    void Awake()
    {
        _star = gameObject.GetComponent<MeshRenderer>();
        HideStar();

        _characterDetails = gameObject.GetComponentInParent<CharacterDetails>();
    }

    public void UpdateQuestPossibility()
    {
        if ( !HasQuest() )
            return;

        if ( ( !_characterDetails.IsAlive() ) || ( !_questChain.IsPossible() ) )
            FailQuestBeforeStarted();
    }


    public QuestPattern GetQuest()
    {
        if( !HasQuest() )
            return null;

        QuestManager.Instance.QuestRemovedFromQuestGiver();

        _isWaitingForQuest = true;
        _hasQuest = false;
        HideStar();
        return _questChain;
    }
    public void SetQuest( QuestPattern questChain )
    {
        if ( ( HasQuest() ) || ( IsWaitingForQuest() ) )
            return;

        _hasQuest = true;
        _questChain = questChain;
        ShowStar();

        UpdateQuestPossibility();
    }
    public bool HasQuest()
    {
        return _hasQuest;
    }
    public bool IsWaitingForQuest()
    {
        return _isWaitingForQuest;
    }
    public void FailQuestBeforeStarted()
    {
        QuestManager.Instance.QuestRemovedFromQuestGiver();

        _isWaitingForQuest = false;
        _hasQuest = false;
        HideStar();
    }

    public void FailWaitingQuest()
    {
        _isWaitingForQuest = false;
    }
    public void CompleteWaitingQuest()
    {
        _isWaitingForQuest = false;
    }

    private void ShowStar()
    {
        _star.enabled = true;
    }
    private void HideStar()
    {
        _star.enabled = false;
    }
}