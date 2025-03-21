//Author: Lior Korok
//File Name: SFXManager.cs
//Project Name: Platformer Game
//Creation Date: Jan, 2024
//Modified Date: Jan. 13, 2025
//Description: Manages the sound effects around the player
//NOTE: The implimitation and the coding of the sound effects are completed,
//however the volume has automatically been set to 0 in the settings due to not having the sound effects that I wanted.

using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [Header("References")] 
    public PlayerMovement playerMovement;
    public PlayerInput playerInput;
    public Grapple grapple;
    public PauseMenu pauseMenu;
    public EndMenu endMenu;

    
    [Header("Sounds")]
    public AudioSource[] audioSource;

    //Constants for the type of sfx
    public const int WALKING = 0;
    public const int WOOSH = 1;
    public const int GRAPPLE = 2;
    
    public void Update()
    {
        //If the player is paused or they finished the level, stop all sound effects
        if (pauseMenu.isPaused || endMenu.isEnd)
        {
            //Go through all sound effects
            for (int i = 0; i < audioSource.Length; i++)
            {
                //Pause the sound effect
                audioSource[i].Pause();
            }
            
            //Frevent from finishing the method
            return;
        }

        //If they player is on the ground, play the walking soundefect
        //DISABLED FOR REASONS MENTIONED ABOVE
        /*if ((playerInput.x != 0 || playerInput.y != 0) && !playerInput.sliding && playerMovement.grounded
        {
            PlaySFX(audioSource[WALKING]);
        }*/

        //Play the woosh sound effect, while adjusting the volume based on the players speed
        audioSource[WOOSH].volume = (playerMovement.rigidBody.linearVelocity.magnitude / 10000f);
        PlaySFX(audioSource[WOOSH]);
        
        //Adjust all of the volume of the sound effects based on the settings
        for (int i = 0; i < audioSource.Length; i++)
        {
            //Change the volume based on the settings
            audioSource[i].volume *= SettingsScript.instance.sfxVolume;
        }
    }

    /// <summary>
    /// Play a looping sound effect 
    /// </summary>
    /// <param name="audio">The sound effect that is looping</param>
    public void PlaySFX(AudioSource audio)
    {
        if (!audio.isPlaying)
        {
            audio.Play();
        }
    }
}
