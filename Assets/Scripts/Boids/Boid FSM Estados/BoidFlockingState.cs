using UnityEngine;

public class BoidFlockingState : BoidStateBase
{
    public override void EnterState() { }

    public override void UpdateState()
    {
        Vector3 separation = flocking.CalculateSeparation() * flocking.separationWeight;
        Vector3 alignment = flocking.CalculateAlignment() * flocking.alignmentWeight;
        Vector3 cohesion = flocking.CalculateCohesion() * flocking.cohesionWeight;
        Vector3 wander = flocking.CalculateWander() * flocking.wanderWeight;
        Vector3 bounds = flocking.CalculateBoundsForce() * flocking.boundsWeight;

        boid.acceleration = separation + alignment + cohesion + wander + bounds;
        boid.ApplySteeringMovement(boid.maxSpeed);
    }

    public override void ExitState() { }
}