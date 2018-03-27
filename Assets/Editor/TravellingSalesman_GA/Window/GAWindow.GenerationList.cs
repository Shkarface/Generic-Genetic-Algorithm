using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KurdifyEditor.GA
{
    public partial class GAWindow
    {

        static Rect ListBoxRect;
        static Vector2 ListBoxScrollPosition;
        static float ListBoxScrollHeight;
        protected void DrawGenerationList()
        {
            var ItemRect = new Rect(0, NextY, 200 - ((ListBoxRect.height <= ListBoxScrollHeight) ? 15f : 0f), 18f);
            ListBoxRect = new Rect(0f, 18, 200, position.height - 36);
            if (Data.Generations.Count > 0)
            {
                ListBoxScrollPosition = GUI.BeginScrollView(ListBoxRect, ListBoxScrollPosition, new Rect(ListBoxRect.x, ListBoxRect.y, 10, ListBoxScrollHeight));
                for (var index = 0; index < SortedGenerations.Count; index++)
                {
                    if (ListBoxScrollPosition.y < ItemRect.y + ItemRect.height && ListBoxScrollPosition.y + ListBoxRect.height > (ItemRect.y - 18))
                    {
                        if (CurrentSelected == SortedGenerations[index])
                        {
                            DrawSolidColor(ItemRect, (focusedWindow != Window) ? ItemSelectedNotFocused : ItemSelected);
                            EditorGUI.LabelField(new Rect(10f, ItemRect.y + 1, ItemRect.width - 5, ItemRect.height - 1), string.Format("Generation {0} \t| {1}", SortedGenerations[index] + 1, Data.Generations[SortedGenerations[index]].FitnessSum), EditorStyles.whiteLabel);
                        }
                        else
                        {
                            if (!FileMenu.IsShown && !EditMenu.IsShown && GUI.Button(ItemRect, "", GUIStyle.none))
                            {
                                CurrentSelected = SortedGenerations[index];
                            }
                            EditorGUI.LabelField(new Rect(10f, ItemRect.y + 1, ItemRect.width - 5, ItemRect.height - 1), string.Format("Generation {0} \t| {1}", SortedGenerations[index] + 1, Data.Generations[SortedGenerations[index]].FitnessSum), EditorStyles.label);
                        }
                    }
                    ItemRect.y += 18f;
                }
                ListBoxScrollHeight = (int)ItemRect.y - NextY + 5f;
                GUI.EndScrollView();
            }
            else
            {
                var rowRect = new Rect(ListBoxRect.x + 5, ListBoxRect.y + (ListBoxRect.height / 2) - 35, ListBoxRect.width - 10, 20);
                _GenerationsToSimulate = EditorGUI.IntSlider(rowRect, _GenerationsToSimulate, 1, 100000); rowRect.y += 25; rowRect.height = 30;
                if (GUI.Button(rowRect, string.Format("Simulate {0} Generation{1}", _GenerationsToSimulate, (_GenerationsToSimulate > 1) ? "s" : "")))
                {
                    Data.NextGenerations(_GenerationsToSimulate);
                    CurrentInspector = InspectorType.Graphs;
                    Refresh();
                }
            }
        }
    }
}