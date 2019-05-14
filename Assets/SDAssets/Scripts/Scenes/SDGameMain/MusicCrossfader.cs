using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// This script crossfades between provided clips.
/// It is currently set to use time left in the game
/// to crossfade music.
/// </summary>
namespace SD
{
    public class MusicCrossfader : MonoBehaviour
    {
        // The master audio mixer.
        public AudioMixer mainMixer;
        // List of audio players.
        private List<AudioSource> audioPlayers = new List<AudioSource>();
        // List of audio tracks
        public List<AudioClip> audioTracks = new List<AudioClip>();
        // Time in seconds of fadeout transition.
        public float fadeTime = 1.0f;
        // The max volume of the sound clip.
        public float maxVolume = 0.5f;

        // This is the number of transitions that will be played between 
        private int transitionCount;
        // This is the count of tranistions completed.
        private int numTransitionscompleted = 0;
        // Bool values to indicate transitions started.
        private bool[] transitionsStarted;

        // This is the ratio (step) of each transition.
        private float ratioThresholdPerTransition;
        // The maximal value for the scaling.
        private float totalUnits = 0.0f;

        // Hold a reference to the object that we will be querying to get our updated transition ratios.
        private Timer ratioScalingSource;

        private void Start()
        {
            // Setup transition info.
            transitionCount = audioTracks.Count - 1;
            //Debug.Log("TransitionCount: " + transitionCount);

            transitionsStarted = new bool[transitionCount];
            for (int i = 0; i < transitionCount; i++)
            {
                transitionsStarted[i] = false;
            }

            // Setup the ratio to compare against for transitions.
            ratioThresholdPerTransition = 1.0f / audioTracks.Count;
            //Debug.Log("TransitionRatio: " + transitionRatio);

            // Now, have the audio players start playing music,
            // but reduce the volume of all of the but the first one.
            for (int i = 0; i < audioTracks.Count; i++)
            {
                // Create the AudioSource components and make sure that they are quiet
                // if they are not the first track.
                AudioSource newSource = gameObject.AddComponent<AudioSource>();
                audioPlayers.Add(newSource);
                audioPlayers[i].outputAudioMixerGroup = mainMixer.FindMatchingGroups("Master")[0];
                audioPlayers[i].playOnAwake = false;
                audioPlayers[i].clip = audioTracks[i];
                if (i > 0)
                {
                    audioPlayers[i].volume = 0.0f;
                }
                else
                {
                    audioPlayers[i].volume = maxVolume;
                }
                audioPlayers[i].Play();
                audioPlayers[i].loop = true;

                // If the audio player isn't the first one (playing the first track),
                // then reduce its volume to 0.
            }

            // Get a reference to the object that we will be using.
            ratioScalingSource = FindObjectOfType<Timer>();
            // Get and store the value of the maximal amount of whatever units
            // that we will use for ratio calculations.
            totalUnits = ratioScalingSource.GetMaxTimeInGame();
        }

        void Update()
        {
            // Old method using the number of fish consumed.
            // Later fourn to be inconsistent with multiplayer
            // where the host player is infinitely spawning fish (up to a cap).
            /*
            int preyFishTotal = gameController.GetPreyFishTotal();
            int preyFishRemaining = gameController.GetPreyFishRemaining();
            float fishConsumedRatio = 1.0f - (float)preyFishRemaining / (float)preyFishTotal;
            int transitionIndexID = Mathf.FloorToInt(fishConsumedRatio / ratioThresholdPerTransition) - 1;
            */

            //Debug.Log("Remaining/total fish: " + preyFishRemaining + "/" + preyFishTotal + " Ratio: " + fishConsumedRatio);

            // Current implementation uses the time in the game and splits it evenly into chunks for calculations.
            float remainingUnits = ratioScalingSource.GetTimeRemaining();
            float fishConsumedRatio = 1.0f - (float)remainingUnits / (float)totalUnits;

            // This variable is initally -1 at the start of the game
            // and goes up in steps of 1 up to the max number of sound tracks.
            int transitionIndexID = Mathf.FloorToInt(fishConsumedRatio / ratioThresholdPerTransition) - 1;

            // If we have a ratio bigger than the ratio that would trigger a transition,
            // check if we need to start a transition.
            if (transitionIndexID >= 0 && transitionIndexID < transitionCount && !transitionsStarted[transitionIndexID])
            {
                // Get the transition that we would be doing given the ratios.
                //Debug.Log("TransitionID: " + transitionIndexID);

                // Now check to see if we have started a transition given the ID.
                // Start it if we haven't.
                if (transitionIndexID < transitionCount &&
                    transitionsStarted[transitionIndexID] == false)
                {
                    transitionsStarted[transitionIndexID] = true;
                    StartCoroutine(AudioCrossfade(transitionIndexID));
                }
            }

        }


        /// <summary>
        /// Async crossfader.
        /// Transitions between the ith and ith+1 items in the list of audioTracks.
        /// </summary>
        /// <param name="transitionIndex"></param>
        /// <returns></returns>
        private IEnumerator AudioCrossfade(int transitionIndex)
        {
            //Debug.Log("AudioCrossfade async called.");

            bool transitionTimeElapsed = false;
            float fadeTimeRemaining = 0.0f;
            float fadeoutVolumeClossness = Mathf.Abs(0.0f - audioPlayers[transitionIndex].volume);
            float fadeinVolumeClossness = Mathf.Abs(maxVolume - audioPlayers[transitionIndex + 1].volume);

            //Debug.Log(fadeoutVolumeClossness + " || " + fadeinVolumeClossness);

            while (!transitionTimeElapsed)
            {
                fadeTimeRemaining += Time.deltaTime;

                // Fade out the current track and...
                if (fadeoutVolumeClossness > (float)1e-2)
                {
                    audioPlayers[transitionIndex].volume = Mathf.Lerp(maxVolume, 0.0f, fadeTimeRemaining / fadeTime);
                }
                // Fade in the next track.
                if (fadeinVolumeClossness > (float)1e-2)
                {
                    audioPlayers[transitionIndex + 1].volume = Mathf.Lerp(0.0f, maxVolume, fadeTimeRemaining / fadeTime);
                }

                // Update the values.
                fadeoutVolumeClossness = Mathf.Abs(0 - audioPlayers[transitionIndex].volume);
                fadeinVolumeClossness = Mathf.Abs(maxVolume - audioPlayers[transitionIndex + 1].volume);

                // Wait for the next frame.
                yield return new WaitForEndOfFrame();

                if(fadeTimeRemaining > fadeTime)
                {
                    transitionTimeElapsed = true;
                }
            }

            // Set volume to zero and stop the previous player.
            audioPlayers[transitionIndex].volume = 0;
            audioPlayers[transitionIndex].Stop();

            // Set volume to the defined max.
            audioPlayers[transitionIndex + 1].volume = maxVolume;

            // Finally, flag this transition as completed.
            numTransitionscompleted++;

            //Debug.Log("AudioCrossfade async complete.");
            yield return null;
        }
    }
}
