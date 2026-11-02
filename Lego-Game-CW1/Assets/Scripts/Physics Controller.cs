using UnityEngine;

public class PhysicsController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody rb;
    public float speed = 5f;
    private float horizontal;
    private float vertical;
    private bool isGrounded;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()

    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }


    private void FixedUpdate()
    {
        
    
    Vector3 movement = new Vector3(horizontal, 0f, vertical);
        rb.linearVelocity = new Vector3(
        movement.x * speed,
        rb.linearVelocity.y,
        movement.z * speed
        );
}

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}

