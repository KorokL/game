//Author: Lior Korok
//File Name: MusicManager.cs
//Project Name: Platformer Game
//Creation Date: Jan, 2024
//Modified Date: Jan. 13, 2025
//Description: Manages the music for the game

using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Referebces")]
    public AudioSource audioSource;

    /// <summary>
    /// Awake is called when an enabled script is being loaded
    /// </summary>
    void Awake()
    {
        //Make the able to be transfered between scenes
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Start is called once before Update
    /// </summary>
    void Start()
    {
        //Find all gameobjects that have the tag "Music:
        GameObject[] go = GameObject.FindGameObjectsWithTag("Music");

        //Checks if there is multiple MusicManagers
        if (go.Length > 1)
        {
            //Destroy the second MusicManager, to avoid duplication
            //When reloading/loading a scene, unity will make 1 duplicate of the gameobject, which is why why only destroy the second MusicManager
            Destroy(go[1]);
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    public void Update()
    {
        //Set the volume based of off the setting
        audioSource.volume = SettingsScript.instance.musicVolume / 100f;
    }
}
