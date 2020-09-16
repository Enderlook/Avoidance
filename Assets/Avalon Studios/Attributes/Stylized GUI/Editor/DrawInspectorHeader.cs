using UnityEngine;
using UnityEditor;

namespace AvalonStudios.Additions.Attributes.StylizedGUIs
{
    public partial class StylizedGUI
    {
        public static void DrawInspectorHeader(Rect rect, string banner, int fontSize = 0)
        {
            Rect headerFullRect = new Rect(rect.position.x, rect.position.y + 10, rect.width, rect.height);
            Rect headerBeginRect = new Rect(headerFullRect.position.x, headerFullRect.position.y, 10, 20);
            Rect headerMidRect = new Rect(headerFullRect.position.x + 10, headerFullRect.position.y, headerFullRect.xMax - 32, 20);
            Rect headerEndRect = new Rect(headerFullRect.xMax - 10, headerFullRect.position.y, 10, 20);
            Rect titleRect = new Rect(headerFullRect.position.x, headerFullRect.position.y, headerFullRect.width, 18);

            GUI.color = new Color(0.83f, 0.83f, 0.83f);

            EditorGUI.LabelField(headerBeginRect, GUIContent.none);
            EditorGUI.LabelField(headerMidRect, GUIContent.none);
            EditorGUI.LabelField(headerEndRect, GUIContent.none);

            GUI.color = Color.white;
            GUIStyle stylesConstants = GUIStylesConstants.TitleStyle(fontSize == 0 ? 0 : fontSize);
            GUI.Label(titleRect, banner, stylesConstants);
        }
    }
}
