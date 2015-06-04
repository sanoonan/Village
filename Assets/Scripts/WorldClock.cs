using UnityEngine;
using System.Collections;

public class WorldClock : MonoBehaviour 
{
    UILabel clockLabel;

	void Start()
    {
        clockLabel = GetComponent<UILabel>();
	}
	
	void Update() 
    {
        clockLabel.text = DaylightScript.GetTimeStamp();
	}
}
