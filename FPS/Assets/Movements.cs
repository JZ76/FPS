using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movements : MonoBehaviour
{
    public float health = 100f;
    
    public CharacterController controller;
    
    [SerializeField] public float force;
    
    public float speed = 4f;
    
    public float Gravity = -10f;
    
    public Transform GroundCheck;
    
    public float groundDistance = 0.3f;
    
    public LayerMask groundMask;
    
    public float jumpHeight = 1f;
    
    private Vector3 velocity;
    
    private bool isOnGround;
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if player not on Ground layer, cannot jump
        isOnGround = Physics.CheckSphere(GroundCheck.position, groundDistance, groundMask);
        if (isOnGround && velocity.y < 0)
        {
            velocity.y = -0.02f;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // move toward camera direction
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isOnGround)
        {
            // Jump!
            velocity.y = Mathf.Sqrt(jumpHeight * Gravity * -1) / 30;
        }
        
        velocity.y += Gravity * Time.deltaTime * Time.deltaTime * 7;
        
        controller.Move(velocity);
    }
    /*
     * I want to add a force to the object that player collied with
     */
    void OnCollisionEnter(Collision col)
    {
        Rigidbody rigidbody = col.rigidbody;
        // must be a rigidbody
        if (rigidbody != null)
        {
            // point to the object
            Vector3 forceVector = col.gameObject.transform.position - transform.position;
            forceVector.y = 0;
            forceVector.Normalize();
            rigidbody.AddForceAtPosition(forceVector * force, transform.position, ForceMode.Impulse);
        }
    }
    /*
     * player will take damage from Zombie, RPG, and Grenade
     */
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health<=0)
        {
            // if health below 0, game over
            Destroy(gameObject);
        }
    }
}
