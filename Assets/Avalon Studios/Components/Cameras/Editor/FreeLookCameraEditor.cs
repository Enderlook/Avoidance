using UnityEngine;
using UnityEditor;

namespace AvalonStudios.Additions.Components.Cameras
{
    [CustomEditor(typeof(FreeLookCamera)), CanEditMultipleObjects, ]
    public class ThirdPersonFollowCameraEditor : Editor
    {
        private FreeLookCamera freeLook;

        private Vector3 oldFreeLookCameraPos;
        private Quaternion oldCameraRotation;
        private float oldFOWCam;
        private float oldFOWFreeLook;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Transform pivot = freeLook.transform.GetChild(0);
            Camera mainCamera = pivot.GetChild(0).GetComponent<Camera>();

            if (oldFreeLookCameraPos != freeLook.CameraPosition)
            {
                Undo.RecordObject(freeLook, "Move camera");
                freeLook.MoveCameraPositionByEditor(freeLook.CameraPosition);
                mainCamera.transform.localPosition = freeLook.CameraPosition;
                oldFreeLookCameraPos = freeLook.CameraPosition;
            }

            if (oldCameraRotation != freeLook.CameraRotation)
            {
                freeLook.MoveCameraRotationByEditor(freeLook.CameraRotation);
                mainCamera.transform.localRotation = freeLook.CameraRotation;
                oldCameraRotation = freeLook.CameraRotation;
            }

            if (oldFOWFreeLook != freeLook.FieldOfView)
            {
                mainCamera.fieldOfView = freeLook.FieldOfView;
                oldFOWFreeLook = freeLook.FieldOfView;
            }

            mainCamera.nearClipPlane = freeLook.NearClippingPlanes;
            mainCamera.farClipPlane = freeLook.FarClippingPlanes;

            EditorGUILayout.LabelField("Camera Helper", EditorStyles.boldLabel);

            if (GUILayout.Button("Save Camera's Options"))
            {
                if (mainCamera)
                {
                    Transform cameraTransform = mainCamera.transform;
                    Vector3 cameraPosition = cameraTransform.localPosition;
                    Vector3 cameraRight = cameraPosition + freeLook.PositionOffsetRight;
                    Vector3 cameraLeft = cameraPosition - freeLook.PositionOffsetLeft;

                    freeLook.CameraPosition = cameraPosition;
                    freeLook.PositionOffsetRight = cameraRight;
                    freeLook.PositionOffsetLeft = cameraLeft;
                    mainCamera.fieldOfView = freeLook.FieldOfView;
                    mainCamera.nearClipPlane = freeLook.NearClippingPlanes;
                }
            }
        }

        private void OnEnable()
        {
            if (!freeLook)
            {
                freeLook = target as FreeLookCamera;
                oldFreeLookCameraPos = freeLook.CameraPosition;
                oldCameraRotation = freeLook.CameraRotation;
                oldFOWFreeLook = freeLook.FieldOfView;
            }
        }
    }
}
