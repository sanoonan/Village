using UnityEngine;
using System.Collections;

public class ShopManager : MonoBehaviour 
{
    public Transform cTransform;        //Place where worker stands.
    public Transform eTransform;        //Place where customer stands.

    public ItemCue[] ShopItems;

    public Locations ShopLocation;

    public bool ShopOpen;

	void Awake () 
    {
        if(cTransform == null)
            cTransform = transform;

        if (eTransform == null)
            eTransform = cTransform;

        ShopOpen = false;
	}

    void Start()
    {
        for(int i = 0; i < ShopItems.Length; i++)
        {
            ShopItems[i].CurrentLocation = ShopLocation;
            ShopItems[i].Status = ItemStatus.Shop;
        }
    }

    public void OpenShop(CharacterCue owner)
    {
        ShopOpen = true;

        for (int i = 0; i < ShopItems.Length; i++)
            if(ShopItems[i] != null)
                ShopItems[i].Owner = owner;
    }

    public void CloseShop()
    {
        ShopOpen = false;
    }

    internal bool AttemptPurchase(CognitiveAgent Owner, string itemUID)
    {
        for(int i = 0; i < ShopItems.Length; i++)
        {
            if(ShopItems[i] != null && ShopItems[i].UniqueNodeID == itemUID)
            {
                Owner.Inventory.PickupItem(ShopItems[i], false);
                return true;
            }
        }

        return false;
    }
}
