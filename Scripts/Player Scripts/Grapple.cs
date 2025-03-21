//Author: Lior Korok
//File Name: Grapple.cs
//Project Name: Platformer Game
//Creation Date: Sept, 2024
//Modified Date: Jan. 13, 2025
//Description: The logic of the grapple gun 

using UnityEngine;

public class Grapple : MonoBehaviour
{
    [Header("References")] 
    public PlayerInput playerInput;
    public EnemyAI enemy;
    public ExplosiveLogic explosive;
    public LineRenderer lineRenderer;
    public SpringJoint joint;
    public Transform grappleTip;
    public Transform camera;
    public Transform player;
    public PauseMenu pauseMenu;
    public EndMenu endMenu;
    public SFXManager sfx;

    [Header("Grapple Info")]
    public Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public LayerMask whatIsNotGrappleable;
    public LayerMask whatIsEnemyButton;
    public LayerMask whatIsExplode;
    public float maxDistance = 150f;

    /// <summary>
    /// Start is called once before the first execution of Update after the MonoBehaviour is created
    /// </summary>
    void Start()
    {
        //Dont render any lines
        lineRenderer.positionCount = 0;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        //If the player is paused, or finished the level, dont grapple
        if (pauseMenu.isPaused || endMenu.isEnd)
        {
            //Stop the grapple
            StopGrapple();
            return;
        }

        //If the player is holding down the grapple key, start the grapple
        if (Input.GetKeyDown(playerInput.grappleKey))
        {
            //Start the grapple
            StartGrapple();
        }

        //If the player released the grapple key, stop the grapple
        if (Input.GetKeyUp(playerInput.grappleKey))
        {
            //Stop the grapple
            StopGrapple();
        }
    }

    /// <summary>
    /// Late update is called after Update has finished
    /// </summary>
    void LateUpdate()
    {
        //If the player is grappling, draw the grapple
        if (joint)
        {
            //Draw the grapple
            DrawRope();
        }
    }

    /// <summary>
    /// Start the grapple
    /// </summary>
    public void StartGrapple()
    {
        //Check if the location of the end of the grapple on an object that is grapplable
        if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, maxDistance, whatIsGrappleable) &&
            !Physics.Raycast(camera.position, camera.forward, maxDistance, whatIsNotGrappleable))
        {
            //Set an end of the grapple to the hit point 
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            //Calculate the distance from the point
            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //Distance the grapple will try to keep from grapple point
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            //Set the grapple settings
            joint.spring = 1f;
            joint.damper = 7f;
            joint.massScale = 15f;

            //Set the linerenderer
            lineRenderer.positionCount = 2;

            //Check if the grapplepoint is on an enemy button, or on an explosive barrel
            if (whatIsEnemyButton == (whatIsEnemyButton | (1 << hit.collider.gameObject.layer)))
            {
                //Destroy the grapple, but draw the rope
                Destroy(joint);
                DrawRope();

                //Connect the enemy with this script
                enemy = hit.collider.gameObject.GetComponentInParent<EnemyAI>();

                //Colour the line red for effect
                lineRenderer.endColor = Color.red;

                //Stop the grapple in 0.1 seconds
                Invoke(nameof(StopGrapple), 0.1f);

                //Explode the enemy
                enemy.ExplodeEnemy();
            }
            else if (whatIsExplode == (whatIsExplode | (1 << hit.collider.gameObject.layer)))
            {
                //Destroy the grapple, but draw the rope
                Destroy(joint);
                DrawRope();
                
                //Connect the explosive with this script
                explosive = hit.collider.gameObject.GetComponent<ExplosiveLogic>();

                //Color the line red for effect
                lineRenderer.endColor = Color.red;

                //Stop the grapple in 0.1 seconds
                Invoke(nameof(StopGrapple), 0.05f);

                //Explode the explosive
                explosive.Explode(hit.point);

            }
            else
            {
                //Color the grapple white
                lineRenderer.endColor = Color.white;
            }

            //Play the grapple sound effect
            //Disabled for reasons mentioned in the sheet
            // sfx.PlaySFX(sfx.audioSource[2]);
        }
    }

    /// <summary>
    /// Draw the grapple rope
    /// </summary>
    public void DrawRope()
    {
        //Set the linerenderer positions
        lineRenderer.SetPosition(0, grappleTip.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }

    /// <summary>
    /// Stop the grapple
    /// </summary>
    public void StopGrapple()
    {
        //Stop drawing the rope and destroy the joint
        lineRenderer.positionCount = 0;
        Destroy(joint);
    }

    /// <summary>
    /// Check if the player is grappling
    /// </summary>
    /// <returns>Return true if joint is enabled</returns>
    public bool IsGrappling()
    {
        return joint != null;
    }
}
