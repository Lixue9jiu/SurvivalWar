using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 5f;
    private void Update() {
        float right = Input.GetAxis("Horizontal");
        float forward = Input.GetAxis("Vertical");

        transform.position += (right * Vector3.right + forward * Vector3.forward) * movementSpeed;
    }
}
