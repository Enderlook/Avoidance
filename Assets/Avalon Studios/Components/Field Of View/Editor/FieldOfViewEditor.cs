using UnityEngine;
using UnityEditor;

namespace AvalonStudios.Additions.Components.FieldOfView
{
    [CustomEditor(typeof(FieldOfView))]
    public class FieldOfViewEditor : Editor
    {
        private FieldOfView fieldOfView;
        private Transform fowTransform;

        private void OnSceneGUI()
        {
            fieldOfView = target as FieldOfView;

            fowTransform = fieldOfView.transform;

            Handles.color = fieldOfView.WireArcColor;
            Handles.DrawWireArc(fowTransform.position, Vector3.up, Vector3.forward, 360, fieldOfView.ViewRadius);

            Vector3 viewAngleA = fieldOfView.DirFromAngle(-fieldOfView.ViewAngle / 2, false);
            Vector3 viewAngleB = fieldOfView.DirFromAngle(fieldOfView.ViewAngle / 2, false);

            Handles.color = fieldOfView.LineArcColor;
            Handles.DrawLine(fowTransform.position, fowTransform.position + viewAngleA * fieldOfView.ViewRadius);
            Handles.DrawLine(fowTransform.position, fowTransform.position + viewAngleB * fieldOfView.ViewRadius);

            Handles.color = fieldOfView.DetectorLineColor;
            foreach(Transform visibleTarget in fieldOfView.GetVisibleTargets)
                Handles.DrawLine(fowTransform.position, visibleTarget.position);
        }
    }
}
