using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LocationManager : MonoBehaviour 
{
    public static LocationManager Instance;

    Dictionary<string, List<Transform>> LocationMems = new Dictionary<string, List<Transform>>();

	void Awake () 
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
            throw new UnityException("Location Manager has already been declared! It cannot exist twice!");
        }
	}
	
	public void AddLocationMemory(string locName, Transform locTransform)
    {
        if (LocationMems.ContainsKey(locName))
            LocationMems[locName].Add(locTransform);
        else
        {
            LocationMems.Add(locName, new List<Transform>() { locTransform });
        }
    }

    public List<Transform> GetLocation(string locName)
    {
        //Debug.Log(locName);
        return new List<Transform>(LocationMems[locName]);
    }

    public Transform GetLocationMemory(string locName)
    {
        //Debug.Log(locName);
        return LocationMems[locName][0];
    }
}
