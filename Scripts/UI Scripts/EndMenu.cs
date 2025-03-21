//Author: Lior Korok
//File Name: EndMenu.cs
//Project Name: Platformer Game
//Creation Date: Sept, 2024
//Modified Date: Jan. 13, 2025
//Description: The logic for when the player finished the level

using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
    [Header("References")]
    public Canvas endCanvas;
    public PlayerMovement playerMovement;
    public TimerScript timerScript;
    
    [Header("End")]
    public bool isEnd = false;

    /// <summary>
    /// Start is called once before the first execution of Update after the MonoBehaviour is created
    /// </summary>
    void Start()
    {
        //Disable the end menu
        endCanvas.enabled = false;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        //If the timer script isnt connected, then connect it
        if (!timerScript)
        {
            //Connect the timer script
            timerScript = GameObject.FindGameObjectsWithTag("Timer")[0].GetComponent<TimerScript>();
        }
        
        //If the game
        if (isEnd)
        {
            //If the player respawns, respawn at the start
            CheckpointsHolder.instance.respawnStart = true;
            
            //If the player enabled the quickfinish, then go to the next level immediately
            if (SettingsScript.instance.quickFinish)
            {
                //Go to the next level
                Next();
            }
            
            //Stop the player
            playerMovement.rigidBody.linearVelocity = Vector3.zero;
            
            //Enable the end canvas
            endCanvas.enabled = true;
        }
    }

    /// <summary>
    /// Reset the level
    /// </summary>
    public void Reset()
    {
        //Check if there is a new personal best, and then reset the timer
        timerScript.CheckAndSetPersonalBest();
        timerScript.ResetTimer();
        
        //Reset the level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Go to the next level
    /// </summary>
    public void Next()
    {
        //Check if there is a new personal best, and then reset the timer
        timerScript.CheckAndSetPersonalBest();
        timerScript.ResetTimer();

        //If the player is at the last level, go to the menu
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCount - 1)
        {
            Menu();
            return;
        }
        
        //Go to the next level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Go to the menu
    /// </summary>
    public void Menu()
    {
        //Check if there is a new personal best, and then reset the timer
        timerScript.CheckAndSetPersonalBest();
        timerScript.ResetTimer();
        
        //Go to the menu
        SceneManager.LoadScene(0);
    }
}
