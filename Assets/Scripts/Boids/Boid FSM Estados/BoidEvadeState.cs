using UnityEngine;

public class BoidEvadeState : BoidStateBase
{
    public override void EnterState() { }

    public override void UpdateState()
    {
        if (boid.detectedHunter == null) return;

        Vector3 evadeForce = CalculateEvade(boid.detectedHunter) * flocking.evadeWeight;
        Vector3 separation = flocking.CalculateSeparation() * (flocking.separationWeight * 1.5f);
        Vector3 bounds = flocking.CalculateBoundsForce() * flocking.boundsWeight;

        boid.acceleration = evadeForce + separation + bounds;
        boid.ApplySteeringMovement(boid.evadeSpeed);
    }

    private Vector3 CalculateEvade(Transform pursuer)
    {
        Vector3 toPursuer = pursuer.position - transform.position;
        toPursuer.y = 0f;
        float distance = toPursuer.magnitude;

        HunterController hunterCtrl = pursuer.GetComponent<HunterController>();
        Vector3 pursuerVel = pursuer.forward * (hunterCtrl != null ? hunterCtrl.moveSpeed : 5f);

        float predictionTime = distance / boid.evadeSpeed;
        Vector3 futurePosition = pursuer.position + pursuerVel * predictionTime;

        Vector3 desired = transform.position - futurePosition;
        desired.y = 0f;
        if (desired.sqrMagnitude < 0.001f) desired = -pursuer.forward;

        desired = desired.normalized * boid.evadeSpeed;
        return Vector3.ClampMagnitude(desired - boid.velocity, boid.maxForce);
    }

    public override void ExitState() { }
}