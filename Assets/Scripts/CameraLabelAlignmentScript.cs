using UnityEngine;
using System.Collections;

public class CameraLabelAlignmentScript : MonoBehaviour 
{
    internal Transform cTransform;

    void Awake()
    {
        cTransform = transform;
    }

    void LateUpdate()
    {
        cTransform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }
}
