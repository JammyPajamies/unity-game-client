using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Description: This script handles the movement for the two jellyfish species.
Each species moves differently. 
*/

public class NPCJellyfishController : MonoBehaviour
{
    //checks the direction of movement
    private bool movingLeft;
    private bool movingRight;
    private bool movingUp;
    private bool movingDown;

    public float swimSpeed = 0;

    // Use this for initialization
    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, -45);
        movingLeft = false;
        movingRight = true;
        movingUp = true;
        movingDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        //standard jellyfish movement
        transform.Translate(0f, swimSpeed * Time.deltaTime, 0f);
        if (transform.position.y <= -165)
        {
            movingDown = false;
            movingUp = true;
            if (movingRight == true)
            {
                transform.rotation = Quaternion.Euler(0, 0, -45);
            }
            if (movingLeft == true)
            {
                transform.rotation = Quaternion.Euler(0, 0, 45);
            }
        }
        if (transform.position.y >= 165)
        {
            movingUp = false;
            movingDown = true;
            if (movingRight == true)
            {
                transform.rotation = Quaternion.Euler(0, 0, -135);
            }
            if (movingLeft == true)
            {
                transform.rotation = Quaternion.Euler(0, 0, 135);
            }
        }
        if (transform.position.x >= 450)
        {
            movingRight = false;
            movingLeft = true;
            if (movingDown == true)
            {
                transform.rotation = Quaternion.Euler(0, 0, 135);
            }
            if (movingUp == true)
            {
                transform.rotation = Quaternion.Euler(0, 0, 45);
            }
        }
        if (transform.position.x <= -450)
        {
            movingLeft = false;
            movingRight = true;
            if (movingDown == true)
            {
                transform.rotation = Quaternion.Euler(0, 0, -135);
            }
            if (movingUp == true)
            {
                transform.rotation = Quaternion.Euler(0, 0, -45);
            }
        }
    }
}
