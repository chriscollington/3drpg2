using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        // Make sure to tag the AudioSource as "BackgroundMusic"
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.tag = "BackgroundMusic";  // Tag it as "BackgroundMusic" for volume control
            audioSource.Play();
        }
    }

    // You can also create methods to stop or pause the music
    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public void PauseMusic()
    {
        if (audioSource != null)
        {
            audioSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}