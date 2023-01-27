using System.Runtime.InteropServices;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public float movementSpeed = 8.0f;
    public float mouseSensitivity = 2.0f;
    public float jumpForce = 2.0f;
    public float sprintMultiplier = 1.3f; // added
    public float crouchHeight = 0.8f; // added
    public float slideForce = 7f; // added

    private float verticalRotation = 0;
    private float verticalVelocity = 0;
    private bool isCrouching = false; // added
    private bool isSliding = false; // added
    private bool canJump = true; //added
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float fireRate = 0.5f;
    private float nextFire;
    private float bulletSpeed = 7000f;
 
    void Start() 
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        // Handle movement
        float forwardSpeed = Input.GetAxis("Vertical") * movementSpeed;
        float sideSpeed = Input.GetAxis("Horizontal") * movementSpeed;

        if (Input.GetKey(KeyCode.LeftShift)) // added
        {
            forwardSpeed *= sprintMultiplier;
            sideSpeed *= sprintMultiplier;
        }

        if (Input.GetKeyDown(KeyCode.C)) // added
        {
            isCrouching = !isCrouching;
            if (isCrouching)
            {
                transform.localScale = new Vector3(1, crouchHeight, 1);
                movementSpeed *= 0.5f;
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
                movementSpeed *= 2f;
            }
        }

         if (Input.GetKeyDown(KeyCode.LeftControl) && !isCrouching) // added
        {
            isSliding = true;
        }
        if(Input.GetKeyUp(KeyCode.LeftControl))
        {
            isSliding = false;
        }

        if (isSliding) // added
        {
            forwardSpeed *= slideForce;
            sideSpeed *= slideForce;
        }

        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        if (Input.GetButton("Jump") && canJump)
        {
            verticalVelocity = jumpForce;
            canJump = false;
        }

        CharacterController cc = GetComponent<CharacterController>();
        if(cc.isGrounded)
        {
            canJump = true;
        }

        if(Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * bulletSpeed);
        }

        Vector3 speed = new Vector3(sideSpeed, verticalVelocity, forwardSpeed);
        speed = transform.rotation * speed;

        cc.Move(speed * Time.deltaTime);

        // Handle mouse look
        float rotLeftRight = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, rotLeftRight, 0);

        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -90, 90);

        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
}
