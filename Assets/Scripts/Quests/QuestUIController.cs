using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class QuestUIController : MonoBehaviour
{
    public static QuestUIController Instance;

    public GameObject openQuestLogButton;
    public GameObject closeQuestLogButton;

    public GameObject questLogPanel;

    public GameObject addSampleQuestButton;

    public GameObject questChainTextPrefab;
    public GameObject questFragmentTextPrefab;

    private List<GameObject> questFragmentTextObjects;

    public GameObject questUIPanel;

    private List<QuestUIPage> questPages;
    private int numPages;
    private int currentPage;

    void Awake()
    {
        Instance = this;
        questPages = new List<QuestUIPage>();
        numPages = 0;
        currentPage = -1;

        questFragmentTextObjects = new List<GameObject>();
        questFragmentTextObjects.Add(questFragmentTextPrefab);
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
        openQuestLogButton.SetActive(false);
        closeQuestLogButton.SetActive(true);
        questLogPanel.SetActive(true);
    }

    public void CloseQuestLog()
    {
        openQuestLogButton.SetActive(true);
        closeQuestLogButton.SetActive(false);
        questLogPanel.SetActive(false);
    }

    public void NextQuestInLog()
    {
        currentPage++;

        if (currentPage >= numPages)
            currentPage = 0;

        SetCurrentPage(currentPage);
    }

    public void PrevQuestInLog()
    {
        currentPage--;

        if (currentPage < 0)
            currentPage = numPages - 1;

        SetCurrentPage(currentPage);
    }


    public void AddQuest(System.Guid questId)
    {
        questPages.Add(new QuestUIPage(questId));
        numPages++;

        if ( currentPage < 0 )
        {
            currentPage = 0;
            SetCurrentPage(currentPage);
        }
    }

    private void SetCurrentPage(int pageNum)
    {
        if (pageNum > numPages)
        {
            Debug.LogError("Cannot get page number " + pageNum + ", there are only " + numPages + " pages");
            return;
        }

		if(currentPage < 0 )
		{
			SetNoActiveQuestsPage();
			return;
		}

        currentPage = pageNum;

        Text questChainText = questChainTextPrefab.GetComponent<Text>();
        questChainText.text = questPages[pageNum].GetQuestChainText();

        int numFrags = questPages[pageNum].GetNumQuestFragmentTexts();

        
        while (numFrags < questFragmentTextObjects.Count)
        {
            GameObject newQuestFragmentTextObject = GameObject.Instantiate(questFragmentTextObjects[questFragmentTextObjects.Count - 1]);
            newQuestFragmentTextObject.transform.localPosition += new Vector3(0.0f, -20.0f, 0.0f);
            questFragmentTextObjects.Add(newQuestFragmentTextObject);
        }

        for (int i = 0; i < questFragmentTextObjects.Count; i++)
        {
            if (i < numFrags)
            {
                questFragmentTextObjects[i].SetActive(true);

                Text questFragmentText = questFragmentTextObjects[i].GetComponent<Text>();
                questFragmentText.text = questPages[pageNum].GetQuestFragmentByNum(i);
            }
            else
                questFragmentTextObjects[i].SetActive(false);
        }
    }

    private void SetNoActiveQuestsPage()
    {
        Text questChainText = questChainTextPrefab.GetComponent<Text>();
        questChainText.text = "No Active Quests";

        for (int i = 0; i < questFragmentTextObjects.Count; i++)
            questFragmentTextObjects[i].SetActive(false);
    }

    public void AddSampleQuest()
    {
        QuestManager.Instance.AddActiveQuest(new QC_KillNpcAndReportToQuestGiver("Senel", "Lloyd"));
    }

    public void UpdateAllPages()
    {
        for (int i = 0; i < numPages; i++)
            questPages[i].Update();

        SetCurrentPage(currentPage);
    }

    public void RemoveQuestPage(QuestUIPage page)
    {
		int removedPageNum = questPages.IndexOf( page );
        questPages.Remove(page);
		if( currentPage == removedPageNum )
			currentPage--;
    }
}
