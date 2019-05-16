/*
 * File Name: MainCameraController.cs
 * Date: Created on Arp 7 2016
 * Description: Moves the main camera along the player's movement
 *
 */

using UnityEngine;
using System.Collections;

namespace SD
{
    public class MainCameraController : MonoBehaviour
    {

        private Rigidbody player;
        private Vector3 playerPosition;
        private Vector3 cameraPosition;
        private GameObject mainCamera;

        void Start()
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            player = PlayerController.GetInstance().GetComponent<Rigidbody>();
            playerPosition = PlayerController.GetInstance().gameObject.GetComponent<Rigidbody>().position;
            mainCamera.transform.position = playerPosition;
            mainCamera.transform.position = new Vector3(0f, 0f, -25f);
        }

        void Update()
        {
            mainCamera.transform.position = player.position;
            cameraPosition = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, -25f);
            mainCamera.transform.position = cameraPosition;
        }
    }
}