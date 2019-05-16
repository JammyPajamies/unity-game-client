using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SD
{
    public class RandomSoundPlayer : MonoBehaviour
    {
        public List<AudioClip> listOfClipsOne;
        public List<AudioClip> listOfClipsTwo;
        public List<AudioClip> listOfClipsThree;

        private AudioSource playerAudioSource;

        // Use this for initialization
        void Start()
        {
            Random.InitState(System.DateTime.Now.Millisecond);

            playerAudioSource = PlayerController.GetInstance().gameObject.GetComponent<AudioSource>();
        }

        // Play a random sound from one of the lists.
        public void PlayRandomSound()
        {
            bool soundPlayed = false;
            int randomList = -1;
            int randomClip = -1;

            while(!soundPlayed)
            {
                randomList = Random.Range(0, 3);

                switch(randomList)
                {
                    case 0:
                        {
                            if (listOfClipsOne.Count == 0)
                                break;
                            
                            randomClip = Random.Range(0, listOfClipsOne.Count);
                            playerAudioSource.PlayOneShot(listOfClipsOne[randomClip]);
                            soundPlayed = true;

                            break;
                        }
                    case 1:
                        {
                            if (listOfClipsTwo.Count == 0)
                                break;

                            randomClip = Random.Range(0, listOfClipsTwo.Count);
                            playerAudioSource.PlayOneShot(listOfClipsTwo[randomClip]);
                            soundPlayed = true;
                            
                            break;
                        }
                    case 2:
                        {
                            if (listOfClipsThree.Count == 0)
                                break;

                            randomClip = Random.Range(0, listOfClipsThree.Count);
                            playerAudioSource.PlayOneShot(listOfClipsThree[randomClip]);
                            soundPlayed = true;
                            
                            break;
                        }
                    default:
                        break;
                }
            }

            //Debug.Log("Playedclip #" + randomClip + " from list #" + randomList);
        }
    }
}
