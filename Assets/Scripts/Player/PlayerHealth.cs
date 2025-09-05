using TMPro;
using UnityEngine;


public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float health = 5f;

    private float currentHealth;
    public TextMeshProUGUI healthText;

    private void Start()
    {
        currentHealth = health;
        healthText.text = $"Health: {currentHealth}";
    }

    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount;
        healthText.text = $"Health: {currentHealth}";

        if (currentHealth <= 0)
        {
            Die();
            healthText.text = "DEAD";
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
