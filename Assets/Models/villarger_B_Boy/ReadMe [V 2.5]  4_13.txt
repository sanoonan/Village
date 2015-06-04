

=====================================================

Villarger_B_Boy  [3D Model]   < V 2.5 > 

=====================================================


Update info 04_13_Update

------------------------------------------------

- Add 2 animations [Sliding].[Rappel]


------------------------------------------------


* Attack explanation

-Attack 

 This motion is wield a Dagger Quickly once.

-Attack01

 This is ranged attack.
 This motion is collected Wind force at Dagger,and
  Attack to enemy far away.
  
-Attack02

 This motion is  defend from enemy 's attack, and Counter attack.
 
-combo

 This motion is attack three time in a row
 
 -Skil
 
  This motion is used wind force on body and attack to enemy on ground. 
  
  

=====================================================
  
 Please refer to below script
======================================================
  
  
  
  
public class PlayerAnimation : MonoBehaviour 
{
 
 PlayerMove playerMove;
 Animation animation;
 
 
 void Start () 
 {
     playerMove = GetComponent<PlayerMove>();
  animation = GetComponentInChildren<Animation>();
  
  animation["Idle"].wrapMode = WrapMode.Loop;
  animation["Run01"].wrapMode = WrapMode.Loop;
  animation["Attack00"].wrapMode = WrapMode.Once;
  animation["Rum01"].speed = 2f;
  
  animation["Attack00"].layer =1;
  
  Transform mt = transform.Find("Blade_Warrior_Prefab/Blade_male/Blade_male Pelvis/Blade_male Spine");
  
  animation["Attack00"].AddMixingTransform(mt);
  
 }
 
 
 void Update () 
 {
   if(Vector3.Distance(transform.position,playerMove.destPoint)>2)
  {
   //run
   if(animation.IsPlaying("Attack00")==false)
   animation.CrossFade("Run");
  }
  
  else
  {
   //idle
    if(animation.IsPlaying("Attack00")==false)
   animation.CrossFade("Idle");
  }
  
  
  if(Input.GetMouseButtonDown(1))
   
  {                                       //0으로 실행하면 더 빠르게 실행된다. 
   animation.CrossFadeQueued("Attack00",0.2f, QueueMode.PlayNow);
  }
  
  
  
 }
 
}
  
  
  
  
  
=========================================================
 





==================================================
It's late..ㅠ_ㅠ

I'm very so sorry... ㅠ_ㅠ


I'll keep on trying my best !!!!(May be...)ㅠ_ㅠ
==================================================






< asset info >

-----------------------------------------------------
model 
-----------------------------------------------------

>>Lod 1

> 1937  tris 
   
> 1111   verts


>>Lod 2

> 1264 tris

> 751 verts

[ No Alpha map! No Stress!!]

[ biped  has been set up.]



 


----------------------------------------------------
Includes 37 Animations
----------------------------------------------------


> Idle

> Greeting

> Talk

> Walk

> L_Walk

> R_Walk

> B_Walk

> Run [No Draw dagger]

> L_Run [No Draw dagger]

> R_Run [No Draw dagger]

> B_Run [No Draw dagger]

> Jump [No Draw dagger]

> Draw Dagger 

> Attack standy

> Attack

> Attack01

> Attack02

> Combo

> Kick

> Skill

> M_Avoid

> L_Avoid

> R_Avoid

> Buff

> Run

> RunAttack

> Sliding

> Rappel

> L_Run

> R_Run

> B_Run

> Jump

> Pick Up

> Damage

> Death

> Dance [ Elevator ]

> Dance [ GangnamStyle ]



===================================================


=================================================== 

instructions

===================================================


1.click to the  [File] > [Open Project] > [Villarger_B_Boy] folder

2.Dubble click [Demo_scene]

3.you can use the model file ~  Too easy~~~



Enjoy!! Cool Guys!!
================================================= 

If you have a question or comment

Send to my E-mail


kimys2848@naver.com

  

Visit my site for more info


>> http://blog.naver.com/kimys2848





