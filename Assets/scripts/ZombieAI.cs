using UnityEngine;
using UnityEngine.UI;

public class ZombieAI : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public float moveSpeed = 3.5f;
    public float detectionRange = 20f;
    public float attackRange = 2f;
    public int attackDamage = 10;
    public float attackCooldown = 1f;

    private Transform player;
    private bool isDead = false;
    private float timeSinceLastAttack = 0f;

    public Slider healthBarSlider;
    public Canvas healthCanvas;
    public Vector3 healthBarOffset = new Vector3(0, 2f, 0);
    public Camera HealthCamera;
    private Animator animator;

    public GameObject healthBarPrefab;

    // Sound slots
    public AudioClip walkingSound;
    public AudioClip attackSound;
    public AudioClip idleSound;
    public AudioClip deathSound;
    public AudioClip gruntSound;

    private AudioSource audioSource;
    private float gruntTimer;
    public float gruntIntervalMin = 3f;
    public float gruntIntervalMax = 10f;

    private bool isGrunting = false;
    private bool isAttacking = false; // To track if the zombie is attacking
    private float attackAnimationTime = 1f; // Estimated attack animation duration (in seconds)

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth;

        // Initialize AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configure AudioSource for 3D spatial sound
        audioSource.spatialBlend = 1f; // Set to 3D sound
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic; // Adjust based on distance
        audioSource.minDistance = 1f; // Full volume within 1 meter
        audioSource.maxDistance = 20f; // Volume fades out by 20 meters

        // Health bar setup
        if (HealthCamera == null)
            HealthCamera = Camera.main;

        if (healthBarPrefab != null)
        {
            GameObject healthBar = Instantiate(healthBarPrefab, transform.position + healthBarOffset, Quaternion.identity);
            healthBar.transform.SetParent(transform);
            healthCanvas = healthBar.GetComponentInChildren<Canvas>();
            healthBarSlider = healthBar.GetComponentInChildren<Slider>();
        }

        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = currentHealth;
        }

        animator = GetComponent<Animator>();

        // Set the initial timer for the first grunt
        gruntTimer = Random.Range(gruntIntervalMin, gruntIntervalMax);
        PlaySound(walkingSound, true);
    }

    private void Update()
    {
        // Skip everything if the game is paused
        if (isDead || PauseMenu.isPaused)
        {
            StopAllSounds();  // Ensure sounds are stopped when paused
            return;  // Skip the rest of the Update logic when paused
        }

        if (healthBarSlider != null)
            healthBarSlider.value = currentHealth;

        if (healthCanvas != null)
        {
            healthCanvas.transform.position = transform.position + healthBarOffset;
            if (HealthCamera != null)
                healthCanvas.transform.rotation = HealthCamera.transform.rotation;
        }

        if (player != null && Vector3.Distance(transform.position, player.position) < detectionRange)
        {
            MoveTowardsPlayer();

            if (Vector3.Distance(transform.position, player.position) <= attackRange && !isAttacking)
            {
                AttackPlayer();
            }
        }

        HandleGrunting();
    }

    private void MoveTowardsPlayer()
    {
        if (isAttacking) return;  // Prevent movement during attack animation

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        // Ensure walking sound plays if not grunting or attacking, and the game is not paused
        if (!isGrunting && !isAttacking && !PauseMenu.isPaused && (!audioSource.isPlaying || audioSource.clip != walkingSound))
        {
            PlaySound(walkingSound, true);
        }
    }

    private void AttackPlayer()
    {
        if (timeSinceLastAttack >= attackCooldown)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                if (animator != null)
                {
                    animator.SetTrigger("Attack"); // Trigger attack animation
                    PlayAttackSound(); // Play the attack sound when the attack animation is triggered
                }
                isAttacking = true;  // Set the attacking state
                Invoke(nameof(ResumePostAttackState), attackAnimationTime); // Resume post-attack state after animation time
            }
            timeSinceLastAttack = 0f;
        }
        else
        {
            timeSinceLastAttack += Time.deltaTime;
        }
    }

    private void HandleGrunting()
    {
        if (isAttacking || PauseMenu.isPaused) return;  // Prevent grunting while attacking or when paused

        gruntTimer -= Time.deltaTime;

        if (gruntTimer <= 0f && !isGrunting)
        {
            isGrunting = true;
            audioSource.Stop(); // Stop walking sound
            PlaySound(gruntSound, false); // Play grunt sound once
            Invoke(nameof(ResumeWalkingSoundAfterGrunt), gruntSound.length);
            gruntTimer = Random.Range(gruntIntervalMin, gruntIntervalMax); // Reset timer
        }
    }

    private void ResumeWalkingSoundAfterGrunt()
    {
        isGrunting = false;
        if (!isAttacking && !PauseMenu.isPaused)
        {
            PlaySound(walkingSound, true); // Resume walking sound if not attacking and the game is not paused
        }
    }

    private void ResumePostAttackState()
    {
        isAttacking = false;
        if (!PauseMenu.isPaused)
        {
            PlaySound(walkingSound, true);  // Resume walking sound after attack is done
        }
        HandleGrunting(); // Ensure grunting can resume if needed
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        if (animator != null)
            animator.SetTrigger("Dead");

        StopAllSounds();  // Stop all sounds when the zombie dies
        PlaySound(deathSound, false);

        ZombieManager zombieManager = FindObjectOfType<ZombieManager>();
        if (zombieManager != null)
            zombieManager.OnZombieDeath();

        // Award points for killing the zombie
        if (ZombiePointsManager.Instance != null)
        {
            ZombiePointsManager.Instance.AddPoints(ZombiePointsManager.Instance.pointsPerZombie);
        }


        Destroy(gameObject, 2f);
    }

    private void PlaySound(AudioClip clip, bool loop)
    {
        if (clip != null && !PauseMenu.isPaused)
        {
            audioSource.clip = clip;
            audioSource.loop = loop;
            audioSource.Play();
        }
    }

    private void StopAllSounds()
    {
        // Stop any currently playing sound
        audioSource.Stop();
    }

    // New method to play the attack sound
    private void PlayAttackSound()
    {
        if (attackSound != null && !PauseMenu.isPaused)
        {
            audioSource.clip = attackSound;
            audioSource.loop = false;
            audioSource.Play();
        }
    }

    // Pause-related methods to stop/resume walking sounds
    public void StopWalkingSound()
    {
        audioSource.Stop();  // Stop the walking sound while the game is paused
    }

    public void ResumeWalkingSoundOnUnpause()
    {
        if (!isDead && !isAttacking && !isGrunting && !PauseMenu.isPaused)
        {
            PlaySound(walkingSound, true);  // Resume walking sound after pause
        }
    }
}