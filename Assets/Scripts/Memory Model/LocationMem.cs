using UnityEngine;
using System.Collections;

public class LocationMem : MonoBehaviour 
{
    public string UniqueLocationName;

	public Transform Root;

	void Start ()
    {
		if(Root == null)
			Root = transform;

        LocationManager.Instance.AddLocationMemory(UniqueLocationName, Root);
        Destroy(this);
	}
}
