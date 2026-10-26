using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[DefaultExecutionOrder(-100)]
public class PlayerMouseLook : MonoBehaviour
{
    [Tooltip("Degrees turned per pixel of mouse movement.")]
    [Min(0f)] public float sensitivity = 0.15f;
    public bool invertY;
    [Range(-80f, 80f)] public float minimumPitch = -20f;
    [Range(-80f, 80f)] public float maximumPitch = 70f;
    [Range(-80f, 80f)] public float startingPitch = 26.565f;

    public float Pitch { get; private set; }
    private float yaw;
    private bool ownsCursor;

    private void OnEnable()
    {
        yaw = transform.eulerAngles.y;
        Pitch = Mathf.Clamp(startingPitch, minimumPitch, maximumPitch);
    }

    private void Update()
    {
        if (!Application.isFocused || Time.timeScale <= 0f)
            return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ReleaseCursor();
            return;
        }

        Mouse mouse = Mouse.current;
        if (mouse == null)
            return;
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            // Click the Game view to start; skip the capture frame to avoid a sudden turn.
            if (mouse.leftButton.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                ownsCursor = true;
                yaw = transform.eulerAngles.y;
            }
            return;
        }
        if (!ownsCursor)
            return;

        // Mouse delta already represents movement this frame: do not multiply by deltaTime.
        Vector2 delta = mouse.delta.ReadValue() * sensitivity;
        yaw = Mathf.Repeat(yaw + delta.x, 360f);
        Pitch = Mathf.Clamp(Pitch + delta.y * (invertY ? 1f : -1f), minimumPitch, maximumPitch);
        // The block stays upright; vertical looking is handled by the camera.
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    private void OnApplicationFocus(bool focused)
    {
        if (!focused)
            ReleaseCursor();
    }

    private void OnDisable()
    {
        ReleaseCursor();
    }

    private void ReleaseCursor()
    {
        if (!ownsCursor)
            return;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ownsCursor = false;
    }

    private void OnValidate()
    {
        maximumPitch = Mathf.Max(minimumPitch, maximumPitch);
        startingPitch = Mathf.Clamp(startingPitch, minimumPitch, maximumPitch);
    }
}
