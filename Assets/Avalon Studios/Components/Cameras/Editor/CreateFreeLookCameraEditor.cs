using AvalonStudios.Additions.Components.Cameras;
using AvalonStudios.Additions.Components.RenderSystem;

using UnityEngine;
using UnityEditor;

public class CreateFreeLookCameraEditor
{
    private const string TAG = "MainCamera";

    [MenuItem("Avalon Studios/Cameras/Free Look Camera")]
    public static void Create()
    {
        GameObject parent = new GameObject("Free Look Camera", typeof(FreeLookCamera));
        GameObject pivot = new GameObject("Pivot");
        GameObject camera = new GameObject("Camera", typeof(Camera), typeof(AudioListener), typeof(Render));

        parent.transform.position = Vector3.zero;
        FreeLookCamera freeLook = parent.GetComponent<FreeLookCamera>();

        pivot.transform.SetParent(parent.transform);
        pivot.transform.localPosition = new Vector3(0, 1, 0);

        camera.transform.SetParent(pivot.transform);
        camera.transform.localPosition = freeLook.CameraPosition;
        camera.transform.localRotation = Quaternion.Euler(freeLook.CameraRotation);
        camera.tag = TAG;
        Camera cameraComponent = camera.GetComponent<Camera>();
        cameraComponent.clearFlags = freeLook.ClearFlags;
        cameraComponent.backgroundColor = freeLook.BackgroundColor;
        cameraComponent.cullingMask = freeLook.CullingMask;
        cameraComponent.orthographic = freeLook.Orthographic;
        cameraComponent.fieldOfView = freeLook.FieldOfView;
        cameraComponent.nearClipPlane = freeLook.NearClippingPlanes;
        cameraComponent.farClipPlane = freeLook.FarClippingPlanes;
        cameraComponent.depth = freeLook.Depth;
        cameraComponent.renderingPath = freeLook.RenderingPath;
        cameraComponent.useOcclusionCulling = freeLook.OcclusionCulling;
        cameraComponent.allowHDR = freeLook.AllowHDR;
        cameraComponent.allowMSAA = freeLook.AllowMSAA;
        cameraComponent.allowDynamicResolution = freeLook.AllowDynamicResolution;
        cameraComponent.targetDisplay = (int)freeLook.TargetDisplay;
    }
}
