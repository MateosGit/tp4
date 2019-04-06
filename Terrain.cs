using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace SecretGarden
{
    public class Terrain
    {
        //private Animal[,] animals;
        public List<Animal> animals;
        public Plant[,] plants;
        public int w { get; set; }
        public int h { get; set; }

        public Terrain(int w, int h)
        {
            animals = new List<Animal>();
            plants = new Plant[w,h];
            this.w = w;
            this.h = h;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    plants[x, y] = null;
                }
            }
        }

        public void Print()
        {
            //Console.Clear();
            Console.SetCursorPosition(0, 0);
            ConsoleColor defaultColor = Console.ForegroundColor;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    bool skip = true;
                    foreach (Animal a in animals)
                    {
                        if (a.GetPosX() == x && a.GetPosY() == y)
                        {
                            if (a.GetAnimalType() == AnimalType.Prey)
                                Console.ForegroundColor = ConsoleColor.Cyan;
                            else
                                Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(a);
                            skip = false;
                        }
                    }

                    if (skip)
                        Console.Write(' ');
                    Console.ForegroundColor = ConsoleColor.Green;
                    if (plants[x, y] != null)
                        Console.Write(plants[x, y]);
                     else
                        Console.Write(' ');
                    Console.ForegroundColor = defaultColor;
                }
                Console.WriteLine();
            }
        }

        public void AddAnimal(Animal a, int x, int y)
        {
            if (x < 0 || y < 0 || x >= w || y >= h || animals.Exists(e => e.GetPosX() == x && e.GetPosY() == y))
                return;
            a.SetPos(x, y);
            animals.Add(a);
        }

        public void AddPlant(Plant p, int x, int y)
        {
            if (x < 0 || x >= plants.GetLength(0) || y < 0 || y >= plants.GetLength(1)
                || plants[x, y] != null)
                return;

            p.SetPos(x, y);
            plants[x, y] = p;
        }
        
        public void Update()
        {
            for (int i = 0; i < animals.Count; i++)
            {
                animals[i].Update(this);
            }

            foreach (Plant p in plants)
            {
                if (p != null)
                    p.Update(this);
            }
        }

        public bool IsTileEmpty(int x, int y)
        {
            //This should be replaced by a loop, they aren't supposed to know how to do this.
            return (!animals.Exists(a => a.GetPosX() == x && a.GetPosY() == y)) && x < w && x >= 0
                && y < h && y >= 0;
        }

        public Plant GetPlant(int x, int y)
        {
            if (x < 0 || x >= w || y < 0 || y >= h)
                return null;
            return plants[x, y];
        }

        public Animal GetAnimal(int x, int y)
        {
            if (x < 0 || x >= w || y < 0 || y >= h)
                return null;
            //This should also be replaced by a loop, they are still not supposed to know how to do this.
            Animal tmp = animals.Find(a => a.GetPosX() == x && a.GetPosY() == y);
            return tmp;
        }

        public Animal FindPrey(int pX, int pY)
        {
            int xmin = pX - 5;
            int ymin = pY - 5;
            int xmax = pX + 5;
            int ymax = pY + 5;
            for (int y = ymin; y < ymax; y++)
            {
                for (int x = xmin; x < xmax; x++)
                {
                    if (!(x < 0 || x >= w || y < 0 || y >= h))
                    {
                        //Once again, this should be replaced by a loop
                        Animal tmp = animals.Find(a => a.GetPosX() == x && a.GetPosY() == y
                                                                        && a.GetAnimalType() == AnimalType.Prey);
                        if (tmp != null)
                            return tmp;
                    }
                }
            }

            return null;
        }

        public void DestroyPlant(int x, int y)
        {
            if (x < 0 || x >= w || y < 0 || y >= h)
                return;

            plants[x, y] = null;
        }
        
        public void DestroyAnimal(Animal a)
        {
            animals.Remove(a);
        }
    }
}