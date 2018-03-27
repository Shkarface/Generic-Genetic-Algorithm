using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace KurdifyEngine.GA
{
    public class Generation<T> : IComparable<Generation<T>>
    {
        public List<Chromosome<T>> Chromosomes { get; private set; }
        public double FitnessSum { get; private set; }
        public int PopulationSize
        {
            get
            {
                return Chromosomes.Count;
            }
        }

        private System.Random _Random;
        private Func<int, T[]> _RandomizeGenome;
        private Func<T[], double> _CalculateFitness;
        /// <summary>
        /// Create a new generation of evolving chromosomes
        /// </summary>
        /// <param name="random">The random object that runs the simulation</param>
        /// <param name="LastGeneration">The parent generation if any, null if first generation</param>
        /// <param name="Elitism">The number of the fittest chromosomes that should stay alive.</param>
        /// <param name="PopulationSize">The population size of the current generation</param>
        public Generation(System.Random random, Generation<T> LastGeneration, int Elitism, int PopulationSize, int GeneSize, Func<T[], double> CalculateFitness, Func<T[], T[], T[]> Crossover, Func<int, T[]> RandomizeGenome = null, float mutationRate = 0.25f)
        {
            this._Random = random;
            Chromosomes = new List<Chromosome<T>>(PopulationSize);

            if (LastGeneration != null)
            {
                var sortedChromomsomes = new List<Chromosome<T>>(LastGeneration.Chromosomes);
                sortedChromomsomes.Sort();
                for (int index = 0; index < PopulationSize; index++)
                {
                    if (index < Elitism)
                    {
                        Chromosomes.Add(sortedChromomsomes[index]);
                    }
                    else
                    {
                        Chromosome<T> Parent1 = sortedChromomsomes[index];
                        Chromosome<T> Parent2 = null;
                        if (_Random.NextDouble() < mutationRate)
                            Parent2 = sortedChromomsomes[PopulationSize - 1 - index];
                        else
                            Parent2 = sortedChromomsomes[(index == PopulationSize - 1) ? _Random.Next(0, PopulationSize - 1) : index + 1];

                        var chr = Parent1.Crossover(Parent2);
                        Chromosomes.Add(chr);
                        FitnessSum += chr.Fitness;
                    }
                }
            }
            else
                for (int index = 0; index < PopulationSize; index++)
                {
                    {
                        var chr = new Chromosome<T>(_Random, GeneSize, CalculateFitness, Crossover, RandomizeGenome);
                        Chromosomes.Add(chr);
                        FitnessSum += chr.Fitness;
                    }
                }
        }
        public Generation(String contentStr, char chromosomeDelimiter, char genomeDelimiter, Func<T[], double> CalculateFitness, Func<T[], T[], T[]> Crossover, Func<int, T[]> RandomizeGenome = null)
        {
            String[] content = contentStr.Split(chromosomeDelimiter);
            Chromosomes = new List<Chromosome<T>>(content.Length);
            for (int i = 0; i < content.Length; i++)
            {
                var chr = new Chromosome<T>(content[i], genomeDelimiter, CalculateFitness, Crossover, RandomizeGenome);
                FitnessSum += chr.Fitness;
                Chromosomes.Add(chr);
            }
        }
        private Chromosome<T> ChooseFitParent(ref List<Chromosome<T>> sortedParents, float mutationRate)
        {
            int rand = 0;

            if (_Random.NextDouble() < mutationRate)
                rand = _Random.Next(PopulationSize / 2, PopulationSize - 1);
            else rand = _Random.Next(0, PopulationSize - 1);

            return Chromosomes[rand];
        }
        public override string ToString()
        {
            return ToString("|");
        }
        public string ToString(string delimiter)
        {
            string s = "";
            for (int i = 0; i < Chromosomes.Count; i++)
            {
                if (i == Chromosomes.Count - 1)
                    s += Chromosomes[i].ToString();
                else s += Chromosomes[i].ToString() + delimiter;
            }
            return s;
        }
        int IComparable<Generation<T>>.CompareTo(Generation<T> other)
        {
            if (this.FitnessSum > other.FitnessSum)
                return -1;
            else if (this.FitnessSum < other.FitnessSum)
                return 1;

            return 0;
        }
    }
}