using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GridSystem
{
    public static class WorldText 
    {
        public const int sortingOrderDefault = 5000;

        private static readonly string blackColor = "#1F1F1F";
        private static Color defaultColor;

        public static TextMeshPro CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAlignmentOptions textAlignment = TextAlignmentOptions.Left, bool autoSize = false, float boxWidth = 20, float boxHeight = 5, int sortingOrder = sortingOrderDefault) {
            if (color == null || color == Color.black) ColorUtility.TryParseHtmlString(blackColor, out defaultColor); else defaultColor = (Color)color;
            return CreateWorldText(parent, text, localPosition, fontSize, defaultColor, textAlignment, autoSize, boxWidth, boxHeight, sortingOrder);
        }
        
        // Create Text in the World
        public static TextMeshPro CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAlignmentOptions textAlignment, bool autoSize, float boxWidth, float boxHeight, int sortingOrder) {
            GameObject gameObject = new GameObject("World_Text", typeof(TextMeshPro));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            ResizeBox(gameObject.GetComponent<RectTransform>(), new Vector2(boxWidth, boxHeight));
            TextMeshPro textMeshPro = gameObject.GetComponent<TextMeshPro>();
            textMeshPro.enableAutoSizing = autoSize;
            textMeshPro.alignment = textAlignment;
            textMeshPro.text = text;
            textMeshPro.fontSizeMax = fontSize;
            textMeshPro.fontSizeMin = fontSize / 5;
            float margin = (float)fontSize / 100;
            textMeshPro.margin = new Vector4(margin, margin, margin, margin);
            textMeshPro.fontSize = fontSize;
            textMeshPro.color = color;
            textMeshPro.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return textMeshPro;
        }

        private static void ResizeBox(RectTransform rectTransform, Vector2 size)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            rectTransform.ForceUpdateRectTransforms();
        }
    }
}