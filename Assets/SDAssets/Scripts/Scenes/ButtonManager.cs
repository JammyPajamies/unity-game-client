using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sound player script for button mouseovers and clicks.
/// </summary>
public class ButtonManager : MonoBehaviour
{
    public List<AudioClip> mouseoverSounds;
    public List<AudioClip> buttonClickSounds;
    public List<AudioClip> transistionSounds;

    private AudioSource audioSource;

	void Start ()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        audioSource = gameObject.GetComponent<AudioSource>();
	}

    // Play a random sound on mouseover.
    public void BtnPlayMouseoverSound()
    {
        audioSource.PlayOneShot(mouseoverSounds[Random.Range(0, (mouseoverSounds.Count))]);
    }

    // Play button click sound on mouse down.
    public void BtnPlayButtonClickSound()
    {
        audioSource.PlayOneShot(buttonClickSounds[Random.Range(0, (buttonClickSounds.Count))]);
    }

    // Play button click sound on transition screen.
    public void BtnPlaySceneTransitionSound()
    {
        audioSource.PlayOneShot(transistionSounds[Random.Range(0, (transistionSounds.Count))]);
    }
}
