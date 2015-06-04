using UnityEngine;
using System.Collections;

public class ItemLabelTracker : MonoBehaviour 
{
    public Transform PoolTransform;
    Transform cTransform;
    CameraLabelAlignmentScript clAlignment;
    UILabel lName;

	void Awake () 
    {
        cTransform = transform;
        lName = GetComponent<UILabel>();
        clAlignment = GetComponent<CameraLabelAlignmentScript>();
        clAlignment.enabled = false;
	}

    public void Attach(MemoryCue memCue)
    {
        cTransform.parent = memCue.CachedTransform;
        cTransform.localPosition = new Vector3(0.0f, 5.0f, 0.0f);
        lName.text = memCue.UniqueNodeID;

        clAlignment.enabled = true;
    }
	
	public void Destroy()
    {
        cTransform.parent = PoolTransform;
        cTransform.localPosition = Vector3.zero;
        clAlignment.enabled = false;
    }

    void Update()
    {
        cTransform.localPosition = new Vector3(0.0f, 5.0f, 0.0f);
    }
}
