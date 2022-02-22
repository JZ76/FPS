using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixedBahaviors_chicken
{
    public LayerMask mask1;
    public LayerMask mask2;
    
    public float steeringWeight = 1;
    public float avoidingWeight = 2;
    public float alignmentWeight = 2;
    public float stayInWeight = 2;
    
    Vector2 currentVelocity;
    public float agentSmoothTime = 100f;

    public MixedBahaviors_chicken(LayerMask mask1, LayerMask mask2)
    {
        this.mask1 = mask1;
        this.mask2 = mask2;
    }
    public Vector2 CalculateMove(ChickenController agent, List<Transform> context, Flock_chicken flock)
    {
        //set up move
        Vector2 move = Vector2.zero;

        //iterate through behaviors
        Vector2 partialSteeringMove = SteeringMove(agent, context) * steeringWeight;
        Vector2 partialAvoidingMove = AvoidingMove(agent, context, flock) * avoidingWeight;
        Vector2 partialAlignmentMove = AlignmentMove(agent, context) * alignmentWeight;
        Vector2 partialStayInMove = StayInMove(agent, context, flock) * stayInWeight;
        
        if (partialSteeringMove != Vector2.zero)
        {
            if (partialSteeringMove.sqrMagnitude > steeringWeight * steeringWeight)
            {
                partialSteeringMove.Normalize();
                partialSteeringMove *= steeringWeight;
            }
            move += partialSteeringMove;
        }
        if (partialAvoidingMove != Vector2.zero)
        {
            if (partialAvoidingMove.sqrMagnitude > avoidingWeight * avoidingWeight)
            {
                partialAvoidingMove.Normalize();
                partialAvoidingMove *= avoidingWeight;
            }
            move += partialAvoidingMove;
        }
        if (partialAlignmentMove != Vector2.zero)
        {
            if (partialAlignmentMove.sqrMagnitude > alignmentWeight * alignmentWeight)
            {
                Debug.Log("get in");
                partialAlignmentMove.Normalize();
                partialAlignmentMove *= alignmentWeight;
            }
            move += partialAlignmentMove;
        }
        if (partialStayInMove != Vector2.zero)
        {
            if (partialStayInMove.sqrMagnitude > stayInWeight * stayInWeight)
            {
                partialStayInMove.Normalize();
                partialStayInMove *= stayInWeight;
            }
            move += partialStayInMove;
        }
        return move;
    }
    
    
    public Vector2 SteeringMove(ChickenController agent, List<Transform> context)
    {
        //if no neighbors, return no adjustment
        if (context.Count == 0)
            return Vector2.zero;

        //add all points together and average
        Vector2 cohesionMove = Vector2.zero;
        context = Filter(agent, context);
        foreach (Transform item in context)
        {
            cohesionMove += new Vector2(item.position.x, item.position.z);
        }
        cohesionMove /= context.Count;

        //create offset from agent position
        cohesionMove -= new Vector2(agent.transform.position.x, agent.transform.position.z);
        cohesionMove = Vector2.SmoothDamp(new Vector2(agent.transform.forward.x, agent.transform.forward.z), cohesionMove, ref currentVelocity, agentSmoothTime);
        return cohesionMove;
    }
    
    public Vector2 AvoidingMove(ChickenController agent, List<Transform> context, Flock_chicken flock)
    {
        //if no neighbors, return no adjustment
        if (context.Count == 0)
            return Vector2.zero;

        //add all points together and average
        Vector2 avoidanceMove = Vector2.zero;
        context = Filter(agent, context);
        int nAvoid = 0;
        foreach (Transform item in context)
        {
            if (Vector2.SqrMagnitude(item.position - agent.transform.position) < flock.SquareAvoidanceRadius)
            {
                nAvoid++;
                avoidanceMove += new Vector2(agent.transform.position.x, agent.transform.position.z) - new Vector2(item.position.x, item.position.z);
            }
        }
        if (nAvoid > 0)
            avoidanceMove /= nAvoid;

        return avoidanceMove;
    }
    
    public Vector2 AlignmentMove(ChickenController agent, List<Transform> context)
    {
        //if no neighbors, maintain current alignment
        if (context.Count == 0)
            return new Vector2(agent.transform.forward.x, agent.transform.forward.z);
        //add all points together and average
        Vector2 alignmentMove = Vector2.zero;
        context = Filter(agent, context);
        foreach (Transform item in context)
        {
            alignmentMove += new Vector2(item.transform.forward.x, item.transform.forward.z);
        }
        alignmentMove /= context.Count;

        return alignmentMove;
    }
    
    public Vector2 center = new Vector2(133.8f,431.02f);
    public float radius = 10f;

    public Vector2 StayInMove(ChickenController agent, List<Transform> context, Flock_chicken flock)
    {
        Vector2 agentCenter = new Vector2(agent.transform.position.x, agent.transform.position.z);
        Vector2 centerOffset = center - agentCenter;
        float t = centerOffset.magnitude / radius;
        if (t < 0.9f)
        {
            return Vector2.zero;
        }
        return centerOffset * t * t;
    }
    public List<Transform> Filter(ChickenController agent, List<Transform> original)
    {
        List<Transform> filtered = new List<Transform>();
        foreach (Transform item in original)
        {
            if (mask1 != item.gameObject.layer && mask2 != item.gameObject.layer)
            {
                filtered.Add(item);
            }
        }
        return filtered;
    }
}
