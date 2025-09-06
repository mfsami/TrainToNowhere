// Bullet.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float damageAmount = 1f;
    public bool parried;                 // set by SwordParry on PERFECT
    private bool processed;

    private void OnTriggerEnter(Collider other)
    {
        if (processed) return;

        int playerLayer = LayerMask.NameToLayer("Player");
        int parryLayer = LayerMask.NameToLayer("Parry");
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        // Touching the parry trigger Let the sword handle it.
        if (other.gameObject.layer == parryLayer) return;

        // Player hit  only if not parried
        if (other.gameObject.layer == playerLayer)
        {
            if (parried) return; // reflected bullets never hurt the player

            processed = true;
            var ph = other.GetComponentInParent<PlayerHealth>(); // player script is on parent
            if (ph != null) ph.Damage(damageAmount);
            Destroy(gameObject);
        }

        if(other.gameObject.layer == enemyLayer)
        {
            processed = true;
            var ph = other.GetComponent<EnemyHealth>(); 
            if (ph != null) ph.Damage(damageAmount);
            Destroy(gameObject);
        }
    }
}
