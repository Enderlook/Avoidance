using AvalonStudios.Additions.Extensions;

using UnityEngine;
using UnityEditor;

namespace AvalonStudios.Additions.Attributes
{
    [CustomPropertyDrawer(typeof(IsProperty))]
    public class IsPropertyDrawer : PropertyDrawer
    {
        private string name;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            name = property.displayName.RenameAutoProperty();
            if (name != "")
                EditorGUI.PropertyField(position, property, new GUIContent(name));
        }
    }
}
