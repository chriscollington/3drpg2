using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject Pausemenu;
    public static bool isPaused;
    public AudioSource backgroundMusic;  // Reference to the AudioSource for background music

    private void Start()
    {
        // Make sure the game is unpaused when the scene starts
        isPaused = false;
        Pausemenu.SetActive(false);
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();  // Ensure music is stopped when the game starts
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Pausemenu.SetActive(true);
        Time.timeScale = 0f; // Freeze time when paused
        isPaused = true;     // Set the pause flag

        // Play the background music only when the pause menu is active
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
    }

    public void ResumeGame()
    {
        Pausemenu.SetActive(false);
        Time.timeScale = 1f; // Resume normal time scale
        isPaused = false;    // Unpause the game

        // Stop the background music when the game resumes
        if (backgroundMusic != null && backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop();
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;  // Make sure to reset time scale before transitioning
        SceneManager.LoadScene("startScreen");
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("You Have Exited The Game");
    }
}