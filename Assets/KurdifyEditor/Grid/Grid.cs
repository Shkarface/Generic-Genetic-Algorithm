using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KurdifyEditor
{
    public class Grid
    {
        public Rect Rect
        {
            get
            {
                return _Rect;
            }
        }
        public Color LineColor = Color.gray;
        public Color ThickLineColor = Color.black;

        private Rect _Rect;
        private bool _IsShown;


        public void Draw(Rect rect, int thickLine, int lineSpace)
        {
            _Rect = rect;
            #region Horizontal Lines
            int nextThickLine = thickLine - 1;
            for (int x = 0; x < ((Rect.height) / lineSpace); x++)
            {
                if (nextThickLine == x)
                {
                    nextThickLine += thickLine;
                    Handles.color = ThickLineColor;
                }
                else Handles.color = LineColor;
                Handles.DrawLine(new Vector3(Rect.x, Rect.y + (x * lineSpace)), new Vector3(Rect.width, Rect.y + (x * lineSpace)));
            }
            nextThickLine = thickLine - 1;
            #endregion
            #region Vertical Lines
            for (int x = 0; x < ((Rect.width) / lineSpace); x++)
            {
                if (nextThickLine == x)
                {
                    nextThickLine += thickLine;
                    Handles.color = ThickLineColor;
                }
                else Handles.color = LineColor;
                Handles.DrawLine(new Vector3(Rect.x + (x * lineSpace), Rect.y), new Vector3(Rect.x + (x * lineSpace), Rect.height));
            }
            #endregion
        }
        public static void DrawBackground(Rect rect, Color color)
        {
            Color lastColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
            GUI.color = lastColor;
        }
    }
}