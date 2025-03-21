//Author: Lior Korok
//File Name: ExplosiveLogic.cs
//Project Name: Platformer Game
//Creation Date: Oct, 2024
//Modified Date: Jan. 13, 2025
//Description: Explosion Logic for the explosive barrel

using UnityEngine;

public class ExplosiveLogic : MonoBehaviour
{
    [Header("References")]
    public PauseMenu pauseMenu;
    public ParticleSystem explosion;
    public Rigidbody rigidbody;
    public Rigidbody playerRigidbody;

    [Header("Explosion")] 
    public float explosionForce = 15;
    public float explosionRadius = 20;
    public float upwardsModifier = 0.25f;

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        //If the game is paused, then stop the barrel from moving due to physics
        if (pauseMenu.isPaused)
        {
            rigidbody.freezeRotation = true;
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
        else
        {
            //Let the barrel move
            rigidbody.freezeRotation = false;
        }
    }

    /// <summary>
    /// Explode the explosive barrel
    /// </summary>
    /// <param name="v">The point where the explosive will explode</param>
    public void Explode(Vector3 v)
    {
        //Check if particles are enabled in the settings 
        if (SettingsScript.instance.particles)
        {
            //Create a new explosion gameobject, then move it to the barrel
            Instantiate(explosion).transform.position = gameObject.transform.position;

            //Playe the explosino animation
            explosion.Play();
        }

        //Create the explosion force
        playerRigidbody.AddExplosionForce(explosionForce, v, explosionRadius, upwardsModifier, ForceMode.Impulse);

        //Destroy the barrel
        Destroy(gameObject);
    }
}
