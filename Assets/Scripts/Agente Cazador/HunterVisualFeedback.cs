using UnityEngine;

[RequireComponent(typeof(HunterController))]
public class HunterVisualFeedback : MonoBehaviour
{
    [Header("Colores por Estado")]
    public Color patrolColor = Color.blue;
    public Color huntColor = Color.red;
    public Color gatherColor = Color.green;
    public Color restColor = Color.gray;

    [Header("Renderer del Cazador")]
    public Renderer targetRenderer;

    private HunterController hunter;
    private Material instanceMaterial;

    private void Awake()
    {
        hunter = GetComponent<HunterController>();

        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer != null)
        {
            
            instanceMaterial = targetRenderer.material;
        }
    }

    private void OnEnable()
    {
        hunter.OnStateChanged += UpdateColor;
    }

    private void OnDisable()
    {
        hunter.OnStateChanged -= UpdateColor;
    }

    private void UpdateColor(HunterController.HunterStateType newState)
    {
        if (instanceMaterial == null) return;

        switch (newState)
        {
            case HunterController.HunterStateType.Patrol:
                instanceMaterial.color = patrolColor;
                break;
            case HunterController.HunterStateType.Hunt:
                instanceMaterial.color = huntColor;
                break;
            case HunterController.HunterStateType.Gather:
                instanceMaterial.color = gatherColor;
                break;
            case HunterController.HunterStateType.Rest:
                instanceMaterial.color = restColor;
                break;
        }
    }
}