using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Min(0f)] private float playerSpeed = 5f;
    [SerializeField, Min(0f)] private float jumpHeight = 1.5f;
    [SerializeField] private float gravityValue = -9.81f;

    public CharacterController controller;

    [Header("Input Actions (optional)")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;

    private PlayerMouseLook mouseLook;
    private float verticalVelocity;
    private bool enabledMoveAction;
    private bool enabledJumpAction;

    private void Reset()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Awake()
    {
        mouseLook = GetComponent<PlayerMouseLook>();
        if (controller == null)
            controller = GetComponent<CharacterController>();

        // Step offset is in world units, so small model scales need a smaller step.
        float scaledHeight = controller.height * Mathf.Abs(controller.transform.lossyScale.y);
        bool wasEnabled = controller.enabled;
        controller.enabled = false;
        controller.stepOffset = Mathf.Min(controller.stepOffset, scaledHeight * 0.5f);
        controller.enabled = wasEnabled;
    }

    private void OnEnable()
    {
        verticalVelocity = 0f;
        enabledMoveAction = EnableIfNeeded(moveAction);
        enabledJumpAction = EnableIfNeeded(jumpAction);
    }

    private static bool EnableIfNeeded(InputActionReference reference)
    {
        if (reference == null || reference.action == null || reference.action.enabled)
            return false;

        reference.action.Enable();
        return true;
    }

    private void OnDisable()
    {
        if (enabledMoveAction && moveAction != null && moveAction.action != null)
            moveAction.action.Disable();
        if (enabledJumpAction && jumpAction != null && jumpAction.action != null)
            jumpAction.action.Disable();
    }

    private void OnValidate()
    {
        gravityValue = Mathf.Min(gravityValue, -0.01f);
    }

    private void Update()
    {
        if (controller == null || !controller.enabled)
            return;

        Vector2 input = Vector2.zero;
        bool jumpPressed = false;

        if (moveAction != null && moveAction.action != null)
            input = moveAction.action.ReadValue<Vector2>();
        if (jumpAction != null && jumpAction.action != null)
            jumpPressed = jumpAction.action.WasPressedThisFrame();

        // Keyboard controls also work when no Input Actions have been assigned.
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (input.sqrMagnitude < 0.001f)
            {
                input.x = ((keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) ? 1f : 0f)
                        - ((keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) ? 1f : 0f);
                input.y = ((keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) ? 1f : 0f)
                        - ((keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) ? 1f : 0f);
            }
            jumpPressed |= keyboard.spaceKey.wasPressedThisFrame;
        }

        // Clamp diagonals so they are no faster than moving straight ahead.
        input = Vector2.ClampMagnitude(input, 1f);
        bool useMouseLook = mouseLook != null && mouseLook.isActiveAndEnabled;
        Vector3 movement = useMouseLook
            ? transform.right * input.x + transform.forward * input.y
            : new Vector3(input.x, 0f, input.y);

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        float gravity = Mathf.Min(gravityValue, -0.01f);
        if (controller.isGrounded && jumpPressed)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;
        controller.Move((movement * playerSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);

        if ((controller.collisionFlags & CollisionFlags.Above) != 0 && verticalVelocity > 0f)
            verticalVelocity = 0f;

        if (!useMouseLook && movement.sqrMagnitude > 0.001f)
            transform.forward = movement;
    }
}




