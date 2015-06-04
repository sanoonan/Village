using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class LocationCue : MemoryCue
{
    void Awake()
    {
        UniqueNodeID = CurrentLocation.ToString();
        CueMarkers = new string[] { string.Format("LOC_{0}", UniqueNodeID) };
    }

    void Start()
    {
        LocationManager.Instance.AddLocationMemory(UniqueNodeID, this.transform);
    }

    //void Update()
    //{
    //    gameObject.name = CurrentLocation.ToString();
    //}
}