using UnityEngine;

namespace AvalonStudios.Additions.Components.MinimapSystem
{
    public static class MinimapUtils
    {
        public static Vector3 CalculatePosition(Vector3 viewPoint, RectTransform maxAnchor)
        {
            viewPoint = new Vector2((viewPoint.x * maxAnchor.sizeDelta.x) - (maxAnchor.sizeDelta.x * 0.5f), (viewPoint.y * maxAnchor.sizeDelta.y) - (maxAnchor.sizeDelta.y * 0.5f));
            return viewPoint;
        }
    }
}
