using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixedBahaviors_zombie
{
    public LayerMask mask1;
    public LayerMask mask2;

    public float steeringWeight = 2f;
    public float avoidingWeight = 2f;
    public float alignmentWeight = 3f;
    public float stayInWeight = 2.5f;

    Vector2 currentVelocity;

    public float agentSmoothTime = 100f;
    
    public Vector2 center = new Vector2(105.06f, 404.87f);
    
    public float radius = 15f;
    
    public MixedBahaviors_zombie(LayerMask mask1, LayerMask mask2)
    {
        this.mask1 = mask1;
        this.mask2 = mask2;
    }

    public Vector2 CalculateMove(Controller_Zombie agent, List<Transform> context, Flock_zombie flock)
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


    public Vector2 SteeringMove(Controller_Zombie agent, List<Transform> context)
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
        cohesionMove = Vector2.SmoothDamp(new Vector2(agent.transform.forward.x, agent.transform.forward.z),
            cohesionMove, ref currentVelocity, agentSmoothTime);
        return cohesionMove;
    }

    public Vector2 AvoidingMove(Controller_Zombie agent, List<Transform> context, Flock_zombie flock)
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
                avoidanceMove += new Vector2(agent.transform.position.x, agent.transform.position.z) -
                                 new Vector2(item.position.x, item.position.z);
            }
        }

        if (nAvoid > 0)
        {
            avoidanceMove /= nAvoid;
        }
        return avoidanceMove;
    }

    public Vector2 AlignmentMove(Controller_Zombie agent, List<Transform> context)
    {
        //if no neighbors, maintain current alignment
        if (context.Count == 0)
        {
            return new Vector2(agent.transform.forward.x, agent.transform.forward.z);
        }
        
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
    
    public Vector2 StayInMove(Controller_Zombie agent, List<Transform> context, Flock_zombie flock)
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

    public List<Transform> Filter(Controller_Zombie agent, List<Transform> original)
    {
        List<Transform> filtered = new List<Transform>();
        foreach (Transform item in original)
        {
            filtered.Add(item);
        }
        return filtered;
    }
}
