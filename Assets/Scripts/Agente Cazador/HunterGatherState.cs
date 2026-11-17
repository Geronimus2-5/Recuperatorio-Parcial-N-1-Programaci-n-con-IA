using UnityEngine;

public class HunterGatherState : HunterStateBase
{
    [Header("Configuración de Recolección")]
    public float gatherDistance = 1.2f;
    public float gatherDuration = 2.5f;

    private float currentGatherTimer = 0f;
    public bool IsGatherComplete => currentGatherTimer >= gatherDuration;

    public override void EnterState()
    {
        currentGatherTimer = 0f;
    }

    public override void UpdateState()
    {
        if (hunter.currentTarget == null) return;

        float dist = Vector3.Distance(transform.position, hunter.currentTarget.transform.position);

        if (dist > gatherDistance)
        {
            
            hunter.MoveTowardsPoint(hunter.currentTarget.transform.position);
        }
        else
        {
            
            currentGatherTimer += Time.deltaTime;
        }
    }

    public override void ExitState()
    {
        currentGatherTimer = 0f;
    }
}