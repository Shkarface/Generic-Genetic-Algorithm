using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KurdifyEditor
{
    public class Menu
    {
        public bool IsShown
        {
            get
            {
                return _IsShown;
            }
        }
        public bool Enabled
        {
            get
            {
                return _Enabled;
            }
            set
            {
                _Enabled = value;
            }
        }
        public float Width
        {
            get
            {
                return _Width;
            }
            set
            {
                _Width = value;
            }
        }
        public MenuItem[] Items;
        public GUIStyle ButtonStyle;
        public bool BackgroundBox;
        public Action<MenuItem> OnItemClicked;


        private bool _IsShown;
        private bool _Enabled = true;
        private Rect _Rect;
        private float _Width = 100;

        public void Show(int x, int y)
        {
            Show(new Vector2(x, y));
        }
        public void Show(Vector2 pos)
        {
            _Rect = new Rect(pos, new Vector2(Width, Items.Length * 18));
            _IsShown = true;
        }
        public void Draw()
        {
            if (!_IsShown) return;
            UnityEditor.EditorGUI.BeginDisabledGroup(!Enabled);
            Rect r = _Rect;
            if (BackgroundBox) GUI.Box(r, "");
            r.height = 18;

            for (int index = 0; index < Items.Length; index++)
            {
                if (ButtonStyle != null)
                {
                    if (GUI.Button(r, Items[index].Name, ButtonStyle))
                        ItemClicked(Items[index]);
                }
                else if (GUI.Button(r, Items[index].Name))
                    ItemClicked(Items[index]);

                r.y += r.height;
            }
            UnityEditor.EditorGUI.EndDisabledGroup();
        }
        public void ProcessEvents(Event e)
        {
            if (e.isMouse)
            {
                bool inRect = _Rect.Contains(e.mousePosition);
                if (e.button == 0 && inRect)
                {
                    _IsShown = true;
                    GUI.changed = true;
                }
                else if (!inRect)
                {
                    _IsShown = false;
                    GUI.changed = true;
                }
            }
        }

        private void ItemClicked(MenuItem item)
        {
            if (OnItemClicked != null)
                OnItemClicked(item);

            GUI.changed = true;
            _IsShown = false;
        }
    }
}