using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>Keyboard cycling prototype. Local +Z is forward; use a unit-scale capsule.</summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public sealed class CapsuleCycleController : MonoBehaviour
{
    [Header("Speed (metres per second)")]
    [Min(0.1f)] public float maxSpeed = 14f;
    [Min(0.1f)] public float acceleration = 3f;
    [Min(0f)] public float coastDeceleration = 0.7f;
    [Min(0.1f)] public float braking = 9f;
    public bool autoPedal = false;

    [Header("Steering")]
    [Min(1f)] public float lowSpeedTurnRate = 85f;
    [Min(1f)] public float highSpeedTurnRate = 35f;
    [Min(0.01f)] public float steeringResponse = 5f;

    [Header("Ground")]
    [Min(0.1f)] public float gravity = 25f;
    [Min(1f)] public float groundStickSpeed = 4f;
    [Tooltip("Press R to return to the starting position.")]
    public bool allowReset = true;

    public float SpeedKmh { get; private set; }
    public bool IsGrounded => controller != null && controller.isGrounded;

    private CharacterController controller;
    private float speed;
    private float verticalSpeed;
    private float steering;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private void Reset()
    {
        CharacterController cc = GetComponent<CharacterController>();
        cc.height = 2f;
        cc.radius = 0.45f;
        cc.center = Vector3.zero;
        cc.slopeLimit = 45f;
        cc.stepOffset = 0.3f;
        cc.skinWidth = 0.05f;
        cc.minMoveDistance = 0f;
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        // A primitive capsule already has a collider. The CharacterController replaces it.
        CapsuleCollider primitiveCollider = GetComponent<CapsuleCollider>();
        if (primitiveCollider != null) primitiveCollider.enabled = false;
        if (GetComponent<Rigidbody>() != null)
        {
            Debug.LogError("Remove the Rigidbody: this controller uses CharacterController movement.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f || !controller.enabled) return;

        ReadKeyboard(out float turn, out bool pedal, out bool brake, out bool reset);
        if (allowReset && reset)
        {
            ResetToStart();
            return;
        }

        steering = Mathf.MoveTowards(steering, turn, steeringResponse * dt);
        if (controller.isGrounded)
        {
            float targetSpeed = !brake && (pedal || autoPedal) ? maxSpeed : 0f;
            float rate = brake ? braking : targetSpeed > speed ? acceleration : coastDeceleration;
            speed = Mathf.MoveTowards(speed, targetSpeed, rate * dt);

            // No turning on the spot. Steering becomes gentler as speed increases.
            float turnRate = Mathf.Lerp(lowSpeedTurnRate, highSpeedTurnRate,
                Mathf.Clamp01(speed / Mathf.Max(0.1f, maxSpeed)));
            transform.Rotate(0f, steering * turnRate * Mathf.Clamp01(speed / 2f) * dt, 0f);
            // Downward travel keeps the capsule in contact with descending roads.
            verticalSpeed = -Mathf.Max(groundStickSpeed, speed);
        }
        else
        {
            verticalSpeed = Mathf.Max(verticalSpeed - gravity * dt, -60f);
        }

        Vector3 oldPosition = transform.position;
        CollisionFlags flags = controller.Move(
            (transform.forward * speed + Vector3.up * verticalSpeed) * dt);
        if ((flags & CollisionFlags.Above) != 0 && verticalSpeed > 0f) verticalSpeed = 0f;

        Vector3 displacement = transform.position - oldPosition;
        displacement.y = 0f;
        float actualSpeed = displacement.magnitude / dt;
        SpeedKmh = actualSpeed * 3.6f;
        // Avoid accumulating full speed while pressing into a wall.
        if ((flags & CollisionFlags.Sides) != 0)
            speed = Mathf.Min(speed, actualSpeed);
    }

    public void ResetToStart()
    {
        controller.enabled = false;
        transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        controller.enabled = true;
        speed = verticalSpeed = steering = SpeedKmh = 0f;
    }

    private static void ReadKeyboard(out float turn, out bool pedal, out bool brake, out bool reset)
    {
        turn = 0f;
        pedal = brake = reset = false;
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;
        turn = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1f : 0f)
             - (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1f : 0f);
        pedal = keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed;
        brake = keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed || keyboard.spaceKey.isPressed;
        reset = keyboard.rKey.wasPressedThisFrame;
#elif ENABLE_LEGACY_INPUT_MANAGER
        turn = (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1f : 0f)
             - (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);
        pedal = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        brake = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.Space);
        reset = Input.GetKeyDown(KeyCode.R);
#endif
    }
}
