using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pushObstacles : MonoBehaviour
{
    [SerializeField] public float force;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rigidbody = hit.controller.attachedRigidbody;

        if (rigidbody != null)
        {
            Vector3 forceVector = hit.gameObject.transform.position - transform.position;
            //forceVector.y = 0;
            //forceVector.Normalize();
            Debug.Log(forceVector);
            rigidbody.AddForceAtPosition(forceVector * force, transform.position, ForceMode.Impulse);
        }
    }
}
