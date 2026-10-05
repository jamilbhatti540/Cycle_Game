using UnityEngine;

/// <summary>Low chase angle inspired by the reference. Attach to an unparented Camera.</summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public sealed class CycleChaseCamera : MonoBehaviour
{
    public Transform target;

    [Header("Reference framing")]
    [Min(0.1f)] public float distance = 5.5f;
    [Tooltip("Height above the target pivot. A default capsule pivot is its centre.")]
    public float height = 1.8f;
    [Range(-10f, 45f)] public float pitch = 8f;
    [Range(20f, 100f)] public float fieldOfView = 60f;
    [Min(0.01f)] public float positionSmoothTime = 0.12f;
    [Min(0.01f)] public float turnSmoothTime = 0.18f;

    [Header("Obstacle avoidance")]
    public LayerMask obstacleMask = ~0;
    [Min(0.05f)] public float collisionRadius = 0.3f;
    [Min(0.01f)] public float collisionPadding = 0.1f;
    [Tooltip("Sphere-cast origin height above the target pivot.")]
    public float collisionPivotHeight = 0.5f;
    [Min(1f)] public float teleportSnapDistance = 10f;

    private Camera viewCamera;
    private Transform previousTarget;
    private Vector3 previousTargetPosition;
    private Vector3 followPosition;
    private Vector3 positionVelocity;
    private float yaw;
    private float yawVelocity;

    private void Awake()
    {
        viewCamera = GetComponent<Camera>();
        viewCamera.orthographic = false;
        viewCamera.nearClipPlane = 0.1f;
    }

    private void OnEnable() { previousTarget = null; }

    private void LateUpdate()
    {
        if (target == null) return;
        if (previousTarget != target ||
            Vector3.Distance(target.position, previousTargetPosition) > teleportSnapDistance)
            SnapToTarget();

        float dt = Time.deltaTime;
        if (dt > 0f)
        {
            followPosition = Vector3.SmoothDamp(followPosition, target.position,
                ref positionVelocity, positionSmoothTime, Mathf.Infinity, dt);
            yaw = Mathf.SmoothDampAngle(yaw, target.eulerAngles.y,
                ref yawVelocity, turnSmoothTime, Mathf.Infinity, dt);
        }

        Quaternion heading = Quaternion.Euler(0f, yaw, 0f);
        Vector3 desiredPosition = followPosition + Vector3.up * height
                                - heading * Vector3.forward * distance;
        Vector3 pivot = target.position + Vector3.up * collisionPivotHeight;
        Vector3 toCamera = desiredPosition - pivot;
        float castLength = toCamera.magnitude;
        if (castLength > 0.001f)
        {
            Vector3 direction = toCamera / castLength;
            float safeLength = castLength;
            // All hits let us filter the player's own collider even on the Default layer.
            RaycastHit[] hits = Physics.SphereCastAll(pivot, collisionRadius, direction,
                castLength, obstacleMask, QueryTriggerInteraction.Ignore);
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform == target || hit.transform.IsChildOf(target)) continue;
                safeLength = Mathf.Min(safeLength, Mathf.Max(0f, hit.distance - collisionPadding));
            }
            desiredPosition = pivot + direction * safeLength;
        }

        // Fixed pitch preserves the low horizon and lower-centre rider framing.
        transform.SetPositionAndRotation(desiredPosition, Quaternion.Euler(pitch, yaw, 0f));
        viewCamera.fieldOfView = fieldOfView;
        previousTarget = target;
        previousTargetPosition = target.position;
    }

    [ContextMenu("Snap To Target")]
    public void SnapToTarget()
    {
        if (target == null) return;
        followPosition = target.position;
        yaw = target.eulerAngles.y;
        positionVelocity = Vector3.zero;
        yawVelocity = 0f;
        previousTarget = target;
        previousTargetPosition = target.position;
    }
}
