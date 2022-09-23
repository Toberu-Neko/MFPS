using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.Collections;
using UnityEditor;
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
        public float slideSpeed;
        [Range(0f, 0.999f)]
        public float airMultiplier;
        public float speedIncreaseMultiplier;
        public float slopeIncreaseMultiplier;
        public float playerHeight;

        private float desiredMoveSpeed;
        private float lastDesiredMoveSpeed;

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
        public LayerMask jumpPad;
        public Transform groundDetector;
        public Camera normalCam;
        public Transform orientation;


        [Header("�שY")]
        private RaycastHit slopeHit;
        public float slopeMaxAngle;
        private bool exitingSlope = false;
        public bool isSliding;

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
            //crouchKey = KeyCode.C;
            //defultFOV = normalCam.fieldOfView;
            //Camera.main.enabled = false;
            rig = GetComponent<Rigidbody>();
            rig.freezeRotation = true;

            readyToJump = true;

            startYScale = transform.localScale.y;
        }
        private void Update()
        {

            /*
            //Controls
            bool sprint = Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift);
            bool jump = Input.GetKeyDown(KeyCode.Space);
            */
            //States
            isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.2f, ground);//Raycast(�����ؼЦ�m, ������V, �������D���Z���A�p�󬰯u, layerMask)
            //bool isJumping = jump && isGrounded;
            //bool isSprinting = sprint && vMove > 0 && !isJumping && isGrounded;


            PlayerInput();
            SpeedControl();
            StateHandler();
            
            

            //Jumping
            /*if (isJumping)
            {
                rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                //jumped = true;
            }*/

            //Drag
            if (isGrounded)
                rig.drag = groundDrag;
            else
                rig.drag = 0;
        }

        void FixedUpdate()
        {
            //Controls
            //bool sprint = Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift);
            //bool jump = Input.GetKeyDown(KeyCode.Space);

            //States
            //isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.3f, ground);//Raycast(�����ؼЦ�m, ������V, �������D���Z���A�p�󬰯u, layerMask)
            //bool isJumping = jump && isGrounded;
            //bool isSprinting = sprint && vMove > 0 && !isJumping && isGrounded;

            Movement();
            //SpeedControl();



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
            sliding,
            crouching,
            air
        }
        private void StateHandler()
        {
            if (isSliding)
            {
                if (OnSlope() && rig.velocity.y < 0.2f)
                {
                    state = MovementState.sliding;
                    
                    desiredMoveSpeed = slideSpeed;
                }
                else
                {
                    state = MovementState.crouching;
                    desiredMoveSpeed = crouchSpeed;
                }
            }
            else if (Input.GetKey(crouchKey))
            {
                state = MovementState.crouching;
                desiredMoveSpeed = crouchSpeed;
            }
            else if (isGrounded && Input.GetKey(sprintKey))
            {
                state = MovementState.sprinting;
                desiredMoveSpeed = sprintSpeed;
            }
            else if(isGrounded)
            {
                state = MovementState.walking;
                desiredMoveSpeed = walkSpeed;
            }
            else
            {
                state = MovementState.air;
            }
            //�ˬd�O�_�ߧY�ܧ�desiredMoveSpeed
            if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0) 
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());

            }
            else
            {
                //
                //StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
            lastDesiredMoveSpeed = desiredMoveSpeed;

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
            JumpPad();
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

            if (OnSlope() && !exitingSlope)
            {
                rig.AddForce(GetSlopeMoveDirection(movementDirection) * moveSpeed * 20f, ForceMode.Force);

                if (rig.velocity.y > 0)
                {
                    rig.AddForce(Vector3.down * 80f, ForceMode.Force);
                }
                //Debug.Log("onSlope");
            }
            else if (isGrounded)
                rig.AddForce(movementDirection * moveSpeed * 10f, ForceMode.Force);
            else if (!isGrounded)
                rig.AddForce(movementDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);

            rig.useGravity = !OnSlope();

        }
        private void Jump()
        {
            exitingSlope = true;
            //reset t velocity
            rig.velocity = new Vector3(rig.velocity.x, 0f, rig.velocity.z);

            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);//Vector3.up ���Ҽ{rotation, transform.up�Ҽ{
        }
        private void resetJump()
        {
            readyToJump = true;

            exitingSlope = false;
        }
        
        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3(rig.velocity.x, 0f, rig.velocity.z);

            if (OnSlope() && !exitingSlope)
            {
                if(rig.velocity.magnitude > moveSpeed)
                {
                    rig.velocity = rig.velocity.normalized * moveSpeed;
                    //Debug.Log(rig.velocity.magnitude);
                }
            }
            else
            {
                //limit velocity if needed
                if (flatVel.magnitude > moveSpeed)
                {
                    Vector3 limitedVel = flatVel.normalized * moveSpeed;
                    rig.velocity = new Vector3(limitedVel.x, rig.velocity.y, limitedVel.z);
                }
            }

        }

        public bool OnSlope()
        {
            //isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.3f, ground);
            if (Physics.Raycast(groundDetector.position, Vector3.down, out slopeHit, 0.3f))
            {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                return angle < slopeMaxAngle && angle != 0;
            }
            return false;
        }

        public Vector3 GetSlopeMoveDirection(Vector3 direction)
        {
            return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
        }

        private IEnumerator SmoothlyLerpMoveSpeed()
        {
            float t_time = 0;
            float t_difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
            float startValue = moveSpeed;

            while (t_time < t_difference)
            {
                moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, t_time / t_difference);
                if (OnSlope())
                {
                    float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                    float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                    t_time +=Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
                }
                else
                t_time += Time.deltaTime * speedIncreaseMultiplier;
                lastDesiredMoveSpeed = desiredMoveSpeed;
                //moveSpeed = desiredMoveSpeed;
                yield return null;
            }

            moveSpeed = desiredMoveSpeed;
            
        }
        private void JumpPad()
        {
            bool isOnJumpPad = Physics.Raycast(groundDetector.position, Vector3.down, 0.2f, jumpPad);

            if (isOnJumpPad)
            {
                rig.AddForce(Vector3.up * 2f, ForceMode.Impulse);
            }
        }
    }
}

