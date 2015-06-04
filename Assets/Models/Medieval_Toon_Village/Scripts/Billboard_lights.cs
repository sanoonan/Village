using UnityEngine;
using System.Collections;

public class Billboard_lights : MonoBehaviour 
{
    public Transform cTransform;
    //Vector3 screenPos;

    void Start()
    {
        cTransform = transform;
    }

    void LateUpdate()
    {
        cTransform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward);

        //screenPos = Camera.main.WorldToScreenPoint(cTransform.position);
        //screenPos = new Vector3(Mathf.Round(screenPos.x ), Mathf.Round(screenPos.y), Mathf.Round(screenPos.z));
        //cTransform.position = Camera.main.ScreenToWorldPoint(screenPos);
    }

    void OnBecameVisible()
    {
        cTransform.gameObject.SetActive(true);
        enabled = true;
    }

    void OnBecameInvisible()
    {
        cTransform.gameObject.SetActive(false);
        enabled = false;
    }


    //private float billBoardSpeed=0.1f;
    //private float initialScaleX;
    //private float initialScaleY;
    //private Vector3 currentScale;
    //private bool lightGrow;
    //public float randCounter;
    //// Use this for initialization
    //void Start () {
    //    initialScaleX=transform.localScale.x;
    //    initialScaleY=transform.localScale.y;
    //    lightGrow=true;
    //    currentScale=transform.localScale;
    //}
	
    //// Update is called once per frame
    //void Update() 
    //{
    //    transform.LookAt(Camera.main.transform.position, Vector3.up);
    //    if (lightGrow){
    //        if (initialScaleX*1.5f > transform.localScale.x){
    //            currentScale.x += initialScaleX*billBoardSpeed*Time.deltaTime;
    //            currentScale.y += initialScaleY*billBoardSpeed*Time.deltaTime;	
    //        }
    //        else lightGrow=false;
    //    }
    //    else{
    //        if (initialScaleX < transform.localScale.x){
    //            currentScale.x -= initialScaleX*billBoardSpeed*Time.deltaTime;	
    //            currentScale.y -= initialScaleY*billBoardSpeed*Time.deltaTime;	
    //        }
    //        else lightGrow=true;
    //    }
    //    transform.localScale=currentScale;
		
    //    randCounter+=Time.deltaTime;
    //    if (randCounter>2.0f){
    //        //Debug.Log (Random.Range (1,4));
    //        randCounter=0;
    //        if ((Random.Range(1,4))>2){
    //            if (lightGrow)lightGrow=false;
    //            else lightGrow=true;
    //        }
    //    }
    //}
}
