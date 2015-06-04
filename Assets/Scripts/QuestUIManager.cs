using UnityEngine;
using System.Collections;

public class QuestUIManager : MonoBehaviour
{
    public UISprite sQuest;
    public UILabel lQuest;
    public GameObject pQuest;

    public void EnableQuest(string questName)
    {
        lQuest.text = questName;
        pQuest.SetActive(true);
    }

    public void DisableQuest()
    {
        pQuest.SetActive(false);
    }

	void Awake() 
    {
        DisableQuest();
	}
}
