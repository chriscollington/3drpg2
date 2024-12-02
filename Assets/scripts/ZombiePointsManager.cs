using UnityEngine;
using TMPro;  // Import the TextMeshPro namespace

public class ZombiePointsManager : MonoBehaviour
{
    // Canvas for displaying points
    public Canvas pointsCanvas;
    public TextMeshProUGUI pointsText;  // Use TextMeshProUGUI for better text rendering

    // Points awarded per zombie
    public int pointsPerZombie = 10;

    // Track total points
    private int totalPoints = 0;

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
    }

    private void Start()
    {
        // Initialize the points text
        UpdatePointsDisplay();
    }

    // Method to increase points when a zombie dies
    public void AddPoints(int points)
    {
        totalPoints += points;
        UpdatePointsDisplay();
    }

    // Method to update the points display
    private void UpdatePointsDisplay()
    {
        if (pointsText != null)
        {
            pointsText.text = "Points: " + totalPoints.ToString();
        }
    }
}