using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KurdifyEditor.GA
{
    public partial class GAWindow
    {
        static Rect InspectorRect;
        static Vector2 InspectorScrollPosition;
        static float InspectorScrollHeight;
        private string[] options = new string[] { "Population", "Data" };
        protected void DrawInspector()
        {
            InspectorRect = new Rect(200, 18f, position.width - 200, position.height - 36);
            DrawSolidColor(new Rect(position.width - InspectorRect.width, NextY, 1f, position.height - 36), (EditorGUIUtility.isProSkin) ? BackgroundColor * 0.75f : new Color(0.1529411764705882f, 0.1529411764705882f, 0.1529411764705882f, 1f));

            if (CurrentGeneration == null)
            {
                EditorGUI.LabelField(new Rect(InspectorRect.x, 0, 60, 20), "Data", EditorStyles.toolbarButton);
                CurrentInspector = InspectorType.Data;
            }
            else
            {
                CurrentInspector = (InspectorType)EditorGUI.Popup(new Rect(InspectorRect.x, 0, 70, 20), (int)CurrentInspector, options, EditorStyles.toolbarPopup);
                if (CurrentInspector == InspectorType.Graphs)
                    CurrentGraphSorting = (SortingType)EditorGUI.Popup(new Rect(InspectorRect.x + 70, 0, 100, 20), (int)CurrentGraphSorting, GraphSortingOptions, EditorStyles.toolbarPopup);
            }

            NextY += 15;
            if (CurrentInspector == InspectorType.Data)
            {
                InspectorScrollPosition = GUI.BeginScrollView(InspectorRect, InspectorScrollPosition, new Rect(InspectorRect.x, InspectorRect.y, InspectorRect.width - ((InspectorScrollHeight > InspectorRect.height) ? 20 : 0), InspectorScrollHeight));
                float rightOffset = 10 + ((InspectorScrollHeight > InspectorRect.height) ? 20 : 0);
                Rect rowRect = new Rect(InspectorRect.x + 5, NextY, InspectorRect.width - rightOffset, 16);


                Data.Population = System.Math.Max(3, EditorGUI.IntField(rowRect, "Population", Data.Population)); rowRect.y += 20;
                Data.Elitism = EditorGUI.IntSlider(rowRect, "Elitism", Data.Elitism, 0, Data.Population - 2); rowRect.y += 25;
                Data.MutationRate = EditorGUI.Slider(rowRect, "Mutation Rate", Data.MutationRate, 0.01f, 1f); rowRect.y += 25;

                GUI.Label(new Rect(rowRect), "Points"); rowRect.y += 20;
                for (int index = 0; index < Data.Points.Length + ((Data.Generations.Count == 0) ? 1 : 0); index++)
                {
                    if (Data.Generations.Count == 0)
                    {
                        Rect elementRect = rowRect;
                        elementRect.width = 20;
                        if ((Data.Points.Length > 2 || (index == Data.Points.Length)) && GUI.Button(elementRect, (index == Data.Points.Length) ? "+" : "-"))
                        {
                            var pointsList = new List<Vector2>(Data.Points);
                            if (index == Data.Points.Length)
                                pointsList.Add(pointsList[pointsList.Count - 1]);
                            else pointsList.RemoveAt(index);
                            Data.Points = pointsList.ToArray();

                        }
                        rowRect.x += 22;
                    }
                    EditorGUI.BeginDisabledGroup((index == Data.Points.Length));
                    EditorGUI.BeginChangeCheck();
                    Rect rect = new Rect(rowRect.x, rowRect.y, rowRect.width - 20, rowRect.height);
                    var v = EditorGUI.Vector2Field(rect, "", (index == Data.Points.Length) ? Data.Points[index - 1] : Data.Points[index]);

                    if (Data.Generations.Count == 0)
                        rowRect.x -= 22;
                    if (index < Data.Points.Length)
                        Data.Points[index] = v;
                    if (EditorGUI.EndChangeCheck())
                        Data.Validate();
                    EditorGUI.EndDisabledGroup();
                    rowRect.y += 23;
                }
                rowRect.y += 2;
                GUI.Label(new Rect(rowRect), "Graph"); rowRect.y += 20;
                NextY = rowRect.y;
                float graphWidth = InspectorRect.width - rightOffset;
                float graphHeight = graphWidth * 0.75f;
                var r = new Rect(rowRect.x, NextY, graphWidth, graphHeight); NextY += r.height;
                GUI.Box(r, "");
                Drawing.DrawLine(r.position, new Vector2(r.x, r.y + r.height), Color.black, 2, false);
                Drawing.DrawLine(new Vector2(r.x, r.y + r.height), new Vector2(r.x + r.width, r.y + r.height), Color.black, 2, false);

                Texture2D circle = DotCircle;
                for (int index = 0; index < Data.Points.Length; index++)
                {
                    Vector2 currentPoint = Data.Points[index];

                    var pointPos = new Vector2
                        (
                            (r.x - 2) + ((currentPoint.x / Data.MaxPoint.x) * (graphWidth * 0.9f)),
                            (r.y + r.height - 2) - ((currentPoint.y / Data.MaxPoint.y) * (graphHeight * 0.9f))
                        );

                    float pointSize = 4.0F * (graphWidth / 200f);
                    float halfPoint = pointSize / 2f;
                    if (circle)
                    {
                        var oldColor = GUI.color;
                        if (!EditorGUIUtility.isProSkin)
                            GUI.color = Color.black;
                        GUI.DrawTexture(new Rect(pointPos.x, pointPos.y, pointSize, pointSize), circle);
                        GUI.color = oldColor;
                    }
                    else GUI.Box(new Rect(pointPos.x - halfPoint, pointPos.y - halfPoint, pointSize, pointSize), "");

                    GUI.Label(new Rect(pointPos.x - 50, pointPos.y - 20, 100, 20), $"#{index} {currentPoint}", LabelMiddleCenter);
                }


                InspectorScrollHeight = NextY;
                GUI.EndScrollView();
            }
            else
            {
                int graphsPerLine = Mathf.Min(Mathf.FloorToInt((InspectorRect.width - 40f) / (GridWidth + 20)), CurrentGeneration.Chromosomes.Count);
                if (graphsPerLine < 1) graphsPerLine = 1;
                int lineCount = Mathf.CeilToInt((float)CurrentGeneration.Chromosomes.Count / (float)graphsPerLine);
                InspectorScrollHeight = 20 + (lineCount * (GridHeight + 20));



                InspectorScrollPosition = GUI.BeginScrollView(InspectorRect, InspectorScrollPosition, new Rect(InspectorRect.x, InspectorRect.y, GridWidth * graphsPerLine + 40, InspectorScrollHeight));
                if (InspectorRect.height <= InspectorScrollHeight)
                    InspectorRect.width -= 15;
                InspectorRect.width -= 15;


                int count = 0;
                for (int lineIndex = 0; lineIndex < lineCount; lineIndex++)
                {
                    for (int graphIndex = 0; graphIndex < graphsPerLine && count < CurrentGeneration.Chromosomes.Count; graphIndex++)
                    {

                        if (InspectorScrollPosition.y < NextY + GridHeight && InspectorScrollPosition.y + InspectorRect.height > NextY)
                        {
                            var r = new Rect(InspectorRect.x + 20 + (graphIndex * (GridWidth + 30)), NextY, GridWidth, GridHeight);
                            GUI.Box(r, "");
                            Drawing.DrawLine(r.position, new Vector2(r.x, r.y + r.height), Color.black, 2, false);
                            Drawing.DrawLine(new Vector2(r.x, r.y + r.height), new Vector2(r.x + r.width, r.y + r.height), Color.black, 2, false);

                            Texture2D circle = DotCircle;
                            for (int index = 0; index < Data.Points.Length; index++)
                            {
                                Vector2 currentPoint = Data.Points[CurrentGeneration.Chromosomes[count].Genome[index]];
                                Vector2 nextPoint = Data.Points[CurrentGeneration.Chromosomes[count].Genome[(index == Data.Points.Length - 1) ? 0 : index + 1]];

                                var pointPos = new Vector2
                                    (
                                        (r.x - 2) + ((currentPoint.x / Data.MaxPoint.x) * (GridWidth * 0.9f)),
                                        (r.y + r.height - 2) - ((currentPoint.y / Data.MaxPoint.y) * (GridHeight * 0.9f))
                                    );

                                var nextPointPos = new Vector2
                                    (
                                        (r.x - 2) + ((nextPoint.x / Data.MaxPoint.x) * (GridWidth * 0.9f)),
                                        (r.y + r.height - 2) - ((nextPoint.y / Data.MaxPoint.y) * (GridHeight * 0.9f))
                                    );

                                Vector2 lineStartPos = pointPos + (Vector2.one * 2);
                                Vector2 lineEndPos = nextPointPos + (Vector2.one * 2);

                                Drawing.DrawLine(lineStartPos, lineEndPos, Color.gray, 1, true);

                                float pointSize = Mathf.Max(4f, 5f * (GridWidth / 210f));
                                float halfPoint = pointSize / 2f;
                                if (circle)
                                {
                                    var oldColor = GUI.color;
                                    if (!EditorGUIUtility.isProSkin)
                                        GUI.color = Color.black;
                                    GUI.DrawTexture(new Rect(pointPos.x - halfPoint, pointPos.y - halfPoint, pointSize, pointSize), circle);
                                    GUI.color = oldColor;
                                }
                                else GUI.Box(new Rect(pointPos.x - halfPoint, pointPos.y - halfPoint, pointSize, pointSize), "");
                            }

                            GUI.Label(new Rect(r.x + 5, r.y + 5, 70, 50), string.Format("#{0}\n{1:0.0}m", count + 1, -CurrentGeneration.Chromosomes[count].Fitness));
                        }
                        count++;
                    }
                    NextY += GridHeight + 20;
                }

                GUI.EndScrollView();
            }
        }
    }
}