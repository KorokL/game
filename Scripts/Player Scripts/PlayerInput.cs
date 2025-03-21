//Author: Lior Korok
//File Name: PlayerInput.cs
//Project Name: Platformer Game
//Creation Date: Sept, 2024
//Modified Date: Jan. 13, 2025
//Description: Grab the inputs that is used to control the player

using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode moveForward = KeyCode.W;
    public KeyCode moveBackward = KeyCode.S;
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode slideKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftShift;
    public KeyCode grappleKey = KeyCode.Mouse0;

    [Header("PlayerStates")]
    public float x; 
    public float y;
    public bool jumping;
    public bool sliding;
    public bool crouching;
    public bool grappling;

    [Header("References")]
    public PlayerMovement player;
    public PauseMenu pauseMenu;
    public EndMenu endMenu;
    public Grapple grapple;

    [Header("SoundFX")] 
    public AudioClip walkingSoundClip;
    public AudioSource walkingSound;
    
    [Header("Misc")] 
    public float slideCrouchSpeedThreshold = 10f;

    /// <summary>
    /// FixedUpdate is called once per a set framerate
    /// </summary>
    void Update()
    {
        //Check if the game is paused or the level is finished
        if (pauseMenu.isPaused || endMenu.isEnd)
        {
            //Dont continue the method
            return;
        }
        
        //Set the x and y to 0 
        y = 0;
        x = 0;

        //Check if the player is holding the movement buttons, then increment/decrement the x/y by 1
        if (Input.GetKey(moveForward))
        {
            y++;
        }
        if (Input.GetKey(moveBackward))
        {
            y--;
        }
        if (Input.GetKey(moveRight))
        {
            x++;
        }
        if (Input.GetKey(moveLeft))
        {
            x--;
        }

        //Check if the player is pressing a specific key, and if certain conditions are met, make that player enabled to do an action
        jumping = Input.GetKey(jumpKey);
        crouching = Input.GetKey(crouchKey) && player.rigidBody.linearVelocity.magnitude < slideCrouchSpeedThreshold;
        sliding = Input.GetKey(slideKey) && player.rigidBody.linearVelocity.magnitude > slideCrouchSpeedThreshold;
        grappling = Input.GetKey(grappleKey) && grapple.joint;

        //If the player pressed or released the slidekey, make the player start or stop sliding
        if (Input.GetKeyDown(slideKey))
        {
            //Start the slide
            player.StartSlide();
        }
        if (Input.GetKeyUp(slideKey))
        {
            //Stop the slide
            player.StopSlide();
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// </summary>
    /// <returns>The plane velocity of the player ralative to where they are looking o</returns>
    public Vector2 FindVelRelativeToLook()
    {
        //Find the look and move angle in degrees
        float lookAngle = player.orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(player.rigidBody.linearVelocity.x, player.rigidBody.linearVelocity.z) * Mathf.Rad2Deg;

        //Find the angle difference between your look and move angle
        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        //FInd the velocity magnuitude
        float magnitude = player.rigidBody.linearVelocity.magnitude;
        float yMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);

        //Return the velocity
        return new Vector2(xMag, yMag);
    }
}
