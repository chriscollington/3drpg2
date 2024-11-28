using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  // Include this for scene management

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthBar;

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");

        // Unlock and show the mouse cursor before loading the startScreen
        Cursor.lockState = CursorLockMode.None;  // Unlock the mouse
        Cursor.visible = true;                   // Make sure the cursor is visible

        // Load the startScreen (main menu)
        SceneManager.LoadScene("startScreen");  // Ensure that "startScreen" is added in the Build Settings
    }
}