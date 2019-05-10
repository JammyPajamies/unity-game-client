using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Description: This script handles the movement for the two jellyfish species.
Each species moves differently. 
*/

public class NPCJellyfishController : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //standard jellyfish movement
        if (gameObject.name == "NpcJellyfish")
        {
            transform.Translate(0f, 15f * Time.deltaTime, 0f);
            if (transform.position.y <= -165)
            {
                transform.rotation = Quaternion.Euler(0, 0, -45);
            }
            if (transform.position.y >= 165)
            {
                transform.rotation = Quaternion.Euler(0, 0, -135);
            }
            if (transform.position.x >= 450)
            {
                transform.rotation = Quaternion.Euler(0, 0, 135);
            }
            if (transform.position.x <= -450)
            {
                transform.rotation = Quaternion.Euler(0, 0, -135);
            }
        }
    }
}
