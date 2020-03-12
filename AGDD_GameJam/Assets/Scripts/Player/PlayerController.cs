using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    
    //runSpeed = 100, then max speed is -2 / 2

    public SpriteRenderer spriteRenderer;
    
    [Header("Movement")]
    public float runSpeed;
    public float crouchSpeed;
    public float jumpSpeed;
    public float jumpTime;
    public float slideTime;
    
    private Rigidbody2D _rb;
    private float _leftJoystickHorizontal;
    private bool _aButton;
    private bool _bButton;
    private bool _xButton;
    private bool _yButton;
    private bool _isGrounded;
    private float _slideTime;
    private bool _isRunning;
    private bool _isSliding;
    private bool _isIdle;
    private bool _isFalling;

    private Animator _animator;
    //Cached properties, faster according to overlord Rider
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsFalling = Animator.StringToHash("IsFalling");
    private static readonly int IsCrouching = Animator.StringToHash("IsCrouching");
    private static readonly int IsSliding = Animator.StringToHash("IsSliding");
    private static readonly int Attack1 = Animator.StringToHash("Attack1");

    // Start is called before the first frame update
    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _isGrounded = false;
        _slideTime = 100f;
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
        
        //Flip the sprite according to horizontal velocity
        FlipSprite();
    }

    private void FixedUpdate()
    {
        _slideTime += Time.fixedDeltaTime;
        
        //Copying rigidbody velocity to a variable
        Vector2 velocity = _rb.velocity;
        
        //TODO Add different movement in air ?
        //TODO Make sure only one input can be active i.e slide and attack in the same frame shouldn't work

        if (_bButton)
        {
            //Player wants to crouch
            //Crouch check
            CrouchMovement();
        }
        
        velocity = HorizontalMovement(velocity);
        //If isSliding and counter is up stop sliding
        
        if (_xButton)
        {
            //TODO limit attack rate here or in update function
            StartCoroutine(nameof(TriggerOneFrame), Attack1);
        }

        velocity = VerticalMovement(velocity);

        //Setting rigidbody velocity equal to changed velocity
        _rb.velocity = velocity;
        _animator.SetFloat(Speed, Mathf.Abs(velocity.x));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        List<ContactPoint2D> collisionList = other.contacts.ToList();
        
        //Loop through list of all contacts and see if any of them
        //Have a normal vector within X range (i.e flat to almost flat ground)
        //If so we know player is grounded
        foreach (var contact in collisionList)
        {
            Vector2 vec = contact.normal;
            if (vec.y == 1.0f && vec.x < 0.3f && vec.x > -0.3f)
            {
                _isGrounded = true;
            }
        }
    }
    
    private void OnCollisionExit2D(Collision2D other)
    {
        List<ContactPoint2D> collisionList = other.contacts.ToList();
        
        //Loop through list of all contacts and see if any of them
        //Have a normal vector within X range (i.e flat to almost flat ground)
        //If so we know player is still grounded, if for loop reaches end player
        //Is not in contact with the ground

        if (collisionList.Count == 0)
        {
            return;
        }
        
        foreach (var contact in collisionList)
        {
            Vector2 vec = contact.normal;
            if (vec.y == 1.0f && vec.x < 0.3f && vec.x > -0.3f)
            {
                _isGrounded = true;
                return;
            }
        }
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
            //Set velocity.x according to controlled input, ignore if player is sliding
            if(!_isSliding) velocity.x = runSpeed * Time.fixedDeltaTime * _leftJoystickHorizontal;
            if (_isGrounded)
            {
                //Play run animation
                //Player is grounded and moving left/right, so disable crouch and crouch animation
                _animator.SetBool(IsCrouching, false);
                if (_yButton && !_isSliding)
                {
                    //Player is running and grounded and wants to slide
                    _isSliding = true;
                    _slideTime = 0f;
                    _animator.SetBool(IsSliding, true);
                }
            }
        }
        else
        {
            //Player is Idle on the x-axis
            //Set velocity.x to zero if there is not input and player is not sliding
            if(!_isSliding) velocity.x = 0f;
        }
        
        //If player is sliding and the slide time is up
        if (_isSliding && _slideTime > slideTime)
        {
            _animator.SetBool(IsSliding, false);
            _isSliding = false;
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
        
        //If A is pressed and player is grounded
        if (_aButton && _isGrounded)
        {
            //Start jumping
            velocity.y += jumpSpeed * Time.fixedDeltaTime;
            _isGrounded = false;
            //Player is jumping
            _animator.SetBool(IsJumping, true);
            _animator.SetBool(IsFalling, false);
        }
        else if (_isGrounded)
        {
            _animator.SetBool(IsJumping, false);
            _animator.SetBool(IsFalling, false);
            //Player is standing on the ground
        }
        //TODO Look into other conditions here like walking of a platform would make you fall

        if (velocity.y < 0 && !_isGrounded)
        {
            //Set animation conditions
            _animator.SetBool(IsJumping, false);
            _animator.SetBool(IsFalling, true);
        }
        return velocity;
    }

    private void CrouchMovement()
    {
        _animator.SetBool(IsCrouching, true);
    }

    private IEnumerator TriggerOneFrame(int trigger) {
        _animator.SetTrigger(trigger);
        yield return null;
        if (_animator != null) {
            _animator.ResetTrigger(trigger);
        }
    }

    
    private void FlipSprite()
    {
        //Flip sprite according to velocity.x
        //Do nothing if velocity is exactly 0 to remember last direction
        if (_rb.velocity.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (_rb.velocity.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }
    
    //TODO Should we limit player movement during attack animation?
    //TODO Should player be able to stop mid slide?
    //TODO Should we increase speed during slide?
    //TODO Handle case where player slides under something and tries to stand up if map structure allows for that
    
    
    //TODO Player or material must have no friction(or low?) material else he gets stuck
    
}
