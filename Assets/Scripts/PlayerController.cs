using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 1f;
    public float mouseSensitivity = 3f;
    public KeyCode speedUpButton = KeyCode.LeftShift;
    private void Update() {
        float dt = Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(new Vector3(-mouseY, 0, 0) * mouseSensitivity * dt, Space.Self);
        transform.Rotate(new Vector3(0, mouseX, 0) * mouseSensitivity * dt, Space.World);

        float right = Input.GetAxisRaw("Horizontal");
        float forward = Input.GetAxisRaw("Vertical");

        transform.position += (right * transform.right + forward * transform.forward) * movementSpeed * dt * (Input.GetKey(speedUpButton) ? 5 : 1);
    }
}
