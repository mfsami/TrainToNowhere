using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform playerBody;
    public Transform enemy;
    public Transform cam;

    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //private void Update()
    //{
    //    // Get mouse input
    //    float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX;
    //    float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY;

    //    xRotation -= mouseY;
    //    xRotation = Mathf.Clamp(xRotation, -90f, 90f);

    //    transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);


    //    playerBody.Rotate(Vector3.up * mouseX);
        
    //}

    private void LateUpdate()
    {
        cam.LookAt(enemy.position);
    }
}
