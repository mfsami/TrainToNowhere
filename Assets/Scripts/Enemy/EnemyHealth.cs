using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float health = 1f;

    private float currentHealth;

    private void Start()
    {
        currentHealth = health;
    }

    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Health:" + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
