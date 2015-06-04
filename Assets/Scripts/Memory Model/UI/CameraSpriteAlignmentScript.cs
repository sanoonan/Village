using UnityEngine;
using System.Collections;

public class CameraSpriteAlignmentScript : MonoBehaviour 
{
    public UILabel lName;
    public Transform cTransform;
    //Vector3 screenPos;

    void Start()
    {
        if (cTransform == null)
            cTransform = transform;

        if(lName != null)
            lName.text = cTransform.parent.name;        
    }

	void LateUpdate ()
    {
        cTransform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward);

        //screenPos = Camera.main.WorldToScreenPoint(cTransform.position);
        //screenPos = new Vector3(Mathf.Round(screenPos.x ), Mathf.Round(screenPos.y), Mathf.Round(screenPos.z));
        //cTransform.position = Camera.main.ScreenToWorldPoint(screenPos);
	}

    //void OnBecameVisible()
    //{
    //    cTransform.gameObject.SetActive(true);
    //    enabled = true;
    //}

    //void OnBecameInvisible()
    //{
    //    cTransform.gameObject.SetActive(false);
    //    enabled = false;
    //}
}
