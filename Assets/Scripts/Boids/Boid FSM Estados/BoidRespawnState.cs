using UnityEngine;

public class BoidRespawnState : BoidStateBase
{
    [Header("Configuración de Respawn")]
    public float respawnDelay = 4f;

    private float timer = 0f;
    public bool IsRespawnComplete => timer >= respawnDelay;

    public override void EnterState()
    {
        timer = 0f;
        boid.velocity = Vector3.zero;
        boid.acceleration = Vector3.zero;
        
        boid.SetVisualsActive(false);
    }

    public override void UpdateState()
    {
        timer += Time.deltaTime;
    }

    public override void ExitState()
    {
        timer = 0f;
    }
}