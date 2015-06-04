using UnityEngine;
using System.Collections;

public class TransformParentChecker : MonoBehaviour 
{
    Transform pTransform;
    Transform cTransform;
	void Awake () 
    {
        cTransform = transform;
        pTransform = cTransform.parent;	
	}
	
	void LateUpdate () 
    {
	    if(cTransform.parent != pTransform)
        {
            Debug.Log("The parent transform has changed!");
            cTransform.parent = null;
            cTransform.localScale = Vector3.one;
            cTransform.localPosition = Vector3.zero;
            cTransform.localRotation = Quaternion.identity;
        }
	}
}
