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

        private float fadeTimeExternal;

        private static MainMixerController mainMixerController;
        private static AudioSource audioSource;

        private Coroutine asyncFadeIn = null;
        private Coroutine asyncFadeOut = null;

        void Awake()
        {
            mainMixerController = this;
        }

        private void Start()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
        }

        public static MainMixerController GetInstance()
        {
            return mainMixerController;
        }

        public AudioMixer GetMainMixer()
        {
            return mainMix;
        }

        public void PlayOneShot(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }

        /// <summary>
        /// Fade in audio over fadeTimeParameter seconds.
        /// </summary>
        /// <param name="fadeTimeParameter"></param>
        public void FadeInAudio(float fadeTimeParameter = float.PositiveInfinity)
        {
            this.fadeTimeExternal = fadeTimeParameter;
            if (asyncFadeOut != null)
            {
                StopCoroutine(asyncFadeOut);
            }
            asyncFadeIn = StartCoroutine(FadeInAudioAsync());
        }

        /// <summary>
        /// Fade out audio over fadeTimeParameter seconds.
        /// </summary>
        /// <param name="fadeTimeParameter"></param>
        public void FadeOutAudio(float fadeTimeParameter = float.PositiveInfinity)
        {
            this.fadeTimeExternal = fadeTimeParameter;
            if(asyncFadeIn != null)
            {
                StopCoroutine(asyncFadeIn);
            }
            asyncFadeOut = StartCoroutine(FadeOutAudioAsync());
        }

        /// <summary>
        /// Fades in all audio.
        /// </summary>
        /// <returns></returns>
        public IEnumerator FadeInAudioAsync()
        {
            float fadeTimeRemaining = 0.0f;
            float fadeTimeForThisFade = fadeTime;

            // If the caller supplied a fade time, use it instead.
            if (!float.IsPositiveInfinity(fadeTimeExternal))
            {
                fadeTimeForThisFade = fadeTimeExternal;
            }

            while (true)
            {
                //Debug.Log("fading in...");
                fadeTimeRemaining += Time.deltaTime;

                mainMix.SetFloat("MasterVolume", Mathf.Lerp(initialVolume, maxVolume, fadeTimeRemaining / fadeTimeForThisFade));

                // Wait for the next frame.
                yield return new WaitForEndOfFrame();

                // Break out if time has elapsed.
                if (fadeTimeRemaining > fadeTimeForThisFade)
                {
                    break;
                }

                // Wait for the next frame.
                yield return new WaitForEndOfFrame();
            }
            float finalVol;
            mainMix.GetFloat("MasterVolume", out finalVol);

            //Debug.Log("Final Volume: " + finalVol);

            //Debug.Log("AudioCrossfade async complete.");
            yield return null;
        }

        /// <summary>
        /// Fades out all audio.
        /// </summary>
        /// <returns></returns>
        public IEnumerator FadeOutAudioAsync()
        {
            float fadeTimeRemaining = 0.0f;
            float fadeTimeForThisFade = fadeTime;
            float startVolume;
            mainMix.GetFloat("MasterVolume", out startVolume);

            // If the caller supplied a fade time, use it instead.
            if (!float.IsPositiveInfinity(fadeTimeExternal))
            {
                fadeTimeForThisFade = fadeTimeExternal;
            }

            while (true)
            {
                //Debug.Log("fading out...");
                fadeTimeRemaining += Time.deltaTime;

                mainMix.SetFloat("MasterVolume", Mathf.Lerp(startVolume, -80.0f, fadeTimeRemaining / fadeTimeForThisFade));

                // Wait for the next frame.
                yield return new WaitForEndOfFrame();

                // Break out if time has elapsed.
                if (fadeTimeRemaining > fadeTimeForThisFade)
                {
                    break;
                }

                // Wait for the next frame.
                yield return new WaitForEndOfFrame();
            }
            float finalVol;
            mainMix.GetFloat("MasterVolume", out finalVol);

            //Debug.Log("Final Volume: " + finalVol);

            //Debug.Log("AudioCrossfade async complete.");
            yield return null;
        }
    }

}