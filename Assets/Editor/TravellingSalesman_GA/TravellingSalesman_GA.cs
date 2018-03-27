using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KurdifyEngine.GA
{
    public class TravellingSalesman_GA
    {
        /// <summary>
        /// the point farthest from the origin
        /// </summary>
        public Vector2 MaxPoint { get; private set; }

        private Thread IOThread;
        public int RandomSeed;
        public int Population = 100;
        public int Elitism = 5;
        public float MutationRate = .1f;
        public Vector2[] Points = new Vector2[3] { Vector2.zero, Vector2.one, new Vector2(0, 1) };
        public List<Generation<int>> Generations
        {
            get
            {
                if (_Generations == null) _Generations = new List<Generation<int>>();
                return _Generations;
            }
        }
        public System.Random Random
        {
            get
            {
                if (_Random == null) _Random = new System.Random(RandomSeed);
                return _Random;
            }
        }

        private List<Generation<int>> _Generations;
        private System.Random _Random;

        public void NextGenerations(int count)
        {
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
                Generations.Add(new Generation<int>(Random, (Generations.Count > 0) ? Generations[Generations.Count - 1] : null, Elitism, Population, Points.Length, CalculateFitness, SequenceCrossover, GetRandomGenes, MutationRate));
            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds > 1000)
                Debug.Log("it took " + (stopwatch.ElapsedMilliseconds / 1000.0) + " seconds to generate " + count + " generations");
            else
                Debug.Log("it took " + stopwatch.ElapsedMilliseconds + " milliseconds to generate " + count + " generations");
        }

        internal void Save(string filepath)
        {
            String filepath_ga = filepath + ".ga";
            StringBuilder content = new StringBuilder(100);
            content.AppendLine($"{RandomSeed}|{Population}|{Elitism}|{MutationRate}");
            for (int i = 0; i < Points.Length; i++)
            {
                if (i == Points.Length - 1)
                    if (Generations.Count > 0)
                        content.Append($"{Points[i].x},{Points[i].y}{Environment.NewLine}");
                    else
                        content.Append($"{Points[i].x},{Points[i].y}");
                else
                    content.Append($"{Points[i].x},{Points[i].y}|");
            }
            for (int i = 0; i < Generations.Count; i++)
            {
                if (i == Generations.Count - 1)
                    content.Append(Generations[i].ToString());
                else
                    content.AppendLine(Generations[i].ToString());
            }

            File.WriteAllText(filepath_ga, content.ToString());
        }

        public Generation<int> NextGeneration()
        {
            Generations.Add(new Generation<int>(Random, (Generations.Count > 0) ? Generations[Generations.Count - 1] : null, Elitism, Population, Points.Length, CalculateFitness, SequenceCrossover, GetRandomGenes, MutationRate));

            return Generations[Generations.Count - 1];
        }

        private int[] SequenceCrossover(int[] parent1, int[] parent2)
        {
            int[] newGenes = new int[parent1.Length];
            List<int> parent2List = new List<int>(parent2);
            int sequenceLength = (int)Mathf.Floor(parent1.Length / 2);
            int sequenceStartIndex = _Random.Next(0, parent1.Length - sequenceLength + 1);

            for (int i = sequenceStartIndex; i < sequenceStartIndex + sequenceLength; i++)
            {
                newGenes[i] = parent1[i];
                if (!parent2List.Remove(newGenes[i]))
                    Debug.Log(string.Format("cant find {0} in parent 2", newGenes[i]));
            }

            int x = 0;
            for (int i = 0; i < newGenes.Length; i++)
            {
                if (i < sequenceStartIndex || i > sequenceStartIndex + sequenceLength - 1)
                {
                    newGenes[i] = parent2List[x];
                    x++;
                }
            }
            #region Debug
            //string geneSeq = "[";
            //Debug.Log(string.Format("Sequence is {0} genes long and starts at {1}", sequenceLength, sequenceStartIndex));
            //geneSeq = "Parent 1: ";
            //for (x = 0; x < newGenes.Length; x++)
            //{
            //    if (x > sequenceStartIndex && x < sequenceStartIndex + sequenceLength + 1)
            //        geneSeq += newGenes[x] + " ";
            //}
            //Debug.Log(geneSeq);
            //geneSeq = "Parent 2: [";
            //for (x = 0; x < parent2List.Count; x++)
            //{
            //    geneSeq += parent2List[x] + ((x == parent2List.Count - 1) ? "]" : ", ");
            //}
            //Debug.Log(geneSeq);
            //geneSeq = "Child: [";
            //for (x = 0; x < newGenes.Length; x++)
            //{
            //    geneSeq += newGenes[x] + ((x == newGenes.Length - 1) ? "]" : ", ");
            //}
            //Debug.Log(geneSeq);
            if (newGenes == parent1 || newGenes == parent2)
                Debug.Log("Child is the same as one or both of it's parents");
            #endregion
            return newGenes;
        }

        private int[] GetRandomGenes(int size)
        {
            int[] genes = new int[size];
            List<int> indices = new List<int>(Enumerable.Range(0, size));
            for (int index = 0; index < size; index++)
            {
                int i = 0;
                if (indices.Count > 0)
                    i = _Random.Next(0, indices.Count - 1);
                genes[index] = indices[i];
                indices.RemoveAt(i);
            }

            return genes;
        }

        private double CalculateFitness(int[] genes)
        {
            //string s = "[";
            double dist = 0f;
            for (int index = 0; index < genes.Length; index++)
            {
                //s += $"{genes[index]}->";
                //if (index == genes.Length - 1) s += genes[0];
                Vector2 nextPoint = (index == genes.Length - 1) ? Points[genes[0]] : Points[genes[index + 1]];
                double newDist = Vector2.Distance(Points[genes[index]], nextPoint);
                //Debug.Log($"Distance from {Points[genes[index]]} to {nextPoint} is {newDist}");
                dist += newDist;
            }
            if (dist < 0)
                throw new System.Exception("Distance cant be less than or equal to zero!");
            //s += "] distance is " + dist;
            //Debug.Log(s);
            return -dist;
        }
        public static TravellingSalesman_GA Load(string filepath)
        {
            var result = new TravellingSalesman_GA();

            String[] content = File.ReadAllLines(filepath);
            String[] attributes = content[0].Split('|');
            String[] points = content[1].Split('|');

            result.RandomSeed = Convert.ToInt32(attributes[0]);
            result.Population = Convert.ToInt32(attributes[1]);
            result.Elitism = Convert.ToInt32(attributes[2]);
            result.MutationRate = Convert.ToSingle(attributes[3]);

            result.Points = new Vector2[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                string[] vectorS = points[i].Split(',');
                float x = Convert.ToSingle(vectorS[0]);
                float y = Convert.ToSingle(vectorS[1]);
                result.Points[i] = new Vector2(x, y);
            }
            for (int i = 2; i < content.Length; i++)
            {
                result.Generations.Add(new Generation<int>(content[i], '|', ',', result.CalculateFitness, result.SequenceCrossover, null));
            }

            result.Validate();
            return result;
        }
        public void Validate()
        {
            MaxPoint = Vector2.zero;
            if (Points.Length < 2)
            {
                Points = new Vector2[2];
                Points[1] = Vector2.one;
            }
            int xMin = 0, yMin = 0;
            int index = 0;
            for (index = 0; index < Points.Length; index++)
            {
                xMin = Mathf.Min(xMin, (int)Points[index].x);
                yMin = Mathf.Min(yMin, (int)Points[index].y);
            }
            for (index = 0; index < Points.Length; index++)
            {
                Points[index] += new Vector2(-xMin, -yMin);
                MaxPoint = new Vector2(Mathf.Max(MaxPoint.x, Points[index].x), Mathf.Max(MaxPoint.y, Points[index].y));
            }

            Population = Math.Max(Population, 3);
            if (((Population) % 2) == 1)
                Population++;
        }
    }
}