using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    // Reference to the Slider UI element for gun volume control
    public Slider volumeSlider;

    private void Start()
    {
        // Load the saved volume from PlayerPrefs if it exists
        if (PlayerPrefs.HasKey("GunVolume"))
        {
            volumeSlider.value = PlayerPrefs.GetFloat("GunVolume");
        }
        else
        {
            volumeSlider.value = 1f; // Default volume is 100% if no saved preference
        }

        volumeSlider.onValueChanged.AddListener(UpdateVolume);

        // Apply the volume to all GunSystem audio sources in the scene
        ApplyVolumeToGunSystems(volumeSlider.value);
    }

    // Method to update the volume and save it to PlayerPrefs
    private void UpdateVolume(float value)
    {
        PlayerPrefs.SetFloat("GunVolume", value); // Save the volume setting

        // Apply the volume to all GunSystem audio sources
        ApplyVolumeToGunSystems(value);
    }

    // Method to apply the saved volume to all GunSystem components in the scene
    private void ApplyVolumeToGunSystems(float volume)
    {
        // Find all GunSystem components in the scene and set their gun sound volume
        GunSystem[] gunSystems = FindObjectsOfType<GunSystem>();
        foreach (GunSystem gunSystem in gunSystems)
        {
            gunSystem.gunSoundVolume = volume; // Set the volume of each GunSystem
            gunSystem.UpdateGunSoundVolume(); // Ensure the gun sound volume is updated
        }
    }
}