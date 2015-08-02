using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentInventory
{
    CharacterDetails OwnerDetails;
    CharacterCue OwnerCue;
    MindsEye OwnerMind;

    List<ItemCue> inventoryItems = new List<ItemCue>();
    public string EquippedUID;

    public AgentInventory(CognitiveAgent Owner)
    {
        OwnerDetails = Owner.CharDetails;
        OwnerCue = Owner.CharacterCue;
        OwnerMind = Owner.MindsEye;

        EquippedUID = string.Empty;
    }

    public AgentInventory(PlayerController Owner)
    {
        OwnerDetails = Owner.CharDetails;
        OwnerCue = Owner.CharacterCue;
        OwnerMind = Owner.MindsEye;

        EquippedUID = string.Empty;
    }

    public bool Contains(string uid)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].UniqueNodeID == uid)
                return true;
        }

        return false;
    }

    public void PickupItem(ItemCue itmCue, bool putInHand)
    {
        ItemLabelTracker lblTracker = itmCue.CachedTransform.GetComponentInChildren<ItemLabelTracker>();
        if (lblTracker != null)
            lblTracker.Destroy();

        inventoryItems.Add(itmCue);
        if (itmCue.RigidBody != null)
        {
            itmCue.RigidBody.detectCollisions = false;
            itmCue.RigidBody.isKinematic = true;
            itmCue.RigidBody.useGravity = false;
        }

        itmCue.CachedTransform.parent = OwnerDetails.GripTarget;
        itmCue.CachedTransform.localPosition = Vector3.zero;
        itmCue.CachedTransform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));

        if (!putInHand)
            itmCue.CachedTransform.gameObject.SetActive(false);
        else
        {
            UnequipItem();
            EquippedUID = itmCue.UniqueNodeID;
            itmCue.CachedTransform.gameObject.SetActive(true);
        }

        itmCue.Owner = OwnerCue;
        itmCue.Status = ItemStatus.Owned;

        OwnerMind.MemoryGraph.BindRelatedCues(itmCue);

        
    }

    public void EquipItem(string uid)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].UniqueNodeID == uid)
                inventoryItems[i].CachedTransform.gameObject.SetActive(true);
            else if (inventoryItems[i].UniqueNodeID == EquippedUID)
                inventoryItems[i].CachedTransform.gameObject.SetActive(false);
        }

        EquippedUID = uid;  //We only set it here so it isn't unequipped again during the above iteration.
    }

    public void UnequipItem()
    {
        if (EquippedUID == string.Empty)
            return;

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].UniqueNodeID == EquippedUID)
            {
                inventoryItems[i].CachedTransform.gameObject.SetActive(false);
                EquippedUID = string.Empty;
                break;
            }
        }
    }

    public void DropItem(string uid)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].UniqueNodeID == uid)
            {
                inventoryItems[i].CachedTransform.gameObject.SetActive(true);
                inventoryItems[i].CachedTransform.parent = null;
                inventoryItems[i].Owner = null;
                inventoryItems[i].Status = ItemStatus.Free;

                if (uid == EquippedUID)
                    EquippedUID = string.Empty;

                if (inventoryItems[i].RigidBody != null)
                {
                    inventoryItems[i].RigidBody.detectCollisions = true;
                    inventoryItems[i].RigidBody.isKinematic = false;
                    inventoryItems[i].RigidBody.useGravity = true;
                }

                OwnerMind.MemoryGraph.UnbindRelatedCues(inventoryItems[i]);
                inventoryItems.RemoveAt(i);

                if (inventoryItems.Count > 0)
                    EquipItem(inventoryItems[0].UniqueNodeID);

                return;
            }
        }
    }

    public void DropCurrentItem()
    {
        if(EquippedUID != string.Empty)
            DropItem(EquippedUID);
    }

    internal bool CurrentlyEquipped(string itemUID)
    {
        return EquippedUID == itemUID;
    }

    internal ItemCue GetEquippedItem()
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].UniqueNodeID == EquippedUID)
                return inventoryItems[i];
        }

        return null;
    }

    internal ItemCue LoseEquippedItem()
    {
        if (EquippedUID == string.Empty)
            return null;

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].UniqueNodeID == EquippedUID)
            {
                EquippedUID = string.Empty;

                ItemCue equipItem = inventoryItems[i];
                OwnerMind.MemoryGraph.UnbindRelatedCues(inventoryItems[i]);
                inventoryItems.RemoveAt(i);

                return equipItem;
            }
        }

        return null;
    }

    internal ItemCue LoseItem(int index)
    {
        ItemCue equipItem = inventoryItems[index];
        OwnerMind.MemoryGraph.UnbindRelatedCues(inventoryItems[index]);
        inventoryItems.RemoveAt(index);

        return equipItem;
    }

    internal ItemCue GetSpareItem(string targetUID)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].UniqueNodeID == EquippedUID)
                continue;

            for (int j = 0; j < inventoryItems[i].CueMarkers.Length; j++)
                if (inventoryItems[i].CueMarkers[j] == targetUID)
                    return LoseItem(i);
        }

        return null;
    }

    public int GetNumInventoryItems()
    {
        return inventoryItems.Count;
    }

    public bool CheckIfHasItemWithCueMarker(string cueMarker)
    {
        int numItems = GetNumInventoryItems();

        for (int i = 0; i < numItems; i++)
        {
            if (inventoryItems[i].hasCueMarker(cueMarker))
                return true;
        }
        return false;
    }
}
