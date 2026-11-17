using UnityEngine;

public class HunterRestState : HunterStateBase
{
    public override void EnterState() { }

    public override void UpdateState()
    {
        hunter.currentEnergy += hunter.energyRegenRate * Time.deltaTime;
        hunter.currentEnergy = Mathf.Clamp(hunter.currentEnergy, 0f, hunter.maxEnergy);

        
    }

    public override void ExitState() { }
}