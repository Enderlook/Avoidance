using UnityEngine;

namespace AvalonStudios.Additions.Attributes.StylizedGUIs
{
    public static class GUIStylesConstants
    {
        public static GUIStyle TitleStyle(int fontSize = 0)
        {
            GUIStyle guiStyle = new GUIStyle();

            guiStyle.richText = true;
            guiStyle.alignment = TextAnchor.MiddleCenter;
            guiStyle.fontStyle = FontStyle.Bold;

            if (fontSize != 0)
                guiStyle.fontSize = fontSize;

            return guiStyle;
        }
    }
}
