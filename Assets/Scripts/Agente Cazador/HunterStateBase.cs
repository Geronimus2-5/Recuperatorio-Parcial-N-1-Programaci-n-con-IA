using UnityEngine;

public abstract class HunterStateBase : MonoBehaviour
{
    protected HunterController hunter;

    public virtual void Initialize(HunterController controller)
    {
        this.hunter = controller;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}