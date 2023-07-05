

using System;
using System.Collections.Generic;
using System.Net;
using static System.Formats.Asn1.AsnWriter;

class Genetikalgoritma
{
    class Chromosome
    {

        public int Id { get; private set; }
        public char[] Genes { get; private set; }
        public int fitness { get; private set; }
        public char[] target { get; private set; }
        public static int generation;
        public static double generation_mean;

        private static readonly char[] GenesArray = new char[] {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
        'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
        'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', ',', '.', '-',
        ';', ':', '_', '!', '"', '#', '%', '&', '/', '(', ')', '=', '?',
        '@', '$', '{', '[', ']', '}', ' '

        };
        private static Random rnd = new Random();

        public Chromosome(char[] target)
        {
            this.Id = Id + 1;
            this.Genes = new char[target.Length];
            this.target = target;
            for (int i = 0; i < Genes.Length; i++)
            {
                Genes[i] = GenesArray[rnd.Next(0, GenesArray.Length)];
            }
            this.fitness = fitness_calculate(Genes, target);
        }
        public Chromosome(char[] gene, char[] target)
        {
            this.Id = Id + 1;
            this.Genes = gene;
            this.target = target;
            this.fitness = fitness_calculate(Genes, target);
        }

        public static void Mutation(Chromosome chromosome, double mutationRate)
        {
            for (int i = 0; i < chromosome.Genes.Length; i++)
            {
                if (rnd.NextDouble() < mutationRate)
                {
                    chromosome.Genes[i] = GenesArray[rnd.Next(0, GenesArray.Length)];
                }
            }
            chromosome.fitness = chromosome.fitness_calculate(chromosome.Genes, chromosome.target);
        }

        public static (Chromosome child1, Chromosome child2) Crossover(Chromosome parent1, Chromosome parent2)
        {
            // Choose a random crossover point
            int crossoverPoint = rnd.Next(0, parent1.Genes.Length);

            // Create the child chromosomes by combining the genes of the parents
            char[] child1Genes = new char[parent1.Genes.Length];
            char[] child2Genes = new char[parent1.Genes.Length];
            for (int i = 0; i < parent1.Genes.Length; i++)
            {
                if (i < crossoverPoint)
                {
                    child1Genes[i] = parent1.Genes[i];
                    child2Genes[i] = parent2.Genes[i];
                }
                else
                {
                    child1Genes[i] = parent2.Genes[i];
                    child2Genes[i] = parent1.Genes[i];
                }
            }

            // Create the child chromosomes and calculate their fitness values
            Chromosome child1 = new Chromosome(child1Genes, parent1.target);
            Chromosome child2 = new Chromosome(child2Genes, parent1.target);

            return (child1, child2);
        }
        public void display()
        {
            Console.WriteLine(new string(Genes) + " " + " Fitness value: " + fitness);
        }

        public int fitness_calculate(char[] genes, char[] target)
        {
            fitness = 0;
            for (int j = 0; j < target.Length; j++)
            {
                if (Genes[j].CompareTo(target[j]) == 0)
                {
                    fitness++;
                }
            }
            return fitness;
        }

    }

    static List<Chromosome> selection(List<Chromosome> population)
    {
        population.Sort((p1, p2) => p2.fitness.CompareTo(p1.fitness));

        List<Chromosome> result = new List<Chromosome>();
        for (int i = 0; i < population.Count / 2; i++)
        {
            result.Add(population[i]);
        }

        return result;
    }

    static List<Chromosome> crossing(List<Chromosome> population, double mutationRate)
    {
        List<Chromosome> elected_population = new List<Chromosome>();
        elected_population = selection(population);
        Random rnd = new Random();
        for (int i = 0; i < population.Count / 4; i++)
        {
            int random1 = rnd.Next(elected_population.Count);
            int random2 = rnd.Next(elected_population.Count);
            (Chromosome child1, Chromosome child2) = Chromosome.Crossover(population[random1], population[random2]);
            Chromosome.Mutation(child1, mutationRate);
            Chromosome.Mutation(child2, mutationRate);
            elected_population.Add(child1);
            elected_population.Add(child2);

        }
        Chromosome.generation++;
        elected_population.Sort((p1, p2) => p2.fitness.CompareTo(p1.fitness));
        Chromosome.generation_mean = 0;
        for (int i = 0; i < elected_population.Count; i++)
        {
            Chromosome.generation_mean += elected_population[i].fitness;
        }
        Chromosome.generation_mean = Chromosome.generation_mean / elected_population.Count;
        Console.WriteLine(Chromosome.generation + ". nesil. Çözüme en yakın sonuç: " + new string(elected_population[0].Genes)
        + " | Uyumlu harf sayısı: " + elected_population[0].fitness + " | Nesil başarı ortalaması: " + Chromosome.generation_mean);
        return elected_population;
    }


    static void Main(string[] args)
    {
        DateTime startTime = DateTime.Now;
        DateTime endTime;
        Chromosome.generation = 0;
        int population_limit = 100;
        string target_str = "ChatGpt";
        char[] target = target_str.ToCharArray();
        List<Chromosome> numbers = new List<Chromosome>();
        for (int i = 0; i < population_limit; i++)
        {
            numbers.Add(new Chromosome(target));
        }
        bool switcher = true;
        while (switcher)
        {
            numbers = crossing(numbers, 0.035);

            for (int i = 0; i < numbers.Count; i++)
            {
                if (numbers[i].fitness == target_str.Length)
                {
                    Console.WriteLine("---------------------------------");
                    Console.WriteLine("Şifre Çözüldü");
                    Console.WriteLine("---------------------------------");
                    Console.WriteLine(Chromosome.generation + ". nesilin en iyi çözümü: " + new string(numbers[i].Genes)
                    + " | Uyumlu harf sayısı: " + numbers[i].fitness + " | Nesil başarı ortalaması: " + Chromosome.generation_mean);
                    endTime = DateTime.Now;
                    TimeSpan timeDifference = endTime - startTime;
                    double milliseconds = timeDifference.TotalMilliseconds;
                    Console.WriteLine("Kodun çözülme süresi: {0} milisaniye", (int)(milliseconds));
                    switcher = false;

                }
            }
        }
    }
}

