using UnityEngine;
using UnityEngine.InputSystem;

namespace GodinhoNelsonBennett.Lab3
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovementControl : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float boostedSpeedMag = 3f;
        [SerializeField] private float collisionBuffer = 1.2f; // Distance to maintain from walls

        private InputAction moveAction;
        private InputAction speedAction;
        private Rigidbody rb;
        private Collider col;

        public void Initialize(InputAction moveAction, InputAction speedAction)
        {
            this.moveAction = moveAction;
            this.speedAction = speedAction;
            this.moveAction.Enable();
            this.speedAction.Enable();
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        private void FixedUpdate()
        {
            if (moveAction == null) return;

            Vector2 input = moveAction.ReadValue<Vector2>();
            if (input.sqrMagnitude < 0.01f) return;

            // Calculate movement direction relative to camera
            Vector3 forward = cameraTransform.rotation * Vector3.forward;
            Vector3 right = cameraTransform.rotation * Vector3.right;
            Vector3 cameraForward = Vector3.ProjectOnPlane(forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.ProjectOnPlane(right, Vector3.up).normalized;
            Vector3 moveDirection = (cameraForward * input.y + cameraRight * input.x).normalized;

            // Calculate speed with boost
            float currentSpeed = speed;
            if (speedAction.IsPressed())
            {
                currentSpeed *= boostedSpeedMag;
            }

            // Calculate movement step
            float step = currentSpeed * Time.fixedDeltaTime;

            // Check for obstacles before moving
            Vector3 targetPosition = rb.position + moveDirection * step;

            // Use the collider's bounds to check for obstacles
            Vector3 direction = moveDirection;
            float distance = step + collisionBuffer;

            // Cast the collider in the movement direction
            if (Physics.CapsuleCast(
                rb.position + Vector3.up * 0.5f,
                rb.position + Vector3.up * 1.5f,
                col.bounds.extents.x,
                direction,
                out RaycastHit hit,
                distance,
                ~0,
                QueryTriggerInteraction.Ignore // Ignore trigger colliders obstacle detection
                ))
            {
                // Move only to the hit point minus buffer
                float safeDistance = Mathf.Max(0, hit.distance - collisionBuffer);
                targetPosition = rb.position + direction * safeDistance;
            }
         

            // Use MovePosition with MoveTowards
            Vector3 newPosition = Vector3.MoveTowards(rb.position, targetPosition, step);
            rb.MovePosition(newPosition);
        }
    }
}