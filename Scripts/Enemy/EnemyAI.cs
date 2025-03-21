//Author: Lior Korok
//File Name: EnemyAI.cs
//Project Name: Platformer Game
//Creation Date: Dec, 2024
//Modified Date: Jan. 13, 2025
//Description: Contains the AI for the enemy 

using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public Camera enemyCamera;
    public PauseMenu pauseMenu;
    public ParticleSystem explosion;
    public Rigidbody playerRigidbody;

    [Header("Layers")]
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;
    
    [Header("Patrolling")]
    public Vector3[] walkPoints;
    public int currentWalkPoint = 0;
    public bool inRangeWalkPoint;
    public float walkPointRange;

    [Header("Chasing")]
    public Vector3 lastPlayerPosition;
    public bool caughtPlayer;
    
    [Header("States")] 
    public float sightRange = 15;
    public float sightAngle = 60;
    public bool isChasingPlayer;
    public float agentSpeed;
    public float agentAngle = 100;
    
    [Header("Misc")] 
    public Bounds playerBounds;
    public Plane[] frustrumPlanes;
    public RaycastHit hit;
    const int ENEMY_LEVEL = 5;

    [Header("Explosion")] 
    public float explosionForce = 200;
    public float explosionRadius = 200;
    
    /// <summary>
    /// Awake is called when a script is being loaded
    /// </summary>
    void Awake()
    {
        //Attach the player and the agent
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        //Set the enemy settings
        enemyCamera.nearClipPlane = 0;
        sightRange = 15;
        sightAngle = 100;
        agentSpeed = 5;
        agentAngle = 100;
        SetAgent();
    }
    /// <summary>
    /// Start is called once before the first execution of Update after the MonoBehaviour is created
    /// </summary>
    void Start()
    {
        //Set the random variable
        Random rand = new Random();

        //Set the string that holds the vector
        string[] stringVector;

        //Store all of the enemy walkPoints
        string[] walkPointsText = File.ReadAllLines(Application.streamingAssetsPath + "/EnemyPatrolPoints/" + gameObject.name + ".txt");

        //Randomizes the walkPoints using an algorithm
        int n = walkPointsText.Length;
        while (n > 1)
        {
            //Swaps two random values in an array
            int k = rand.Next(n--);
            string temp = walkPointsText[n];
            walkPointsText[n] = walkPointsText[k];
            walkPointsText[k] = temp;
        }
        
        //Go through each walkpoint
        for (int i = 0; i < walkPointsText.Length; i++)
        {
            //Set the walkpoints into a global array
            stringVector = walkPointsText[i].Substring(1, walkPointsText[i].Length - 2).Split(',');
            walkPoints[i] = new Vector3(float.Parse(stringVector[0]), float.Parse(stringVector[1]), float.Parse(stringVector[2]));

            //If the level is the enemy level, offset each walkpoint by a value so that the enemy destination isn't clipped in a wall
            if (SceneManager.GetActiveScene().buildIndex == ENEMY_LEVEL)
            {
                //Offset the x and z of the walkpoint by 8 or -8
                walkPoints[i].x += rand.Next(0, 1 + 1) == 0 ? 8 : -8;
                walkPoints[i].z += rand.Next(0, 1 + 1) == 0 ? 8 : -8;
            }
        }

        //Set the enemy to the first walkpoint
        agent.transform.position = walkPoints[currentWalkPoint];
        
        //Pause the explosion animation, as enemy hasnt been exploded yet
        explosion.Pause();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        //If the game is paused, or the player has finished the level stop the agent        
        if (pauseMenu.isPaused || caughtPlayer)
        {
            //Stop the agent
            agent.SetDestination(transform.position);

            //Prevent the method from continuing
            return;
        }
        
        //Sets the bound of the player and the frustum plane so the enemy can see the player
        SetPlayerBoundsAndPlane();

        //Checks if the enemy is chasing the player
        if (isChasingPlayer)
        {
            //If it is, then conitnue chasing the player
            ChasePlayer();
        }
        else
        {
            //If not, patrol
            Patroling();
        }
    }
    
    /// <summary>
    /// Sets the player bounds and the frustum planes so the enemy can see the player
    /// </summary>
    public void SetPlayerBoundsAndPlane()
    {
        playerBounds = new Bounds(player.position, player.localScale);
        frustrumPlanes = GeometryUtility.CalculateFrustumPlanes(enemyCamera);
    }

    /// <summary>
    /// Make the enemy patrol the area
    /// </summary>
    public void Patroling()
    {
        //Checks if the player is in sight of the enemy
        if (CheckPlayer())
        {
            //Set the enemy to chase the player
            isChasingPlayer = true;
            
            //Prevent the method from continuing
            return;
        }
        
        //Set and hold the distance to the current walkpoint
        Vector3 distanceToTarget = transform.position - walkPoints[currentWalkPoint];

        //Check if the enemy in the range of the current walkpoint
        if (distanceToTarget.magnitude < 1f)
        {
            //Set the in range walkpoint to true
            inRangeWalkPoint = true;
        }
        
        //Checks if the enemy is inrange of its current walkpoint
        if (inRangeWalkPoint) 
        {
            //Go to the next walkpoint
            currentWalkPoint++;

            //If the walkpoint is at the end, loop around
            if (currentWalkPoint == walkPoints.Length)
            {
                currentWalkPoint = 0;
            }
            
            //Set the in range of the walk point to false
            inRangeWalkPoint = false;
        }

        //Set the agent settings
        sightRange = 15;
        sightAngle = 100;
        agentSpeed = 5;
        SetAgent();
        
        //Set the destination of the agent to the current walkpoint
        agent.SetDestination(walkPoints[currentWalkPoint]);
    }

    /// <summary>
    /// Chase the player if they are in range
    /// </summary>
    public void ChasePlayer()
    {
        //Holds the distance to last seen player position, and the distance to the player
        Vector3 distToLastPlayer = transform.position - lastPlayerPosition;
        Vector3 distToPlayer = transform.position - player.position;
        
        //Set the settings for the agent
        sightRange = 25;
        sightAngle = 120;
        agentSpeed = 10;
        SetAgent();

        //If the agent is in range of the player, then catch the player
        if (distToPlayer.magnitude < 1.5f)
        {
            //Catch the player, and stop the game
            pauseMenu.PauseLogic();
            caughtPlayer = true;
            pauseMenu.forcedPause = true;
        }
        
        //Check if the player is around the player, and the enemy can see the player, or if the enemy is at the last position of the player
        if (CheckPlayer())
        {
            //If it can, then set the last player position to the player's current position
            lastPlayerPosition = player.position;
        }
        else if (PlaneMagnitude(distToLastPlayer) < 1)
        {
            //Stop chasing the player
            isChasingPlayer = false;
            Patroling();
        }

        //Set the agent destination to the last time it saw the player
        agent.SetDestination(lastPlayerPosition);
    }

    /// <summary>
    /// Check to see if the enemy can see the player
    /// </summary>
    /// <returns>True if the enemy finds the player</returns>
    public bool CheckPlayer()
    {
        //Checks if the enemy is chasing the player
        if (!isChasingPlayer)
        {
            //Checks if the player is in range of the enmy
            if (Physics.CheckSphere(transform.position, sightRange, whatIsPlayer))
            {
                //Checks if the enemy can see the player through its camera, and that vision is not obstructed by a wall
                if (GeometryUtility.TestPlanesAABB(frustrumPlanes, playerBounds) && TryCastPlayer())
                {
                    return true;
                }
            }
        }
        else
        {
            //Checks if the enemy can see the player through its camera, and that vision is not obstructed by a wall
            if (Physics.CheckSphere(transform.position, sightRange, whatIsPlayer) && TryCastPlayer())
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Explode the enemy if the player has caught it
    /// </summary>
    public void ExplodeEnemy()
    {
        //Checks if the particles are enabled in the settings
        if (SettingsScript.instance.particles)
        {
            //Instantiate the explosion object to the enemies posotion 
            Instantiate(explosion).transform.position = gameObject.transform.position;

            //Play the explosion animation
            explosion.Play();
        }

        //Add explosion force to the player
        playerRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, 0f, ForceMode.Impulse);
        
        //Destroy the enemy
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Checks if the view of the player is obstructed by a wall
    /// </summary>
    /// <returns>True if there is a wall in between the player and the enemy</returns>
    public bool TryCastPlayer()
    {
        //Holds the direction of the player from the enemy position
        Vector3 direction = player.position - transform.position;

        //Returns the raycast
        return !Physics.Raycast(transform.position, direction.normalized, out hit, direction.magnitude, whatIsGround);
    }

    /// <summary>
    /// Sets the agent settings
    /// </summary>
    public void SetAgent()
    {
        //Sets the maximum viewing distance and its field of view
        enemyCamera.farClipPlane = sightRange;
        enemyCamera.fieldOfView = sightAngle;
        
        //Set the speed of the agent
        agent.speed = agentSpeed;
        agent.angularSpeed = agentAngle;
    }

    /// <summary>
    /// Calculate the magnitude on a flat plane
    /// </summary>
    /// <param name="v">The velocity</param>
    /// <returns>A number that describes the flat speed</returns>
    public float PlaneMagnitude(Vector3 v)
    {
        return Mathf.Sqrt(Mathf.Pow(v.x, 2) + Mathf.Pow(v.z, 2));
    }
}
