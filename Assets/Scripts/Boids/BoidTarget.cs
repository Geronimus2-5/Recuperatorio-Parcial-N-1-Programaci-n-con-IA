using UnityEngine;
using System;

[RequireComponent(typeof(BoidFlocking))]
public class BoidTarget : MonoBehaviour
{
    public enum BoidStateType { Flocking, Evade, SeekObject, Consume, Dead, Respawn }

    [Header("Estado Actual")]
    public BoidStateType currentStateType = BoidStateType.Flocking;
    private BoidStateBase currentState;

    [Header("Componentes de Estado (FSM)")]
    public BoidFlockingState flockingState;
    public BoidEvadeState evadeState;
    public BoidSeekObjectState seekObjectState;
    public BoidConsumeState consumeState;
    public BoidDeadState deadState;
    public BoidRespawnState respawnState;

    [Header("Vida")]
    public int maxHealth = 2;
    public int currentHealth = 2;
    public bool isDead => currentStateType == BoidStateType.Dead || currentStateType == BoidStateType.Respawn;

    [Header("Movimiento y Física")]
    public float maxSpeed = 6f;
    public float evadeSpeed = 8.5f;
    public float maxForce = 12f;
    public float rotationSpeed = 10f;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public Vector3 acceleration;

    [Header("Sensores Locales")]
    public float visionRadius = 8f;
    public float separationRadius = 1.8f;
    public float alignmentCohesionRadius = 5f;
    public LayerMask boidLayer;
    public LayerMask hunterLayer;
    public LayerMask poiLayer;

    [Header("Distancias de Interacción y Zona")]
    public float stopDistanceFromPOI = 1.3f;
    public BoidZoneBounds zoneBounds;

    
    [HideInInspector] public Transform detectedHunter;
    [HideInInspector] public PointOfInterest detectedPOI;

    private BoidFlocking flockingModule;
    private Collider boidCollider;
    private Renderer[] renderers;

    public event Action<BoidStateType> OnStateChanged;

    private void Awake()
    {
        flockingModule = GetComponent<BoidFlocking>();
        boidCollider = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();

        if (zoneBounds == null)
            zoneBounds = FindFirstObjectByType<BoidZoneBounds>();

        if (flockingState == null) flockingState = GetComponent<BoidFlockingState>();
        if (evadeState == null) evadeState = GetComponent<BoidEvadeState>();
        if (seekObjectState == null) seekObjectState = GetComponent<BoidSeekObjectState>();
        if (consumeState == null) consumeState = GetComponent<BoidConsumeState>();
        if (deadState == null) deadState = GetComponent<BoidDeadState>();
        if (respawnState == null) respawnState = GetComponent<BoidRespawnState>();

        flockingState.Initialize(this, flockingModule);
        evadeState.Initialize(this, flockingModule);
        seekObjectState.Initialize(this, flockingModule);
        consumeState.Initialize(this, flockingModule);
        deadState.Initialize(this, flockingModule);
        respawnState.Initialize(this, flockingModule);
    }

    private void Start()
    {
        InitializeBoid();
    }

    public void InitializeBoid()
    {
        currentHealth = maxHealth;
        SetVisualsActive(true);
        if (boidCollider != null) boidCollider.enabled = true;

        Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;
        velocity = new Vector3(randomDir.x, 0f, randomDir.y) * (maxSpeed * 0.6f);

        ChangeState(BoidStateType.Flocking);
    }

    private void Update()
    {
        
        if (!isDead)
        {
            ScanEnvironment();
        }

        
        EvaluateTransitions();

        
        if (currentState != null)
        {
            currentState.UpdateState();
        }
    }

    private void ScanEnvironment()
    {
        
        detectedHunter = null;
        Collider[] hunterHits = Physics.OverlapSphere(transform.position, visionRadius, hunterLayer);
        if (hunterHits.Length > 0)
        {
            detectedHunter = hunterHits[0].transform;
        }

        
        detectedPOI = null;
        Collider[] poiHits = Physics.OverlapSphere(transform.position, visionRadius, poiLayer);
        float closestDist = Mathf.Infinity;

        foreach (Collider hit in poiHits)
        {
            PointOfInterest poi = hit.GetComponent<PointOfInterest>();
            if (poi != null && poi.gameObject.activeInHierarchy && !poi.IsDestroyed)
            {
                float d = Vector3.Distance(transform.position, poi.transform.position);
                if (d < closestDist)
                {
                    closestDist = d;
                    detectedPOI = poi;
                }
            }
        }
    }

    private void EvaluateTransitions()
    {
        
        if (currentStateType == BoidStateType.Respawn)
        {
            if (respawnState.IsRespawnComplete)
            {
                if (zoneBounds != null)
                    transform.position = zoneBounds.GetRandomPositionInside();

                InitializeBoid();
            }
            return;
        }

        
        if (currentHealth <= 0)
        {
            if (currentStateType != BoidStateType.Dead)
                ChangeState(BoidStateType.Dead);
            return;
        }

       
        if (detectedHunter != null)
        {
            if (currentStateType != BoidStateType.Evade)
                ChangeState(BoidStateType.Evade);
            return;
        }

        
        if (detectedPOI != null)
        {
            float distToPOI = Vector3.Distance(transform.position, detectedPOI.transform.position);
            if (distToPOI <= stopDistanceFromPOI + 0.2f)
            {
                if (currentStateType != BoidStateType.Consume)
                    ChangeState(BoidStateType.Consume);
            }
            else
            {
                if (currentStateType != BoidStateType.SeekObject)
                    ChangeState(BoidStateType.SeekObject);
            }
            return;
        }

       
        if (currentStateType != BoidStateType.Flocking)
        {
            ChangeState(BoidStateType.Flocking);
        }
    }

    public void ChangeState(BoidStateType newStateType)
    {
        if (currentState != null)
        {
            currentState.ExitState();
        }

        currentStateType = newStateType;

        switch (newStateType)
        {
            case BoidStateType.Flocking: currentState = flockingState; break;
            case BoidStateType.Evade: currentState = evadeState; break;
            case BoidStateType.SeekObject: currentState = seekObjectState; break;
            case BoidStateType.Consume: currentState = consumeState; break;
            case BoidStateType.Dead: currentState = deadState; break;
            case BoidStateType.Respawn: currentState = respawnState; break;
        }

        currentState.EnterState();
        OnStateChanged?.Invoke(currentStateType);
    }

    public void ApplySteeringMovement(float speedLimit)
    {
        acceleration = Vector3.ClampMagnitude(acceleration, maxForce);
        velocity += acceleration * Time.deltaTime;
        velocity.y = 0f;
        velocity = Vector3.ClampMagnitude(velocity, speedLimit);

        
        float minSpeed = speedLimit * 0.35f;
        if (velocity.sqrMagnitude > 0.001f && velocity.magnitude < minSpeed)
        {
            velocity = velocity.normalized * minSpeed;
        }

        if (velocity.sqrMagnitude > 0.01f)
        {
            transform.position += velocity * Time.deltaTime;
            Quaternion targetRot = Quaternion.LookRotation(velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        acceleration = Vector3.zero;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            ChangeState(BoidStateType.Dead);
        }
    }

    
    public void FinishGathering()
    {
        if (currentStateType == BoidStateType.Dead)
        {
            ChangeState(BoidStateType.Respawn);
        }
    }

    public void SetVisualsActive(bool active)
    {
        if (renderers != null)
        {
            foreach (Renderer r in renderers) r.enabled = active;
        }
        if (boidCollider != null)
        {
            boidCollider.enabled = active;
        }
    }

    private void OnValidate()
    {
        if (separationRadius >= alignmentCohesionRadius)
        {
            separationRadius = Mathf.Max(0.5f, alignmentCohesionRadius - 0.5f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, alignmentCohesionRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}