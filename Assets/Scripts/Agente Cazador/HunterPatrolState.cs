using UnityEngine;

public class HunterPatrolState : HunterStateBase
{
    [Header("Configuración de Patrulla")]
    public Transform[] waypoints;
    public float waypointThreshold = 0.5f;
    public bool pingPongMode = true; 

    private int currentIndex = 0;
    private int direction = 1;

    public override void EnterState() { }

    public override void UpdateState()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentIndex];
        hunter.MoveTowardsPoint(targetWaypoint.position);

        float dist = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                                      new Vector3(targetWaypoint.position.x, 0, targetWaypoint.position.z));

        if (dist <= waypointThreshold)
        {
            AdvanceWaypoint();
        }
    }

    private void AdvanceWaypoint()
    {
        if (waypoints.Length <= 1) return;

        if (pingPongMode)
        {
            currentIndex += direction;
            if (currentIndex >= waypoints.Length)
            {
                currentIndex = waypoints.Length - 2;
                direction = -1;
            }
            else if (currentIndex < 0)
            {
                currentIndex = 1;
                direction = 1;
            }
        }
        else
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
        }
    }

    public override void ExitState() { }
}