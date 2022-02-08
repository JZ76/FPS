using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M107 : MonoBehaviour
{
    public float damage = 200f;

    public float range = 450f;

    public float fireRate = 1f;

    public Camera player;

    public ParticleSystem muzzleFlash;

    public float hitForce = 10f;

    public GameObject bulletShell;
    public GameObject hitEffectTerrain;
    public GameObject hitEffectBody;
    public GameObject hitEffectRock;
    public GameObject hitEffectElse;

    public int maxAmmo = 5;
    
    private int currentAmmo = 5;

    public float reloadTime = 6f;

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
        ridRigidbody.AddForce(transform.right  * -throwForce, ForceMode.VelocityChange);
        Destroy(shell, 15f);
        RaycastHit hit;
        if (Physics.Raycast(player.transform.position, player.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            if (hit.transform.name.Contains("Rock"))
            {
                Instantiate(hitEffectRock, hit.point, Quaternion.LookRotation(hit.normal));
            }else if (hit.transform.name.Contains("Terrain"))
            {
                Instantiate(hitEffectTerrain, hit.point, Quaternion.LookRotation(hit.normal));
            }else if (hit.transform.name.Contains("target"))
            {
                Instantiate(hitEffectBody, hit.point, Quaternion.LookRotation(hit.normal));
            }else
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
