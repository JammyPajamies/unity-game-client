using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Audio;

namespace SD {
    public class SDPersistentData : MonoBehaviour {

        private int playerFinalScore;
        // -1 = error, 0 = mackeral, 1 = striped bass, 2 = barracuda
        private int playerFishSelectionIndex = 0;
        // -1 = error, 0 = mackeral, 1 = striped bass, 2 = barracuda
        private int opponentFishSelectionIndex = 0;
        private int roundsCompleted;
        private bool isGameCompleted;
        private int winningScore;
        private int gameResult;
        private DateTime roundStartTime;

        private static SDPersistentData sdPersistentData;
        /*private SDConnectionManager cManager; TODO
        private SDMessageQueue mQueue;*/
        
        private static AudioSource audioSource;
        public AudioMixer mainMixer;

        public Animator fadeInAnimator;

        void Awake() {
            if (sdPersistentData) {
                DestroyImmediate (gameObject);
            } else {
                DontDestroyOnLoad (gameObject);
                sdPersistentData = this;
            }
            // The timer should run even if the application is in the background.
            Application.runInBackground = true;
        }

        void Start() {
            initializeData ();
            /*cManager = SDConnectionManager.getInstance (); TODO 
            mQueue = SDMessageQueue.getInstance ();
            if (cManager && mQueue) {
                if (!mQueue.callbackList.ContainsKey (Constants.SMSG_DISCONNECT))
                    mQueue.AddCallback (Constants.SMSG_DISCONNECT, ResponseSDOpponentDisconnect);
            }*/
            if (SDMain.networkManager != null) {
                SDMain.networkManager.Listen (NetworkCode.SD_DISCONNECT, ResponseSDOpponentDisconnect);
            }

            // Find the audio mixer and ask it to fade in.
            FindObjectOfType<MainMixerController>().FadeInAudio();
            // Finally, fade in the screen.
            fadeInAnimator.SetTrigger("FadeIn");
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        void Update () {

        }

        void OnDestroy() {
            // Switching back to the default setting.
            Application.runInBackground = false;
        }

        public void initializeData() {
            setPlayerFinalScore (0);
            setRoundsCompleted (0);
            setIsGameCompleted (false);  // is the current game completed.
            setWinningScore(0);
            setGameResult (0);
            setRoundStartTime (DateTime.UtcNow.ToString());
        }

        public static SDPersistentData GetInstance() {
            return sdPersistentData;
        }

        // Plays a one-shot audio clip.
        // TODO: put into audio mixer controller and make that a persistent object instead of using this script.
        public void PlayOneShot(AudioClip clip)
        {
            audioSource.outputAudioMixerGroup = mainMixer.FindMatchingGroups("Master")[0];
            audioSource.playOnAwake = false;
            audioSource.volume = 0.5f;
            audioSource.loop = false;
            audioSource.clip = clip;
            audioSource.enabled = true;
            audioSource.PlayOneShot(clip);
        }

        public void setPlayerFinalScore(int score) {
            playerFinalScore = score;
        }

        public int GetPlayerFishSelectionIndex()
        {
            return playerFishSelectionIndex;
        }

        public void SetPlayerFishSelectionIndex(int fish)
        {
            playerFishSelectionIndex = fish;
        }

        public int GetOpponentFishSelectionIndex()
        {
            return opponentFishSelectionIndex;
        }

        public void SetOpponentFishSelectionIndex(int fish)
        {
            opponentFishSelectionIndex = fish;
        }

        public int getPlayerFinalScore() {
            return playerFinalScore;
        }

        public void setRoundsCompleted(int rounds) {
            roundsCompleted = rounds;
        }

        public int getRoundsCompleted() {
            return roundsCompleted;
        }

        public void setIsGameCompleted(bool isCompleted) {
            isGameCompleted = isCompleted;
        }

        public bool getIsGameCompleted() {
            return isGameCompleted;
        }

        public void setWinningScore(int score) {
            winningScore = score;
        }

        public int getWinningScore() {
            return winningScore;
        }

        public void setGameResult(int status) {
            gameResult = status;
        }

        public int getGameResult() {
            return gameResult;
        }

        public void setRoundStartTime(string dateTimeString) {
            roundStartTime = DateTime.Parse (dateTimeString);
            Debug.Log ("The Round start time is " + roundStartTime);
            Debug.Log ("UTC Now is " + DateTime.UtcNow);
        }

        public DateTime getRoundStartTime() {
            return roundStartTime;
        }

        public void ResponseSDOpponentDisconnect(NetworkResponse r) {
            Debug.Log ("Opponent Disconnected");
        }
    }
}
