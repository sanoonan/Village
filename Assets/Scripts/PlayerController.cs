using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour 
{
    public MindsEye MindsEye;
    internal AgentInventory Inventory;
    public CharacterCue CharacterCue;               //A pointer to the component which contains info about the current character's cue information.

    #region Agent Components
    internal CharacterDetails CharDetails;          //A pointer to the component with the info about the current character.
    internal CharacterController CharController;
    #endregion

    #region Cached Components
    internal Animation Animation;       //Cached animation component       
    internal Transform Transform;       //Cached transform
    #endregion

    #region Animation Variables
    internal float MoveSpeed = 0.0f;
    internal float WalkSpeed = 2.0f;
    internal float TrotSpeed = 4.0f;
    internal float RunSpeed = 6.0f;

    internal Vector3 MoveDirection = Vector3.zero;

    internal float RotateSpeed = 500.0f;
    internal float TrotAfterSeconds = 3.0f;
    internal float WalkTimeStart = 0.0f;

    internal float SpeedSmoothing = 10.0f;

    internal CharacterState CharacterState = CharacterState.Idle;
    internal bool Running = false;

    internal Dictionary<string, string> AnimationKeys;            //A lookup that compares generic animation names (i.e. run) to the corressponding animation name on the model (i.e. VB_RUN).
    internal float AnimationSpeed = 1.0f;                                //How fast the animations should play.
    private bool isMoving;
    private bool movingBack;
    private float lockCameraTimer;
    private CollisionFlags collisionFlags;
    private float verticalSpeed;
    private float gravity = 21.0f;
    #endregion

    void Awake()
    {
        //Cache our frequent lookups.
        Transform = GetComponent<Transform>();

        //Get our basic components.
        Animation = GetComponent<Animation>();
        CharController = GetComponent<CharacterController>();
        CharDetails = GetComponent<CharacterDetails>();

        MoveDirection = Transform.TransformDirection(Vector3.forward);  //Get their initial facing direction.

        //Add the basic animation info.
        AnimationKeys = new Dictionary<string, string>();
        AnimationKeys.Add("run", "VB_Run");
        AnimationKeys.Add("walk", "VB_Walk");
        AnimationKeys.Add("idle", "VB_Idle");
        AnimationKeys.Add("greet", "VB_Greeting");
        AnimationKeys.Add("talk", "VB_Talk");

        Inventory = new AgentInventory(this);
    }

	// Update is called once per frame
	void Update ()
    {
        #region Moving and Animation
        Transform camTransform = Camera.main.transform;
        Vector3 fwd = camTransform.TransformDirection(Vector3.forward);
        fwd.y = 0;
        fwd = fwd.normalized;

        Vector3 right = new Vector3(fwd.z, 0.0f, -fwd.x);
        float vert = Input.GetAxisRaw("Vertical");
        float hor = Input.GetAxisRaw("Horizontal");

        Vector3 movement = Vector3.zero;

        //Are we moving or looking backwards?
        if (vert < -0.2f)
            movingBack = true;
        else
            movingBack = false;

        bool wasMoving = isMoving;
        isMoving = Mathf.Abs(hor) > 0.1f || Mathf.Abs(vert) > 0.1f;

        Vector3 targetDirection = hor * right + vert * fwd;

        lockCameraTimer += Time.smoothDeltaTime;
        if (isMoving != wasMoving)
            lockCameraTimer = 0.0f;

        // We store speed and direction seperately,
        // so that when the character stands still we still have a valid forward direction
        // moveDirection is always normalized, and we only update it if there is user input.
        if (targetDirection != Vector3.zero)
        {
            // If we are really slow, just snap to the target direction
            if (MoveSpeed < WalkSpeed * 0.9f)
            {
                MoveDirection = targetDirection.normalized;
            }
            // Otherwise smoothly turn towards it
            else
            {
                MoveDirection = Vector3.RotateTowards(MoveDirection, targetDirection, RotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
                MoveDirection = MoveDirection.normalized;
            }
        }

        // Smooth the speed based on the current target direction
        float curSmooth = SpeedSmoothing * Time.deltaTime;

        // Choose target speed
        //We want to support analog input but make sure you can't walk faster diagonally than just forward or sideways
        float targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);

        CharacterState = CharacterState.Idle;

        // Pick speed modifier
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            targetSpeed *= RunSpeed;
            CharacterState = CharacterState.Running;
        }
        else if (Time.time - TrotAfterSeconds > WalkTimeStart)
        {
            targetSpeed *= TrotSpeed;
            CharacterState = CharacterState.Trotting;
        }
        else
        {
            targetSpeed *= WalkSpeed;
            CharacterState = CharacterState.Walking;
        }

        MoveSpeed = Mathf.Lerp(MoveSpeed, targetSpeed, curSmooth);

        // Reset walk time start when we slow down
        if (MoveSpeed < WalkSpeed * 0.3f)
            WalkTimeStart = Time.time;

        if (IsGrounded())
            verticalSpeed = 0.0f;
        else
            verticalSpeed -= gravity * Time.deltaTime;

        // Calculate actual motion
        movement = MoveDirection * MoveSpeed + new Vector3(0.0f, verticalSpeed, 0.0f);
        movement *= Time.deltaTime;

        // Move the controller
        collisionFlags = CharController.Move(movement);

        if (CharController.velocity.sqrMagnitude < 0.1f)
            Animation.CrossFade(AnimationKeys["idle"]);
        else
        {
            if (CharacterState == CharacterState.Running)
            {
                Animation[AnimationKeys["run"]].speed = Mathf.Clamp(CharController.velocity.magnitude, 0.0f, AnimationSpeed);
                Animation.CrossFade(AnimationKeys["run"]);
            }
            else if (CharacterState == CharacterState.Trotting)
            {
                Animation[AnimationKeys["walk"]].speed = Mathf.Clamp(CharController.velocity.magnitude, 0.0f, AnimationSpeed);
                Animation.CrossFade(AnimationKeys["walk"]);
            }
            else if (CharacterState == CharacterState.Walking)
            {
                Animation[AnimationKeys["walk"]].speed = Mathf.Clamp(CharController.velocity.magnitude, 0.0f, AnimationSpeed * 0.75f);
                Animation.CrossFade(AnimationKeys["walk"]);
            }
        }

        if(IsGrounded())
            Transform.rotation = Quaternion.LookRotation(MoveDirection);
        else
        {
            Vector3 xzMove = movement;
            xzMove.y = 0;
            if (xzMove.sqrMagnitude > 0.001f)
                Transform.rotation = Quaternion.LookRotation(xzMove);
        }
        #endregion

        if(Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            MemoryCue closestItem = null;
            float closestDist = float.MaxValue;
            foreach(var entry in MindsEye.ActiveCues)
            {
                if(entry.Value.CueType == CueType.Item && !Inventory.CurrentlyEquipped(entry.Key) && Vector3.Distance(Transform.position, entry.Value.CachedTransform.position) < closestDist)
                {
                    closestItem = entry.Value;
                    closestDist = Vector3.Distance(Transform.position, entry.Value.CachedTransform.position);        //Change this so it is only calculated once!
                }
            }
        
            if(closestItem != null)
            {
                ItemCue itmCue = (ItemCue)closestItem;
                if(itmCue.Status == ItemStatus.Free)
                    Inventory.PickupItem(itmCue, true);
            }
        }
        else if(Input.GetKeyDown(KeyCode.KeypadMinus))
            Inventory.DropCurrentItem();
        else if(Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            CharacterCue closestChar = null;
            float closestDist = float.MaxValue;
            foreach (var entry in MindsEye.ActiveCues)
            {
                if (entry.Value.CueType == CueType.Character)
                {
                    CharacterCue charCue = (CharacterCue)entry.Value;
                    if(charCue.CharDetails.CognitiveAgent.HasQuest && Vector3.Distance(Transform.position, entry.Value.CachedTransform.position) < closestDist)
                    {
                        closestChar = charCue;
                        closestDist = Vector3.Distance(Transform.position, entry.Value.CachedTransform.position);
                    }
                }
            }

            if (closestChar != null)
                Message.DispatchMessage(CharDetails.AgentID, closestChar.CharDetails.AgentID, TelegramType.QuestTrade);
        }
        else if(Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            CharacterCue closestChar = null;
            float closestDist = float.MaxValue;
            foreach (var entry in MindsEye.ActiveCues)
            {
                if (entry.Value.CueType == CueType.Character)
                {
                    CharacterCue charCue = (CharacterCue)entry.Value;
                    if (Vector3.Distance(Transform.position, entry.Value.CachedTransform.position) < closestDist)
                    {
                        closestChar = charCue;
                        closestDist = Vector3.Distance(Transform.position, entry.Value.CachedTransform.position);
                    }
                }
            }

            if (closestChar != null)
                Message.DispatchMessage(CharDetails.AgentID, closestChar.CharDetails.AgentID, TelegramType.DiscussEvent);
        }
    }

    public bool IsMoving()
    {
        return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5;
    }

    public float GetLockCameraTimer()
    {
        return lockCameraTimer;
    }

    public bool IsMovingBackwards()
    {
        return movingBack;
    }

    public Vector3 GetDirection()
    {
        return MoveDirection;
    }

    public bool IsGrounded()
    {
        return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
    }

    public bool IsJumping()
    {
        return false;
    }
}
