using UnityEngine;
using System.Collections;

public class EventTag : MonoBehaviour 
{
    public string Tag;
    public Transform cTransform;

    void Awake()
    {
        if(cTransform == null)
            cTransform = transform;
    }
}
