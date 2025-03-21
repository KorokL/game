//Author: Lior Korok
//File Name: TimerScript.cs
//Project Name: Platformer Game
//Creation Date: Sept, 2024
//Modified Date: Jan. 13, 2025
//Description: Script for the timer in levels

using System.Diagnostics;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerScript : MonoBehaviour
{
    [Header("References")]
    public Stopwatch stopwatch = new Stopwatch();
    public TextMeshProUGUI timerTxt;
    public EndMenu endMenu;
    public PauseMenu pauseMenu;
    public static TimerScript instance;
    
    [Header("Timer")]
    public string stringTime;
    public int millisecondsPassed;

    /// <summary>
    /// Start is called once before the first execution of Update after the MonoBehaviour is created
    /// </summary>
    void Start()
    {
        //Make sure that only one timer exists at a time
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        //If we are in the menu, destroy the object
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            //Destroy the timer
            Destroy(gameObject);
            
            //End the method
            return;
        }
        
        //If the end or the pause menu isnt connected, connect them
        if (!endMenu || !pauseMenu)
        {
            endMenu = GameObject.FindGameObjectsWithTag("End")[0].GetComponent<EndMenu>();
            pauseMenu = GameObject.FindGameObjectsWithTag("Pause")[0].GetComponent<PauseMenu>();
        }
        
        //If the player hasen't finished the level, or isnt paused, start and continue the stopwatch
        if (!endMenu.isEnd && !pauseMenu.isPaused)
        {
            //Start the stopwatch
            stopwatch.Start();
        }
        else
        {
            //Stop the stopwatch
            stopwatch.Stop();
        }

        //Convert the timer to a string (NOTE: this will cause an error on the first frame of the level, but this is accounted for)
        stringTime = stopwatch.Elapsed.ToString().Substring(3, 9).Replace('.', ':');

        //Put the time on the screen
        timerTxt.text = stringTime;
    }

    /// <summary>
    /// Reset the timer
    /// </summary>
    public void ResetTimer()
    {
        //Reset the stopwatch
        stopwatch.Reset();
    }
    
    /// <summary>
    /// Check if you set a new time, and set it
    /// </summary>
    public void CheckAndSetPersonalBest()
    {
        //Store the path of your times
        string path = Application.streamingAssetsPath + "/Times.txt";
        
        //Grab all of your times
        string[] lines = File.ReadAllLines(path);
        
        //Check if you got a new personal best on the current level
        if (stopwatch.ElapsedMilliseconds < long.Parse(lines[SceneManager.GetActiveScene().buildIndex - 1].Substring(9)))
        {
            //Change the time on the current level for your personal best
            lines[SceneManager.GetActiveScene().buildIndex - 1] = stringTime + " " + stopwatch.ElapsedMilliseconds;

            //Write the time on the text file
            File.WriteAllLines(path, lines);
        }
    }
}
