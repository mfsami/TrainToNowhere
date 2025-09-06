using TMPro;
using UnityEngine;

public class ProjectileDistance : MonoBehaviour
{
    public Transform bulletPos;
    public Transform parryPos;
    public TextMeshProUGUI distanceText;


    private void Update()
    {
        float dist = Vector3.Distance(bulletPos.position, parryPos.position);
        distanceText.text = $"Distance: {dist}";

    }
}
