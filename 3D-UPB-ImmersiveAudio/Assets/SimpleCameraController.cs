using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleCameraController : MonoBehaviour
{
    public InputActionReference moveAction;
    public InputActionReference lookAction;

    public float moveSpeed = 5f;
    public float lookSensitivity = 0.1f;

    private float yaw;
    private float pitch;

    private void Start()
    {
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
    }

    private void Update()
    {
        Vector3 moveValue = moveAction.action.ReadValue<Vector3>();
        Vector2 lookValue = lookAction.action.ReadValue<Vector2>();

        transform.Translate(
            moveValue * moveSpeed * Time.deltaTime,
            Space.Self
        );

        yaw += lookValue.x * lookSensitivity;
        pitch -= lookValue.y * lookSensitivity;

        pitch = Mathf.Clamp(pitch, -89f, 89f);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}