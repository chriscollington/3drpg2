using UnityEngine;
using TMPro;

public class GunSystem : MonoBehaviour
{
    // Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    // Bools
    bool shooting, readyToShoot, reloading;

    // References
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    // Graphics 
    public GameObject muzzleFlash, bulletHoleGraphic;
    public TextMeshProUGUI text;
    public CameraShake camShake;
    public float camShakeMagnitude, camShakeDuration;

    // Audio
    public AudioClip machineGunSound;  // Reference to the machine gun sound effect
    private AudioSource audioSource;   // AudioSource component

    // Volume control for gun sound
    [Range(0f, 1f)] public float gunSoundVolume = 1f; // 1 is 100% volume, 0 is mute

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;

        // Initialize the AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Load the volume setting from PlayerPrefs
        if (PlayerPrefs.HasKey("GunVolume"))
        {
            gunSoundVolume = PlayerPrefs.GetFloat("GunVolume"); // Set the volume from saved preferences
        }
        else
        {
            gunSoundVolume = 1f; // Default volume if not set
        }

        // Update the audio source volume based on the current gunSoundVolume
        UpdateGunSoundVolume();
    }

    private void Update()
    {
        MyInput();

        // Set Text
        text.SetText(bulletsLeft + " / " + magazineSize);
    }

    private void MyInput()
    {
        // Skip input if the game is paused
        if (PauseMenu.isPaused)
            return;

        if (allowButtonHold)
            shooting = Input.GetKey(KeyCode.Mouse0);
        else
            shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
            Reload();

        // Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }

    private void Shoot()
    {
        // Skip shooting if the game is paused
        if (PauseMenu.isPaused)
            return;

        readyToShoot = false;

        // Play shooting sound with volume control
        if (machineGunSound != null && audioSource != null)
        {
            audioSource.volume = gunSoundVolume; // Set volume based on gunSoundVolume
            audioSource.PlayOneShot(machineGunSound);
        }

        // Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        // Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        // Muzzle flash
        GameObject flash = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity, attackPoint);
        Destroy(flash, 0.1f);

        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range))
        {
            if (((1 << rayHit.collider.gameObject.layer) & whatIsEnemy) != 0)
            {
                Debug.Log(rayHit.collider.name);

                if (rayHit.collider.CompareTag("Zombie"))
                    rayHit.collider.GetComponent<ZombieAI>().TakeDamage(damage);
            }
            else
            {
                Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.LookRotation(rayHit.normal));
            }
        }

        // Camera shake
        camShake.Shake(camShakeDuration, camShakeMagnitude);

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }

    // Method to update the gun sound volume (called by OptionsMenu)
    public void UpdateGunSoundVolume()
    {
        audioSource.volume = gunSoundVolume; // Apply the volume to the AudioSource
    }
}