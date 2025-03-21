//Author: Lior Korok
//File Name: RotateGrapple.cs
//Project Name: Platformer Game
//Creation Date: Sept, 2024
//Modified Date: Jan. 13, 2025
//Description: Rotate the grapple gun in relation to the grapple point

using UnityEngine;

public class RotateGrapple : MonoBehaviour
{
    [Header("References")]
    public Grapple grapple;

    [Header("Rotation Values")]
    public Quaternion desiredRotation;
    public float rotationSpeed;

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        //Check if the grapple is rotating
        if (!grapple.IsGrappling())
        {
            //Set the desired rotation and the speed
            desiredRotation = transform.parent.rotation;
            rotationSpeed = 10;
        }
        else
        {
            //Set the desired rotation and the speed
            desiredRotation = Quaternion.LookRotation(grapple.grapplePoint - transform.position);
            rotationSpeed = 50;
        }

        //Rotate the grapple based on the resired rotation and speed
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
    }
}
