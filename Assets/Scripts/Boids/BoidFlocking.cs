using UnityEngine;

[RequireComponent(typeof(BoidTarget))]
public class BoidFlocking : MonoBehaviour
{
    [Header("Pesos Configurables")]
    public float separationWeight = 2.2f;
    public float alignmentWeight = 1.2f;
    public float cohesionWeight = 1.0f;
    public float wanderWeight = 0.6f;
    public float evadeWeight = 3.0f;
    public float boundsWeight = 4.0f;

    [Header("Control de Zona")]
    

    private BoidTarget boid;
    private float wanderAngle = 0f;

    private void Awake()
    {
        boid = GetComponent<BoidTarget>();
        wanderAngle = Random.Range(0f, 360f);
    }

   
    public Vector3 CalculateSeparation()
    {
        Vector3 steer = Vector3.zero;
        int count = 0;

     
        Collider[] neighbors = Physics.OverlapSphere(transform.position, boid.separationRadius, boid.boidLayer);
        foreach (Collider col in neighbors)
        {
            if (col.gameObject == gameObject) continue;
            BoidTarget other = col.GetComponent<BoidTarget>();
            if (other != null && other.currentStateType == BoidTarget.BoidStateType.Respawn) continue;

            Vector3 diff = transform.position - col.transform.position;
            diff.y = 0f;
            float dist = diff.magnitude;
            if (dist > 0.001f)
            {
                steer += diff.normalized / dist;
                count++;
            }
        }

        
        Collider[] pois = Physics.OverlapSphere(transform.position, boid.separationRadius, boid.poiLayer);
        foreach (Collider col in pois)
        {
            Vector3 diff = transform.position - col.transform.position;
            diff.y = 0f;
            float dist = diff.magnitude;
            if (dist > 0.001f)
            {
                steer += diff.normalized / dist;
                count++;
            }
        }

        if (count > 0)
        {
            steer /= count;
            steer = steer.normalized * boid.maxSpeed - boid.velocity;
            steer = Vector3.ClampMagnitude(steer, boid.maxForce);
        }

        return steer;
    }

  
    public Vector3 CalculateAlignment()
    {
        Vector3 avgVelocity = Vector3.zero;
        int count = 0;

        Collider[] neighbors = Physics.OverlapSphere(transform.position, boid.alignmentCohesionRadius, boid.boidLayer);
        foreach (Collider col in neighbors)
        {
            if (col.gameObject == gameObject) continue;
            BoidTarget other = col.GetComponent<BoidTarget>();
            if (other != null && !other.isDead)
            {
                avgVelocity += other.velocity;
                count++;
            }
        }

        if (count > 0)
        {
            avgVelocity /= count;
            if (avgVelocity.sqrMagnitude > 0.001f)
            {
                Vector3 desired = avgVelocity.normalized * boid.maxSpeed;
                return Vector3.ClampMagnitude(desired - boid.velocity, boid.maxForce);
            }
        }
        return Vector3.zero;
    }


    public Vector3 CalculateCohesion()
    {
        Vector3 centerOfMass = Vector3.zero;
        int count = 0;

        Collider[] neighbors = Physics.OverlapSphere(transform.position, boid.alignmentCohesionRadius, boid.boidLayer);
        foreach (Collider col in neighbors)
        {
            if (col.gameObject == gameObject) continue;
            BoidTarget other = col.GetComponent<BoidTarget>();
            if (other != null && !other.isDead)
            {
                centerOfMass += other.transform.position;
                count++;
            }
        }

        if (count > 0)
        {
            centerOfMass /= count;
            Vector3 desired = centerOfMass - transform.position;
            desired.y = 0f;
            if (desired.sqrMagnitude > 0.001f)
            {
                desired = desired.normalized * boid.maxSpeed;
                return Vector3.ClampMagnitude(desired - boid.velocity, boid.maxForce);
            }
        }
        return Vector3.zero;
    }

  
    public Vector3 CalculateWander()
    {
        float circleDistance = 2.0f;
        float circleRadius = 1.2f;
        float angleChange = 60f;

        wanderAngle += Random.Range(-angleChange, angleChange) * Time.deltaTime;

        Vector3 forwardDir = boid.velocity.sqrMagnitude > 0.01f ? boid.velocity.normalized : transform.forward;
        Vector3 circleCenter = forwardDir * circleDistance;
        Vector3 displacement = new Vector3(Mathf.Cos(wanderAngle), 0f, Mathf.Sin(wanderAngle)) * circleRadius;

        Vector3 desired = (circleCenter + displacement).normalized * boid.maxSpeed;
        return Vector3.ClampMagnitude(desired - boid.velocity, boid.maxForce);
    }

  
    public Vector3 CalculateBoundsForce()
    {
        if (boid.zoneBounds == null) return Vector3.zero;

        return boid.zoneBounds.CalculateReturnForce(
            transform.position,
            boid.velocity,
            boid.maxSpeed,
            boid.maxForce
        );
    }
}