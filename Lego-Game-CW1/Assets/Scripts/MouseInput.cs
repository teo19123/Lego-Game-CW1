using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInput : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed = 10f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnMouseDrag()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 movement = new Vector3(mouseX, 0f, mouseY);
        Vector3 displacement = movement * speed * Time.deltaTime;

        //transform.Translate(displacement, Space.World);

        float depth = Camera.main.WorldToScreenPoint(transform.position).z;
        Debug.Log($"Camera depth: {depth}");

        //Debug.Log(movement);
    }
}
