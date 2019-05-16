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

    private static ButtonManager buttonManager;

    private void Awake()
    {
        buttonManager = this;
    }

    void Start ()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public static ButtonManager GetInstance()
    {
        return buttonManager;
    }

    // Play a random sound on mouseover.
    public void BtnPlayMouseoverSound()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(mouseoverSounds[Random.Range(0, (mouseoverSounds.Count))]);
    }

    // Play button click sound on mouse down.
    public void BtnPlayButtonClickSound()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(buttonClickSounds[Random.Range(0, (buttonClickSounds.Count))]);
    }

    // Play button click sound on transition screen.
    public void BtnPlaySceneTransitionSound()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(transistionSounds[Random.Range(0, (transistionSounds.Count))]);
    }
}
