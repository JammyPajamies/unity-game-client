using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SD
{
    public class MainMixerController : MonoBehaviour
    {
        public AudioMixer mainMix;
        // Time in seconds of fade transition.
        public float fadeTime = 3.0f;
        // Min DB for volume.
        public float initialVolume = -80.0f;
        // Max DB for volume.
        public float maxVolume = 0.0f;

        /// <summary>
        /// Fades in all audio.
        /// </summary>
        /// <returns></returns>
        public IEnumerator FadeInAudio()
        {
            float fadeTimeRemaining = 0.0f;

            while (true)
            {
                fadeTimeRemaining += Time.deltaTime;

                mainMix.SetFloat("MasterVolume", Mathf.Lerp(initialVolume, maxVolume, fadeTimeRemaining / fadeTime));

                // Wait for the next frame.
                yield return new WaitForEndOfFrame();

                // Break out if time has elapsed.
                if (fadeTimeRemaining > fadeTime)
                {
                    break;
                }

                // Wait for the next frame.
                yield return new WaitForEndOfFrame();
            }
            float finalVol;
            mainMix.GetFloat("MasterVolume", out finalVol);

            Debug.Log("Final Volume: " + finalVol);

            //Debug.Log("AudioCrossfade async complete.");
            yield return null;
        }

        /// <summary>
        /// Fades out all audio.
        /// </summary>
        /// <returns></returns>
        public IEnumerator FadeOutAudio()
        {
            float fadeTimeRemaining = 0.0f;
            float startVolume;
            mainMix.GetFloat("MasterVolume", out startVolume);

            while (true)
            {
                fadeTimeRemaining += Time.deltaTime;

                mainMix.SetFloat("MasterVolume", Mathf.Lerp(startVolume, -80.0f, fadeTimeRemaining / fadeTime));

                // Wait for the next frame.
                yield return new WaitForEndOfFrame();

                // Break out if time has elapsed.
                if (fadeTimeRemaining > fadeTime)
                {
                    break;
                }

                // Wait for the next frame.
                yield return new WaitForEndOfFrame();
            }
            float finalVol;
            mainMix.GetFloat("MasterVolume", out finalVol);

            Debug.Log("Final Volume: " + finalVol);

            //Debug.Log("AudioCrossfade async complete.");
            yield return null;
        }
    }

}