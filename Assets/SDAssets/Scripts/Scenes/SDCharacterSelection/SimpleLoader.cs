//Authored by Marlo Sandoval
//Description: Temporary scene loader for the character selection screen

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace SD
{
    public class SimpleLoader : MonoBehaviour
    {
        public Animator fadeOutAnimator;
        private Button playButton;
        private SDReadySceneManager readyManager;

        private void Start()
        {
            playButton = this.gameObject.GetComponent<Button>();
            readyManager = GameObject.FindObjectOfType<SDReadySceneManager>();
        }

        public void loadMainMenu()
        {
            SceneManager.LoadScene("SDReadyScene");
        }

        public void loadGame()
        {
            // Find the audio mixer and ask it to fade out.
            playButton.interactable = false;
            readyManager.StartGame();
            StartCoroutine(FindObjectOfType<MainMixerController>().FadeOutAudio());
            StartCoroutine(MainGameTransition());
        }

        // Transition to the main game scene after the audio has finished fading.
        // Also, fade out GUI
        private IEnumerator MainGameTransition()
        {
            fadeOutAnimator.SetTrigger("FadeOut");
            yield return new WaitForSeconds(FindObjectOfType<MainMixerController>().fadeTime);

            if (FindObjectOfType<SDPersistentData>() != null)
            {
                FindObjectOfType<SDPersistentData>().GetComponent<AudioSource>().enabled = false;
            }
        }
    }
}
