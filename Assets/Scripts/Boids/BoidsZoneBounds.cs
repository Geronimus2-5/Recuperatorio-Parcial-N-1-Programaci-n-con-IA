using UnityEngine;

public class BoidZoneBounds : MonoBehaviour
{
    [Header("Dimensiones del Área de Movimiento")]
    public Vector2 zoneSize = new Vector2(30f, 30f); 
    public float borderMargin = 2.5f; 
    public float spawnHeight = 0.5f;

    
    public float MinX => transform.position.x - (zoneSize.x * 0.5f);
    public float MaxX => transform.position.x + (zoneSize.x * 0.5f);
    public float MinZ => transform.position.z - (zoneSize.y * 0.5f);
    public float MaxZ => transform.position.z + (zoneSize.y * 0.5f);

    
    public Vector3 GetRandomPositionInside(float extraMargin = 1.5f)
    {
        float x = Random.Range(MinX + extraMargin, MaxX - extraMargin);
        float z = Random.Range(MinZ + extraMargin, MaxZ - extraMargin);
        return new Vector3(x, spawnHeight, z);
    }

    
    public bool IsNearOrOutsideBounds(Vector3 position)
    {
        return position.x < MinX + borderMargin ||
               position.x > MaxX - borderMargin ||
               position.z < MinZ + borderMargin ||
               position.z > MaxZ - borderMargin;
    }

   
    public Vector3 CalculateReturnForce(Vector3 currentPos, Vector3 currentVelocity, float maxSpeed, float maxForce)
    {
        if (!IsNearOrOutsideBounds(currentPos))
            return Vector3.zero;

        Vector3 center = new Vector3(transform.position.x, currentPos.y, transform.position.z);
        Vector3 toCenter = (center - currentPos);
        toCenter.y = 0f;

        if (toCenter.sqrMagnitude < 0.001f)
            return Vector3.zero;

        Vector3 desired = toCenter.normalized * maxSpeed;
        Vector3 steer = desired - currentVelocity;
        return Vector3.ClampMagnitude(steer, maxForce * 1.5f);
    }

    private void OnDrawGizmos()
    {
       
        Gizmos.color = Color.green;
        Vector3 center = new Vector3(transform.position.x, spawnHeight, transform.position.z);
        Vector3 outerSize = new Vector3(zoneSize.x, 0.5f, zoneSize.y);
        Gizmos.DrawWireCube(center, outerSize);

        
        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.4f);
        Vector3 innerSize = new Vector3(
            Mathf.Max(1f, zoneSize.x - borderMargin * 2f),
            0.2f,
            Mathf.Max(1f, zoneSize.y - borderMargin * 2f)
        );
        Gizmos.DrawWireCube(center, innerSize);
    }
}