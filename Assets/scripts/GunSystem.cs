using System.Runtime.CompilerServices;
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

        // Play shooting sound
        if (machineGunSound != null && audioSource != null)
        {
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
}