using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        Vector3 direction = cameraTransform.position - transform.position;
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
