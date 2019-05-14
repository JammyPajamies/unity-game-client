using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Description: This handles the code for spawning fishes. 
*/

public class NPCFishSpawner : MonoBehaviour
{

    public GameObject prefab;

    //determines how many spawns will appear
    private int currentSpawnCount = 1;
    public int maxSpawnCount = 1;

    //position coordinates
    private int xPos;
    private int yPos;
    public int xLowerBounds = 0;
    public int xUpperBounds = 0;
    public int yLowerBounds = 0;
    public int yUpperBounds = 0;

    // Use this for initialization
    void Start()
    {
        createSpawn();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void createSpawn()
    {
        while (currentSpawnCount <= maxSpawnCount)
        {
            xPos = Random.Range(xLowerBounds, xUpperBounds);
            yPos = Random.Range(yLowerBounds, yUpperBounds);
            Instantiate(prefab, new Vector3(xPos, yPos, 0), Quaternion.identity);
            currentSpawnCount += 1;
        }
    }
}
