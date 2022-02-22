using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Controller_Zombie_NavMesh : MonoBehaviour
{
    public float health = 100f;
    
    public float damage = 20f;
    
    public float radius = 25f;
    
    [Range(0, 360)] public float angle = 60f;

    public NavMeshAgent agent;
    
    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;
    
    public float reloadTime = 3f;

    private bool isReloading = false;
    
    public bool canSeePlayer;

    Transform target;
    
    public Animator Animator;

    void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    void Update()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, 2, targetMask);

        if (rangeChecks.Length != 0)
        {
            Vector3 directionToTarget = (playerRef.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < 90)
            {
                if (isReloading)
                {
                    return;
                }

                Animator.SetBool("canAttack", true);
                playerRef.transform.GetComponent<Movements>().TakeDamage(damage);

                StartCoroutine(Reload());
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(1);
        Animator.SetBool("canAttack", false);
        yield return new WaitForSeconds(reloadTime - 1);
        isReloading = false;
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            if (canSeePlayer)
            {
                Animator.SetBool("isFlocking", false);
                Animator.SetBool("isSeen", true);
                agent.SetDestination(target.position);
            }
            else
            {
                Animator.SetBool("isFlocking", true);
                Animator.SetBool("isSeen", false);
                if (health < 100f)
                {
                    agent.SetDestination(playerRef.transform.position);
                }
            }

            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                canSeePlayer = true;
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
        }
        else
        {
            canSeePlayer = false;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        agent.SetDestination(playerRef.transform.position);
        canSeePlayer = true;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
