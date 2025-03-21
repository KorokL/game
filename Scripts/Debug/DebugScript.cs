//Author: Lior Korok
//File Name: DebugScript.cs
//Project Name: Platformer Game
//Creation Date: Sept, 2024
//Modified Date: Jan. 13, 2025
//Description: A debug script to shot FPS, and other values

using TMPro;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    //Components nessesary to show fps
    public TextMeshProUGUI text;
    public PlayerMovement pm;

    //Store the fps text and the deltatime
    public string fpsText;
    public float deltaTime;

    // Update is called once per frame
    void Update()
    {
        //Check if debug mode has been disabled in the settings
        if (SettingsScript.instance.debugMode == false)
        {
            //Disable the gameobject
            gameObject.SetActive(false);
            return;
        }
        
        //Get the fps of the game
        GetFPS();
        
        //Print the fps on a hud
        text.text = "FPS: " + fpsText;
    }

    /// <summary>
    /// Gets the current running fps of the game
    /// </summary>
    public void GetFPS()
    {
        //Calculate the fps of the running game
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText = Mathf.Ceil(fps).ToString();
    }
}
