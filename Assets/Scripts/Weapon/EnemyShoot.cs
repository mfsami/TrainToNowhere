using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public Transform FirePoint;
    public Transform playerTransform;
    public float shootForce = 30f;
    public GameObject bullet;
    public float yLift = 7f;

    public float rayDistance = 200f;

    [Header("Fire timing")]
    public float firstShotDelay = 0.25f;
    public float fireInterval = 3f;

    void OnEnable()
    {
        // fire every 3 seconds after a short delay
        InvokeRepeating(nameof(Shoot), firstShotDelay, fireInterval);
    }

    void OnDisable()
    {
        CancelInvoke(nameof(Shoot));
    }

    void Update()
    {
        LookAt();
    }

    void Shoot()
    {
        if (!FirePoint || !bullet) return;

        Vector3 origin = FirePoint.position + FirePoint.forward * 0.02f;
        Ray ray = new Ray(origin, FirePoint.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            // Raise a bit to hit head
            Vector3 lifted = hit.point + Vector3.up * yLift;

            Debug.DrawLine(origin, lifted, Color.red, 0.1f);

            Vector3 dir = (lifted - FirePoint.position).normalized;


            GameObject currentBullet = Instantiate(bullet, FirePoint.position, Quaternion.LookRotation(dir));
            Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
            if (rb) rb.linearVelocity = dir * shootForce;  
        }
        else
        {
            Debug.DrawRay(origin, ray.direction * rayDistance, Color.yellow, 0.1f);
        }
    }

    void LookAt()
    {
        if (playerTransform)
            transform.LookAt(playerTransform);
    }
}
