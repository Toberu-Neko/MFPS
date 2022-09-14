using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Neko.SelfLearning
{
    public class MouseLook : MonoBehaviour
    {
        public static bool cursorLocked = true;
        public Transform player, eyes, camera;

        public float xSensitivity, ySensitivity;
        public float maxAngle;

        private Quaternion camCenter;
        void Start()
        {
            camCenter = eyes.localRotation;//設定鏡頭+眼睛原始位置
        }

        void Update()
        {
            updateCursorLock();
            setX();
            setY();
        }
        void updateCursorLock()
        {
            if (cursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    cursorLocked = false;
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    cursorLocked = true;
                }
            }
        }
        void setX()
        {
            float t_input = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
            Quaternion t_adj = Quaternion.AngleAxis(t_input, Vector3.up);
            Quaternion t_delta = player.localRotation * t_adj;
            player.localRotation = t_delta;

        }
        void setY()
        {
            float t_input = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
            Quaternion t_adj = Quaternion.AngleAxis(t_input, -Vector3.right);
            Quaternion t_delta = camera.localRotation * t_adj;


            if(Quaternion.Angle(camCenter, t_delta) < maxAngle)
            {
                //eyes.localRotation = t_delta;
                camera.localRotation = t_delta;
            }
        }
    }
}

