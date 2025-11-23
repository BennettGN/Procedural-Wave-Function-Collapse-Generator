using UnityEngine;
using UnityEngine.InputSystem;
namespace GodinhoNelsonBennett.Lab3
{
    public class CameraRotater : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset;

        [Header("Rotation Settings")]
        [SerializeField] private float sensitivity = 100f;
        [SerializeField] private float rotationSpeed = 10f; // Degrees per second

        private InputAction lookAction;
        private float yaw;
        private float pitch;

        public void Initialize(InputAction lookAction)
        {
            this.lookAction = lookAction;
        }

        private void Start()
        {
            //Get current camera angles
            Vector3 angles = transform.eulerAngles;
            yaw = angles.y;
            pitch = angles.x;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            //Get mouse input
            Vector2 lookInput = lookAction.ReadValue<Vector2>();
            yaw += lookInput.x * sensitivity * Time.deltaTime;
            pitch -= lookInput.y * sensitivity * Time.deltaTime;

            // Clamp pitch to avoid flipping / gimbal lock
            pitch = Mathf.Clamp(pitch, -80f, 80f);

            //Transform based on updated pitch and yaw
            Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // Position camera relative to target
            transform.position = target.position + transform.rotation * offset;
        }
    }
}