using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float damageAmount = 1f;
    private bool processed;



    private void OnTriggerEnter(Collider other)
    {
        if (processed) return;

        int playerLayer = LayerMask.NameToLayer("Player");

        // Damage anything that implements damage hook, then destroy.
        

        if (other.gameObject.layer == playerLayer)
        {
            processed = true;

            var dmg = other.GetComponent<IDamageable>();
            dmg.Damage(damageAmount);
            Destroy(gameObject);

        }

        //else
        //{
        //    Destroy(gameObject);
        //}
    }
}
