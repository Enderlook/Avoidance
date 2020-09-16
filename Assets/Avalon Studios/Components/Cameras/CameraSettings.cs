﻿using AvalonStudios.Additions.Utils.InputsManager;

using UnityEngine;

namespace AvalonStudios.Additions.Components.Cameras
{
    [System.Serializable]
    public class CameraSettings
    {
        public AxisControl XAxis { get { return xAxis; } set { xAxis = value; } }

        public AxisControl YAxis { get { return yAxis; } set { yAxis = value; } }

        public float MinimumAngle { get { return minumumAngle; } set { minumumAngle = value; } }

        public float MaximumAngle { get { return maximumAngle; } set { maximumAngle = value; } }

        public float RotationSpeed { get { return rotationSpeed; } set { rotationSpeed = value; } }

        public float ZoomFieldOfView => zoomFieldOfView;

        public float ZoomSpeed => zoomSpeed;

        public MouseInputManager AimButton => aimButton;

        public MouseInputManager SwitchCameraViewDirectionButton => switchCameraViewDirectionButton;

        public MovementSettings Movement { get { return movement; } set { movement = value; } }

        [Header("Camera Options")]

        [SerializeField]
        private AxisControl xAxis = new AxisControl { InputAxisName = "Mouse X" };

        [SerializeField]
        private AxisControl yAxis = new AxisControl { InputAxisName = "Mouse Y" };

        [SerializeField, Tooltip("The minimum angle that the camera can reach")]
        private float minumumAngle = -30f;

        [SerializeField, Tooltip("The maximum angle that the camera can reach")]
        private float maximumAngle = 70f;

        [SerializeField, Tooltip("Speed of the camera rotation")]
        private float rotationSpeed = 5f;

        [Header("Zoom")]

        [SerializeField, Tooltip("The zoom of the field of view")]
        private float zoomFieldOfView = 30f;

        [SerializeField, Tooltip("Speed of the camera zoom")]
        private float zoomSpeed = 3f;

        [Header("Inputs Settings")]

        [SerializeField]
        private MouseInputManager aimButton = null;

        [SerializeField]
        private MouseInputManager switchCameraViewDirectionButton = null;

        [Header("Movement Settings")]

        [SerializeField]
        private MovementSettings movement = null;
    }

    [System.Serializable]
    public class AxisControl
    {
        public float Sensitivity => sensitivity;

        public string InputAxisName { get { return inputAxisName; } set { inputAxisName = value; } }

        [SerializeField, Tooltip("Sensitivity of the axis")]
        private float sensitivity = 5f;

        [SerializeField, Tooltip("The name of the input")]
        private string inputAxisName = "";
    }

    [System.Serializable]
    public class MovementSettings
    {
        // Properties

        public KeyInputManager ForwardMovement => forwardMovement;

        public KeyInputManager BackwardMovement => backwardMovement;

        public KeyInputManager LeftMovement => leftMovement;

        public KeyInputManager RightMovement => rightMovement;

        public KeyInputManager UpMovement => upMovement;

        public KeyInputManager DownMovement => downMovement;

        public KeyInputManager InputSpeedUpMovement => inputSpeedUpMovement;

        public float MovementSpeed => movementSpeed;

        public float SpeedUpMovement => speedUpMovement;

        public float FollowSpeed { get { return followSpeed; } set { followSpeed = value; } }

        // Variables

        [SerializeField]
        private KeyInputManager forwardMovement = null;

        [SerializeField]
        private KeyInputManager backwardMovement = null;

        [SerializeField]
        private KeyInputManager leftMovement = null;

        [SerializeField]
        private KeyInputManager rightMovement = null;

        [SerializeField]
        private KeyInputManager upMovement = null;

        [SerializeField]
        private KeyInputManager downMovement = null;

        [SerializeField, Tooltip("Input to increase speed for movement")]
        private KeyInputManager inputSpeedUpMovement = null;

        [SerializeField]
        private float movementSpeed = 1f;

        [SerializeField, Tooltip("Increase speed when the assigned key is press")]
        private float speedUpMovement = 5f;

        [SerializeField]
        private float followSpeed = 5f;
    }
}
