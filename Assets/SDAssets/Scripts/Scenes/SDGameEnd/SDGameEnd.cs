using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SD {
    public class SDGameEnd : MonoBehaviour {

        private SDPersistentData persistentObject;
        private Text txtResult;
        private Text txtScore;
        private Text txtOpponentScore;

        // Audio mixer for fade in of sound.
        private MainMixerController mainMixer;
        private AudioSource audioPlayer;
        public AudioClip clip;
        // The fade in and out transitions for the scene.
        public Animator fadeInAnimator;

        // Use this for initialization
        void Start ()
        {
            // Fade in the screen.
            fadeInAnimator.SetTrigger("FadeIn");

            mainMixer = MainMixerController.GetInstance();
            mainMixer.FadeInAudio();

            audioPlayer = gameObject.AddComponent<AudioSource>();
            audioPlayer.outputAudioMixerGroup = mainMixer.GetMainMixer().FindMatchingGroups("Master")[0];
            audioPlayer.playOnAwake = true;
            audioPlayer.loop = true;
            audioPlayer.volume = 0.5f;
            audioPlayer.clip = clip;

            txtResult = GameObject.Find ("TxtResult").GetComponent<Text> ();
            txtScore = GameObject.Find ("TxtScore").GetComponent<Text> ();
            txtOpponentScore = GameObject.Find ("TxtOpponentScore").GetComponent<Text> ();

            displayResult ();
            Constants.PLAYER_NUMBER = 0;

            audioPlayer.Play();
        }

        void displayResult() {
            persistentObject = SDPersistentData.GetInstance ();
            int finalScore = 0;
            int opponentScore = 0;
            if (GameController.getInstance() != null)
            {
                opponentScore = GameController.getInstance().getOpponentScore();
            }
            int winningScore = 0;
            int gameResult = 0;
            string result = null;

            if (persistentObject) {
                finalScore = GameController.getInstance ().getPlayerScore ();
                winningScore = persistentObject.getWinningScore ();
                gameResult = persistentObject.getGameResult ();
            } else {
                if (GameController.getInstance ()) {
                    finalScore = GameController.getInstance ().getPlayerScore ();
                }
            }

            if (gameResult == Constants.PLAYER_WIN) {
                result = "Congratulations ! You won this round !";
            } else if (gameResult == Constants.PLAYER_LOSE) {
                result = "Sorry ! You lost this round. ";
                if (GameController.getInstance ().getHasSurrendered ()) // TODO: Keeping it generic for now.
                   result = "Your gamed ended before time. " + result;
            } else if (gameResult == Constants.PLAYER_DRAW) {
                result = "A draw ! Your score is the same as your opponent's.";
            } else { 
                result = "Game result undefined in single player mode. ";
            }

            txtScore.text = "Your score: " + finalScore;
            txtOpponentScore.text = "Your opponent's score: " + opponentScore;
            txtResult.text = result;
        }
    }
}