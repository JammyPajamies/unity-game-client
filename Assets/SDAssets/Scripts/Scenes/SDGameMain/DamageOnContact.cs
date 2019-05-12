﻿/* 
 * File Name: DamageOnContact.cs
 * Description: The player takes a damage when contacting to non-prey objects or eating poisonous fish.
 */

using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

namespace SD
{
    public class DamageOnContact : MonoBehaviour
    {
        private GameController gameController;
        private GameObject mainCamera;


        private const int damage = -20; // Score to be recieved by eating prey
        private float lastDamage = 0;
        public GameObject bleeding;

        public AudioClip audioClip;
        private AudioSource playerAudioSource;

        // Use this for initialization
        void Start()
        {
            gameController = GameController.getInstance();
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            playerAudioSource = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                Instantiate(bleeding, other.transform.position, Quaternion.identity);
                playerAudioSource.PlayOneShot(audioClip);
                if (gameController.GetHealth() > 0)
                {
                    gameController.UpdateHealth(damage);
                }
                lastDamage = 0;
            }
        }

        void OnTriggerStay(Collider other)
        {
            if (gameObject.tag == "Predator" && other.tag == "Player")
            {
                lastDamage += Time.deltaTime;
                if (lastDamage >= 2)
                {
                    Instantiate(bleeding, other.transform.position, Quaternion.identity);
                    playerAudioSource.PlayOneShot(audioClip);
                    if (gameController.GetHealth() > 0)
                    {
                        gameController.UpdateHealth(damage);
                    }
                    lastDamage = 0;
                }
            }
        }
    }
}
