using System.Collections;
using TMPro;
using UnityEngine;

public class SwordParry : MonoBehaviour
{


    [Header("Refs")]
    public Transform sword;
    public Transform poseIdle;
    public Transform poseParry;
    public TextMeshProUGUI parryStyle;
    public BoxCollider parryTrigger; // isTrigger = true
    public PlayerHealth playerHealth;
    public bool parried;
    

    [Header("Timings")]
    public float raiseTime = 0.06f;
    public float lowerTime = 0.10f;

    [Header("States")]
    private bool isBlocking;
    private bool isParryWindowActive;

    Coroutine parryRoutine;

    [Header("Parry data")]
    public float parryWindowDuration = 0.2f;
    public float reflectSpeed = 150f;

    

    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // internal 0..1 blend between idle (0) and parry (1)
    float blend = 0f;

    void Start()
    {
        // start at idle pose
        if (sword && poseIdle)
        {
            sword.localPosition = poseIdle.localPosition;
            sword.localRotation = poseIdle.localRotation;
        }

        // Disable parry collider
        parryTrigger.enabled = false;
    }

    void Update()
    {

        
        // --- input ---
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            parryTrigger.enabled = true;
            HandleBlockPressed();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            parryTrigger.enabled = false;
            HandleBlockReleased();
        }

        // --- pose ---
        float speed = isBlocking
            ? 1f / Mathf.Max(raiseTime, 0.0001f)
            : 1f / Mathf.Max(lowerTime, 0.0001f);

        float target = isBlocking ? 1f : 0f;
        blend = Mathf.MoveTowards(blend, target, speed * Time.deltaTime);

        if (sword && poseIdle && poseParry)
        {
            float t = ease.Evaluate(blend);
            sword.localPosition = Vector3.LerpUnclamped(poseIdle.localPosition, poseParry.localPosition, t);
            sword.localRotation = Quaternion.SlerpUnclamped(poseIdle.localRotation, poseParry.localRotation, t);
        }
    }

    void HandleBlockPressed()
    {
        isBlocking = true;
        parryRoutine = StartCoroutine(ParryWindow());
    }

    IEnumerator ParryWindow()
    {
        //Debug.Log("Parry window opened");

        // Parry window opens
        isParryWindowActive = true;

        // Collider trigger enabled
        
        //Debug.Log("Parry window opened. Timer started");
        yield return new WaitForSeconds(parryWindowDuration);

        // closes after timer
        isParryWindowActive = false;
        //Debug.Log("Parry window closed");
    }

    void HandleBlockReleased()
    {
        isBlocking = false;
        isParryWindowActive = false;
    }
    private void OnTriggerEnter(Collider other)
    {


        if (isParryWindowActive && other.CompareTag("Bullet")) // PERFECT
        {
            var rb = other.GetComponent<Rigidbody>();
            if (rb)
            {
                var v = rb.linearVelocity;
                rb.linearVelocity = -v.normalized * reflectSpeed;
            }

            var b = other.GetComponent<Bullet>();
            if (b) b.parried = true;               // <- mark the specific bullet as parried

            parryStyle.text = "Parry: PERFECT";
        }
        else if (isBlocking && other.CompareTag("Bullet"))      // OKAY block
        {
            if (playerHealth) playerHealth.Damage(0.5f);
            Destroy(other.gameObject);             // <- consume so it can’t hit the body later
            parryStyle.text = "Parry: OKAY";
        }
    }


}
