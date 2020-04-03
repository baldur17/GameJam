using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Timeline;
using Debug = UnityEngine.Debug;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    
    //runSpeed = 100, then max speed is -2 / 2

    public SpriteRenderer spriteRenderer;

    #region Public variables

    [Header("Movement")]
    public float runSpeed;
    public float crouchSpeed;
    public float slideSpeed;
    public float slideTime;
    public float slideRate;
    public float timeBetweenSlides;

    [Header("Other")] 
    public LayerMask ledgeLayer;

    public float restartDelay;

    [Header("Jump Feel")]
    public Transform feetPos;
    public float checkRadius;
    public LayerMask whatIsGround;
    public float jumpSpeed;
    public float jumpTime;

    public GameObject dustParticles;
    public GameObject trailEffect;
    public float startTimeBetweenTrail;

    //public Animator camAnim;


    [Header("Ledge Properties")] 
    
    public GameObject ledgeObject;
    public float ledgeRadius;

    public float holdVerticalTimeUp;
    public float holdVerticalTimeDown;

    [HideInInspector] public bool isDetectable, isDead;
    
    [Header("Audio")] 
    public AudioSource heartbeat;
    public AudioSource deathSound;

    #endregion

    #region Private variables

    private Rigidbody2D _rb;
    private float _leftJoystickHorizontal;
    private float _leftJoystickVertical;
    private bool _aButton;
    private bool _bButton;
    private bool _xButton;
    private bool _yButton;
    private bool _isGrounded;
    private bool _isRunning;
    private bool _isSliding;
    private bool _isIdle;
    private bool _isFalling;
    private bool _isLedgeGrabbing;
    private float _grabTimer;
    private float _grabRate;
    private float _jumpTimer;
    private float _jumpRate;
    //_slideTime is how long the player slides for i.e the animation
    //_slideTimer control if the player can slide again
    private float _slideTime;
    private float _slideTimer;

    private float _attackTimer;
    private float _attackRate;

    private bool _isDead = false;
    private float _timeSinceDeath;

    private float _jumpTimeCounter;
    private bool _isJumping;

    private bool _spawnDust;

    private float _initialGravity;
    
    

    private Animator _animator;
    //Cached properties, faster according to overlord Rider
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsFalling = Animator.StringToHash("IsFalling");
    private static readonly int IsCrouching = Animator.StringToHash("IsCrouching");
    private static readonly int IsSliding = Animator.StringToHash("IsSliding");
    private static readonly int Attack1 = Animator.StringToHash("Attack1");
    private static readonly int IsGrabbing = Animator.StringToHash("IsGrabbing");
    private static readonly int IsDead = Animator.StringToHash("IsDead");
    private float _timeBetweenTrail;

    private float _timeSinceVerticalUp = 0.0f;
    private bool _verticalUpActive = false;
    private float _timeSinceVerticalDown = 0.0f;
    private bool _verticalDownActive = false;

    private float _timeSinceLedgeGrab = 0.1f;
    private float _timeSincePause = 0.05f;
    
    
    #endregion

    // Start is called before the first frame update
    private void Start()
    {
        //Ignore the collisions between layer 11 (player) and layer 9 (Enemy)
        Physics2D.IgnoreLayerCollision(11, 9);
        
        //Initialize isDetectable
        isDetectable = true;
        isDead = false;
        
        //_grabTimer control if player can grab, _grabRate is how long until player can try to grab again
        _grabTimer = 0f;
        _grabRate = 0.5f;
        _slideTimer = 0f;
        //Small cooldown on jump to allow for ledge grab to work p roperly, not meant as an actual cooldown
        _jumpRate = 0.5f;
        _jumpTimer = 0f;
        //Small cooldown on attack
        _attackRate = 0.3f;
        _attackTimer = 0f;

        // Reset the time since death
        _timeSinceDeath = 0f;
        
        //Initialize variables
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _isGrounded = false;
        _isLedgeGrabbing = false;
        _slideTime = 100f;
        _animator = gameObject.GetComponent<Animator>();
        // _ledgeColliderPosition = transform.position;

        _initialGravity = _rb.gravityScale;

    }

    // Update is called once per frame
    private void Update()
    {

        

        
        
        if (Time.timeScale == 1f)
        {
            _timeSincePause += Time.deltaTime;
        }
        else
        {
            _timeSincePause = 0f;
        }

        if (_timeSincePause <= 0.05f)
        {
            return;
        }

        _leftJoystickVertical = Input.GetAxis("LeftJoystickVertical");
        _leftJoystickHorizontal = Input.GetAxisRaw("LeftJoystickHorizontal");
        _aButton = Input.GetButtonDown("AButton");
        _bButton = Input.GetButton("BButton");
        _xButton = Input.GetButton("XButton");
        _yButton = Input.GetButton("YButton");

        _isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
        
        //Flip the sprite according to horizontal velocity
        FlipSprite();
        // SetLedgerColliderPosition();

        VerticalJoystickLedge();

        if (Time.time >= _grabTimer)
        {
            LedgeCheck();
        }
        
        if (_timeSinceLedgeGrab >= 0.1f)
        {
            var velocity = _rb.velocity;
            velocity = VerticalMovement(velocity);
            _rb.velocity = velocity;
        }
        else
        {
            _timeSinceLedgeGrab += Time.deltaTime;
        }
        
        

        if (_timeSinceDeath >= restartDelay && _isDead)
        {
            GameManager.instance.RestartLevel();
        }
        if (_isDead)
        {
            _timeSinceDeath += Time.deltaTime;
        }

        if (_isGrounded)
        {
            if (_spawnDust)
            {
                //camAnim.SetTrigger("shake");

                Instantiate(dustParticles, feetPos.position, Quaternion.identity);
                _spawnDust = false;
            }
        }
        else
        {
            _spawnDust = true;
        }
        
        if (_animator.GetBool(IsCrouching) && _isGrounded)
        {
            if (!heartbeat.isPlaying)
            {
                heartbeat.Play();
            }
        }
        else
        {
            if (heartbeat.isPlaying)
            {
                heartbeat.Stop();
            }
        }
    }

    private void VerticalJoystickLedge()
    {
        if (_isLedgeGrabbing && _leftJoystickVertical <= -.4f)
        {
            if (_timeSinceVerticalUp >= holdVerticalTimeUp)
            {
                _aButton = true;
                _timeSinceVerticalUp = 0.0f;
            }

            _timeSinceVerticalUp += Time.deltaTime;
        }
        else
        {
            _timeSinceVerticalUp = 0.0f;
        }

        if (_isLedgeGrabbing && _leftJoystickVertical >= .4f)
        {
            if (_timeSinceVerticalDown >= holdVerticalTimeDown)
            {
                _bButton = true;
                _timeSinceVerticalDown = 0.0f;
            }

            _timeSinceVerticalDown += Time.deltaTime;
        }
        else
        {
            _timeSinceVerticalDown = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        if (_timeSincePause <= 0.05f)
        {
            return;
        }
        
        _slideTime += Time.fixedDeltaTime;
        
        //Copying rigidbody velocity to a variable
        Vector2 velocity = _rb.velocity;

        if (_bButton)
        {
            //Player wants to crouch
            //Crouch check
            CrouchMovement();
        }

        // If player is holding either right or left there will be instansiated a particle effect
        // like dust spawning from ground when running
        if (!(0.1f >= _leftJoystickHorizontal && _leftJoystickHorizontal >= -0.1f))
        {
            if (_timeBetweenTrail <= 0 && _isGrounded)
            {
                Instantiate(trailEffect, feetPos.position, Quaternion.identity);
                _timeBetweenTrail = startTimeBetweenTrail;
            }
            else
            {
                _timeBetweenTrail -= Time.deltaTime;
            }
        }
        if (!_isLedgeGrabbing) velocity = HorizontalMovement(velocity);
        
        if (_xButton && Time.time >= _attackTimer)
        {
            _attackTimer = Time.time + _attackRate;
            StartCoroutine(nameof(TriggerOneFrame), Attack1);
            // _animator.SetTrigger(Attack1);

            // attackWoosh.Play();
        }
        


        _rb.velocity = velocity;

        //Setting rigidbody velocity equal to changed velocity
        // _rb.velocity = velocity;
        _animator.SetFloat(Speed, Mathf.Abs(velocity.x));
        
    }

    /// <summary>
    /// Sets _isGrounded if player is in contact with ground
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter2D(Collision2D other)
    {
//         List<ContactPoint2D> collisionList = other.contacts.ToList();
//         
//         //Loop through list of all contacts and see if any of them
//         //Have a normal vector within X range (i.e flat to almost flat ground)
//         //If so we know player is grounded
//         
//         foreach (var contact in collisionList)
//         {
//             Vector2 vec = contact.normal;
// ;            if (vec.y >= 0.8f)
//             {
//                 _isGrounded = true;
//             }
//         }
    }
    
    /// <summary>
    /// Handles setting the _isGrounded depending on exit collision detection
    /// </summary>
    /// <param name="other">All exit collisions</param>
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
            if (vec.y >= 0.8f)
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
        if (_isDead) return Vector2.zero;
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (_leftJoystickHorizontal != 0f)
        {
            // Disable crouch if player moves left/right
            _animator.SetBool(IsCrouching, false);
            //Move Horizontally
            //Set velocity.x according to controlled input, ignore if player is sliding
            if(!_isSliding && !_isLedgeGrabbing) velocity.x = runSpeed * Time.fixedDeltaTime * _leftJoystickHorizontal;
            if (_isGrounded)
            {
                //Play run animation
                //Player is grounded and moving left/right, so disable crouch and crouch animation
//                _animator.SetBool(IsCrouching, false);
                //Player wants to slide, is not currently sliding and he can slide according to slideRate
                if (_yButton && !_isSliding && Time.time >= _slideTimer)
                {
                    //Player is running and grounded and wants to slide
                    _isSliding = true;
                    _slideTime = 0f;
                    _slideTimer = Time.time + slideRate;
                    _animator.SetBool(IsSliding, true);
                    velocity.x = slideSpeed * Time.fixedDeltaTime * Mathf.Sign(_leftJoystickHorizontal);
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
        // If player is dead he should not move
        if (_isDead) return Vector2.zero;
        // ?
        _animator.SetBool(IsFalling, false);
        
        // If Player lets go of jump button he jumping is set to false
        if (Input.GetButtonUp("AButton"))
        {
            _isJumping = false;
        }
        
        // If player is on ground or ledge, he should begin jumping if jump button is pressed
        if (((_isGrounded || _isLedgeGrabbing) && Time.time >= _jumpTimer) && _aButton)
        {
            _rb.gravityScale = _initialGravity;
            
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
            
            //Cooldown of jump to allow for ledge grab
            _jumpTimer = Time.time + _jumpRate;
            
            // Create a particle effect when he jumps
            Instantiate(dustParticles, feetPos.position, Quaternion.identity);
            // set jumping to true for holding down jump button
            _isJumping = true;
            // initiate the jump constraints
            _jumpTimeCounter = jumpTime;
            // Set y velocity to jump speed
            // Vector2 jumpForceToAdd = new Vector2(0, jumpSpeed);
            //
            // _rb.AddForce(jumpForceToAdd, ForceMode2D.Impulse);
            velocity.y = Vector2.up.y * jumpSpeed;
            
            //Player is jumping
            _isLedgeGrabbing = false;
            _animator.SetBool(IsJumping, true);
            _animator.SetBool(IsFalling, false);
            _animator.SetBool(IsGrabbing, false);
            _animator.SetBool(IsCrouching, false);
        }
        // If the player holds down the a button and is still jumping the velocity should increase
        if (Input.GetButton("AButton") && _isJumping && !(_isLedgeGrabbing))
        {
            if (_jumpTimeCounter > 0)
            {
                velocity.y = Vector2.up.y * jumpSpeed;
            }
            else
            {
                _isJumping = false;
            }
        }
        _jumpTimeCounter -= Time.deltaTime;
        // //If A is pressed and player is either grounded and is allowed to jump according to timer or the gravity is less than 0.1 (a.k.a hanging)
        // if (_aButton && (_isGrounded && Time.time >= _jumpTimer || Math.Abs(_rb.gravityScale) < 0.1f))
        // {
        //
        //     
        //     //Start jumping
        //     
        //
        // }
        if (_isGrounded && !_isJumping && !_isLedgeGrabbing)
        {
            _animator.SetBool(IsJumping, false);
            _animator.SetBool(IsFalling, false);
            _animator.SetBool(IsGrabbing, false);
            //Player is standing on the ground
            
        }
        /*
        else if (_aButton && (Math.Abs(_rb.gravityScale) < 0.1f || Time.time >= _jumpTimer))
        {
            //Cooldown of jump to allow for ledge grab
            _jumpTimer = Time.time + _jumpRate;
            
            //Start jumping
            velocity.y = jumpSpeed * Time.fixedDeltaTime;
            _isGrounded = false;
            //Player is jumping
            _animator.SetBool(IsJumping, true);
            _animator.SetBool(IsFalling, false);
            _animator.SetBool(IsGrabbing, false);
            _isLedgeGrabbing = false;

            //Set gravity scale back
            _rb.gravityScale = 1;
            _grabTimer = Time.time + _grabRate;

        }
        */

        if (velocity.y < -0.1f)
        {
            _isGrounded = false;
            //Set animation conditions
            _animator.SetBool(IsJumping, false);
            _animator.SetBool(IsFalling, true);
        }
        return velocity;
    }

    /// <summary>
    /// Plays crouch animation and handles setting gravity back to 1 if player was ledge grabbing
    /// </summary>
    private void CrouchMovement()
    {
        _grabTimer = Time.time + _grabRate;
        _rb.gravityScale = _initialGravity;
        _animator.SetBool(IsGrabbing, false);
        _isLedgeGrabbing = false;
        _animator.SetBool(IsCrouching, true);
    }

    /// <summary>
    /// Coroutine that handles setting the attack trigger for just one frame
    /// </summary>
    /// <param name="trigger"></param>
    /// <returns>Void</returns>
    private IEnumerator TriggerOneFrame(int trigger) {
        _animator.SetTrigger(trigger);
        yield return null;
        if (_animator != null) {
            _animator.ResetTrigger(trigger);
        }
    }


    /// <summary>
    /// Flips player sprite according to velocity.x
    /// </summary>
    private void FlipSprite()
    {
        Vector3 scale = transform.localScale;
        if (_rb.velocity.x < -0.1f)
        {
            scale.x = -Mathf.Abs(transform.localScale.x);
        }
        else if (_rb.velocity.x > 0.1f)
        {
            scale.x = Mathf.Abs(transform.localScale.x);
        }

        transform.localScale = scale;
    }

    /// <summary>
    /// Position of collider that detects ledges
    /// </summary>
    private void SetLedgerColliderPosition()
    {
        // var transform1 = transform;
        // var position = transform1.position;
        // _ledgeColliderPosition = transform1.localScale.x < 0 ? new Vector2(-0.29f + position.x, 0.5f + position.y) : new Vector2(0.29f + position.x, 0.5f + position.y);

    }

    
    /// <summary>
    /// Checks collision on ledge layer and handles grabbing ledge if there is collision
    /// </summary>
    private void LedgeCheck()
    {
        Collider2D[] ledgeDetected = Physics2D.OverlapCircleAll(ledgeObject.transform.position, ledgeRadius, ledgeLayer);

        if (_jumpTimeCounter >= jumpTime - 0.1f) return;

        if (ledgeDetected.Length > 0)
        {
            if (!_isLedgeGrabbing) _timeSinceLedgeGrab = 0f;
            //Player is grabbing a ledge
            _animator.SetBool(IsGrabbing, true);
            _isLedgeGrabbing = true;
            _rb.gravityScale = 0;
            _rb.velocity = new Vector2(0f, 0f);
        }
        else
        {
            _animator.SetBool(IsGrabbing, false);
            _isLedgeGrabbing = false;
            _rb.gravityScale = _initialGravity;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(ledgeObject.transform.position, ledgeRadius);
    }

    public void Detected()
    {
        if (_isDead) return;
        _animator.SetBool(IsDead, true);
        _isDead = true;
        _timeSinceDeath = 0f;
        if (!deathSound.isPlaying) deathSound.Play();
    }

    public bool GetCrouch()
    {
        return _animator.GetBool(IsCrouching);
    }

    public bool GetIsDead()
    {
        return _isDead;
    }
}
