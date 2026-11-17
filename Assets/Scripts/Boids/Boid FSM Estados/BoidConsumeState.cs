using UnityEngine;

public class BoidConsumeState : BoidStateBase
{
    [Header("Configuración de Consumo")]
    public float consumeInterval = 1.0f;
    public int consumeDamage = 1;

    private float consumeTimer = 0f;

    public override void EnterState()
    {
        consumeTimer = 0f;
    }

    public override void UpdateState()
    {
        
        boid.velocity = Vector3.Lerp(boid.velocity, Vector3.zero, 10f * Time.deltaTime);

        if (boid.detectedPOI == null) return;

        FacePoint(boid.detectedPOI.transform.position);

        consumeTimer += Time.deltaTime;
        if (consumeTimer >= consumeInterval)
        {
            consumeTimer = 0f;
            boid.detectedPOI.TakeConsumeDamage(consumeDamage);
        }
    }

    private void FacePoint(Vector3 point)
    {
        Vector3 dir = point - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion rot = Quaternion.LookRotation(dir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, boid.rotationSpeed * Time.deltaTime);
        }
    }

    public override void ExitState()
    {
        consumeTimer = 0f;
    }
}