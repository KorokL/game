//Author: Lior Korok
//File Name: RespawnScript.cs
//Project Name: Platformer Game
//Creation Date: Sept, 2024
//Modified Date: Jan. 13, 2025
//Description: Respawn logic when a scene loads

using UnityEngine;

public class RespawnScript : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;
    public PlayerCam playerCam;
    public Transform startPos;

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        //If the player is respawning at the start of the level, then set the positions and rotations
        if (CheckpointsHolder.instance.respawnStart)
        {
            //Set the respawn positions and rotations to the start positions
            CheckpointsHolder.instance.respawnPos = startPos.position;
            CheckpointsHolder.instance.respawnRot = startPos.eulerAngles;
        }
        
        //Respawn the player at the respective position and rotation
        playerMovement.transform.position = CheckpointsHolder.instance.respawnPos;
        playerCam.yRotation = CheckpointsHolder.instance.respawnRot.y;

        //Destroy this script
        Destroy(this);
    }
}
