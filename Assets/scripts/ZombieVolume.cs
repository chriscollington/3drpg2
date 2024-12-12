using UnityEngine;
using UnityEngine.UI;

public class ZombieVolume : MonoBehaviour
{
    // Reference to the Slider UI element for adjusting volume
    public Slider volumeSlider;

    private void Start()
    {
        // Load the saved volume from PlayerPrefs if it exists
        if (PlayerPrefs.HasKey("ZombieVolume"))
        {
            volumeSlider.value = PlayerPrefs.GetFloat("ZombieVolume");
        }
        else
        {
            volumeSlider.value = 1f; // Default volume is 100% if no saved preference
        }

        // Set up listener to handle volume changes
        volumeSlider.onValueChanged.AddListener(UpdateVolume);

        // Apply the initial volume to all Zombie AI audio sources
        ApplyVolumeToZombieAI();
    }

    // Method to update the volume and save it to PlayerPrefs
    private void UpdateVolume(float value)
    {
        // Save the volume setting to PlayerPrefs
        PlayerPrefs.SetFloat("ZombieVolume", value);

        // Apply the updated volume to all Zombie AI audio sources
        ApplyVolumeToZombieAI();
    }

    // Apply the volume setting to all Zombie AI AudioSources in the scene
    private void ApplyVolumeToZombieAI()
    {
        // Find all objects of type ZombieAI in the scene
        ZombieAI[] zombies = FindObjectsOfType<ZombieAI>();

        foreach (var zombie in zombies)
        {
            // Set the volume of the zombie's audio source to the slider's value
            if (zombie != null && zombie.GetComponent<AudioSource>() != null)
            {
                zombie.GetComponent<AudioSource>().volume = volumeSlider.value;
            }
        }
    }
}