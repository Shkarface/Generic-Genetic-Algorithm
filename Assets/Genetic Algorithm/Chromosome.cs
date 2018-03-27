using System;
using System.Collections;
using System.Collections.Generic;

namespace KurdifyEngine.GA
{
    public class Chromosome<T> : IComparable<Chromosome<T>>
    {
        /// <summary>
        /// The order-based gene array (Genome) of the chromosome
        /// </summary>
        public T[] Genome
        {
            get
            {
                return _Genome;
            }
            private set
            {
                if (_Genome != value)
                {
                    _Genome = value;
                    _Fitness = _CalculateFitness(Genome);
                }
            }
        }
        /// <summary>
        /// the length of the gene sequence
        /// </summary>
        public int GenomeLength
        {
            get
            {
                return Genome.Length;
            }
        }

        /// <summary>
        /// the fitness of the order of the genome
        /// </summary>
        public double Fitness
        {
            get
            {
                return _Fitness;
            }
        }

        private T[] _Genome;
        private double _Fitness;
        private System.Random _Random;
        private Func<T[], double> _CalculateFitness;
        private Func<T[], T[], T[]> _Crossover;

        public Chromosome(System.Random random, int genomeLength, Func<T[], double> CalculateFitness, Func<T[], T[], T[]> crossOver, Func<int, T[]> RandomizeGenome = null)
        {
            _Random = random;
            _Genome = new T[genomeLength];
            _CalculateFitness = CalculateFitness;
            _Crossover = crossOver;

            if (RandomizeGenome != null)
            {
                _Genome = RandomizeGenome(GenomeLength);
                _Fitness = _CalculateFitness(Genome);
            }
        }
        public Chromosome(string contentStr, char genomeDelimiter, Func<T[], double> CalculateFitness, Func<T[], T[], T[]> crossOver, Func<int, T[]> RandomizeGenome = null)
        {
            String[] content = contentStr.Split(genomeDelimiter);
            _Genome = new T[content.Length];
            _CalculateFitness = CalculateFitness;
            _Crossover = crossOver;
            for (int i = 0; i < _Genome.Length; i++)
            {
                _Genome[i] = content[i].ConvertValue<T>();
            }
            _Fitness = _CalculateFitness(Genome);
        }
        public Chromosome<T> Crossover(Chromosome<T> OtherParent)
        {
            Chromosome<T> child = new Chromosome<T>(_Random, GenomeLength, _CalculateFitness, _Crossover);
            child.Genome = _Crossover(this.Genome, OtherParent.Genome);

            return child;
        }

        public override string ToString()
        {
            return ToString(",");
        }
        public string ToString(string delimiter)
        {
            string s = "";
            for (int i = 0; i < GenomeLength; i++)
            {
                if (i == GenomeLength - 1)
                    s += Genome[i].ToString();
                else s += Genome[i].ToString() + delimiter;
            }
            return s;
        }
        int IComparable<Chromosome<T>>.CompareTo(Chromosome<T> other)
        {
            if (this.Genome.Length != other.Genome.Length)
                throw new InvalidOperationException("Can't compare chromosomes of different gene lengths");
            else
            {
                if (this.Fitness > other.Fitness)
                    return -1;
                else if (this.Fitness < other.Fitness)
                    return 1;
                else return 0;
            }
        }
    }
}