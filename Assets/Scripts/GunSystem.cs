
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunSystem : MonoBehaviour
{
    // Gun stats
    public int gunDamage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    private int bulletsLeft, bulletsShot;

    private bool shooting, readyToShoot, reloading;

    // Reference
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;
    private Rigidbody playerRB;
    
    // Graphics
    public CamShake camShake;
    public float camShakeMagnitude, camShakeDuration;
    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
    private void Update()
    {
        GunInput();
    }

    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
    }
    
    private void GunInput()
    {
        if (allowButtonHold)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }
        
        // Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;
        
        // Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        if (playerRB.velocity.magnitude > 0)
        {
            spread = spread * 1.5f;
        }
        else spread = "normal spread";
        
        // Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);
        
        // RayCast
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out rayHit, range, whatIsEnemy))
        {
            Debug.Log(rayHit.collider.name);

            if (rayHit.collider.CompareTag("Enemy"))
            {
                // Null Reference ShootingAi, TakeDamage
                rayHit.collider.GetComponent<ShootingAi>().TakeDamage(gunDamage);
            }
        }
        
        // ShakeCamera
        camShake.Shake(camShakeMagnitude, camShakeDuration);
        
        bulletsLeft--;
        bulletsShot--;
        
        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShooting);
        }
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
