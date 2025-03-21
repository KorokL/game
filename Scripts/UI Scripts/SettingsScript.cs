//Author: Lior Korok
//File Name: SettingsScript.cs
//Project Name: Platformer Game
//Creation Date: Oct, 2024
//Modified Date: Jan. 13, 2025
//Description: Holds the settings options of the game

using UnityEngine;

public class SettingsScript : MonoBehaviour
{
    [Header("Settings")]
    public float wallRunShakeAmount = 45;
    public float cameraSensitivity = 500;
    public float fov = 90;
    public bool quickFinish;
    public bool particles = true;
    public float musicVolume = 0f;
    public float sfxVolume = 0f;
    public bool debugMode = false;
        
    [Header("Singleton")]
    public static SettingsScript _Instance;

    /// <summary>
    /// Returns the variables and everything that is in this script
    /// </summary>
    public static SettingsScript instance
    {
        //If the settings script is called from another script, get all variables from this script
        get
        {
            //If an instance isnt declared, then create one
            if (!_Instance)
            {
                //Create an instnace that can be transfered between scenes
                _Instance = new GameObject().AddComponent<SettingsScript>();
                _Instance.name = _Instance.GetType().ToString();
                DontDestroyOnLoad(_Instance.gameObject);
            }

            //Return the variables
            return _Instance;
        }
    }
}