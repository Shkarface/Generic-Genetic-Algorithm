using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KurdifyEditor.GA
{
    public partial class GAWindow
    {
        public static GUIStyle LabelMiddleLeft
        {
            get
            {
                if (_LabelMiddleLeft == null)
                {
                    _LabelMiddleLeft = new GUIStyle((EditorGUIUtility.isProSkin) ? EditorStyles.whiteLabel : EditorStyles.label);
                    _LabelMiddleLeft.alignment = TextAnchor.MiddleLeft;
                }
                return _LabelMiddleLeft;
            }
        }
        public static GUIStyle LabelMiddleCenter
        {
            get
            {
                if (_LabelMiddleCenter == null)
                {
                    _LabelMiddleCenter = new GUIStyle(EditorStyles.label);
                    _LabelMiddleCenter.alignment = TextAnchor.MiddleCenter;
                }
                return _LabelMiddleCenter;
            }
        }
        public static GUIStyle LabelMiddleRight
        {
            get
            {
                if (_LabelMiddleRight == null)
                {
                    _LabelMiddleRight = new GUIStyle(EditorStyles.label);
                    _LabelMiddleRight.alignment = TextAnchor.MiddleRight;
                }
                return _LabelMiddleRight;
            }
        }
        public static Texture2D DotCircle
        {
            get
            {
                if (_DotCircle == null)
                {
                    _DotCircle = new Texture2D(32,32);
                    for(int y = 0; y < 32; y++) 
                        for(int x = 0; x < 32; x++)
                        {
                            _DotCircle.SetPixel(x, y, Color.white);
                        }
                    _DotCircle.Apply();
                }
                return _DotCircle;
            }
        }

        internal static Texture2D _DotCircle;
        internal static GUIStyle _LabelMiddleLeft;
        internal static GUIStyle _LabelMiddleCenter;
        internal static GUIStyle _LabelMiddleRight;
    }
}