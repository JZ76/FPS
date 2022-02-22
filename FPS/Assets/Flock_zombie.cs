using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock_zombie : MonoBehaviour
{
    public GameObject agentPrefab;

    List<GameObject> agents = new List<GameObject>();

    public MixedBahaviors_zombie behavior;

    public LayerMask mask1;
    public LayerMask mask2;

    public int startingCount = 20;


    public float driveFactor = 10f;

    public float maxSpeed = 2f;

    public float neighborRadius = 3f;

    public float avoidanceRadiusMultiplier = 0.5f;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;

    public float SquareAvoidanceRadius
    {
        get { return squareAvoidanceRadius; }
    }

    // Start is called before the first frame update
    void Start()
    {
        behavior = new MixedBahaviors_zombie(mask1, mask2);
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        for (int i = 0; i < startingCount; i++)
        {
            GameObject newAgent = Instantiate(
                agentPrefab,
                transform.position * Random.Range(0.99f, 1.01f),
                transform.rotation
            );
            newAgent.name = "target_zombie_" + i;
            newAgent.GetComponent<Controller_Zombie>().Initialize(this);
            agents.Add(newAgent);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject zombie in agents)
        {
            if (zombie != null)
            {
                Controller_Zombie agent = zombie.GetComponent<Controller_Zombie>();
                if (agent != null)
                {
                    List<Transform> context = GetNearbyObjects(agent);
                    Vector2 move = behavior.CalculateMove(agent, context, this);
                    move *= driveFactor;
                    if (move.sqrMagnitude > squareMaxSpeed)
                    {
                        move = move.normalized * maxSpeed;
                    }
                    agent.Move(move);
                }
            }
        }
    }

    List<Transform> GetNearbyObjects(Controller_Zombie agent)
    {
        List<Transform> context = new List<Transform>();
        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighborRadius);
        foreach (Collider c in contextColliders)
        {
            if (c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }
        return context;
    }
}