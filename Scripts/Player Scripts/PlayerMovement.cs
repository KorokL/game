//Author: Lior Korok
//File Name: PlayerMovement.cs
//Project Name: Platformer Game
//Creation Date: Sept, 2024
//Modified Date: Jan. 13, 2025
//Description: Move the player (a capsule) around in a three dimensional space

using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform camHolder;
    public Transform orientation;
    public Rigidbody rigidBody;
    public PlayerCam playerCam;
    public PlayerInput playerInput;
    public PauseMenu pauseMenu;
    public EndMenu endMenu;
    public TimerScript timerScript;

    [Header("General Movement")]
    public float currentSpeed = 4500;
    public float moveSpeed = 4500;
    public float crouchSpeed = 2250;
    public float maxSpeed = 25;
    public bool grounded;
    public LayerMask whatIsGround;

    [Header("Directions")]
    public Vector3 moveDirection;
    public Vector3 speedDirection;

    [Header("Counter Movement")]
    public float counterMovement = 0.175f;
    public float threshold = 0.5f;
    public float maxSlopeAngle = 35f;

    [Header("Sliding")]
    public Vector3 slideScale = new Vector3(1, 0.5f, 1);
    public Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;
    public float maxSlidingSpeed = 35;

    [Header("Wallrunning")]
    public Vector3 wallNormalVector;
    public bool useWallRunning = true;
    public bool isWallRunning;
    public float actualWallRotation;
    public float wallRotationVel;
    public float wallRunGravity = 1;
    public float initialForce = 20f;
    public float escapeForce = 1000f;
    public float wallRunRotation;
    public float wallRunRotateAmount = 10f;
    public bool onWall;

    [Header("Jumping")]
    public bool readyToJump = true;
    public float jumpCooldown = 0.25f;
    public float jumpForce = 550f;
    
    [Header("Cancelling")]
    public bool cancellingGrounded;
    public bool cancellingWallRun;
    public bool cancellingWall;

    [Header("Misc")]
    public float groundAngle;
    public RaycastHit slopeHit;
    
    //Holds the normal vector of the player
    public Vector3 normalVector = Vector3.up;
    
    //Holds the previous posi
    public Vector3 previousPosition;
    
    //Holds the players velocity when they are paused
    public Vector3 pauseVelocity;

    //The layer and the object of the checkpoint
    public LayerMask whatIsCheckpoint;
    public GameObject respawnObj;

    //Stores the layers of the end
    public LayerMask whatIsEnd;
    
    //Stores the layers of the death plane
    public LayerMask whatIsDeath;
    
    //Stores the layer of the bounce pad, and its force
    public LayerMask whatIsBounce;
    public float bounceForce = 3000;

    /// <summary>
    /// Start is called once before the first execution of Update after the MonoBehaviour is created
    /// </summary>
    void Start()
    {
        //Grabs the components
        playerInput = GetComponent<PlayerInput>();
        rigidBody = GetComponent<Rigidbody>();
        
        //Grabs and stores the scale of the player
        playerScale = transform.localScale;
    }

    /// <summary>
    /// FixedUpdate is called once per fixed framerate frame
    /// </summary>
    void FixedUpdate()
    {
        //Checks if the player is currently paused or finished the level
        if (pauseMenu.isPaused || endMenu.isEnd)
        {
            //Disable the players movement and freeze them in air
            rigidBody.linearVelocity = Vector3.zero;
            rigidBody.useGravity = false;
            return;
        }
        else
        {
            //Continue the players movement
            rigidBody.useGravity = true;
        }

        //Calculate neccesary variables used in movement
        CalculateFloorAngle();
        CalculateDirections();

        //Counter the movements first, then move the player
        CounterMovement(playerInput.x, playerInput.y, playerInput.FindVelRelativeToLook());
        Movement();
    }

    /// <summary>
    /// LateUpdate is called after Update and FixedUpdate
    /// </summary>
    void LateUpdate()
    {
        //Checks if the player is currently paused or finished the level
        if (pauseMenu.isPaused || endMenu.isEnd)
        {
            //Dont continue with the method
            return;
        }

        //Check if the player is wall running
        if (isWallRunning)
        {
            //Make the player run on the wall
            WallRunning();
        }
        
        //Store the previous position of the player
        previousPosition = transform.position;
    }

    /// <summary>
    /// Make the player start sliding/crouching
    /// </summary>
    public void StartSlide()
    {
        //Make the player smaller
        transform.localScale = slideScale;

        //Check if the player is sliding or crouching
        if (playerInput.sliding)
        {
            //Check if the player is on the ground and has a specific speed
            if (grounded && rigidBody.linearVelocity.magnitude <= maxSlidingSpeed)
            {
                //Force the player down on the ground, then make them slide in their move direction
                rigidBody.AddForce(Vector3.down * 50f, ForceMode.Impulse);
                rigidBody.AddForce(moveDirection * slideForce);
            }
        }
        else if (playerInput.crouching)
        {
            //Set the current speed to the crouch speed
            currentSpeed = crouchSpeed;
        }
    }

    /// <summary>
    /// Make the player return back to normal when sliding
    /// </summary>
    public void StopSlide()
    {
        //Make the player back to normal scale, and move them up so they are not clipping in the ground
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

        //Set the current speed back to normal
        currentSpeed = moveSpeed;
    }

    /// <summary>
    /// Move the player
    /// </summary>
    public void Movement()
    {
        //Holds the speed multiplier
        float multiplier;

        //Check if the player is on ground, or sliding and adjust the speed multiplier on that
        if (!grounded)
        {
            multiplier = 1.05f;
        }
        else
        {
            multiplier = 1f;
        }
        if (playerInput.sliding && !OnSlope())
        {
            multiplier = 0f;
        }

        //Extra gravity to prevent bouncing
        rigidBody.AddForce(Vector3.down * (Time.deltaTime * 400));

        //Check if the player is pressing the jump button
        if (playerInput.jumping)
        {
            //Make the player jump
            Jump();
        }

        //Check if the player is on the floor and is ready to jump
        if (grounded && readyToJump)
        {
            //Check if the player is sliding
            if (playerInput.sliding)
            {
                //Extra gravity to prevent bouncing
                rigidBody.AddForce(Vector3.down * (Time.deltaTime * 3000f));

                //If on the player is on a slope, move the player depending on the slope angle
                rigidBody.AddForce(GetSlopeDirection(Vector3.down) * (float)Math.Pow(groundAngle, 1.1));
                rigidBody.AddForce(orientation.transform.forward * (playerInput.y * (float)Math.Pow(groundAngle, 1.3)));
                rigidBody.AddForce(orientation.transform.right * (playerInput.x * (float)Math.Pow(groundAngle, 1.3)));
                return;
            }
            
            //Check if the player is on a slope
            if (OnSlope())
            {
                //Move the player in relationi to the slope
                rigidBody.AddForce(GetSlopeDirection(orientation.transform.forward * playerInput.y) * (currentSpeed * Time.deltaTime * multiplier));
                rigidBody.AddForce(GetSlopeDirection(orientation.transform.right * playerInput.x) * (currentSpeed * Time.deltaTime * multiplier));

                //Extra gravity to prevent bouncing
                rigidBody.AddForce(Vector3.down * 80f);
            }
        }

        // Move the player
        rigidBody.AddForce(orientation.transform.right * (playerInput.x * currentSpeed * Time.deltaTime * multiplier));
        rigidBody.AddForce(orientation.transform.forward * (playerInput.y * currentSpeed * Time.deltaTime * multiplier));
    }

    /// <summary>
    /// MAke the player jump
    /// </summary>
    public void Jump()
    {
        //Check if the player is on the floor or wall, and is ready to jump
        if (grounded && readyToJump)
        {
            //Make the player not ready to jump
            readyToJump = false;

            //Add the forces neccesary for the jumping effect
            rigidBody.AddForce(Vector3.up * (jumpForce * 1.5f));
            rigidBody.AddForce(normalVector * (jumpForce * 0.5f));

            //Reset the jump in 0.25 seconds
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        else if (isWallRunning && readyToJump)
        {
            //Jump off the wall
            CancelWallRun();
        }
    }

    /// <summary>
    /// Start the wall run
    /// </summary>
    public void StartWallRun()
    {
        //Check if the player is on the ground and we want to wall run
        if (!grounded && useWallRunning)
        {
            //Check if the player is not on the wall
            if (!isWallRunning)
            {
                //Cancel all y momentum and then applies an upwards force.
                rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, 0f, rigidBody.linearVelocity.z);
                rigidBody.AddForce(Vector3.up * initialForce, ForceMode.Impulse);
            }

            //Set wallrunning to true
            isWallRunning = true;
        }
    }

    /// <summary>
    /// Cancel the wall run
    /// </summary>
    public void CancelWallRun()
    {
        //Jump off the wall
        rigidBody.AddForce(wallNormalVector * (escapeForce * Time.deltaTime), ForceMode.Impulse);
        
        //Set wallrunning to false
        isWallRunning = false;
    }

    /// <summary>
    /// Add forces nessesary during the wall run
    /// </summary>
    public void WallRunning()
    {
        //Stick the player on the wall during the wall run
        rigidBody.AddForce(-wallNormalVector * (escapeForce * Time.deltaTime));
    }

    /// <summary>
    /// Reset the jump
    /// </summary>
    public void ResetJump()
    {
        //Make the player ready to jump
        readyToJump = true;
    }

    
    /// <summary>
    /// Counter the movement of the player
    /// </summary>
    /// <param name="x">If the player is pressing the sideways keys (A and D)</param>
    /// <param name="y">If the player is pressing the forword/backward keys (W and S)</param>
    /// <param name="mag">The velocity of the player relative to where they are looking</param>
    public void CounterMovement(float x, float y, Vector3 mag)
    {
        //If the player is above speed, cancel out the x/y to prevent adding forces
        if (x != 0 && Math.Abs(mag.x) > maxSpeed)
        {
            //Set the x to 0
            playerInput.x = 0f;
        }
        if (y != 0 && Math.Abs(mag.y) > maxSpeed)
        {
            //Set the y to 0
            playerInput.y = 0f;
        }
        
        //If the player is in the air, dont counter movement
        if (!grounded || playerInput.jumping)
        {
            return;
        }

        //Slow down sliding when on ground
        if (playerInput.sliding)
        {
            //Slow down sliding based on your current speed
            rigidBody.AddForce(-rigidBody.linearVelocity.normalized * (currentSpeed * slideCounterMovement * Time.deltaTime));
            return;
        }

        //If the player is not pressing buttons and the speed is above a threshold, counter the movement
        if (MathF.Abs(mag.x) > threshold && x == 0)
        {
            //Counter the sideways movement
            rigidBody.AddForce(orientation.transform.right * (currentSpeed * -mag.x * counterMovement * Time.deltaTime));
        }
        if (MathF.Abs(mag.y) > threshold && y == 0)
        {
            //Counter the forward/backward movement
            rigidBody.AddForce(orientation.transform.forward * (currentSpeed * -mag.y * counterMovement * Time.deltaTime));
        }

        //Cehck if the player is generally above the maximum speed
        if (GetCurrentSpeed() > maxSpeed)
        {
            //Counter the movement force
            rigidBody.AddForce(-rigidBody.linearVelocity.normalized * (currentSpeed * 2 * counterMovement * Time.deltaTime));
        }
    }

    /// <summary>
    /// Check if the player is collided with a trigger
    /// </summary>
    /// <param name="other">The object the player has collided with</param>
    public void OnTriggerEnter(Collider other)
    {
        //Check if the player has collided with a checkpoint
        if (whatIsCheckpoint == (whatIsCheckpoint | (1 << other.gameObject.layer)))
        {
            //Set the respawn object to the gameobject
            respawnObj = other.gameObject;

            //Set teh respawn point to the gameobject position
            CheckpointsHolder.instance.respawnStart = false;
            CheckpointsHolder.instance.respawnPos = new Vector3(respawnObj.transform.position.x, respawnObj.transform.position.y - respawnObj.transform.localScale.y / 2,
                respawnObj.transform.position.z);
            CheckpointsHolder.instance.respawnRot = respawnObj.transform.eulerAngles;
        }

        //Check if the player has collided with the end object
        if (whatIsEnd == (whatIsEnd | (1 << other.gameObject.layer)))
        {
            //Enable the end menu
            endMenu.isEnd = true;
        }

        //Check if the player has collided with the end plane
        if (whatIsDeath == (whatIsDeath | (1 << other.gameObject.layer)))
        {
            //Respawn the player
            pauseMenu.Respawn();
        }
    }

    /// <summary>
    /// Check if the player has collided with an object
    /// </summary>
    /// <param name="other">The object the player has collided with</param>
    public void OnCollisionEnter(Collision other)
    {
        //Check if the player has collided with a bounce pad
        if (whatIsBounce == (whatIsBounce | (1 << other.gameObject.layer)))
        {
            //Make the player bounce in the direction of where the bounce pad is facing relative to the player
            rigidBody.AddForce(other.contacts[0].normal * bounceForce);
        }

        //Check if the player has collided with the end object
        if (whatIsEnd == (whatIsEnd | (1 << other.gameObject.layer)))
        {
            //Enable the end menu
            endMenu.isEnd = true;
        }
    }

    /// <summary>
    /// Check if the player is currently in collision with an object
    /// </summary>
    /// <param name="other">The object that the player is in collision with</param>
    public void OnCollisionStay(Collision other)
    {
        //If the player is not touching the ground, dont do anything
        if (whatIsGround != (whatIsGround | (1 << other.gameObject.layer)))
        {
            //Dont continue with the method
            return;
        }

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++)
        {
            //Check if the player is in contact with a floor/slope
            if (IsFloor(other.contacts[i].normal))
            {
                //Check if the player was wallrunning
                if (isWallRunning)
                {
                    //Make them stop wallruning
                    isWallRunning = false;
                }

                //Checks if the player has landed from a jump to decrease its speed
                if (!grounded && GetCurrentSpeed() > 1)
                {
                    rigidBody.AddForce(-speedDirection * jumpForce * 2);
                }

                //Set the variables to make the player on the ground
                grounded = true;
                cancellingGrounded = false;
                CancelInvoke(nameof(StopGrounded));
            }
            
            //Check if the player is on the wall
            if (OnWall(other.contacts[i].normal))
            {
                //Set the wall noraml to the collider normal
                wallNormalVector = other.contacts[i].normal;
                
                //Startthe wall run, and set its variables 
                StartWallRun();
                onWall = true;
                cancellingWall = false;
                CancelInvoke(nameof(StopWall));
            }
        }

        //Set the delay for the ground/wall cancel
        float delay = 3f;

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        if (!cancellingGrounded)
        {
            //Cencel and stop the ground (make the player be in the air)
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
        if (!cancellingWall)
        {
            //Cencel and stop the wall (make the player be in the air)
            cancellingWall = true;
            Invoke(nameof(StopWall), Time.deltaTime * delay);
        }
    }

    /// <summary>
    /// Check if the player is on a floor
    /// </summary>
    /// <param name="v">The normal of an object</param>
    /// <returns>True if the ground is less then the max slope angle</returns>
    public bool IsFloor(Vector3 v)
    {
        return Vector3.Angle(Vector3.up, v) < maxSlopeAngle;
    }

    /// <summary>
    /// Checks if the player is on a wall
    /// </summary>
    /// <param name="v">The normal of an object</param>
    /// <returns>True if the wall is at a ~90 degree angle</returns>
    public bool OnWall(Vector3 v)
    {
        return Math.Abs(90f - Vector3.Angle(Vector3.up, v)) < 0.1f;
    }

    /// <summary>
    /// Calculate the ground angle
    /// </summary>
    public void CalculateFloorAngle()
    {
        //Cast a ray downward relative to the player to check if there is a floor
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1f + 0.5f))
        {
            //Calculate the ground angle
            groundAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
        }
        else
        {
            //Set the angle to 0
            groundAngle = 0;
        }
    }

    /// <summary>
    /// Check if the player is on a slope
    /// </summary>
    /// <returns>Returns true if the player is on a slope</returns>
    public bool OnSlope()
    {
        //Check the ground angle if its not flat, and less then the max slope angle
        return groundAngle != 0 && groundAngle < maxSlopeAngle;
    }

    /// <summary>
    /// Stop the player from being grounded (make the player in the air)
    /// </summary>
    public void StopGrounded()
    {
        grounded = false;
    }

    /// <summary>
    /// Stop the player from being on the wall (make the player in the air)
    /// </summary>
    public void StopWall()
    {
        onWall = false;
        isWallRunning = false;
    }

    /// <summary>
    /// Calculate the slope direction relative to a direction
    /// </summary>
    /// <param name="direction">The direction of an object</param>
    /// <returns>The direction of the slope relative to the direction of a player</returns>
    public Vector3 GetSlopeDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    /// <summary>
    /// Get the current flat speed of the player
    /// </summary>
    /// <returns>A number that is the speed</returns>
    public double GetCurrentSpeed()
    {
        //Calculate the flat speed of the player
        return Math.Sqrt(rigidBody.linearVelocity.x * rigidBody.linearVelocity.x + rigidBody.linearVelocity.z * rigidBody.linearVelocity.z);
    }

    /// <summary>
    /// Calculate the directions of the player
    /// </summary>
    public void CalculateDirections()
    {
        //Calculate the move and speed direction of the player
        moveDirection = orientation.forward * playerInput.y + orientation.right * playerInput.x;
        speedDirection = transform.position - previousPosition;
    }
}