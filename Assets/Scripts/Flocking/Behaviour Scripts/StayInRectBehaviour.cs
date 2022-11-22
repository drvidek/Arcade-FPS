using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Stay in radius")]
public class StaynRectBehaviour : FlockBehaviour
{
    [SerializeField] private Vector3 _centre;
    [SerializeField] private float _radius = 15f;

    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        Vector3 _centreOffset = _centre - (Vector3)agent.transform.position;
        float t = _centreOffset.magnitude / _radius;

        if (t < 0.9f)
        {
            return Vector3.zero;
        }
        return _centreOffset * t * t;
    }
}