using UnityEngine;
using UnityEngine.Animations;

public class EnemyShoot : MonoBehaviour
{
    public Transform FirePoint;
    public Transform playerTransform;

    public float rayDistance = 200f;



    private void Update()
    {
        LookAt();
        ShootDebug();

        
    }
    void ShootDebug()
    {
        if (!FirePoint) return;

        Ray ray = new Ray(FirePoint.position, FirePoint.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red, 0f);      // exact to hit
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.yellow, 0f); // full length
        }
    }

    public void LookAt()
    {
        transform.LookAt(playerTransform);
    }
}
