using UnityEngine;
using System.Collections;

public enum CueType
{
    Character,
    Item,
    Location,
    Event
};

public enum ItemStatus
{
    Free,
    Owned,
    Shop
};

public enum Locations
{
    Dock = 0,
    EasternCrossroads,
    EasternForest,
    EasternGardens,
    EasternHousing,
    EasternLookout,
    EasternPeninsula,
    EasternStreets,
    Lake,
    LandBridge,
    LowerStreets,
    MarketSquare,
    NorthernCrossroads,
    NorthernFishingSpot,
    NorthernGardens,
    NorthernHousing,
    NorthernLake,
    NorthernStreets,
    SouthernGardens,
    SouthernGate,
    SouthernPinnusula,
    SouthernStreets,
    SouthernWestGate,
    SouthernWestPeninsula,
    TownSquare,
    WesternCheckpoint,
    WesternGardens,
    WesternGate,
    WesternHousing,
    WesternLookout,
    WesternPeninsula,
    WesternStreets,
    Unknown
};

//[ExecuteInEditMode]
public class MemoryCue : MonoBehaviour
{
    public Transform CachedTransform;
    public string[] CueMarkers;
    public string UniqueNodeID;

    public CueType CueType;

    public Locations CurrentLocation;

    void Awake()
    {
        if (CachedTransform == null)
            CachedTransform = transform.parent;
    }

    void OnTriggerEnter(Collider other)
    {
        //Step 1 - Find out if we have any prior memory of this item / person / place.
        LocationCue locCue = other.GetComponent<LocationCue>();
        if (locCue == null) return;

        CurrentLocation = locCue.CurrentLocation;
    }

    public bool hasCueMarker(string marker)
    {
        int numMarkers = CueMarkers.Length;

        for (int i = 0; i < numMarkers; i++)
            if (marker == CueMarkers[i])
                return true;

        return false;
    }

    public string GetNameOfEntity()
    {
        GameObject parentObject = gameObject.transform.parent.gameObject;
        return parentObject.name;
    }

    //void Update()
    //{
    //    gameObject.name = "MemNode - " + UniqueNodeID.ToString();
    //}
}
