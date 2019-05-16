using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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
        Component[] trails;
        
        // Audio section.
        // The master audio mixer.
        public AudioMixer mainMixer;
        public AudioClip swimmingSFX;
        public float maxSwimmingSFXVolume = 0.25f;

        private AudioSource audioPlayer;
        private float swimSFXRatio = 0.0f;

        // Use this for initialization
        void Start()
        { 
            audioPlayer = gameObject.AddComponent<AudioSource>();
            audioPlayer.outputAudioMixerGroup = mainMixer.FindMatchingGroups("Master")[0];
            audioPlayer.playOnAwake = true;
            audioPlayer.loop = true;
            audioPlayer.clip = swimmingSFX;
            audioPlayer.Play();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //playerController = PlayerController.GetInstance().gameObject.GetComponent<PlayerController>();
            playerController = PlayerController.GetInstance();
            trails = GetComponentsInChildren<TrailRenderer>();
            // Get and clamp the ratio of currentSpeed/max
            alphaRatio = Mathf.Clamp(playerController.GetCurrentToMaxSpeedRatio(), 0, maxTrailAlpha);
            swimSFXRatio = Mathf.Clamp(playerController.GetCurrentToMaxSpeedRatio(), 0, maxSwimmingSFXVolume);

            // Go through and adjust the alpha level on the trails.
            foreach (TrailRenderer trail in trails)
            {
                trail.material.SetColor("_TintColor", new Color(
                    alphaRatio,
                    alphaRatio,
                    alphaRatio,
                    alphaRatio));
            }
            audioPlayer.volume = swimSFXRatio;
        }

        [ExecuteInEditMode]
        void OnValidate()
        {
            maxTrailAlpha = Mathf.Clamp01(maxTrailAlpha);
            maxTrailAlpha = Mathf.Clamp01(maxTrailAlpha);
        }
    }
}
