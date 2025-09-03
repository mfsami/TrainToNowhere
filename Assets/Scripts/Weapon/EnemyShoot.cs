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
    
    public float fireInterval = 3f;
    private float shootTimer;

    

    void Update()
    {
        LookAt();

        // update timer
        shootTimer += Time.deltaTime;

        if(shootTimer > fireInterval)
        {
            // reset timer
            shootTimer = 0;

            // shoot projectile
            Shoot();
        }
    }

    void Shoot()
    {
        

        Vector3 origin = FirePoint.position + FirePoint.forward * 0.02f;
        Ray ray = new Ray(origin, FirePoint.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            // Raise a bit to hit head
            Vector3 lifted = hit.point + Vector3.up * yLift;
            Vector3 dir = (lifted - FirePoint.position).normalized;

            // DEBUG RAY
            Debug.DrawLine(origin, lifted, Color.red, 0.1f);


            GameObject currentBullet = Instantiate(bullet, FirePoint.position, Quaternion.LookRotation(dir));
            Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
            if (rb) rb.linearVelocity = dir * shootForce;  
        }
    }

    void LookAt()
    {
        if (playerTransform)
            transform.LookAt(playerTransform);
    }
}
