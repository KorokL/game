//Author: Lior Korok
//File Name: UIFunctions.cs
//Project Name: Platformer Game
//Creation Date: Sept, 2024
//Modified Date: Jan. 13, 2025
//Description: Sets and manages all of the functions for the UI in the main menu

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using File = System.IO.File;

public class UIFunctions : MonoBehaviour
{
    [Header("References")]
    public Camera camera;
    public TextMeshProUGUI[] settingsTmp;
    public Slider[] slider;
    
    [Header("Camera Info")]
    public float currentRotation;
    public float endRotation;
    
    public float cameraDir;
    
    [Header("Timer")] 
    public TextMeshProUGUI[] timerTmp;

    [Header("Defaults")] 
    public int wallRunShakeDefault = 45;
    public int sensitivityDefault = 500;
    public int fovDefault = 90;
    public int musicDefault = 40;
    public int sfxDefault = 0;
    
    //Constants for all of the settings
    const int SCREEN_SHAKE = 0;
    const int SENSITIVITY = 1;
    const int FOV = 2;
    const int QUICK_FINISH = 3;
    const int PARTICLES = 4;
    const int MUSIC = 5;
    const int SFX = 6;
    const int DEBUG_MODE = 7;
    
    /// <summary>
    /// Start is called once before Update
    /// </summary>
    void Start()
    {
        //Dont lock the cursor and make it visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        //Update the times on levels
        UpdateTimers();
        
        //Update the settings page
        UpdateSettings();

        //Lerp the camera directions for smooth transitions
        cameraDir = Mathf.Lerp(cameraDir, endRotation, 5 * Time.deltaTime);
        camera.transform.eulerAngles = new Vector3(0f, cameraDir, 0f);
    }

    /// <summary>
    /// Turns the camera left by 90 degrees
    /// </summary>
    public void TurnLeft()
    {
        //Set the camera direction to the current direction
        cameraDir = camera.transform.eulerAngles.y;
        
        //Set the end camera direction to 90 degrees to the left
        //This is a bugfix, if i do endRotation -= 90, it will bug out
        endRotation = cameraDir - 90;
        
        //Snap the end rotation to a constant (bugfix)
        SnapRotation();
    }

    /// <summary>
    /// Turns the camera right by 90 degrees
    /// </summary>
    public void TurnRight()
    {
        //Set the camera direction to the current direction
        cameraDir = camera.transform.eulerAngles.y;

        //Set the end camera direction to 90 degrees to the right
        //This is a bugfix, if i do endRotation += 90, it will bug out
        endRotation = cameraDir + 90;

        //Snap the end rotation to a constant (bugfix)
        SnapRotation();
    }

    /// <summary>
    /// Play a level selected
    /// </summary>
    /// <param name="level">The level the user selected</param>
    public void PlayLevel(int level)
    {
        //Set the respawn point to the start
        CheckpointsHolder.instance.respawnStart = true;

        //Load the level
        SceneManager.LoadScene(level);
    }

    /// <summary>
    /// Exit the application
    /// </summary>
    public void ExitGame()
    {
        //Alt + F4's the game
        Application.Quit();
    }

    /// <summary>
    /// Sets the slider for the wallrun shake amount
    /// </summary>
    public void ShakeSlider()
    {
        //Sets the wallrun shake amount to the slider amount
        SettingsScript.instance.wallRunShakeAmount = (int)slider[SCREEN_SHAKE].value;
    }

    /// <summary>
    /// Reset the wallrun shake amount
    /// </summary>
    public void ResetShake()
    {
        //Reset the shake amount back to its default value
        SettingsScript.instance.wallRunShakeAmount = wallRunShakeDefault;
    }

    /// <summary>
    /// Sets the slider for the sensitivity
    /// </summary>
    public void SensitivitySlider()
    {
        //Sets the sensitivity to the slider amount
        SettingsScript.instance.cameraSensitivity = (int)slider[SENSITIVITY].value;
    }

    /// <summary>
    /// Reset the sensitivity amount
    /// </summary>
    public void ResetSensitivity()
    {
        //Reset the sensitivity amount back to its default value
        SettingsScript.instance.cameraSensitivity = sensitivityDefault;
    }

    /// <summary>
    /// Set the slider for the FOV
    /// </summary>
    public void FOVSlider()
    {
        //Sets the FOV to the slider amount
        SettingsScript.instance.fov = (int)slider[FOV].value;
    }

    /// <summary>
    /// Reset the FOV to its default
    /// </summary>
    public void ResetFOV()
    {
        //Resets the fov back to its default value
        SettingsScript.instance.fov = fovDefault;
    }

    /// <summary>
    /// Sets the slider for the music volume
    /// </summary>
    public void MusicSlider()
    {
        //Sets the music volume to the slider amount
        SettingsScript.instance.musicVolume = (int)slider[MUSIC].value;
    }

    /// <summary>
    /// Reset the music volume to its default
    /// </summary>
    public void ResetMusic()
    {
        //Resets the music volume back to its default amount
        SettingsScript.instance.musicVolume = musicDefault;
    }

    /// <summary>
    /// Sets the slider for the sound effects
    /// </summary>
    public void SFXSlider()
    {
        //Sets the sound effect volume to the slider amount
        SettingsScript.instance.sfxVolume = (int)slider[SFX].value;
    }

    /// <summary>
    /// Resets the sound effect volume to its default
    /// </summary>
    public void ResetSFX()
    {
        //Reset teh sound effect volume to its default
        SettingsScript.instance.sfxVolume = sfxDefault;
    }

    /// <summary>
    /// Sets the quick finish setting
    /// </summary>
    public void QuickFinish()
    {
        //Swaps the quick finish bool setting
        SettingsScript.instance.quickFinish = !SettingsScript.instance.quickFinish;
    }

    /// <summary>
    /// Sets the particles setting
    /// </summary>
    public void Particles()
    {
        //Swaps the particles bool setting
        SettingsScript.instance.particles = !SettingsScript.instance.particles;
    }

    /// <summary>
    /// Sets the debug mode setting
    /// </summary>
    public void DebugMode()
    {
        //Swaps the debug mode setting
        SettingsScript.instance.debugMode = !SettingsScript.instance.debugMode;
    }
    
    /// <summary>
    /// Update the setting menu to reflect the actual settings
    /// </summary>
    public void UpdateSettings()
    {
        //Update the wallrun screen shake setting
        slider[SCREEN_SHAKE].value = SettingsScript.instance.wallRunShakeAmount;
        if (SettingsScript.instance.wallRunShakeAmount == 0)
        {
            //If the setting is at 0, tell the player it is off
            settingsTmp[SCREEN_SHAKE].text = "Off";
        }
        else if (SettingsScript.instance.wallRunShakeAmount == 90)
        {
            //when you are on the wall, your camera is going to go crazy. why do you want to do that? do you want motion sickness?
            settingsTmp[SCREEN_SHAKE].text = "why";
        }
        else
        {
            //Display the value of the setting
            settingsTmp[SCREEN_SHAKE].text = SettingsScript.instance.wallRunShakeAmount.ToString();
        }

        //Update the sensitivity setting
        slider[SENSITIVITY].value = SettingsScript.instance.cameraSensitivity;
        if (SettingsScript.instance.cameraSensitivity == 0)
        {
            //If the setting is at 0, tell the player it is off (if the sensitivity is off, then uhhhhhhh)
            settingsTmp[SENSITIVITY].text = "Disabled";
        }
        else if (SettingsScript.instance.cameraSensitivity == 3000)
        {
            //a very sensitive mouse makes the game horrible. trust me, im the game dev
            settingsTmp[SENSITIVITY].text = "MAXIMUM POWER";
        }
        else
        {
            //Display the value of the setting
            settingsTmp[SENSITIVITY].text = SettingsScript.instance.cameraSensitivity.ToString();
        }

        //Update the fov setting
        slider[FOV].value = SettingsScript.instance.fov;
        if (SettingsScript.instance.fov == 0)
        {
            //i think if fov is 0, you just wont see anything. but unity is unity, so idk
            settingsTmp[FOV].text = "Disabled";
        }
        else if (SettingsScript.instance.fov == 150)
        {
            //this is a minecraft reference actually. if you set the fov there to max, it says quake pro
            settingsTmp[FOV].text = "Quake Pro";
        }
        else
        {
            //Display the value of the setting
            settingsTmp[FOV].text = SettingsScript.instance.fov.ToString();
        }

        //Update the quick finish setting
        if (SettingsScript.instance.quickFinish)
        {
            settingsTmp[QUICK_FINISH].text = "On";
        }
        else if (!SettingsScript.instance.quickFinish)
        {
            settingsTmp[QUICK_FINISH].text = "Off";
        }

        //Update the particles setting
        if (SettingsScript.instance.particles)
        {
            settingsTmp[PARTICLES].text = "On";
        }
        else if (!SettingsScript.instance.particles)
        {
            settingsTmp[PARTICLES].text = "Off";
        }

        //Update the music volume setting
        slider[MUSIC].value = SettingsScript.instance.musicVolume;
        if (SettingsScript.instance.fov == 0)
        {
            //If the setting is at 0, tell the user it is off
            settingsTmp[MUSIC].text = "Off";
        }
        else
        {
            //Display the value of the setting
            settingsTmp[MUSIC].text = SettingsScript.instance.musicVolume.ToString();
        }

        //Update the sfx volume setting
        slider[SFX].value = SettingsScript.instance.sfxVolume;
        if (SettingsScript.instance.sfxVolume == 0)
        {
            //If the setting is at 0, tell the user it is off
            settingsTmp[SFX].text = "Off";
        }
        else
        {
            //Display the value of the setting
            settingsTmp[SFX].text = SettingsScript.instance.sfxVolume.ToString();
        }

        //Update the debug mode setting
        if (SettingsScript.instance.debugMode)
        {
            settingsTmp[DEBUG_MODE].text = "On";
        }
        else if (!SettingsScript.instance.debugMode)
        {
            settingsTmp[DEBUG_MODE].text = "Off";
        }
    }

    //Update the times of completion on each level
    public void UpdateTimers()
    {
        //Grab all of the times from the file
        string[] times = File.ReadAllLines(Application.streamingAssetsPath + "/Times.txt");
        
        //Go through each time on each level
        for (int i = 0; i < timerTmp.Length; i++)
        {
            //If the time is at a default value, then that means they havent set a time
            if (times[i].Substring(0, 9).Equals("99:99:999"))
            {
                //Tell the player that a time has not been set
                timerTmp[i].text = "No time set";
            }
            else
            {
                //Show the time for that level
                timerTmp[i].text = times[i].Substring(0, 9);
            }
        }
    }
    
    /// <summary>
    /// Snap the rotation for camera transition (bugfix, is nessesary)
    /// </summary>
    public void SnapRotation()
    {
        //Snap the end rotation to a final value
        if (endRotation > -45 && endRotation < 45)
        {
            endRotation = 0;
        }
        if (endRotation > -135 && endRotation < -45)
        {
            endRotation = -90;
        }
        if (endRotation > 45 && endRotation < 135)
        {
            endRotation = 90;
        }
        if (endRotation > 405 && endRotation < 495)
        {
            endRotation = 450;
        }
    }
}
