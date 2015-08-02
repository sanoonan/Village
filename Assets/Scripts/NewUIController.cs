using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NewUIController : MonoBehaviour
{
    public static NewUIController Instance;
    public GameObject _itemButtonsObject;

    void Awake()
    {
        Instance = this;
    }

	// Use this for initialization
	void Start ()
    {
        DisableItemButtons();
	}
	
	// Update is called once per frame
	void Update ()
    {
	}

    public void GiveItemButtonCallback()
    {
        PlayerController.Instance.TransferResponse(true);
        DisableItemButtons();
    }

    public void CancelItemButtonCallback()
    {
        PlayerController.Instance.TransferResponse(false);
        DisableItemButtons();
    }


    public void InitItemTransferUI( string NPCname, string itemName )
    {
        EnableItemButtons(NPCname, itemName);
    }

    private void EnableItemButtons( string NPCname, string itemName )
    {
        GameObject giveItemButtonObject = _itemButtonsObject.transform.FindChild("GiveItemButton").gameObject;
        GameObject giveItemButtonTextObject = giveItemButtonObject.transform.FindChild("Text").gameObject;
        Text giveItemButtonText = giveItemButtonTextObject.GetComponent<Text>();
        giveItemButtonText.text = "Give " + itemName + " to " + NPCname + " ?";

        _itemButtonsObject.SetActive(true);
    }

    private void DisableItemButtons()
    {
        GameObject itemButtonsObject = GameObject.Find("ItemButtons");
        itemButtonsObject.SetActive(false);
    }
}
