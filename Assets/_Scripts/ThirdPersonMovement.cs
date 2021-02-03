using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Character Controller based on Brackeys
    // THIRD PERSON MOVEMENT in Unity: https://youtu.be/4HpC--2iowE
// Gravity & Jump from
    // FIRST PERSON MOVEMENT in Unity: https://youtu.be/_QajrabyTJc

public class ThirdPersonMovement : MonoBehaviour
{
    public float speed = 6f;                // run speed
    public float jumpHeight = 1.75f;        // jump height in units/meters
    //public int maxJumps = 1;
    public float gravityMultiplier = 1f;
    public float turnSmoothTime = 0.1f;     // smooth turn

    public Transform groundCheck;
    public float groundRadius;
    public LayerMask groundMask;

    public CharacterController controller;
    public Animator animator;

    private Vector3 inputDir;
    private Vector3 velocity;
    private float gravity;
    private float turnSmoothVelocity;
    private bool isGrounded;
    private bool jumpButtonPressed;
    private Transform cam;
    //private int currentJumps;

    void Start()
    {
        cam = Camera.main.transform;
        gravity = Physics.gravity.y;
        if (controller == null) controller = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        GetInput();
        CheckGround();
        Jump();
        Gravity();
        Move();
    }

    void GetInput()
    {
        // can easily be modified to use new Input System
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        inputDir = new Vector3(horizontal, 0f, vertical).normalized;
        jumpButtonPressed = Input.GetButtonDown("Jump");
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);
        animator.SetBool("isGrounded", isGrounded);
        //if (isGrounded) currentJumps = 0;
    }

    void Move()
    {
        float moveSpeed = inputDir.magnitude;
        animator.SetFloat("moveSpeed", moveSpeed);

        if (moveSpeed >= 0.1f)
        {
            // Calculate target angle from z axis in radians relative to camera
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            // Smoothly damp rotation angle towards target angle
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            // Set character rotation to angle
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            // Calculate move in direction of target angle
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            // Move character controller
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }

    void Jump()
    {
        if (jumpButtonPressed && isGrounded) // && currentJumps < maxJumps)
        {
            // Physics formula that allows you to specify jump based on height
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * -gravity * gravityMultiplier);
            //isGrounded = false;
            //currentJumps++;
        }
    }

    void Gravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            //currentJumps = 0;
        }
        velocity.y += gravity * gravityMultiplier * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
