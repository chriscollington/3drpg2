using UnityEngine;
using TMPro;  
public class ZombiePointsManager : MonoBehaviour
{
    // Canvas for displaying points
    public Canvas pointsCanvas;
    public TextMeshProUGUI pointsText;  // Use TextMeshProUGUI for better text rendering
    public TextMeshProUGUI highScoreText; // To display the high score

    // Points awarded per zombie
    public int pointsPerZombie = 10;

    // Track total points and high score
    private int totalPoints = 0;
    private int highScore = 0;

    // Singleton reference to the ZombiePointsManager
    public static ZombiePointsManager Instance;

    private void Awake()
    {
        // Ensure only one instance of the manager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Load the high score from PlayerPrefs if it exists
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void Start()
    {
        // Initialize the points and high score text
        UpdatePointsDisplay();
        UpdateHighScoreDisplay();
    }

    // Method to increase points when a zombie dies
    public void AddPoints(int points)
    {
        totalPoints += points;
        UpdatePointsDisplay();

        // Check if the current score is a new high score
        if (totalPoints > highScore)
        {
            highScore = totalPoints;
            PlayerPrefs.SetInt("HighScore", highScore); // Save the new high score
            UpdateHighScoreDisplay();  // Update the high score display
        }
    }

    // Method to update the points display
    private void UpdatePointsDisplay()
    {
        if (pointsText != null)
        {
            pointsText.text = "Points: " + totalPoints.ToString();
        }
    }

    // Method to update the high score display
    private void UpdateHighScoreDisplay()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore.ToString();
        }
    }
}