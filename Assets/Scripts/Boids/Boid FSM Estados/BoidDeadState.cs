using UnityEngine;

public class BoidDeadState : BoidStateBase
{
    public override void EnterState()
    {
        
        boid.velocity = Vector3.zero;
        boid.acceleration = Vector3.zero;
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState() { }
}