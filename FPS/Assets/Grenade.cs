using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float damage = 150f;
    
    public float delay = 3f;
    
    public GameObject explodeEffect;
    
    public float radius = 3f;
    
    public float explosionForce = 100f;
    
    private float countDown;

    private bool isBoom = false;

    // Start is called before the first frame update
    void Start()
    {
        // start countdown after threw a grenade
        countDown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countDown -= Time.deltaTime;
        if (countDown <= 0 && !isBoom)
        {
            // when countdown to zero, explode only once
            Explode();
            isBoom = true;
        }
    }

    void Explode()
    {
        // get everything in radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in colliders)
        {
            Rigidbody rigidbody = collider.GetComponent<Rigidbody>();
            // if it is a rigidbody, add a explosion force to it
            if (rigidbody != null)
            {
                rigidbody.AddExplosionForce(explosionForce, transform.position, radius);
            }
            // make damage to zombie, chicken, and player
            Controller_Zombie zombie = collider.transform.GetComponent<Controller_Zombie>();
            if (zombie != null)
            {
                zombie.TakeDamage(damage);
            }

            Controller_Zombie_NavMesh zombie_nav = collider.transform.GetComponent<Controller_Zombie_NavMesh>();
            if (zombie_nav != null)
            {
                zombie_nav.TakeDamage(damage);
            }

            ChickenController chicken = collider.transform.GetComponent<ChickenController>();
            if (chicken != null)
            {
                chicken.TakeDamage(damage);
            }

            Movements player = collider.transform.GetComponent<Movements>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
        // destroy the grenade object
        Destroy(Instantiate(explodeEffect, transform.position, transform.rotation), 0.9f);
        Destroy(gameObject);
    }
}
