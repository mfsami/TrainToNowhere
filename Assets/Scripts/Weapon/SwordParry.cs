using UnityEngine;

public class SwordParryHold : MonoBehaviour
{
    [Header("Refs")]
    public Transform sword;      
    public Transform poseIdle;   
    public Transform poseParry;

    [Header("Timings")]
    public float raiseTime = 0.06f;   // to parry
    public float lowerTime = 0.10f;   // back to idle
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // 0 = idle, 1 = fully parry
    float blend;

    void Start()
    {
        ApplyPose(poseIdle);
        blend = 0f;
    }

    void Update()
    {

        bool wantParry = Input.GetMouseButton(0);

        // Different speeds for up vs down
        float speed = (wantParry ? (1f / Mathf.Max(raiseTime, 0.0001f))
                                 : (1f / Mathf.Max(lowerTime, 0.0001f)));

        // Move blend toward target (0 or 1)
        float target = wantParry ? 1f : 0f;
        blend = Mathf.MoveTowards(blend, target, speed * Time.deltaTime);

        // Eased interpolation between poses
        float t = ease.Evaluate(blend);
        sword.localPosition = Vector3.LerpUnclamped(poseIdle.localPosition, poseParry.localPosition, t);
        sword.localRotation = Quaternion.SlerpUnclamped(poseIdle.localRotation, poseParry.localRotation, t);
    }

    void ApplyPose(Transform pose)
    {
        sword.localPosition = pose.localPosition;
        sword.localRotation = pose.localRotation;
    }

    
}
