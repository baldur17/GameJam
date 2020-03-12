using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public float gravityModifier = 1f;
    public float minGroundNormalY = .65f;
    
    
    protected Vector2 Velocity;
    protected Rigidbody2D Rigidbody2D;

    protected const float MinMoveDistance = 0.001f;
    protected const float ShellRadius = 0.0f;
    protected bool Grounded;
    protected Vector2 GroundNormal;
    
    
    protected ContactFilter2D ContactFilter2D;
    protected RaycastHit2D[] HitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> HitBufferList = new List<RaycastHit2D>(16);
    
    private void OnEnable()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        //Not checking contact against triggers
        ContactFilter2D.useTriggers = false;
        //Use the settings from the Physics2D settings to determined which layer to detect against
        ContactFilter2D.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        ContactFilter2D.useLayerMask = true;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
        Velocity += Physics2D.gravity * (gravityModifier * Time.fixedDeltaTime);

        //Set grounded as false each frame until collision with ground is confirmed
        Grounded = false;

        //Change in position
        Vector2 deltaPosition = Velocity * Time.fixedDeltaTime;

        Vector2 movement = Vector2.up * deltaPosition.y;
        
        Move(movement, true);
    }

    private void Move(Vector2 movement, bool yMovement)
    {
        //Set distance
        float distance = movement.magnitude;
        
        //Ignore this if distance in less than MinMoveDistance
        if (distance > MinMoveDistance)
        {
            int count = Rigidbody2D.Cast(movement, ContactFilter2D, HitBuffer, distance + ShellRadius);
            HitBufferList.Clear();

            for (int i = 0; i < count; i++)
            {
                //List of object that are going to overlap
                HitBufferList.Add(HitBuffer[i]);
            }

            for (int i = 0; i < HitBufferList.Count; i++)
            {
                //Check the normal of each RayCast2D and compare it to a minimum value
                Vector2 currentNormal = HitBufferList[i].normal;
                //Determine if the player is grounded or not
                if (currentNormal.y > minGroundNormalY)
                {
                    Grounded = true;
                    if (yMovement)
                    {
                        GroundNormal = currentNormal;
                        currentNormal.x = 0f;
                    }
                }
                //Getting the difference between velocity and current normal
                //and determining if we need to subtract from the current velocity
                //to prevent object from entering into another collider
                float projection = Vector2.Dot(Velocity, currentNormal);
                if (projection < 0)
                {
                    Velocity -= projection * currentNormal;
                }
                
                //
                float modifiedDistance = HitBufferList[i].distance - ShellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
            
        }
        
        Rigidbody2D.position += movement.normalized * distance;
    }
}
