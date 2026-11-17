using UnityEngine;
using System;

public class HunterController : MonoBehaviour
{
    public enum HunterStateType { Patrol, Hunt, Gather, Rest }

    [Header("Estado Actual")]
    public HunterStateType currentStateType;
    private HunterStateBase currentState;

    [Header("Componentes de Comportamiento")]
    public HunterPatrolState patrolState;
    public HunterHuntState huntState;
    public HunterGatherState gatherState;
    public HunterRestState restState;

    [Header("Parámetros Generales del Cazador")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float visionRadius = 12f;

    [Header("Combate y Rangos")]
    public float TBA = 2.0f; 
    [HideInInspector] public float tbaTimer = 0f;
    public float rangeAttackRadius = 8f;  
    public float meleeAttackRadius = 2.5f; 

    [Header("Energía")]
    public float maxEnergy = 100f;
    public float currentEnergy = 100f;
    public float energyDrainRate = 12f; 
    public float energyRegenRate = 20f; 

    [Header("Referencias Dinámicas")]
    public BoidTarget currentTarget;
    public LayerMask boidLayer;

    
    public event Action<HunterStateType> OnStateChanged;

    public bool IsAttackReady => tbaTimer >= TBA;

    private void Awake()
    {
        
        if (patrolState == null) patrolState = GetComponent<HunterPatrolState>();
        if (huntState == null) huntState = GetComponent<HunterHuntState>();
        if (gatherState == null) gatherState = GetComponent<HunterGatherState>();
        if (restState == null) restState = GetComponent<HunterRestState>();

        patrolState.Initialize(this);
        huntState.Initialize(this);
        gatherState.Initialize(this);
        restState.Initialize(this);
    }

    private void Start()
    {
        currentEnergy = maxEnergy;
        tbaTimer = TBA; 
        ChangeState(HunterStateType.Patrol);
    }

    private void Update()
    {
        
        if (tbaTimer < TBA)
        {
            tbaTimer += Time.deltaTime;
        }

        
        EvaluateTransitions();

        
        if (currentState != null)
        {
            currentState.UpdateState();
        }
    }

    private void EvaluateTransitions()
    {
        switch (currentStateType)
        {
            case HunterStateType.Patrol:
                
                BoidTarget deadBoid = FindNearestBoid(true);
                if (deadBoid != null)
                {
                    currentTarget = deadBoid;
                    ChangeState(HunterStateType.Gather);
                    return;
                }

                
                if (IsAttackReady)
                {
                    BoidTarget aliveBoid = FindNearestBoid(false);
                    if (aliveBoid != null)
                    {
                        currentTarget = aliveBoid;
                        ChangeState(HunterStateType.Hunt);
                        return;
                    }
                }
                break;

            case HunterStateType.Hunt:
                
                if (currentEnergy <= 0f)
                {
                    currentEnergy = 0f;
                    currentTarget = null;
                    ChangeState(HunterStateType.Rest);
                    return;
                }

                
                if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy ||
                    Vector3.Distance(transform.position, currentTarget.transform.position) > visionRadius)
                {
                    currentTarget = null;
                    ChangeState(HunterStateType.Patrol);
                    return;
                }

                
                if (currentTarget.isDead)
                {
                    ChangeState(HunterStateType.Gather);
                    return;
                }
                break;

            case HunterStateType.Gather:
                
                if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy || !currentTarget.isDead)
                {
                    currentTarget = null;
                    ChangeState(HunterStateType.Patrol);
                    return;
                }

                
                if (gatherState.IsGatherComplete)
                {
                    currentTarget.FinishGathering();
                    currentTarget = null;
                    ChangeState(HunterStateType.Patrol);
                    return;
                }
                break;

            case HunterStateType.Rest:
                if (currentEnergy >= maxEnergy)
                {
                    currentEnergy = maxEnergy;
                    ChangeState(HunterStateType.Patrol);
                    return;
                }
                break;
        }
    }

    public void ChangeState(HunterStateType newStateType)
    {
        if (currentState != null)
        {
            currentState.ExitState();
        }

        currentStateType = newStateType;

        switch (newStateType)
        {
            case HunterStateType.Patrol: currentState = patrolState; break;
            case HunterStateType.Hunt: currentState = huntState; break;
            case HunterStateType.Gather: currentState = gatherState; break;
            case HunterStateType.Rest: currentState = restState; break;
        }

        currentState.EnterState();
        OnStateChanged?.Invoke(currentStateType);
    }

    public void ResetTBA()
    {
        tbaTimer = 0f;
    }

    
    public BoidTarget FindNearestBoid(bool searchDead)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, visionRadius, boidLayer);
        BoidTarget nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            
            BoidTarget boid = hit.GetComponentInParent<BoidTarget>();
            if (boid != null && boid.gameObject.activeInHierarchy && boid.isDead == searchDead)
            {
                float dist = Vector3.Distance(transform.position, boid.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearest = boid;
                }
            }
        }
        return nearest;
    }

    

    public void MoveTowardsPoint(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position);
        direction.y = 0f; 

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    private void OnValidate()
    {
        
        if (meleeAttackRadius >= rangeAttackRadius)
        {
            meleeAttackRadius = Mathf.Max(0.5f, rangeAttackRadius - 0.5f);
        }
    }

   
    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rangeAttackRadius);

        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRadius);
    }
}