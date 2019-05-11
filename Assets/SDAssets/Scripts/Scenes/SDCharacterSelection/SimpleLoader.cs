//Authored by Marlo Sandoval
//Description: Temporary scene loader for the character selection screen

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace SD
{
    public class SimpleLoader : MonoBehaviour
    {
        public Animator fadeOutAnimator;

        public void loadMainMenu()
        {
            SceneManager.LoadScene("SDReadyScene");
        }

        public void loadGame()
        {
            // Find the audio mixer and ask it to fade out.
            StartCoroutine(FindObjectOfType<MainMixerController>().FadeOutAudio());
            StartCoroutine(MainGameTransition());
        }

        // Transition to the main game scene after the audio has finished fading.
        // Also, fade out GUI
        private IEnumerator MainGameTransition()
        {
            fadeOutAnimator.SetTrigger("FadeOut");
            yield return new WaitForSeconds(FindObjectOfType<MainMixerController>().fadeTime);

            FadeToLevel("SDGameMain");
        }

        public void FadeToLevel(string levelName)
        {
            if (FindObjectOfType<SDPersistentData>() != null)
            {
                FindObjectOfType<SDPersistentData>().GetComponent<AudioSource>().enabled = false;
            }
            SceneManager.LoadScene(levelName);
        }
    }
}
