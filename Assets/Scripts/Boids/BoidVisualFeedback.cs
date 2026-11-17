using UnityEngine;

[RequireComponent(typeof(BoidTarget))]
public class BoidVisualFeedback : MonoBehaviour
{
    [Header("Colores por Estado del Boid")]
    public Color flockingColor = Color.cyan;
    public Color evadeColor = new Color(1f, 0.35f, 0f);
    public Color objectInteractColor = Color.yellow;
    public Color deadColor = Color.black;

    [Header("Renderer del Boid")]
    public Renderer targetRenderer;

    private BoidTarget boid;
    private Material instanceMaterial;

    private void Awake()
    {
        boid = GetComponent<BoidTarget>();

        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer != null)
            instanceMaterial = targetRenderer.material;
    }

    private void OnEnable()
    {
        boid.OnStateChanged += UpdateColor;
        UpdateColor(boid.currentStateType);
    }

    private void OnDisable()
    {
        boid.OnStateChanged -= UpdateColor;
    }

    private void UpdateColor(BoidTarget.BoidStateType state)
    {
        if (instanceMaterial == null) return;

        switch (state)
        {
            case BoidTarget.BoidStateType.Flocking:
                instanceMaterial.color = flockingColor;
                break;
            case BoidTarget.BoidStateType.Evade:
                instanceMaterial.color = evadeColor;
                break;
            case BoidTarget.BoidStateType.SeekObject:
            case BoidTarget.BoidStateType.Consume:
                instanceMaterial.color = objectInteractColor;
                break;
            case BoidTarget.BoidStateType.Dead:
                instanceMaterial.color = deadColor;
                break;
        }
    }
}