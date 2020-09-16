using AvalonStudios.Additions.Components.Cameras;
using AvalonStudios.Additions.Components.RenderSystem;

using UnityEngine;
using UnityEditor;

public class CreateThirdPersonFollowCameraEditor
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
        camera.transform.localPosition = new Vector3(0, 1, -10);
        camera.transform.localRotation = Quaternion.Euler(Vector3.zero);
        camera.tag = TAG;
        Camera cameraComponent = camera.GetComponent<Camera>();
        cameraComponent.depth = -1;
        cameraComponent.fieldOfView = 70f;
        cameraComponent.nearClipPlane = .5f;
        cameraComponent.farClipPlane = 1000f;

        if (freeLook != null)
        {
            freeLook.PivotPosition = pivot.transform.localPosition;
            freeLook.CameraPosition = camera.transform.localPosition;
            freeLook.CameraRotation = camera.transform.localRotation.eulerAngles;
            freeLook.FieldOfView = cameraComponent.fieldOfView;
            freeLook.NearClippingPlanes = cameraComponent.nearClipPlane;
            freeLook.FarClippingPlanes = cameraComponent.farClipPlane;
        }
        else
            Debug.Log("Error Free Look Camera doesn't exist.");
    }
}
