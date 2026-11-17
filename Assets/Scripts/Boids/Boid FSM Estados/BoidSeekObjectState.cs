using UnityEngine;

public class BoidSeekObjectState : BoidStateBase
{
    [Header("Configuración de Arrive")]
    public float arriveSlowingRadius = 3f;

    public override void EnterState() { }

    public override void UpdateState()
    {
        if (boid.detectedPOI == null) return;

        Vector3 arrive = CalculateArrive(boid.detectedPOI.transform.position);
        Vector3 separation = flocking.CalculateSeparation() * flocking.separationWeight;
        Vector3 bounds = flocking.CalculateBoundsForce() * flocking.boundsWeight;

        boid.acceleration = arrive + separation + bounds;
        boid.ApplySteeringMovement(boid.maxSpeed);
    }

    private Vector3 CalculateArrive(Vector3 targetPos)
    {
        Vector3 toTarget = targetPos - transform.position;
        toTarget.y = 0f;
        float dist = toTarget.magnitude;

        if (dist <= boid.stopDistanceFromPOI)
        {
            return -boid.velocity;
        }

        float effectiveDist = dist - boid.stopDistanceFromPOI;
        float rampedSpeed = boid.maxSpeed * (effectiveDist / arriveSlowingRadius);
        float clippedSpeed = Mathf.Min(rampedSpeed, boid.maxSpeed);

        Vector3 desired = (toTarget / dist) * clippedSpeed;
        return Vector3.ClampMagnitude(desired - boid.velocity, boid.maxForce);
    }

    public override void ExitState() { }
}