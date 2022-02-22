using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Zombie : MonoBehaviour
{
    public float health = 100f;
    
    public float damage = 20f;
    
    public float radius = 25f;
    
    [Range(0, 360)] public float angle = 60f;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;
    
    public float reloadTime = 3f;

    private bool isReloading = false;
    
    public bool canSeePlayer;

    Transform target;
    
    public float speed = 2;
    
    public float rotationSpeed = 1;
    
    Vector3[] path;
    
    int targetIndex;

    Flock_zombie agentFlock;

    public Flock_zombie AgentFlock
    {
        get { return agentFlock; }
    }

    Collider agentCollider;

    public Collider AgentCollider
    {
        get { return agentCollider; }
    }

    public Animator Animator;

    /*
     * the zombie has three main behaviors, flocking, pathfinding, and attacking
     * when zombie doesn't see the player, they will move as flocking behavior
     * once the player inside their sights, they will use path finding to find a shortest way to the position of player at
     * when close enough to the player, the zombie can perform an attack, which will damage 20 points to the player
     */
    void Start()
    {
        // we will use collider to avoid collision between zombies
        agentCollider = GetComponent<Collider>();
        // also need player 
        playerRef = GameObject.FindGameObjectWithTag("Player");
        // in this coroutine, we update sights and pathfinding
        // the reason that not using update function is they are computation consuming, and we don't have to update 
        // them every frame
        StartCoroutine(FOVRoutine());
    }

    void Update()
    {
        // here, I just check whether a zombie into attack range
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
                // play attack animation and make damage to the player
                Animator.SetBool("canAttack", true);
                playerRef.transform.GetComponent<Movements>().TakeDamage(damage);
                // cool down
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

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }

                currentWaypoint = path[targetIndex];
            }

            // face towards next waypoint
            Vector3 targetDir = currentWaypoint - this.transform.position;
            float step = this.rotationSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
            transform.rotation = Quaternion.LookRotation(newDir);

            // move towards next waypoint
            transform.position = Vector3.MoveTowards(this.transform.position, 
                                                    currentWaypoint, 
                                                        this.speed * Time.deltaTime);
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                // draw brick on Gizmos mode for debugging
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);
                
                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }

    public void Initialize(Flock_zombie flock)
    {
        agentFlock = flock;
    }

    public void Move(Vector2 velocity)
    {
        // if can see the player, stop flocking
        if (!canSeePlayer)
        {
            Vector3 move = new Vector3();
            Debug.Log(velocity);
            //move.y = -10 * Time.deltaTime * 15;
            // convert Vector2 to Vector3
            move.x = velocity.x;
            move.z = velocity.y;
            transform.forward = move;
            transform.position += move * Time.deltaTime;
        }
    }
    
    private IEnumerator FOVRoutine()
    {
        // check for sights and path planing every 0.2 second
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            if (canSeePlayer)
            {
                // chasing the player
                Animator.SetBool("isFlocking", false);
                Animator.SetBool("isSeen", true);
                PathManager.RequestPath(transform.position, target.position, this, OnPathFound);
            }
            else
            {
                // if the player out of sights, stop chasing the player
                Animator.SetBool("isFlocking", true);
                Animator.SetBool("isSeen", false);
                // if the zombie got damage, then even if the zombie didn't see the player, still starting chasing the player
                if (health < 100f)
                {
                    PathManager.RequestPath(transform.position, playerRef.transform.position, this, OnPathFound);
                }
            }
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        // first, whether the player in the radius, targetMask is player layer
        // only player game object in player layer
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            target = rangeChecks[0].transform;
            // get the direction
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            // second, whether the direction in field of view
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                // get the distance
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                canSeePlayer = true;
                // if player hide behind obstacle, like rocks, zombie cannot see the player
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                }
                else
                {
                    canSeePlayer = false;
                }
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
        PathManager.RequestPath(transform.position, playerRef.transform.position, this, OnPathFound);
        canSeePlayer = true;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
