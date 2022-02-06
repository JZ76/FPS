using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movements : MonoBehaviour
{
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
        isOnGround = Physics.CheckSphere(GroundCheck.position, groundDistance, groundMask);
        if (isOnGround && velocity.y < 0)
        {
            velocity.y = -0.02f;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isOnGround)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * Gravity * -1) / 20;
        }
        
        velocity.y += Gravity * Time.deltaTime * Time.deltaTime * 7;
        
        controller.Move(velocity);
    }
    void OnCollisionEnter(Collision col)
    {
        Rigidbody rigidbody = col.rigidbody;

        if (rigidbody != null)
        {
            Vector3 forceVector = col.gameObject.transform.position - transform.position;
            forceVector.y = 0;
            forceVector.Normalize();
            Debug.Log(forceVector);
            rigidbody.AddForceAtPosition(forceVector * force, transform.position, ForceMode.Impulse);
        }
        
    }
}
