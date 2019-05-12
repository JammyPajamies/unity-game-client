/* 
 * File Name: DestroyByContact.cs
 * Description: Destroys attached objects if they collide with the player.
 */

using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

namespace SD
{
    public class DestroyByContact : MonoBehaviour
    {
        private GameController gameController;
        private int newScoreValue = 10; // Score to be recieved by eating prey
        private GameObject mainCamera;

        // The random sound player object that will be used to play a random sound on consumption.
        private RandomSoundPlayer randomSound;

        // Use this for initialization
        void Start()
        {
            gameController = GameController.getInstance();
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            randomSound = gameObject.GetComponent<RandomSoundPlayer>();
        }

        // Destroys the attached object upon a collison with the player
        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                // Get info and inform the network that the prey fish has been destroyed.
                int npcFishId = gameObject.GetComponentInParent<NPCFishController>().getNPCFishData().id;
                int npcFishSpeciesId = gameObject.GetComponentInParent<NPCFishController>().getNPCFishData().speciesId;
                //Debug.Log ("Consumed prey with ID: " + npcFishId);
                if (SDMain.networkManager != null)
                {
                    GameManager.getInstance().DestroyNPCFish(npcFishId, npcFishSpeciesId);
                }

                // Tell the player audio source to play a random consume sound.
                randomSound.PlayRandomSound();
                // Then add the points.
                gameController.AddUnscoredPoint(newScoreValue);
                // Finally, destroy the consumed fish.
                gameController.destroyPrey(npcFishId);

            }
        }
    }
}
