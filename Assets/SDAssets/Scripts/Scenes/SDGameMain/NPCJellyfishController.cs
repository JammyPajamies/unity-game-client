using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Description: This script handles the movement for the two jellyfish species.
Each species moves differently. 
*/

public class NPCJellyfishController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        //standard jellyfish movement
        if (gameObject.name== "NpcJellyfish") {
            transform.Translate(0f, 1f * Time.deltaTime, 0f);
        }

        //man-o-war movement
        if (gameObject.name == "NpcManOWar")
        {
            transform.Translate(.5f * Time.deltaTime, 0f, 0f);
        }
    }
}
