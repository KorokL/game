//Author: Lior Korok
//File Name: CheckpointsHolder.cs
//Project Name: Platformer Game
//Creation Date: Sept, 2024
//Modified Date: Jan. 13, 2025
//Description: Holds the checkpoints

using UnityEngine;

public class CheckpointsHolder : MonoBehaviour
{
    [Header("Respawn info")]
    public Vector3 respawnPos;
    public Vector3 respawnRot;
    public bool respawnStart = true;
    
    public static CheckpointsHolder _Instance;

    /// <summary>
    /// Returns the variables and everything that is in this script
    /// </summary>
    public static CheckpointsHolder instance
    {
        //If the settings script is called from another script, get all variables from this script
        get
        {
            //If an instance isnt declared, then create one
            if (!_Instance)
            {
                //Create an instnace that can be transfered between scenes
                _Instance = new GameObject().AddComponent<CheckpointsHolder>();
                _Instance.name = _Instance.GetType().ToString();
                DontDestroyOnLoad(_Instance.gameObject);
            }
            
            //Return the variables
            return _Instance;
        }
    }
}

