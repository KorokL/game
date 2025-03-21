//Author: Lior Korok
//File Name: MoveCamera.cs
//Project Name: Platformer Game
//Creation Date: Sept, 2024
//Modified Date: Jan. 13, 2025
//Description: Move the camera

using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [Header("References")]
    public Transform cameraPosition;

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        //Move the camera to its proper positions
        transform.position = cameraPosition.position;
    }
}
