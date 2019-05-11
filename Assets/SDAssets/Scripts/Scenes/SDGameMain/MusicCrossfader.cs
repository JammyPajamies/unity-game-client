using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script crossfades between provided clips.
/// It is currently set to use the ratio of remaining/total prey fish
/// for transition breakpoints.
/// </summary>
namespace SD
{
    public class MusicCrossfader : MonoBehaviour
    {
        // List of audio players.
        private List<AudioSource> audioPlayers = new List<AudioSource>();
        // List of audio tracks
        public List<AudioClip> audioTracks = new List<AudioClip>();
        // Time in seconds of fadeout transition.
        public float fadeoutTime = 1.0f;
        // The max volume of the sound clip.
        public float maxVolume = 0.5f;

        // This is the number of transitions that will be played between 
        private int transitionCount;
        // This is the count of tranistions completed.
        private int numTransitionscompleted = 0;
        // Bool values to indicate transitions completed.
        private bool[] transitionsCompleted;
        // Bool values to indicate transitions started.
        private bool[] transitionsStarted;

        // This is the ratio (step) of each transition.
        private float transitionRatio;

        // Get remaining/total fish counts from the game controller.
        private GameController gameController;

        private void Start()
        {
            // Setup transition info.
            transitionCount = audioTracks.Count - 1;

            // Setup and zero out the bool arrays.
            transitionsCompleted = new bool[transitionCount];
            for (int i = 0; i < transitionCount; i++)
            {
                transitionsCompleted[i] = false;
            }

            transitionsStarted = new bool[transitionCount];
            for (int i = 0; i < transitionCount; i++)
            {
                transitionsStarted[i] = false;
            }

            // Setup the ratio to compare against for transitions.
            transitionRatio = 1 / transitionCount;

            // Now, have the audio players start playing music,
            // but reduce the volume of all of the but the first one.
            for(int i = 0; i < audioTracks.Count; i++)
            {
                AudioSource newSource = gameObject.AddComponent<AudioSource>();
                audioPlayers.Add(newSource);
                audioPlayers[i].clip = audioTracks[i];
                audioPlayers[i].Play();
                audioPlayers[i].loop = true;
                audioPlayers[i].volume = 0.5f;

                Debug.Log("Created audioScource #" + i);

                // If the audio player isn't the first one (playing the first track),
                // then reduce its volume to 0.
                if (i > 0)
                {
                    audioPlayers[i].volume = 0.0f;
                }
            }

            gameController = GameController.getInstance();
        }

        void Update()
        {

            float fishConsumedRatio = gameController.GetPreyFishTotal() - gameController.GetPreyFishRemaining() / gameController.GetPreyFishTotal();

            Debug.Log("Remaining/total fish: " + gameController.GetPreyFishRemaining() + "/" + gameController.GetPreyFishTotal() + " Ratio: " + fishConsumedRatio);
            // If we have a ratio bigger than the ratio that would trigger a transition,
            // check if we need to start a transition.
            if (fishConsumedRatio > transitionRatio * (numTransitionscompleted + 1))
            {
                // Ge tthe transition that we would be doing given the ratios.
                int transitionID = 1 - Mathf.FloorToInt(fishConsumedRatio / transitionRatio);
                Debug.Log("TransitionID: " + transitionID);

                // Now check to see if we have started a transition given the ID.
                // Start it if we haven't.
                if(transitionID > 0 &&
                    transitionID < transitionCount &&
                    transitionsStarted[transitionID] == false &&
                    transitionsCompleted[transitionID] == false)
                {
                    transitionsStarted[transitionID] = true;
                }
            }

            AudioCrossfade();
        }

        private void AudioCrossfade()
        {
            // Finally, handle the actual crissfade logic.
            for (int i = 0; i < transitionCount; i++)
            {
                // If we have a transition flagged to run and haven't finished it,
                // then contiue the transition.
                if (transitionsStarted[i] && !transitionsCompleted[i])
                {
                    // Fade out the current track and...
                    audioPlayers[i].volume = Mathf.Lerp(audioPlayers[i].volume, 0, fadeoutTime * Time.deltaTime);
                    // Fade in the next track.
                    audioPlayers[i + 1].volume = Mathf.Lerp(audioPlayers[i + 1].volume, maxVolume, fadeoutTime * Time.deltaTime);

                    // Now check for completion of crossfading.
                    float fadeoutVolumeClossness = Mathf.Abs(0 - audioPlayers[i].volume);
                    float fadeinVolumeClossness = Mathf.Abs(maxVolume - audioPlayers[i + 1].volume);

                    // If they are really close, then hard se tto them to predetermined values and mark
                    // the transition as completed.
                    if (fadeoutVolumeClossness < 1e-6 && fadeinVolumeClossness < 1e-6)
                    {
                        // Set volume to zero and stop the previous player.
                        audioPlayers[i].volume = 0;
                        audioPlayers[i].Stop();

                        // Set volume to the defined max.
                        audioPlayers[i + 1].volume = maxVolume;

                        // Finally, flag this transition as completed.
                        transitionsCompleted[i] = true;
                        numTransitionscompleted++;
                    }
                }
            }
        }
    }
}
