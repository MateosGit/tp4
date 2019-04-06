using System;
using System.Threading;

namespace SecretGarden
{
    internal class Program
    {
        public static void Run(int width, int height, int nbSheep, int nbWolf, int nbGrass, int steps)
        {

            Terrain terrain = new Terrain(width, height);
            Random rng = new Random();
            
            for (int i = 0; i < nbGrass; i++)
            {
                terrain.AddPlant(new Grass(), rng.Next(terrain.w), rng.Next(terrain.h));
            }

            for (int i = 0; i < nbSheep; i++)
            {
                terrain.AddAnimal(new Sheep(), rng.Next(terrain.w), rng.Next(terrain.h));
            }

            for (int i = 0; i < nbWolf; i++)
            {
                terrain.AddAnimal(new Wolf(), rng.Next(terrain.w), rng.Next(terrain.h));
            }

            while (steps > 0)
            {
                terrain.Update();
                terrain.Print();
                Thread.Sleep(500);
                steps--;
            }
        }

        public static void Main(string[] args)
        {
            Run(30, 15, 10, 2, 10, 3000);            
        }
    }
}