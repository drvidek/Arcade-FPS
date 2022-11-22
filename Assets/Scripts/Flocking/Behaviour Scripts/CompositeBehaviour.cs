using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Composite")]
public class CompositeBehaviour : FlockBehaviour
{
    [System.Serializable]
    public struct BehaviourGroup
    {
        public FlockBehaviour behaviour;
        public float weights;
    }

    public BehaviourGroup[] behaviours;
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        Vector3 move = Vector3.zero;

        foreach (BehaviourGroup behave in behaviours)
        {
            Vector3 partialMove = behave.behaviour.CalculateMove(agent, context, flock) * behave.weights;

            if (partialMove != Vector3.zero)
            {
                if (partialMove.sqrMagnitude > behave.weights * behave.weights)
                {
                    partialMove.Normalize();
                    partialMove *= behave.weights;
                }
            }

            move += partialMove;
        }

        return move;
    }
}