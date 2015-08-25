using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class QuestUIController : MonoBehaviour
{
    public static QuestUIController Instance;

    public GameObject _openQuestLogButton;
    public GameObject _closeQuestLogButton;

    public GameObject _questLogPanel;

    public GameObject _addSampleQuestButton;

    public GameObject _questPatternTextPrefab;
    public GameObject _questPointTextPrefab;

    private List<GameObject> _questPointTextObjects;


    private List<QuestUIPage> _questPages;
    private int _numPages;
    private int _currentPage;

    private bool _noActiveQuests = true;

    void Awake()
    {
        Instance = this;
        _questPages = new List<QuestUIPage>();
        _numPages = 0;
        _currentPage = -1;

        _questPointTextObjects = new List<GameObject>();
        _questPointTextObjects.Add(_questPointTextPrefab);
    }


	void Start ()
    {
        CloseQuestLog();
        SetNoActiveQuestsPage();
	}
	

	void Update ()
    {
	}



    public void OpenQuestLog()
    {
        _openQuestLogButton.SetActive(false);
        _closeQuestLogButton.SetActive(true);
        _questLogPanel.SetActive(true);
    }

    public void CloseQuestLog()
    {
        _openQuestLogButton.SetActive(true);
        _closeQuestLogButton.SetActive(false);
        _questLogPanel.SetActive(false);
    }

    public void NextQuestInLog()
    {
        if ( _noActiveQuests )
        {
            _currentPage = -1;
        }
        else
        {
            _currentPage++;

            if ( _currentPage >= _numPages )
                _currentPage = 0;
        }
        
        SetCurrentPage(_currentPage);
    }

    public void PrevQuestInLog()
    {
        _currentPage--;

        if (_currentPage < 0)
            _currentPage = _numPages - 1;

        SetCurrentPage(_currentPage);
    }


    public void AddQuest(System.Guid questId)
    {
        _questPages.Add(new QuestUIPage(questId));
        _numPages++;

        _noActiveQuests = false;

        if ( _currentPage < 0 )
        {
            _currentPage = 0;
            SetCurrentPage(_currentPage);
        }
    }

    private void SetCurrentPage(int pageNum)
    {
        if (pageNum > _numPages)
        {
            Debug.LogError("Cannot get page number " + pageNum + ", there are only " + _numPages + " pages");
            return;
        }

		if( _noActiveQuests )
		{
			SetNoActiveQuestsPage();
			return;
		}

        _currentPage = pageNum;

        Text questChainText = _questPatternTextPrefab.GetComponent<Text>();
        questChainText.text = _questPages[pageNum].GetQuestChainText();

        int numFrags = _questPages[pageNum].GetNumQuestFragmentTexts();

        
        while (numFrags < _questPointTextObjects.Count)
        {
            GameObject newQuestFragmentTextObject = GameObject.Instantiate(_questPointTextObjects[_questPointTextObjects.Count - 1]);
            newQuestFragmentTextObject.transform.localPosition += new Vector3(0.0f, -20.0f, 0.0f);
            _questPointTextObjects.Add(newQuestFragmentTextObject);
        }

        for (int i = 0; i < _questPointTextObjects.Count; i++)
        {
            if (i < numFrags)
            {
                _questPointTextObjects[i].SetActive(true);

                Text questFragmentText = _questPointTextObjects[i].GetComponent<Text>();
                questFragmentText.text = _questPages[pageNum].GetQuestFragmentByNum(i);
            }
            else
                _questPointTextObjects[i].SetActive(false);
        }
    }

    private void SetNoActiveQuestsPage()
    {
        Text questChainText = _questPatternTextPrefab.GetComponent<Text>();
        questChainText.text = "No Active Quests";

        for (int i = 0; i < _questPointTextObjects.Count; i++)
            _questPointTextObjects[i].SetActive(false);
    }

    public void AddSampleQuest()
    {
        QuestManager.Instance.AddQuestToQuestGiver(new QPa_KillNpcAndReportToNpc("Senel", "Lloyd", "Senel" ));
    }

    public void UpdateAllPages()
    {
        for (int i = 0; i < _numPages; i++)
            _questPages[i].UpdatePage();

        SetCurrentPage(_currentPage);
    }

    public void RemoveQuestPage(QuestUIPage page)
    {
		int removedPageNum = _questPages.IndexOf( page );
        _questPages.Remove(page);

        _numPages--;

		if( _currentPage == removedPageNum )
			_currentPage--;

        if ( _currentPage < 0 )
            _noActiveQuests = true;
    }
}
