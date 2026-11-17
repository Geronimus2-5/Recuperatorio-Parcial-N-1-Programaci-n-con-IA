using UnityEngine;

public abstract class BoidStateBase : MonoBehaviour
{
    protected BoidTarget boid;
    protected BoidFlocking flocking;

    public virtual void Initialize(BoidTarget controller, BoidFlocking flockingModule)
    {
        this.boid = controller;
        this.flocking = flockingModule;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}