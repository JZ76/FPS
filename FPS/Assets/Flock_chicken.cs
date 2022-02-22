using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock_chicken : MonoBehaviour
{
    public GameObject agentPrefab;
    
    List<GameObject> agents = new List<GameObject>();
    
    public MixedBahaviors_chicken behavior;

    public LayerMask mask1;
    public LayerMask mask2;
    
    public int startingCount = 6;


    public float driveFactor = 10f;
    
    public float maxSpeed = 1f;
    
    public float neighborRadius = 3f;
    
    public float avoidanceRadiusMultiplier = 3f;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

    // Start is called before the first frame update
    void Start()
    {
        behavior = new MixedBahaviors_chicken(mask1, mask2);
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
            newAgent.name = "target_chicken_" + i;
            newAgent.GetComponent<ChickenController>().Initialize(this);
            agents.Add(newAgent);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject chicken in agents)
        {
            if (chicken != null)
            {
                ChickenController agent = chicken.GetComponent<ChickenController>();
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

    List<Transform> GetNearbyObjects(ChickenController agent)
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