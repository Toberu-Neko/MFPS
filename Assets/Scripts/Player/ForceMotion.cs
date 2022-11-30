using System.Collections;
using UnityEngine;
using Photon.Pun;

namespace Com.Neko.SelfLearning
{
    public class ForceMotion : MonoBehaviourPunCallbacks
    {
        #region Variables
        private float moveSpeed;//原為orgSpeed
        [Header("玩家速度")]
        public float walkSpeed;
        public float sprintSpeed;
        public float groundDrag;
        public float slideSpeed;
        [Range(0f, 2f)]
        public float sprintFOVModifier = 1.45f;

        [Range(0f, 0.999f)]
        public float airMultiplier;
        public float speedIncreaseMultiplier;
        public float slopeIncreaseMultiplier;
        public float playerHeight;

        private float desiredMoveSpeed;
        private float lastDesiredMoveSpeed;

        [Header("跳躍")]
        public float jumpForce;
        public float jumpCooldown;
        bool readyToJump;
        [Header("蹲下")]
        public float crouchSpeed;
        public float crouchSuckToGorundMutiplier;
        public float crouchYScale;
        private float startYScale;

        [Header("走路晃動")]
        public Transform weaponParent;
        private Vector3 weaponParentOrigin;
        private float movingTimer;
        private float standingTimer;
        private Vector3 targetHeadBobWeaponPosition;

        [Header("Keybind")]
        public KeyCode jumpKey = KeyCode.Space;
        public KeyCode sprintKey = KeyCode.LeftShift;
        public KeyCode crouchKey = KeyCode.C;
        //public KeyCode keepCrouchKey = KeyCode.LeftControl;



        [Header("附加物件")]
        public LayerMask ground;
        public LayerMask jumpPad;
        public Transform groundDetector;
        public Camera normalCam;
        public Transform orientation;
        public GameObject cameraParent;
        

        [Header("斜坡")]
        private RaycastHit slopeHit;
        public float slopeMaxAngle;
        private bool exitingSlope = false;
        public bool isSliding;

        [Header("物件")]
        //public Camera basicCam;
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
            cameraParent.SetActive(photonView.IsMine);


            //crouchKey = KeyCode.C;
            //defultFOV = normalCam.fieldOfView;
            //Camera.main.enabled = false;
            if(normalCam != null)
            {
               defultFOV = normalCam.fieldOfView;
            }
            rig = GetComponent<Rigidbody>();
            rig.freezeRotation = true;
            weaponParentOrigin = weaponParent.localPosition;

            readyToJump = true;

            startYScale = transform.localScale.y;
        }
        private void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            /*
            //Controls
            bool sprint = Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift);
            bool jump = Input.GetKeyDown(KeyCode.Space);
            */
            //States
            isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.2f, ground);//Raycast(偵測目標位置, 偵測方向, 偵測離主角距離，小於為真, layerMask)
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

            if(vMove == 0 && hMove == 0)
            {
                HeadBob(standingTimer, 0.025f, 0.025f);
                standingTimer += Time.deltaTime;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetHeadBobWeaponPosition, Time.deltaTime * 2f);
            }
            else if(state == MovementState.sliding)
            {
                HeadBob(standingTimer, 0.025f, 0.025f);
                standingTimer += Time.deltaTime;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetHeadBobWeaponPosition, Time.deltaTime * 2f);
            }
            else if(state == MovementState.sprinting)
            {
                HeadBob(movingTimer, 0.15f, 0.075f);
                movingTimer += Time.deltaTime * 7;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetHeadBobWeaponPosition, Time.deltaTime * 6f);
            }
            else
            {
                HeadBob(movingTimer, 0.035f, 0.035f);
                movingTimer += Time.deltaTime * (moveSpeed / 2f);
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetHeadBobWeaponPosition, Time.deltaTime * 6f);
            }
            //weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetHeadBobWeaponPosition, Time.deltaTime * 8f);
        }

        void FixedUpdate()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            //Controls
            //bool sprint = Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift);
            //bool jump = Input.GetKeyDown(KeyCode.Space);

            //States
            //isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.3f, ground);//Raycast(偵測目標位置, 偵測方向, 偵測離主角距離，小於為真, layerMask)
            //bool isJumping = jump && isGrounded;
            //bool isSprinting = sprint && vMove > 0 && !isJumping && isGrounded;

            Movement();
            //SpeedControl();



            //FOV
            /*if (isSprinting)
            {
                //Lerp(a,b,c)=a經過c秒變成b
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
            float FOVset = moveSpeed / walkSpeed *.75f;
            if (FOVset > 1)
                FOVset *= .75f;
            if (FOVset < 1)
                FOVset *= 1.4f;
            if (isSliding)
            {
                if (OnSlope() && rig.velocity.y < 0.2f)
                {
                    state = MovementState.sliding;
                    
                    desiredMoveSpeed = slideSpeed;

                    normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, defultFOV * FOVset, Time.deltaTime * 8f);
                }
                else
                {
                    state = MovementState.crouching;
                    desiredMoveSpeed = crouchSpeed;
                    if(FOVset<1)
                    normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, defultFOV * FOVset, Time.deltaTime * 8f);
                }
            }
            else if (Input.GetKey(crouchKey))
            {
                state = MovementState.crouching;
                desiredMoveSpeed = crouchSpeed;
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, defultFOV * FOVset, Time.deltaTime * 8f);

            }
            else if (isGrounded && Input.GetKey(sprintKey))
            {
                state = MovementState.sprinting;
                desiredMoveSpeed = sprintSpeed;
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, defultFOV * FOVset, Time.deltaTime * 8f);

            }
            else if(isGrounded)
            {
                state = MovementState.walking;
                desiredMoveSpeed = walkSpeed;
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, defultFOV, Time.deltaTime * 8f);
            }
            else
            {
                state = MovementState.air;
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, defultFOV * FOVset, Time.deltaTime * 8f);

            }
            //檢查是否立即變更desiredMoveSpeed
            if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > sprintSpeed - walkSpeed && moveSpeed != 0) 
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
            hMove = Input.GetAxisRaw("Horizontal");//水平A+1, D-1
            vMove = Input.GetAxisRaw("Vertical");//垂直W+1, S=1

            if(Input.GetKeyDown(jumpKey) && isGrounded && readyToJump)
            {
                //Jump
                readyToJump = false;
                Jump();
                #region Invoke用法（註解）
                /*
                public void Invoke(string methodName, float time);
                    -Invoke ( 委派的funtion,幾秒後開始調用 )

                public void InvokeRepeating(string methodName, float time, float repeatRate);
                    -InvokeRepeating ( 委派的funtion, 幾秒後開始調用, 開始調用後每幾秒再調用 ) 

                public bool IsInvoking(string methodName);
                    -IsInvoking ( 委派的funtion ) 判斷是否正在調用中
                */
                #endregion
                Invoke(nameof(resetJump), jumpCooldown);//Invoke(nameof(A), b) A=Function, b=幾秒後執行;
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

            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);//Vector3.up 不考慮rotation, transform.up考慮
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
                if (OnSlope() && (hMove != 0 &&vMove !=0))
                {
                    float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                    float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                    t_time +=Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
                }
                else if (hMove == 0 && vMove == 0)
                {
                    t_time -= Time.deltaTime * speedIncreaseMultiplier;
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
                rig.velocity = (Vector3.up * 10f);
            }
        }

        void HeadBob(float p_z, float p_XIntensity, float p_YIntensity)
        {
            targetHeadBobWeaponPosition = weaponParentOrigin + new Vector3(Mathf.Cos(p_z) * p_XIntensity, Mathf.Sin(p_z * 2) * p_YIntensity, weaponParentOrigin.z) ;
        }
    }
}

