using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth = 3;
    public bool IsDestroyed => currentHealth <= 0;

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public void TakeConsumeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Destroy(gameObject);
        }
    }
}