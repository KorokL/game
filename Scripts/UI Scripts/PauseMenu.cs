//Author: Lior Korok
//File Name: PauseMenu.cs
//Project Name: Platformer Game
//Creation Date: Sept, 2024
//Modified Date: Jan. 13, 2025
//Description: The logic for the pause menu

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    public Canvas pauseCanvas;
    public PlayerMovement playerMovement;
    public EndMenu endMenu;
    public TimerScript timerScript;
    public EnemyAI enemy;
    public TextMeshProUGUI respawnTxt;
    
    [Header("Pause Menu")]
    public bool isPaused = false;
    public bool forcedPause = false;

    /// <summary>
    /// Start is called once before the first execution of Update after the MonoBehaviour is created
    /// </summary>
    void Start()
    {
        //Disable the pause menu
        pauseCanvas.enabled = false;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        //If the the timer script isn't connected, connect it
        if (!timerScript)
        {
            //Connect the timerScript
            timerScript = GameObject.FindGameObjectsWithTag("Timer")[0].GetComponent<TimerScript>();
        }
        
        //Pause the game if the pausekey is pressed
        if ((Input.GetKeyDown(KeyCode.Escape) && !endMenu.isEnd))
        {
            //Pause the game
            PauseLogic();
        }

        //Respawn if the respawn key is pressed
        if (Input.GetKey(KeyCode.R))
        {
            //Respawn
            Respawn();
        }

        //If you are holding down the shift button, then the button text will change to "restart", instead of "respawn"
        if (Input.GetKey(KeyCode.LeftShift))
        {
            respawnTxt.text = "Restart";
        }
        else
        {
            respawnTxt.text = "Respawn";
        }
    }

    /// <summary>
    /// Logic for pausing the game
    /// </summary>
    public void PauseLogic()
    {
        //If the game is forced to pause, like dying to an enemy, dont allow it to unpause
        if (forcedPause)
        {
            return;
        }

        //If the game is paused, unpause it and vise versa
        if (isPaused)
        {
            //Unpause the game
            isPaused = false;
            pauseCanvas.enabled = false;
            playerMovement.rigidBody.linearVelocity = playerMovement.pauseVelocity;
        }
        else
        {
            //Pause the game
            isPaused = true;
            pauseCanvas.enabled = true;
            playerMovement.pauseVelocity = playerMovement.rigidBody.linearVelocity;
        }
    }
    
    /// <summary>
    /// Respawn the player
    /// </summary>
    public void Respawn()
    {
        //If the player is holding shift while pressing the button, then it will restart the level
        if (Input.GetKey(KeyCode.LeftShift) || CheckpointsHolder.instance.respawnStart)
        {
            //Reset the timer
            timerScript.ResetTimer();
            
            //Respawn from the start
            CheckpointsHolder.instance.respawnStart = true;
        }
        
        //Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu()
    {
        //Reset the timer
        timerScript.ResetTimer();
        
        //Load the menu
        SceneManager.LoadScene(0);
    }
}
