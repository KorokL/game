//Author: Lior Korok
//File Name: PlayerCam.cs
//Project Name: Platformer Game
//Creation Date: Sept, 2024
//Modified Date: Jan. 13, 2025
//Description: Manages the camera of the player

using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Header("Sensitivity")]
    public float sensX;
    public float sensY;

    [Header("References")]
    public Transform orientation;
    public PlayerMovement playerMovement;
    public Camera camera;
    public PauseMenu pauseMenu;
    public EndMenu endMenu;

    [Header("Rotation")]
    public float xRotation;
    public float yRotation;
    public float zRotation;

    [Header("Misc")] 
    public float wallRunRotationAmount;

    // Update is called once per frame
    void Update()
    {
        //If the player is paused, or finished the level unlock the cursur and show it
        if (pauseMenu.isPaused || endMenu.isEnd)
        {
            //Unlock the cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }
        else
        {
            //Lock the cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        //Set the fov based off of the setting
        camera.fieldOfView = SettingsScript.instance.fov;
        
        //Holds the position of the mouse
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * SettingsScript.instance.cameraSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * SettingsScript.instance.cameraSensitivity;

        //Adds the mouse position to the rotation of the player camera 
        yRotation += mouseX;
        xRotation -= mouseY;

        //Rotate the camera if player is wallrunning
        WallRunRotate();

        //Clamp the vertical rotation
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        //Rotate the actual gameobject of the camera
        transform.rotation = Quaternion.Euler(xRotation, yRotation, zRotation);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    /// <summary>
    /// Rotate the camera on the z axis if the player is on the wall
    /// </summary>
    public void WallRunRotate()
    {
        //Find the z rotation of the camera based off of the wallrun
        zRotation = Mathf.Lerp(zRotation, FindWallRunCamera(), 5 * Time.deltaTime);
    }

    /// <summary>
    /// Find the how much to rotate the camera based off of the camera direction
    /// </summary>
    /// <returns>Return the amount to rotate the camera</returns>
    public float FindWallRunCamera()
    {
        //If the player is not on the wall, dont rotate the camera
        if (!playerMovement.isWallRunning)
        {
            return 0;
        }

        //Calculate the wall run rotation amount based off of where the player is looking
        float num = Vector3.SignedAngle(new Vector3(0f, 0f, 1f), playerMovement.wallNormalVector, Vector3.up);
        float num2 = Mathf.DeltaAngle(transform.rotation.eulerAngles.y, num);
        float wallRunRotation = (0f - num2 / 90f) * SettingsScript.instance.wallRunShakeAmount;

        //Clamp the rotation amount from the setting menu
        return Mathf.Clamp(wallRunRotation, -SettingsScript.instance.wallRunShakeAmount, SettingsScript.instance.wallRunShakeAmount);
    }
}
