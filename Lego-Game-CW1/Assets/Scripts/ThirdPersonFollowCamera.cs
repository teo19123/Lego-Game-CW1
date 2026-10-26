using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class ThirdPersonFollowCamera : MonoBehaviour
{
    public Transform target;
    [Min(0.5f)] public float distance = 4f;
    public float height = 2f;
    [Min(0.01f)] public float smoothTime = 0.18f;
    [Min(0.01f)] public float turnSmoothTime = 0.25f;

    private Vector3 followVelocity;
    private float yaw;
    private float yawVelocity;
    private Renderer targetRenderer;
    private Transform lastTarget;
    private PlayerMouseLook mouseLook;

    private void Start()
    {
        if (target == null)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
                target = player.transform;
        }
        SnapToTarget();
    }

    public void SnapToTarget()
    {
        if (target == null)
            return;
        lastTarget = target;
        mouseLook = target.GetComponent<PlayerMouseLook>();
        targetRenderer = target.GetComponentInChildren<Renderer>();
        yaw = target.eulerAngles.y;
        yawVelocity = 0f;
        followVelocity = Vector3.zero;
        Vector3 focus = GetFocusPoint();
        transform.position = GetCameraPosition(focus);
        transform.LookAt(focus, Vector3.up);
    }

    private Vector3 GetFocusPoint()
    {
        return targetRenderer != null ? targetRenderer.bounds.center : target.position + Vector3.up * 0.5f;
    }

    private Vector3 GetCameraPosition(Vector3 focus)
    {
        // World-space distances keep the view consistent with scaled character models.
        if (mouseLook != null && mouseLook.isActiveAndEnabled)
        {
            float orbitDistance = Mathf.Sqrt(distance * distance + height * height);
            return focus + Quaternion.Euler(mouseLook.Pitch, target.eulerAngles.y, 0f)
                * Vector3.back * Mathf.Max(0.5f, orbitDistance);
        }
        Vector3 behind = Quaternion.Euler(0f, yaw, 0f) * Vector3.back;
        return focus + behind * Mathf.Max(0.5f, distance) + Vector3.up * height;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;
        if (target != lastTarget)
            SnapToTarget();

        yaw = Mathf.SmoothDampAngle(yaw, target.eulerAngles.y, ref yawVelocity,
            Mathf.Max(0.01f, turnSmoothTime));
        Vector3 focus = GetFocusPoint();
        transform.position = Vector3.SmoothDamp(transform.position, GetCameraPosition(focus),
            ref followVelocity, Mathf.Max(0.01f, smoothTime));
        transform.LookAt(focus, Vector3.up);
    }
}

