using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script sets the alpha level of the trails made by the player.
/// Uses the ratio of the current player's speed over their max speed.
/// </summary>
namespace SD
{
    public class SwimTrailAlphaScaler : MonoBehaviour
    {
        public float maxTrailAlpha = 0.5f;

        private PlayerController playerController;
        private float alphaRatio = 0.0f;
        private float speedBoostDuration = 0f;
        private bool areTrailsOn = false;
        Component[] buffTrails;

        // Use this for initialization
        void Start()
        {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

            buffTrails = GetComponentsInChildren<TrailRenderer>();

            alphaRatio = playerController.GetCurrentToMaxSpeedRatio();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // Get and clamp the ratio of currentSpeed/max
            alphaRatio = Mathf.Clamp(playerController.GetCurrentToMaxSpeedRatio(), 0, maxTrailAlpha);

            // Go through and adjust the alpha level on the trails.
            foreach (TrailRenderer trail in buffTrails)
            {
                trail.material.SetColor("_TintColor", new Color(
                    alphaRatio,
                    alphaRatio,
                    alphaRatio,
                    alphaRatio));
            }
        }

        [ExecuteInEditMode]
        void OnValidate()
        {
            maxTrailAlpha = Mathf.Clamp01(maxTrailAlpha);
            maxTrailAlpha = Mathf.Clamp01(maxTrailAlpha);
        }
    }
}
