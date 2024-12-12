using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeControl : MonoBehaviour
{
    public Slider musicVolumeSlider; // Reference to the Slider UI element
    private AudioSource[] allAudioSources;  // Array to store all AudioSources in the scene

    void Start()
    {
        // Load the saved volume from PlayerPrefs if it exists
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            musicVolumeSlider.value = 1f; // Default volume is 100% if no saved preference
        }

        // Set up listener to handle volume changes
        musicVolumeSlider.onValueChanged.AddListener(UpdateMusicVolume);

        // Apply the initial volume to all AudioSources
        allAudioSources = FindObjectsOfType<AudioSource>();
        ApplyMusicVolumeToAllAudioSources();
    }

    // Method to update the volume and save it to PlayerPrefs
    private void UpdateMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value); // Save the volume setting
        ApplyMusicVolumeToAllAudioSources(); // Apply volume to all AudioSources
    }

    // Apply the volume setting to all AudioSources in the scene
    private void ApplyMusicVolumeToAllAudioSources()
    {
        foreach (var audioSource in allAudioSources)
        {
            // Only modify AudioSources related to music or main menu sounds
            if (audioSource.CompareTag("BackgroundMusic") || audioSource.CompareTag("MainMenuSound"))
            {
                audioSource.volume = musicVolumeSlider.value;
            }
        }
    }
}