using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NewUIController : MonoBehaviour
{
    public static NewUIController Instance;
    private GameObject _itemButtonsObject;
    private GameObject _conversationPanel;


    void Awake()
    {
        Instance = this;

        _itemButtonsObject = gameObject.transform.FindChild( "ItemButtons" ).gameObject;
        _conversationPanel = gameObject.transform.FindChild( "ConversationPanel" ).gameObject;
    }

	// Use this for initialization
	void Start ()
    {
        DisableAllUI();
	}
	
	// Update is called once per frame
	void Update ()
    {
	}


    public void ConversationButtonCallback()
    {
        PlayerController.Instance.ConversationResponse();
        DisableConversationPanel();
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

    public void InitConversationUI( string npcName )
    {
        EnableConversationPanel( npcName );
    }






    private void EnableItemButtons( string NPCname, string itemName )
    {
        GameObject giveItemButtonObject = _itemButtonsObject.transform.FindChild("GiveItemButton").gameObject;
        GameObject giveItemButtonTextObject = giveItemButtonObject.transform.FindChild("Text").gameObject;
        Text giveItemButtonText = giveItemButtonTextObject.GetComponent<Text>();
        giveItemButtonText.text = "Give " + itemName + " to " + NPCname + " ?";

        _itemButtonsObject.SetActive(true);
    }

    private void DisableAllUI()
    {
        DisableItemButtons();
        DisableConversationPanel();
    }

    private void DisableItemButtons()
    {
        _itemButtonsObject.SetActive(false);
    }




    private void EnableConversationPanel( string NPCname )
    {
        GameObject npcNameTextObject = _conversationPanel.transform.FindChild( "NpcName" ).gameObject;
        Text npcNameText = npcNameTextObject.GetComponent<Text>();
        npcNameText.text = NPCname;

        _conversationPanel.SetActive( true );
    }
    private void DisableConversationPanel()
    {
        _conversationPanel.SetActive( false );
    }
}
