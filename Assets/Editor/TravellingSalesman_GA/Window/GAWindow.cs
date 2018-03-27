using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace KurdifyEditor.GA
{
    public partial class GAWindow : EditorWindow
    {

        private int _GenerationsToSimulate = 100;
        [UnityEditor.MenuItem("KurdifyEngine/Genetic Algorithms")]
        static void Init()
        {
            Window.titleContent = new GUIContent("GenAlgorithms");
            Window.minSize = new Vector2(450, 80);
            Window.Focus();
            Window.Show();
        }
        protected void OnGUI()
        {
            Event e = Event.current;
            if (e.type == EventType.KeyDown && !EditMenu.IsShown && !FileMenu.IsShown)
            {
                if (e.keyCode == KeyCode.DownArrow)
                    CurrentSelected++;
                else if (e.keyCode == KeyCode.UpArrow)
                    CurrentSelected--;

                Window.Repaint();
            }
            
            Grid.DrawBackground(new Rect(0, 0, position.width, position.height), BackgroundColor);

            NextY = 0;
            DrawToolbar();
            DrawStatusBar();

            if (Data != null)
            {
                DrawGenerationList();
                DrawInspector();
            }
            else
                GUI.Label(new Rect(0, 18, position.width, 30), "No Simulation Loaded", LabelMiddleCenter);

            if (FileMenu.IsShown)
            {
                FileMenu.Draw();
                FileMenu.ProcessEvents(e);
            }
            else if (EditMenu.IsShown)
            {
                EditMenu.Draw();
                EditMenu.ProcessEvents(e);
            }

            if (GUI.changed) Repaint();
        }
        protected void Refresh()
        {
            if (Data == null || Data.Generations.Count == 0) return;

            if (CurrentGeneration == null) return;
            CurrentGeneration.Chromosomes.Sort();
            if (CurrentGraphSorting == SortingType.WorstFirst)
                CurrentGeneration.Chromosomes.Reverse();
        }
        protected void DrawSolidColor(Rect rect, Color color)
        {
            Color lastColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
            GUI.color = lastColor;
        }
    }
}