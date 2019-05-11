using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script handles the visibility of the trails
/// that appear when a player gets a speed bonus powerup.
/// </summary>
namespace SD
{
    public class TrailController : MonoBehaviour
    {
        private GameController gameController;
        private float speedBoostDuration = 0f;
        private bool areTrailsOn = false;
        Component[] buffTrails;

        // Use this for initialization
        void Start()
        {
            gameController = GameController.getInstance();
            speedBoostDuration = FindObjectOfType<SpeedBuffController>().GetMaxBuffDuration();

            buffTrails = GetComponentsInChildren<TrailRenderer>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //speed power-up
            if (!areTrailsOn && gameController.getSpeedBoostStatus() == true)
            {
                areTrailsOn = true;

                // Get trails and enable them.
                foreach (TrailRenderer trail in buffTrails)
                {
                    if (!trail.isVisible)
                    {
                        trail.enabled = true;
                    }
                }
                
                areTrailsOn = false;
            }
            else if (gameController.getSpeedBoostStatus() == false)
            {
                // Disable the extra trails.
                foreach (TrailRenderer trail in buffTrails)
                {
                    trail.enabled = false;
                }
            }
        }
    }
}