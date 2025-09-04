using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float damageAmount = 1f;

    private void OnTriggerEnter(Collider other)
    {
        // Damage anything that implements damage hook, then destroy.
        var dmg = other.GetComponent<IDamageable>();
        if (dmg != null)
        {
            dmg.Damage(damageAmount);
            Destroy(gameObject);
        }
    }
}
