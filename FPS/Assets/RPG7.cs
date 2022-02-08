using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class RPG7 : MonoBehaviour
{
    public float fireForce = 100f;

    public GameObject RocketPrefab;
    
    public GameObject fakeRocketPrefab;

    public int maxAmmo = 1;
    
    private int currentAmmo = 1;

    public float reloadTime = 10f;

    private bool isReloading = false;

    public Animator ReloadAnimator;
    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = maxAmmo;
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
        if (Input.GetButtonDown("Fire1"))
        {
            Launch();
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        ReloadAnimator.SetBool("isReloading", true);
        yield return new WaitForSeconds(reloadTime - 0.25f);
        ReloadAnimator.SetBool("isReloading", false);
        GameObject fakeRocket = Instantiate(fakeRocketPrefab, new Vector3(transform.position.x,transform.position.y, transform.position.z) , transform.rotation);
        fakeRocket.transform.parent = gameObject.transform;
        yield return new WaitForSeconds(0.25f);
        currentAmmo = maxAmmo;
        isReloading = false;
    }
    void Launch()
    {
        GameObject grenade = Instantiate(RocketPrefab, transform.position, transform.rotation);
        Rigidbody ridRigidbody = grenade.GetComponent<Rigidbody>();
        ridRigidbody.AddForce(transform.forward * -fireForce, ForceMode.VelocityChange);
        
        GameObject rocket = gameObject.transform.GetChild(0).gameObject;
        Destroy(rocket);
        currentAmmo--;
    }
}
