//Author: Lior Korok
//File Name: KeyGeneration.cs
//Project Name: Platformer Game
//Creation Date: Sept, 2024
//Modified Date: Jan. 13, 2025
//Description: Generate the key and end position for level 5

using System.IO;
using UnityEngine;
using Random = System.Random;

public class KeyGeneration : MonoBehaviour
{
    [Header("References")]
    public GameObject endObject;
    /// <summary>
    /// Start is called once before the first execution of Update after the MonoBehaviour is created
    /// </summary>
    void Start()
    {
        //Randomize the positions of the key and the end object
        transform.position = RandomizePos();
        endObject.transform.position = RandomizePos();

        //Disable the end object because the key isn't picked up yet
        endObject.SetActive(false);
    }

    /// <summary>
    /// Randomizes the position based on a grid in this level
    /// </summary>
    /// <returns>The randomize position</returns>
    public Vector3 RandomizePos()
    {
        //Create the random variable
        Random rand = new Random();
        
        //Holds all of the possible random positions from the file
        string[] allRandPos = File.ReadAllLines(Application.streamingAssetsPath + "/RandPos.txt");

        //Picks a random position
        int randPos = rand.Next(0, allRandPos.Length);

        //Holds the coordinates in as a string
        string[] stringVector = allRandPos[randPos].Substring(1, allRandPos[randPos].Length - 2).Split(',');

        //Colds the coords as a vector 3
        Vector3 pos = new Vector3(float.Parse(stringVector[0]), float.Parse(stringVector[1]), float.Parse(stringVector[2]));

        //Offset the position by 8 or -8 so it is not in a wall
        var randX = rand.Next(0, 2) == 0 ? 8 : -8;
        var randZ = rand.Next(0, 2) == 0 ? 8 : -8;

        //Add the offset to the position
        pos.x += randX;
        pos.z += randZ;
        
        //Return the randomized position
        return pos;
    }
    
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        //Rotate the key
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 1, 0);
    }

    /// <summary>
    /// Check if the player is collided with a trigger
    /// </summary>
    /// <param name="other">The object that the player has collided with</param>
    void OnTriggerEnter(Collider other)
    {
        //Disable the key        
        gameObject.SetActive(false);
        
        //Enable the end object
        endObject.SetActive(true);
    }
}
