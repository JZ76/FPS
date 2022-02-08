using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowG : MonoBehaviour
{
    public float throwForce = 10f;

    public GameObject grenadePrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            ThrowGrenade();
        }
    }

    void ThrowGrenade()
    {
        GameObject grenade = Instantiate(grenadePrefab, transform.position, transform.rotation);
        Rigidbody ridRigidbody = grenade.GetComponent<Rigidbody>();
        ridRigidbody.AddForce(transform.up * throwForce, ForceMode.VelocityChange);
    }
}