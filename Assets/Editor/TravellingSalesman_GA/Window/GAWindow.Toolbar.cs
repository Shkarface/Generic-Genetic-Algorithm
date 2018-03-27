using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using KurdifyEngine.GA;

namespace KurdifyEditor.GA
{
    public partial class GAWindow
    {
        internal void DrawToolbar()
        {
            EditorGUI.LabelField(new Rect(0, NextY, position.width, 18), "", EditorStyles.toolbar);
            if (GUI.Button(new Rect(0, 0, 40, 18), "File", EditorStyles.toolbarButton))
            {
                FileMenu.Show(0, 18);
            }
            else if (GUI.Button(new Rect(40, 0, 40, 18), "Edit", EditorStyles.toolbarButton))
            {
                if (Data == null) EditMenu.Enabled = false;
                else EditMenu.Enabled = true;
                EditMenu.Show(40, 18);
            }
            if (Data != null)
                CurrentGenerationsSorting = (SortingType)EditorGUI.Popup(new Rect(100, 0, 100, 20), (int)CurrentGenerationsSorting, GenerationsSortingOptions, EditorStyles.toolbarPopup);


            if (Data != null)
            {
                if (CurrentInspector == 0)
                    GridSize = EditorGUI.Slider(new Rect(position.width - 205, 0, 200, 18), GridSize, 0.5f, 10f);
            }

            NextY += 18;
        }

        public void DrawStatusBar()
        {
            EditorGUI.LabelField(new Rect(0, position.height - 18, position.width, 18), "", EditorStyles.toolbar);
            if (Data != null && Data.Points.Length > 0 && Data.Generations.Count > 0)
            {
                GUI.Label(new Rect(5, position.height - 18, 500, 18), string.Format("{0} Gene{1}/Chromosome | {2} Chromosomes/Generation  |  {3} Generation{4}", Data.Points.Length, (Data.Points.Length > 1) ? "s" : "", Data.Population, Data.Generations.Count, (Data.Generations.Count > 1) ? "s" : "", InspectorRect.width), LabelMiddleLeft);
            }
            GUI.Label(new Rect(position.width - 305, position.height - 18, 300, 18), InspectorRect.ToString(), LabelMiddleRight);
        }

        private void SaveSimulation()
        {
            if (Data != null)
            {
                Data.Save(System.IO.Path.Combine( System.IO.Path.GetDirectoryName(CurrentLoadedSimulationFilepath), System.IO.Path.GetFileNameWithoutExtension(CurrentLoadedSimulationFilepath)));
            }
        }
        private void LoadSimulation()
        {
            string file = EditorUtility.OpenFilePanel("Load Simulation", "", "ga");
            if (!string.IsNullOrEmpty(file))
            {
                CurrentLoadedSimulationFilepath = file;
                file = FileUtil.GetProjectRelativePath(file);
                Data = TravellingSalesman_GA.Load(file);
                Refresh();
            }
        }

        protected void FileMenuItemClicked(MenuItem obj)
        {
            switch (obj.ID)
            {
                case 1: LoadSimulation(); break;
                case 2: SaveSimulation(); break;
                case 3: Data = null; break;
            }
        }
        protected void EditMenuItemClicked(MenuItem obj)
        {
            switch (obj.ID)
            {
                case 0: Data.NextGeneration(); break;
                case 1:
                    {
                        Data.NextGenerations(5);
                        break;
                    }
                case 2:
                    {
                        Data.NextGenerations(10);
                        break;
                    }
                case 3:
                    {
                        Data.NextGenerations(50);
                        break;
                    }
                case 4:
                    {
                        Data.NextGenerations(100);
                        break;
                    }
                case 5:
                    {
                        Data.NextGenerations(5000);
                        break;
                    }
                case 6: Data.Generations.Clear(); break;
            }
            Refresh();
        }
    }
}