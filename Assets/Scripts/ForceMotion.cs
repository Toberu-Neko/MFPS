using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Neko.SelfLearning
{
    public class ForceMotion : MonoBehaviour
    {
        #region Variables
        private float moveSpeed;//�쬰orgSpeed
        [Header("���a�t��")]
        public float walkSpeed;
        public float sprintSpeed;
        public float groundDrag;
        [Range(0f, 0.999f)]
        public float airMultiplier;
        [Header("���D")]
        public float jumpForce;
        public float jumpCooldown;
        bool readyToJump;
        [Header("�ۤU")]
        public float crouchSpeed;
        public float crouchSuckToGorundMutiplier;
        public float crouchYScale;
        private float startYScale;

        
        [Header("Keybind")]
        public KeyCode jumpKey = KeyCode.Space;
        public KeyCode sprintKey = KeyCode.LeftShift;
        public KeyCode crouchKey = KeyCode.C;
        //public KeyCode keepCrouchKey = KeyCode.LeftControl;

        [Header("���[����")]
        public LayerMask ground;
        public Transform groundDetector;
        public Camera normalCam;
        public Transform orientation;

        public MovementState state;
        private float hMove, vMove;
        private float defultFOV;
        Vector3 movementDirection;
        //private float adjustedSpeed;
        private Rigidbody rig;

        bool isGrounded;
        //bool jump, jumped = false;

        #endregion
        #region Monobehaviour Callbacks
        void Start()
        {
            crouchKey = KeyCode.C;
            defultFOV = normalCam.fieldOfView;
            Camera.main.enabled = false;
            rig = GetComponent<Rigidbody>();
            rig.freezeRotation = true;
            startYScale = transform.localScale.y;
        }
        private void Update()
        {
            StateHandler();
            PlayerInput();
            SpeedControl();

            //Controls
            bool sprint = Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift);
            bool jump = Input.GetKeyDown(KeyCode.Space);

            //States
            isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);//Raycast(�����ؼЦ�m, ������V, �������D���Z���A�p�󬰯u, layerMask)
            bool isJumping = jump && isGrounded;
            bool isSprinting = sprint && vMove > 0 && !isJumping && isGrounded;

            //Jumping
            if (isJumping)
            {
                rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                //jumped = true;
            }

            //Drag
            if (isGrounded)
                rig.drag = groundDrag;
            else
                rig.drag = 0;
        }

        void FixedUpdate()
        {
            //Controls
            bool sprint = Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift);
            bool jump = Input.GetKeyDown(KeyCode.Space);

            //States
            isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);//Raycast(�����ؼЦ�m, ������V, �������D���Z���A�p�󬰯u, layerMask)
            bool isJumping = jump && isGrounded;
            //bool isSprinting = sprint && vMove > 0 && !isJumping && isGrounded;

            Movement();


            //FOV
            /*if (isSprinting)
            {
                //Lerp(a,b,c)=a�g�Lc���ܦ�b
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, defultFOV * sprintFOVModifier, Time.deltaTime * 8f);
            }
            else
            {
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, defultFOV, Time.deltaTime * 8f);
            }*/

        }
        #endregion
        #region States
        public enum MovementState
        {
            walking,
            sprinting,
            crouching,
            air
        }
        private void StateHandler()
        {
            if (Input.GetKey(crouchKey))
            {
                state = MovementState.crouching;
                moveSpeed = crouchSpeed;
            }
            else if (isGrounded && Input.GetKey(sprintKey))
            {
                state = MovementState.sprinting;
                moveSpeed = sprintSpeed;
            }
            else if(isGrounded)
            {
                state = MovementState.walking;
                moveSpeed = walkSpeed;
            }
            else
            {
                state = MovementState.air;
            }
        }
        #endregion

        private void PlayerInput()
        {
            hMove = Input.GetAxisRaw("Horizontal");//����A+1, D-1
            vMove = Input.GetAxisRaw("Vertical");//����W+1, S=1

            if(Input.GetKeyDown(jumpKey) && isGrounded && readyToJump)
            {
                //Jump
                readyToJump = false;
                Jump();
                #region Invoke�Ϊk�]���ѡ^
                /*
                public void Invoke(string methodName, float time);
                    -Invoke ( �e����funtion,�X���}�l�ե� )

                public void InvokeRepeating(string methodName, float time, float repeatRate);
                    -InvokeRepeating ( �e����funtion, �X���}�l�ե�, �}�l�եΫ�C�X��A�ե� ) 

                public bool IsInvoking(string methodName);
                    -IsInvoking ( �e����funtion ) �P�_�O�_���b�եΤ�
                */
                #endregion
                Invoke(nameof(resetJump), jumpCooldown);//Invoke(nameof(A), b) A=Function, b=�X������;
            }
            //Crouch
            if (Input.GetKeyDown(crouchKey))
            {
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                rig.AddForce(Vector3.down * crouchSuckToGorundMutiplier, ForceMode.Impulse);
            }
            if (Input.GetKeyUp(crouchKey))
            {
                transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            }
        }
        private void Movement()
        {
            /*//OldMovement
            Vector3 t_direction = new Vector3(hMove, 0, vMove);
            t_direction.Normalize();
            Vector3 t_targetVelocity = transform.TransformDirection(t_direction) * moveSpeed * Time.deltaTime;

            if (isGrounded)
                rig.AddForce(t_targetVelocity * moveSpeed * 10f, ForceMode.Force);
            if (!isGrounded)
                rig.AddForce(t_targetVelocity * moveSpeed * 10f * airMultiplier, ForceMode.Force);*/

            movementDirection = orientation.forward * vMove + orientation.right * hMove;
            /*Vector3 t_direction = new Vector3(hMove, 0, vMove);
            t_direction.Normalize();
            Vector3 t_targetVelocity = transform.TransformDirection(t_direction) * moveSpeed * Time.deltaTime;*/

            if (isGrounded)
                rig.AddForce(movementDirection * moveSpeed * 10f, ForceMode.Force);
            if (!isGrounded)
                rig.AddForce(movementDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        }
        private void Jump()
        {
            //reset t velocity
            rig.velocity = new Vector3(rig.velocity.x, 0f, rig.velocity.z);

            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);//Vector3.up ���Ҽ{rotation, transform.up�Ҽ{
        }
        private void resetJump()
        {
            readyToJump = true;
        }
        
        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3(rig.velocity.x, 0f, rig.velocity.z);

            //limit velocity if needed
            if(flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rig.velocity = new Vector3 (limitedVel.x, rig.velocity.y, limitedVel.z);
            }
        }
    }
}

