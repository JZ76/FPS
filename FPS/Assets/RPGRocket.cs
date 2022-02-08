using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGRocket : MonoBehaviour
{
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
        }
        Destroy(Instantiate(explodeEffect, transform.position, transform.rotation), 3f);
        Destroy(gameObject);
        
    }
}
