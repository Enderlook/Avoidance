using AvalonStudios.Additions.Extensions;

using UnityEngine;

namespace AvalonStudios.Additions.Components.Cameras
{
    public class FPSCamera : FreeLookCamera
    {
        private float xRotation = 0f;

        private Vector3 cameraPos;
        private bool realocateCamera = false;

        public override void Awake()
        {
            StopCameraActions = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (mode == Mode.FPS)
                realocateCamera = false;
            if (mode == Mode.FreeLook)
                realocateCamera = true;

            if (Application.isPlaying || Application.isEditor)
                camera = transform.FindTransformChildOfType<Camera>();

            GetCamera = camera;
            cameraPos = camera.transform.position;

            if (mode == Mode.FPS)
                resetRotation = true;
            if (mode == Mode.FreeLook)
                resetRotation = false;
        }

        public override void Update()
        {
            if (mode == Mode.None || StopCameraActions)
                return;

            Rotate(Time.deltaTime);
            Zoom(cameraSettings.AimButton.Execute(), Time.deltaTime, camera);

            if (mode == Mode.FPS)
                cameraPos = camera.transform.position;

            if (mode == Mode.FreeLook)
                Movement(Time.deltaTime, camera);
        }

        protected override void LateUpdate()
        {
            if (mode == Mode.None || StopCameraActions)
                return;

            if (mode == Mode.FPS && realocateCamera)
            {
                Follow(follow.position, follow.rotation, Time.deltaTime);
                realocateCamera = false;
            }
            if (mode == Mode.FreeLook && !realocateCamera)
                realocateCamera = true;
        }

        protected override void Rotate(float time)
        {
            float mouseX = 0;
            float mouseY = 0;

            if (mode == Mode.FPS)
            {
                mouseX += Input.GetAxis(cameraSettings.XAxis.InputAxisName) * cameraSettings.XAxis.Sensitivity / cameraSettings.RotationSpeed;
                mouseY += Input.GetAxis(cameraSettings.YAxis.InputAxisName) * cameraSettings.YAxis.Sensitivity;

                xRotation -= mouseY;
                mouseX = Mathf.Repeat(mouseX, 360);
                xRotation = Mathf.Clamp(xRotation, cameraSettings.MinimumAngle, cameraSettings.MaximumAngle);
                Quaternion cameraRotation = Quaternion.Euler(xRotation, 0, 0);
                Quaternion newRotation = Quaternion.Slerp(camera.transform.localRotation, cameraRotation, time * cameraSettings.RotationSpeed);
                camera.transform.localRotation = newRotation;
                follow.Rotate(Vector3.up * mouseX);
            }

            if (mode == Mode.FreeLook)
            {
                newX += 1.4f * Input.GetAxis(cameraSettings.XAxis.InputAxisName);
                newY += 1.4f * Input.GetAxis(cameraSettings.YAxis.InputAxisName);

                Vector3 eulerAngleAxis = new Vector3();
                eulerAngleAxis.x = -newY;
                eulerAngleAxis.y = newX;

                newX = Mathf.Repeat(newX, 360);
                newY = Mathf.Clamp(newY, cameraSettings.MinimumAngle, cameraSettings.MaximumAngle);

                Quaternion newRotation = Quaternion.Slerp(camera.transform.localRotation, Quaternion.Euler(eulerAngleAxis), time * cameraSettings.RotationSpeed);

                camera.transform.localRotation = newRotation;
            }
        }

        protected override void Follow(Vector3 targetPosition, Quaternion targetRotation, float time) =>
            camera.transform.position = Application.isEditor ? cameraPos : cameraPos;
    }
}
