using UnityEngine;
using System.Collections;

public class EventCue : MemoryCue
{
    public string SharedEventCueID;
    public int[] eventParams;
}

public class SharedEventCue
{
    public string SubEventNodeID;

    public void ParseEvent()
    {

    }
}