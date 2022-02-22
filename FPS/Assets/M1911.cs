using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M1911 : MonoBehaviour
{
    public float damage = 40f;

    public float range = 35f;

    public float fireRate = 5f;

    public Camera player;

    public ParticleSystem muzzleFlash;

    public float hitForce = 5f;

    public GameObject bulletShell;
    public GameObject hitEffectTerrain;
    public GameObject hitEffectBody;
    public GameObject hitEffectRock;
    public GameObject hitEffectElse;

    public int maxAmmo = 10;

    private int currentAmmo = 10;

    public float reloadTime = 2f;

    private bool isReloading = false;

    public Animator ReloadAnimator;

    private float nextTTF = 0f;

    private float throwForce = 2f;

    // Start is called before the first frame update
    void Start()
    {

    }

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
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButtonDown("Fire1") && Time.time >= nextTTF)
        {
            nextTTF = Time.time + 1 / fireRate;
            Shoot();
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        ReloadAnimator.SetBool("isReloading", true);
        yield return new WaitForSeconds(reloadTime - 0.25f);
        ReloadAnimator.SetBool("isReloading", false);
        yield return new WaitForSeconds(0.25f);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    void Shoot()
    {
        muzzleFlash.Play();
        GameObject shell = Instantiate(bulletShell, transform.position, transform.rotation);
        Rigidbody ridRigidbody = shell.GetComponent<Rigidbody>();
        ridRigidbody.AddForce(transform.right * -throwForce, ForceMode.VelocityChange);
        Destroy(shell, 15f);
        RaycastHit hit;
        if (Physics.Raycast(player.transform.position, player.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
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

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * hitForce);
            }
        }

        currentAmmo--;
    }
}
