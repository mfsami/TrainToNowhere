using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float damageAmount = 1f;
    private IDamageable iDamageable;

    private void OnTriggerEnter(Collider collision)
    {
        // This now works on both player and enemy health since both of those scripts use the IDamagable interface we created
        // This is because since we are implementing parrying, the bullet should be able to damage both the player + enemy
        
        iDamageable = collision.gameObject.GetComponent<IDamageable>();
        if(iDamageable != null)
        {
            iDamageable.Damage(damageAmount);
        }
    }
}
