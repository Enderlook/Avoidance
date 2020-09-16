using AvalonStudios.Additions.Attributes.StylizedGUIs;

using UnityEngine;
using UnityEditor;

namespace AvalonStudios.Additions.Attributes
{
    [CustomPropertyDrawer(typeof(StyledHeader))]
    public class StyledHeaderPropertyDrawer : PropertyDrawer
    {
        private StyledHeader styledHeader;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            styledHeader = attribute as StyledHeader;

            GUI.enabled = true;

            StylizedGUI.DrawInspectorHeader(position, styledHeader.header, styledHeader.defaultFontSize ? 0 : styledHeader.fontSize);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 40;
    }
}
