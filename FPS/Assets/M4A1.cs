using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M4A1 : MonoBehaviour
{
    public float damage = 20f;

    public float range = 150f;

    public float fireRate = 7f;

    public Camera player;

    public ParticleSystem muzzleFlash;

    public float hitForce = 3f;

    public GameObject bulletShell;
    public GameObject hitEffectTerrain;
    public GameObject hitEffectBody;
    public GameObject hitEffectRock;
    public GameObject hitEffectElse;

    public int maxAmmo = 31;
    
    private int currentAmmo = 31;

    public float reloadTime = 3f;

    private bool isReloading = false;

    public Animator ReloadAnimator;
    
    private float nextTTF = 0f;

    private float throwForce = 2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /*
     * when switch to a new weapon, player can shoot immediately, unless this weapon also need reloading
     */
    private void OnEnable()
    {
        isReloading = false;
        ReloadAnimator.SetBool("isReloading", false);
    }
    // Update is called once per frame
    void Update()
    {
        if (isReloading)
        {
            return;
        }
        if (currentAmmo <= 0 || Input.GetKeyDown(KeyCode.R))
        {
            // start reloading
            StartCoroutine(Reload());
            return;
        }
        if (Input.GetButton("Fire1") && Time.time >= nextTTF)
        {
            // shoot the gun
            nextTTF = Time.time + 1 / fireRate;
            Shoot();
        }
    }
    
    IEnumerator Reload()
    {
        isReloading = true;
        // start animation
        ReloadAnimator.SetBool("isReloading", true);
        yield return new WaitForSeconds(reloadTime - 0.25f);
        // stop animation
        ReloadAnimator.SetBool("isReloading", false);
        yield return new WaitForSeconds(0.25f);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    void Shoot()
    {
        muzzleFlash.Play();
        // keep creating bullet shell
        GameObject shell = Instantiate(bulletShell, transform.position, transform.rotation);
        Rigidbody ridRigidbody = shell.GetComponent<Rigidbody>();
        // throw the bullet shell
        ridRigidbody.AddForce(transform.right  * -throwForce, ForceMode.VelocityChange);
        // destroy bullet shell after 15 second
        Destroy(shell, 15f);
        RaycastHit hit;
        // if hit something
        if (Physics.Raycast(player.transform.position, player.transform.forward, out hit, range))
        {
            // different object has different hit effect
            if (hit.transform.name.Contains("Rock"))
            {
                Instantiate(hitEffectRock, hit.point, Quaternion.LookRotation(hit.normal));
            }
            else if (hit.transform.name.Contains("Terrain"))
            {
                Instantiate(hitEffectTerrain, hit.point, Quaternion.LookRotation(hit.normal));
            }
            else if (hit.transform.name.Contains("target"))
            {
                Instantiate(hitEffectBody, hit.point, Quaternion.LookRotation(hit.normal));
                // make damage to zombie or chicken
                Controller_Zombie zombie = hit.transform.GetComponent<Controller_Zombie>();
                if (zombie != null)
                {
                    zombie.TakeDamage(damage);
                }

                Controller_Zombie_NavMesh zombie_nav = hit.transform.GetComponent<Controller_Zombie_NavMesh>();
                if (zombie_nav != null)
                {
                    zombie_nav.TakeDamage(damage);
                }
                
                ChickenController chicken = hit.transform.GetComponent<ChickenController>();
                if (chicken != null)
                {
                    chicken.TakeDamage(damage);
                }
            }
            else
            {
                Instantiate(hitEffectElse, hit.point, Quaternion.LookRotation(hit.normal));
            }
            // add force when hit an object
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * hitForce);
            }
        }
        // remember to minus one
        currentAmmo--;
    }
}
