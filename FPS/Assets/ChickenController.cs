using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenController : MonoBehaviour
{
    public float health = 10f;
    Flock_chicken agentFlock;
    public Flock_chicken AgentFlock { get { return agentFlock; } }

    Collider agentCollider;
    public Collider AgentCollider { get { return agentCollider; } }
    
    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<Collider>();
    }

    public void Initialize(Flock_chicken flock)
    {
        agentFlock = flock;
    }

    public void Move(Vector2 velocity)
    {
        Vector3 move = new Vector3();
        //move.y = -10 * Time.deltaTime * 15;
        move.x = velocity.x;
        move.z = velocity.y;
        transform.forward = move;
        transform.position += move * Time.deltaTime;
    }
    
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health<=0)
        {
            Destroy(gameObject);
        }
    }
}
