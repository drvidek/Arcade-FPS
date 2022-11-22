using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Cohesion")]
public class CohesionBehaviour : FilteredFlockBehaviour
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        if (context.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 cohesionMove = Vector3.zero;
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);

        int count = 0;
        foreach (Transform item in filteredContext)
        {
            //if (Vector3.SqrMagnitude(item.position - agent.transform.position) <= flock.)
            //{
            cohesionMove += (Vector3)item.position;
            count++;
            //}
        }
        if (count != 0)
        {
            cohesionMove /= count;
        }

        //direction from a to b = b - a
        cohesionMove -= (Vector3)agent.transform.position;

        return cohesionMove;
    }
}
