using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationController : MonoBehaviour
{
    private float _animationSpeed = 1.0f;  

    private Animation _animation;
    private Dictionary<string, string> _animationKeys;

    private bool _animationsLocked = false;

	void Awake ()
    {
        _animation = GetComponent<Animation>();

        _animationKeys = new Dictionary<string, string>();
        _animationKeys.Add( "run", "VB_Run" );
        _animationKeys.Add( "walk", "VB_Walk" );
        _animationKeys.Add( "idle", "VB_Idle" );
        _animationKeys.Add( "greet", "VB_Greeting" );
        _animationKeys.Add( "talk", "VB_Talk" );
        _animationKeys.Add( "death", "VB_Death" );
        _animationKeys.Add( "attack", "VB_Attack00" );
	}

    public void SetAnimation( CharacterState characterState, float speed )
    {
        if ( _animationsLocked )
            return;

        string animationKey = GetAnimationKey( characterState );
        float animSpeed = _animationSpeed;

        if ( characterState == CharacterState.Walking )
        {
            animSpeed *= 0.75f;
        }

        _animation[_animationKeys[animationKey]].speed = Mathf.Clamp( speed, 0.0f, animSpeed );

        SetAnimation( characterState );
    }

    public void SetAnimation( CharacterState characterState )
    {
        if ( _animationsLocked )
            return;

        float animSpeed = _animationSpeed;
        string animationKey = GetAnimationKey( characterState );

        if ( characterState == CharacterState.Greeting )
        {
            animSpeed *= 0.5f;
        }

        if ( ( characterState == CharacterState.Greeting ) || ( characterState == CharacterState.Dying ) )
        {
            _animation[_animationKeys[animationKey]].wrapMode = WrapMode.Once;
        }

        _animation[_animationKeys[animationKey]].speed = animSpeed;
        _animation.CrossFade( _animationKeys[animationKey] );
    }

    public bool IsAnimationPlaying( CharacterState characterState )
    {
        string animationKey = GetAnimationKey( characterState );
        return _animation.IsPlaying( _animationKeys[animationKey] );
    }

    private string GetAnimationKey( CharacterState characterState )
    {
        string animationKey = "";

        if ( ( characterState == CharacterState.Walking ) || ( characterState == CharacterState.Trotting ) )
            animationKey = "walk";
        else if ( characterState == CharacterState.Running )
            animationKey = "run";
        else if ( characterState == CharacterState.Idle )
            animationKey = "idle";
        else if ( characterState == CharacterState.Greeting )
            animationKey = "greet";
        else if ( characterState == CharacterState.Talking )
            animationKey = "talk";
        else if ( characterState == CharacterState.Dying )
        {
            _animationsLocked = true;
            animationKey = "death";
        }
        else if ( characterState == CharacterState.Attacking )
            animationKey = "attack";

        return animationKey;
    }

    
}
