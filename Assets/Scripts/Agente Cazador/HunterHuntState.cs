using UnityEngine;

public class HunterHuntState : HunterStateBase
{
    [Header("Configuración de Persecución")]
    public float predictionTime = 0.5f;

    public override void EnterState() { }

    public override void UpdateState()
    {
        
        hunter.currentEnergy -= hunter.energyDrainRate * Time.deltaTime;

        if (hunter.currentTarget == null) return;

        float distToTarget = Vector3.Distance(transform.position, hunter.currentTarget.transform.position);

        
        if (distToTarget <= hunter.meleeAttackRadius)
        {
            
            FaceTarget(hunter.currentTarget.transform.position);
            TryAttack(2, "Cuerpo a Cuerpo (Melee)");
        }
        else if (distToTarget <= hunter.rangeAttackRadius)
        {
            
            FaceTarget(hunter.currentTarget.transform.position);
            TryAttack(1, "A Distancia (Ranged)");
        }
        else
        {
            
            Vector3 predictedPos = hunter.currentTarget.transform.position + (hunter.currentTarget.velocity * predictionTime);
            hunter.MoveTowardsPoint(predictedPos);
        }
    }

    private void TryAttack(int damageAmount, string attackType)
    {
        
        if (hunter.IsAttackReady)
        {
            hunter.currentTarget.TakeDamage(damageAmount);
            hunter.ResetTBA(); 
            Debug.Log($"[Cazador] Ejecutó ataque {attackType} por {damageAmount} de daño.");
        }
    }

    private void FaceTarget(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, hunter.rotationSpeed * Time.deltaTime);
        }
    }

    public override void ExitState() { }
}