using UnityEngine;
using System.Collections;

public class ItemUIPooler : MonoBehaviour 
{
    public static ItemUIPooler Instance;
    public ItemLabelTracker[] lNames;
    int aPos = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
            throw new UnityException("Item UI Pooler has already been declared! It cannot exist twice!");
        }
    }

    public void PlaceLabel(MemoryCue memCue)
    {
        ItemCue itmCue = (ItemCue)memCue;
        if (itmCue.Status == ItemStatus.Owned || itmCue.CachedTransform.GetComponentInChildren<ItemLabelTracker>() != null)
            return;

        lNames[aPos].Attach(itmCue);
        aPos = (aPos + 1) % lNames.Length;
    }
}
