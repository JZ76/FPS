using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float delay = 3f;
    public GameObject explodeEffect;
    public float radius = 3f;
    public float explosionForce = 100f;
    private float countDown;

    private bool isBoom = false;
    // Start is called before the first frame update
    void Start()
    {
        
        countDown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countDown -= Time.deltaTime;
        if (countDown <= 0 && !isBoom)
        {
            Explode();
            isBoom = true;
        }
    }

    void Explode()
    {
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in colliders)
        {
            Rigidbody rigidbody = collider.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.AddExplosionForce(explosionForce, transform.position, radius);
            }
        }
        Destroy(Instantiate(explodeEffect, transform.position, transform.rotation), 0.9f);
        Destroy(gameObject);
    }
}
