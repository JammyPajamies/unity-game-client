using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

namespace SD
{
    public class SDCharacterSelectionSceneManager : MonoBehaviour
    {
        // The animators that will help us fade in and out.
        public Animator fadeInAnimator;
        public Animator fadeOutAnimator;
        // Keep track of the back to ready scene button.
        public Button readySceneButton;
        // Keep track of the ready button so we can disable interacting with it after it has been clicked.
        public Button playerReadyButton;

        private static SDCharacterSelectionSceneManager sceneManager;

        private bool isPlayerReady{ get; set; }
        private bool isOpponentReady { get; set; }

        public static SDCharacterSelectionSceneManager getInstance()
        {
            return sceneManager;
        }

        // Use this for initialization
        void Start()
        {
            sceneManager = this;
            isPlayerReady = false;
            isOpponentReady = false;

            if (SDMain.networkManager != null)
            {
                SDMain.networkManager.Listen(NetworkCode.SD_PLAYER_POSITION, ResponseSDStartSync);
                SDMain.networkManager.Listen(NetworkCode.SD_START_GAME, ResponseSDStartGame);
            }
            else
            {
                Debug.LogWarning("Could not obtain a connection to Sea Divided Server. Falling back to offline mode.");
            }
        }

        /// <summary>
        /// Signals that this player is ready to play.
        /// </summary>
        /// <param name="r"></param>
        public void ResponseSDStartGame(NetworkResponse r)
        {
            ResponseSDStartGame response = r as ResponseSDStartGame;
            Debug.Log("ResponseSDStartGame called.");
            SDPersistentData.getInstance().setRoundStartTime(response.startDateTime);
            if (response.status == 0)
            {
                // Send a request to the opponent indicating that this client is ready to play.
                // After that, wait for the other player.
                SDMain.networkManager.Send(SDPlayerPositionProtocol.Prepare(
                    0.ToString(), 0.ToString(), 0.ToString()));
                Debug.Log("Waiting for opponent to respond..");
            }
            else
            {
                Debug.Log("Encountered an error in starting the game.");
            }
        }

        /// <summary>
        /// Waits for a signal from the other player that they are ready to play.
        /// </summary>
        /// <param name="r"></param>
        public void ResponseSDStartSync(NetworkResponse r)
        {
            ResponseSDPlayerPosition response = r as ResponseSDPlayerPosition;
            Debug.Log("The opponent is ready to play, loading the game scene.");
            this.isOpponentReady = true;
            ReadyCheck();
        }

        /// <summary>
        /// Loads the ready scene.
        /// </summary>
        public void LoadMainMenu()
        {
            SceneManager.LoadScene("SDReadyScene");
        }

        /// <summary>
        /// Signals that this player is ready to load the scene.
        /// If multiplayer, we will switch to a waiting state for the other player.
        /// If we are in single player, we will transition immediately.
        /// Also, disable going back to the ready scene.
        /// </summary>
        public void LoadMainScene()
        {
            playerReadyButton.interactable = false;
            readySceneButton.interactable = false;
            playerReadyButton.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 1.0f);
            readySceneButton.GetComponent<Image>().color = new Color(1f, 0f, 0f, 1.0f);

            // If we are in multiplayer, signal to the other player that we are ready.
            // Call the function to transition into the game.
            if (SDMain.networkManager != null)
            {
                SDMain.networkManager.Send(SDStartGameProtocol.Prepare(Constants.USER_ID));
                isPlayerReady = true;
                ReadyCheck();
            }
            else
            {
                // If we are in single player mode, just transition immediately.
                Debug.LogWarning("Starting game without server component.");
                StartCoroutine(MainGameTransition());
            }
        }

        /// <summary>
        /// Check to see if both players are ready to go.
        /// Call functions to begin transitioning if both are ready to go.
        /// </summary>
        private void ReadyCheck()
        {
            if (this.isPlayerReady && this.isOpponentReady)
            {
                Debug.Log("Loading the Game Scene..");
                try
                {
                    SDMain.networkManager.Ignore(NetworkCode.SD_PLAYER_POSITION, ResponseSDStartSync);
                }
                catch (Exception e)
                {
                }
                // If both players are good to go, then begin the transition sequence.
                StartCoroutine(MainGameTransition());
            }
        }

        /// <summary>
        /// Fades out audio and fades video to black.
        /// </summary>
        /// <returns></returns>
        private IEnumerator MainGameTransition()
        {
            StartCoroutine(FindObjectOfType<MainMixerController>().FadeOutAudio());
            fadeOutAnimator.SetTrigger("FadeOut");
            yield return new WaitForSeconds(FindObjectOfType<MainMixerController>().fadeTime);

            if (FindObjectOfType<SDPersistentData>() != null)
            {
                FindObjectOfType<SDPersistentData>().GetComponent<AudioSource>().enabled = false;
            }

            SceneManager.LoadScene("SDGameMain");
        }
    }
}
