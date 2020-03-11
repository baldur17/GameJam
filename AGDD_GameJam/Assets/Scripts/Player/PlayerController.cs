using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    
    //runSpeed = 100, then max speed is -2 / 2
    
    
    [Header("Movement")]
    public float runSpeed;
    public float crouchSpeed;
    public float jumpSpeed;
    public float jumpTime;
    
    private Rigidbody2D _rb;
    private float _leftJoystickHorizontal;
    private bool _aButton;
    private bool _bButton;
    private bool _xButton;
    private bool _yButton;
    private bool _isGrounded;
    private float _jumpTime;

    private bool _isRunning;
    private bool _isCrouching;
    private bool _isSliding;
    private bool _isIdle;
    private bool _isFalling;

    private Animator _animator;
    //Cached properties, faster according to overlord Rider
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsFalling = Animator.StringToHash("IsFalling");
    private static readonly int IsCrouching = Animator.StringToHash("IsCrouching");

    // Start is called before the first frame update
    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _isGrounded = false;
        _isCrouching = false;
        _jumpTime = 100f;
        _animator = gameObject.GetComponent<Animator>();
    }

        // Update is called once per frame
    private void Update()
    {
        _leftJoystickHorizontal = Input.GetAxis("LeftJoystickHorizontal");
        _aButton = Input.GetButton("AButton");
        _bButton = Input.GetButton("BButton");
        _xButton = Input.GetButton("XButton");
        _yButton = Input.GetButton("YButton");
    }

    private void FixedUpdate()
    {
        _jumpTime += Time.fixedDeltaTime;
        
        //Copying rigidbody velocity to a variable
        Vector2 velocity = _rb.velocity;
        
        //TODO Add different movement in air ?
        //TODO Make sure only one input can be active i.e slide and attack in the same frame shouldn't work

        if (_bButton)
        {
            //Player wants to crouch
            //Crouch check
            _isCrouching = true;
            _animator.SetBool(IsCrouching, true);
        }
        
        velocity = HorizontalMovement(velocity);
        
        if (_xButton)
        {
            //Player wants to attack
        }

        if (_yButton)
        {
            //Player wants to slide
        }

        velocity = VerticalMovement(velocity);

        //Setting rigidbody velocity equal to changed velocity
        _rb.velocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //If the normal of the collision point is within a certain range
        //Then player is grounded
        Vector2 vec = other.contacts[0].normal;
        if (vec.y == 1.0f && vec.x < 0.3f && vec.x > -0.3f)
        {
            _isGrounded = true;
        }
    }
    
    private void OnCollisionExit2D(Collision2D other)
    {
        //Once player exits any collision he is no longer grounded
        _isGrounded = false;
    }

    /// <summary>
    /// Handles Horizontal movement, Walking / Idle
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns>New updated velocity of the player</returns>
    private Vector2 HorizontalMovement(Vector2 velocity)
    {        
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (_leftJoystickHorizontal != 0f)
        {
            //Move Horizontally
            //Player is running
            velocity.x = runSpeed * Time.fixedDeltaTime * _leftJoystickHorizontal;
            if (_isGrounded)
            {
                //Play run animation
                //Setting animator speed value equal to horizontal velocity, this condition plays an animation
                _animator.SetFloat(Speed, Mathf.Abs(velocity.x));
                
                //Player is grounded and moving left/right, so disable crouch and crouch animation
                _isCrouching = false;
                _animator.SetBool(IsCrouching, false);
            }
        }
        else
        {
            //Player is Idle on the x-axis
            velocity.x = 0f;
            if (_isGrounded)
            {
                //Setting animator speed value equal to horizontal velocity, this condition plays an animation
                _animator.SetFloat(Speed, Mathf.Abs(velocity.x));
            }
        }

        return velocity;
    }
    
    /// <summary>
    /// Handles vertical movement, jumping / falling
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns>New update velocity of the player</returns>
    private Vector2 VerticalMovement(Vector2 velocity)
    {
        _animator.SetBool(IsFalling, false);
        
        //If A is pressed and jumpReleased is false
        if (_aButton && _isGrounded)
        {
            //Start jumping
            _jumpTime = 0f;
            velocity.y += jumpSpeed * Time.fixedDeltaTime;
            _isGrounded = false;
            //Player is jumping
            _animator.SetBool(IsJumping, true);
            _animator.SetBool(IsFalling, false);
        }
        else if (_isGrounded)
        {
            velocity.y = 0f;
            _animator.SetBool(IsJumping, false);
            _animator.SetBool(IsFalling, false);
            //Player is standing on the ground
        }
        //TODO Look into other conditions here like walking of a platform would make you fall
        else if(_jumpTime > jumpTime)
        {
            //Gravity kicks in, i.e player is falling
            velocity.y += Physics2D.gravity.y * Time.fixedDeltaTime;
            
            //Play falling animation
            //Set animation Conditions
            _animator.SetBool(IsJumping, false);
            _animator.SetBool(IsFalling, true);
        }

        if (velocity.y < 0)
        {
            //Set animation conditions
            _animator.SetBool(IsJumping, false);
            _animator.SetBool(IsFalling, true);
        }

        return velocity;
    }

    private Vector2 CrouchMovement(Vector2 velocity)
    {
        //Check if player is crouching, if he is, slow movement and change collider if needed.
        
        //Play crouch animation if, make sure animation is only played once
        
        return velocity;
    }

    private Vector2 SlidingMovement(Vector2 velocity)
    {
        //Player must be running to slide, check if condition meet i.e running and hitting key and grounded
        
        //increase movement speed during slide???
        //change collider if needed
        //Play animation once
        
        //TODO Keep in mind what happens if slide ends under a ceiling (go automatically into crouch?)
        
        return velocity;
    }
}
