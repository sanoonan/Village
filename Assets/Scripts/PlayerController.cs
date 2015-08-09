using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour 
{
    private bool _enableControl = true;

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
    internal Vector3 MoveDirection = Vector3.zero;

    private float _moveSpeed = 0.0f;
    private float _walkSpeed = 2.0f;
    private float _trotSpeed = 4.0f;
    private float _runSpeed = 6.0f;

    internal float RotateSpeed = 500.0f;
    internal float TrotAfterSeconds = 3.0f;
    internal float WalkTimeStart = 0.0f;

    internal float SpeedSmoothing = 10.0f;

    internal CharacterState _characterState = CharacterState.Idle;
    internal bool Running = false;
                         
    private bool isMoving;
    private bool movingBack;
    private float lockCameraTimer;
    private CollisionFlags collisionFlags;
    private float verticalSpeed;
    private float gravity = 21.0f;
    #endregion

    public static PlayerController Instance;

    public AnimationController _animationController;

    void Awake()
    {
        Instance = this;

        //Cache our frequent lookups.
        Transform = GetComponent<Transform>();

        //Get our basic components.
        CharController = GetComponent<CharacterController>();
        CharDetails = GetComponent<CharacterDetails>();

        _animationController = GetComponent<AnimationController>();

        MoveDirection = Transform.TransformDirection(Vector3.forward);  //Get their initial facing direction.

        Inventory = new AgentInventory(this);
    }

	// Update is called once per frame
	void Update ()
    {
        if (!_enableControl)
            return;

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
            if (_moveSpeed < _walkSpeed * 0.9f)
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

        

        // Pick speed modifier
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            targetSpeed *= _runSpeed;
            _characterState = CharacterState.Running;
        }
        else if (Time.time - TrotAfterSeconds > WalkTimeStart)
        {
            targetSpeed *= _trotSpeed;
            _characterState = CharacterState.Trotting;
        }
        else
        {
            targetSpeed *= _walkSpeed;
     //       _characterState = CharacterState.Walking;
        }

        _moveSpeed = Mathf.Lerp( _moveSpeed, targetSpeed, curSmooth );

        // Reset walk time start when we slow down
        if ( _moveSpeed < _walkSpeed * 0.3f )
            WalkTimeStart = Time.time;

        if (IsGrounded())
            verticalSpeed = 0.0f;
        else
            verticalSpeed -= gravity * Time.deltaTime;

        // Calculate actual motion
        movement = MoveDirection * _moveSpeed + new Vector3( 0.0f, verticalSpeed, 0.0f );
        movement *= Time.deltaTime;

        // Move the controller
        collisionFlags = CharController.Move(movement);

        if ( CharController.velocity.sqrMagnitude < 0.1f )
        {
            if ( _characterState != CharacterState.Attacking )
            {
                _characterState = CharacterState.Idle;
            }

            _animationController.SetAnimation( _characterState );
        }
        else
        {
            if ( ( _characterState != CharacterState.Running ) && ( _characterState != CharacterState.Trotting ) )
            {
                _characterState = CharacterState.Walking;
            }

            _animationController.SetAnimation( _characterState, CharController.velocity.magnitude );
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
                if (itmCue.Status == ItemStatus.Free)
                {
                    Inventory.PickupItem(itmCue, true);
                    QuestManager.Instance.AttemptToComplete(new QFCD_AcquireItem(itmCue));
                }                
            }
        }
        else if(Input.GetKeyDown(KeyCode.KeypadMinus))
            Inventory.DropCurrentItem();
        else if(Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            CharacterCue closestChar = GetClosestCharacter();

//            if (closestChar != null)
 //               Message.DispatchMessage(CharDetails.AgentID, closestChar.CharDetails.AgentID, TelegramType.QuestTrade);

            if (closestChar != null)
            {
                _transferNPC = closestChar;
                StartCoroutine( ItemTransferCoroutine() );
            }
        }
        else if(Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            CharacterCue closestChar = GetClosestCharacter();

            if (closestChar != null)
			{
                Message.DispatchMessage(CharDetails.AgentID, closestChar.CharDetails.AgentID, TelegramType.DiscussEvent);

                _conversationNPC = closestChar;
                StartCoroutine( ConversationCoroutine() );
			}
        }
        else if( Input.GetKeyDown( KeyCode.KeypadDivide ) )
        {
            StartCoroutine( AttackCoroutine() );
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

    public void EnableControl()
    {
        _enableControl = true;
    }
    public void DisableControl()
    {
        _enableControl = false;
    }


    private CharacterCue _transferNPC;
    private bool _transferRespondedTo = false;
    private bool _transferConfirmed = false;
    private IEnumerator ItemTransferCoroutine()
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0.01f;

        _transferRespondedTo = false;
        string NPCname = _transferNPC.GetNameOfEntity();
        string itemName = Inventory.GetEquippedItem().GetNameOfEntity();
        NewUIController.Instance.InitItemTransferUI(NPCname, itemName);

        while (!_transferRespondedTo)
        {
            yield return null;
        }

        if (_transferConfirmed)
        {
			ItemCue transferItem = Inventory.LoseEquippedItem();
            _transferNPC.Inventory.PickupItem( transferItem, true );

            int NpcId = AgentManager.Instance.GetAgentIdByName( NPCname );
            QuestManager.Instance.AttemptToComplete( new QFCD_GiveItemToNPC( NpcId, transferItem ) ); 
        }

		Time.timeScale = originalTimeScale;
    }

    public void TransferResponse( bool response )
    {
        _transferConfirmed = response;
        _transferRespondedTo = true;
    }

    private CharacterCue _conversationNPC;
    private bool _conversationComplete = false;
    private IEnumerator ConversationCoroutine()
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0.01f;
        
        _conversationComplete = false;
        string NPCname = _conversationNPC.GetNameOfEntity();
        NewUIController.Instance.InitConversationUI( NPCname );
        
        while (!_conversationComplete)
        {
            yield return null;
        }

        int NpcId = AgentManager.Instance.GetAgentIdByName( NPCname );
        QuestManager.Instance.AttemptToComplete( new QFCD_TalkToNPC( NpcId ) );

        Time.timeScale = originalTimeScale;
    }

    public void ConversationResponse()
    {
        _conversationComplete = true;
    }

    private CharacterCue GetClosestCharacter()
    {
        CharacterCue closestChar = null;
        float closestDist = 2.0f;
        foreach ( var entry in MindsEye.ActiveCues )
        {
            if ( entry.Value.CueType == CueType.Character )
            {
                CharacterCue charCue = ( CharacterCue )entry.Value;

                if( !charCue.CharDetails.IsAlive() )
                    continue;

                if ( Vector3.Distance( Transform.position, entry.Value.CachedTransform.position ) < closestDist )
                {
                    closestChar = charCue;
                    closestDist = Vector3.Distance( Transform.position, entry.Value.CachedTransform.position );
                }
            }
        }
        return closestChar;
    }

    private IEnumerator AttackCoroutine()
    {
        if ( _characterState == CharacterState.Attacking )
            yield break;

        _characterState = CharacterState.Attacking;

        CharacterCue closestChar = GetClosestCharacter();
        if ( closestChar != null )
        {
            QuestManager.Instance.AttemptToComplete( new QFCD_KillNPC( closestChar.GetAgentId() ) );
            closestChar.CharDetails._cognitiveAgent.Die();
        }

        while ( true )
        {
            if ( ( Input.GetKeyUp( KeyCode.KeypadDivide ) ) || ( _characterState != CharacterState.Attacking ) )
            {
                _characterState = CharacterState.Idle;
                yield break;
            }
            yield return null;
        }
    }
}
