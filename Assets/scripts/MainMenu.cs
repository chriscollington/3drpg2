using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public AudioClip backgroundMusic;  // Public variable to assign background music in the inspector
    private AudioSource audioSource;
    public Slider musicVolumeSlider;  // Reference to the slider UI element for controlling music volume

    void Start()
    {
        // Initialize the AudioSource component
        audioSource = GetComponent<AudioSource>();

        // Play background music if it is assigned
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;  // Set the background music to loop
            audioSource.Play();
            audioSource.tag = "MainMenuSound"; // Tag it as "MainMenuSound" for volume control
        }

        // Initialize the volume slider to the saved volume setting
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            musicVolumeSlider.value = 1f; // Default volume is 100% if no saved preference
        }

        // Add listener to change volume on slider value change
        musicVolumeSlider.onValueChanged.AddListener(UpdateMusicVolume);
        UpdateMusicVolume(musicVolumeSlider.value); // Apply the initial volume when the scene starts
    }

    // Update the music volume and save the setting
    private void UpdateMusicVolume(float value)
    {
        audioSource.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value); // Save the volume setting
    }

    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("You Have Exited The Game");
    }
}