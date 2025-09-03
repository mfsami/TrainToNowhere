using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float health = 3f;

    private float currentHealth;

    private void Start()
    {
        currentHealth = health;
    }

    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Health:" + currentHealth);

        if(currentHealth < 0)
        {
            Die();
            Debug.Log("Dead");
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
