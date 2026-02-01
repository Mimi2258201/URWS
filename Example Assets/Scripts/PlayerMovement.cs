#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

using UnityEngine;

namespace UniversalRangedWeaponSystem.Examples
{
    [RequireComponent(typeof(CharacterController)), AddComponentMenu("Weapon System/Examples/Player Movement", 1)]
    public class PlayerMovement : MonoBehaviour
    {
        public CharacterController controller {  get; private set; }

        [Tooltip("The speed of player movement in metres per second.")]
        public float speed = 12f;

        [Tooltip("The acceleration that is applied to the player when they are falling to the ground.\nMetres per second squared.")]
        public float gravity = -10f;

        [Tooltip("The force applied to the player when jumping.")]
        public float jumpHeight = 2f;

        [Tooltip(" Overrides the Step Offset of the attached Character controller so that it can set the Step Offset to zero when in the air.")]
        public float stepOffsetOverride = 0.3f;


        [Tooltip("This should be set to an empty object that is at the player’s head.\nUsed to validate whether the player is touching the ceiling while jumping.")]
        public Transform ceilingCheck;

        [Tooltip("This should be set to an empty object that is at the player’s feet.\nUsed to validate whether the player is touching the ground.")]
        public Transform groundCheck;

        [Tooltip("The distance the player can be from the ground before being considered in the air. (Helps with steps and slopes).")]
        public float groundDistance = 0.4f;

        [Tooltip(" A Layer Mask of all layers that should be considered the ground.")]
        public LayerMask groundMask = ~0;

        Vector3 velocity;
        [HideInInspector]
        public bool isGrounded;
        private Vector3 explosionForce = Vector3.zero;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            float x;
            float z;
            bool jumpPressed = false;

    #if ENABLE_INPUT_SYSTEM
            var delta = InputManager.instance.moveActionValue;
            x = delta.x;
            z = delta.y;
            jumpPressed = InputManager.instance.hasJumpedThisFrame;
    #else
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
            jumpPressed = Input.GetButtonDown("Jump");
    #endif

            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0f)
            {
                velocity.y = -10f;
                controller.stepOffset = stepOffsetOverride;
            }
            //when on the ground add ground force and reset stepOffset

            Vector3 move = transform.right * x + transform.forward * z;
            //reorient axis I think

            controller.Move(move * speed * Time.deltaTime);
            //apply movement

            if (jumpPressed && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                controller.stepOffset = 0f;
            }
            //jump (set stepOffset to zero because we don't need it and it causes issues when hitting ledges)

            velocity.y += gravity * Time.deltaTime;
            //gravity


            if(Physics.CheckSphere(ceilingCheck.position, groundDistance, groundMask) && velocity.y > 0)
            {
                velocity.y = 0f;
            }
            controller.Move(velocity * Time.deltaTime);
            //stop the jump early if player hits their head




            //add impact force
            if (explosionForce.magnitude > 0.2)
            {
                controller.Move(-explosionForce * Time.deltaTime);
                explosionForce -= explosionForce * Time.deltaTime * 5;
            }
        }

        public void AddImpact(Vector3 dir, float force)
        {
            dir.Normalize();
            explosionForce += dir * 10f;
        }
    }
}
