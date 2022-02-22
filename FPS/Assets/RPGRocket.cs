using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGRocket : MonoBehaviour
{
    public float damage = 300f;
    public GameObject explodeEffect;
    public float radius = 8f;
    public float explosionForce = 100f;
    private bool hasExploded = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnCollisionEnter(Collision col)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in colliders)
        {
            Rigidbody rigidbody = collider.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.AddExplosionForce(explosionForce, transform.position, radius);
            }
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
        Destroy(Instantiate(explodeEffect, transform.position, transform.rotation), 3f);
        Destroy(gameObject);
    }
}
