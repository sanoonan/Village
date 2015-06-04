using UnityEngine;
using System.Collections;

public class characterButton : MonoBehaviour {

	public GameObject frog;
	
	
	
	private Rect FpsRect ;
	private string frpString;
	
	private GameObject instanceObj;
	public GameObject[] gameObjArray=new GameObject[9];
	public AnimationClip[] AniList  = new AnimationClip[4];
	
	float minimum = 2.0f;
	float maximum = 50.0f;
	float touchNum = 0f;
	string touchDirection ="forward"; 
	private GameObject Villarger_A_Girl_prefab;
	
	// Use this for initialization
	void Start () {
		
		//frog.animation["dragon_03_ani01"].blendMode=AnimationBlendMode.Blend;
		//frog.animation["dragon_03_ani02"].blendMode=AnimationBlendMode.Blend;
		//Debug.Log(frog.GetComponent("dragon_03_ani01"));
		
		//Instantiate(gameObjArray[0], gameObjArray[0].transform.position, gameObjArray[0].transform.rotation);
	}
	
 void OnGUI() {
	  if (GUI.Button(new Rect(20, 20, 70, 40),"Idle")){
		 frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Idle");
	  }
	    if (GUI.Button(new Rect(90, 20, 70, 40),"Greeting")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Greeting");
	  }
		   if (GUI.Button(new Rect(160, 20, 70, 40),"Talk")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Talk");
	  }
	     if (GUI.Button(new Rect(230, 20, 70, 40),"Walk")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Walk");
	  } 
		  if (GUI.Button(new Rect(300, 20, 70, 40),"L_Walk")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_L_Walk");
	  } 
		  if (GUI.Button(new Rect(370, 20, 70, 40),"R_Walk")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_R_Walk");
	  }
		  if (GUI.Button(new Rect(440, 20, 70, 40),"B_Walk")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_B_Walk");
	  }
		if (GUI.Button(new Rect(510, 20, 70, 40),"Run")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Run");
	  } 
		if (GUI.Button(new Rect(580, 20, 70, 40),"L_Run")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_L_Run");
	  }
		
		if (GUI.Button(new Rect(650, 20, 70, 40),"R_Run")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_R_Run");
	  }
	
		if (GUI.Button(new Rect(720, 20, 70, 40),"B_Run")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_B_Run");
	  }
		if (GUI.Button(new Rect(790, 20, 70, 40),"Jump")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Jump");
	  } 
			if (GUI.Button(new Rect(860, 20, 70, 40),"DrawDagger")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Once;
		  	frog.GetComponent<Animation>().CrossFade("VB_DrawDagger");
	  } 
			if (GUI.Button(new Rect(20, 60, 70, 40),"ATK_standy")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Attackstandy");
	  }
			if (GUI.Button(new Rect(90, 60, 70, 40),"Attack00")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Attack00");
	  }
			if (GUI.Button(new Rect(160, 60, 70, 40),"Attack01")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Attack01");
	  }
			if (GUI.Button(new Rect(230, 60, 70, 40),"Attack02")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Attack02");
	  }
			if (GUI.Button(new Rect(300, 60, 70, 40),"Combo")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Combo");
	  }
			if (GUI.Button(new Rect(370, 60, 70, 40),"Kick")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Once;
		  	frog.GetComponent<Animation>().CrossFade("VB_Kick");
	  }
			if (GUI.Button(new Rect(440, 60, 70, 40),"Skill")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Skill");
	  }
			if (GUI.Button(new Rect(510, 60, 70, 40),"M_Avoid")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_M_Avoid");
			
	  }	if (GUI.Button(new Rect(580, 60, 70, 40),"L_Avoid")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_L_Avoid");
			
	  }	if (GUI.Button(new Rect(650, 60, 70, 40),"R_Avoid")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_R_Avoid");
	  }
		 	if (GUI.Button(new Rect(720, 60, 70, 40),"Buff")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Buff");
	  }
			if (GUI.Button(new Rect(790, 60, 70, 40),"Run01")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Run01");
	  }
			if (GUI.Button(new Rect(860, 60, 70, 40),"RunAttack")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_RunAttack");
	  }
		
			if (GUI.Button(new Rect(20, 100, 70, 40),"Sliding")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Sliding");
	  }
		
			if (GUI.Button(new Rect(90, 100, 70, 40),"Rappel")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Rappel");
	  }
		if (GUI.Button(new Rect(160, 100, 70, 40),"L_Run01")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_L_Run01");
	  }
		
		if (GUI.Button(new Rect(230, 100, 70, 40),"R_Run01")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_R_Run01");
	  }
		if (GUI.Button(new Rect(300, 100, 70, 40),"B_Run01")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_B_Run01");
	  }
			if (GUI.Button(new Rect(370, 100, 70, 40),"Jump01")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Jump01");
	  }
			if (GUI.Button(new Rect(440, 100, 70, 40),"PickUp")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Pickup");
	  }
				if (GUI.Button(new Rect(510, 100, 70, 40),"Damage")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Damage");
	  }
				if (GUI.Button(new Rect(580, 100, 70, 40),"Death")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Death");
	  }
			if (GUI.Button(new Rect(650, 100, 70, 40),"Elevator")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Elevator");
	  }
			if (GUI.Button(new Rect(720, 100, 140, 40),"GangnamStyle")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_GangnamStyle");
	  }  
				if (GUI.Button(new Rect(20, 580, 140, 40),"V  2.5")){
		  frog.GetComponent<Animation>().wrapMode= WrapMode.Loop;
		  	frog.GetComponent<Animation>().CrossFade("VB_Idle");
	  } 
				//if (GUI.Button(new Rect(600, 480, 140, 40),"Ver 1.2")){
		 // frog.animation.wrapMode= WrapMode.Loop;
		  //	frog.animation.CrossFade("Idle");
	  //}
		
		
 }
	
	// Update is called once per frame
	void Update () {
		
		//if(Input.GetMouseButtonDown(0)){
		
			//touchNum++;
			//touchDirection="forward";
		 // transform.position = new Vector3(0, 0,Mathf.Lerp(minimum, maximum, Time.time));
			//Debug.Log("touchNum=="+touchNum);
		//}
		/*
		if(touchDirection=="forward"){
			if(Input.touchCount>){
				touchDirection="back";
			}
		}
	*/
		 
		//transform.position = Vector3(Mathf.Lerp(minimum, maximum, Time.time), 0, 0);
	if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
		//frog.transform.Rotate(Vector3.up * Time.deltaTime*30);
	}
	
}
