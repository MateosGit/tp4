using System;

namespace SecretGarden
{
    public enum AnimalType
    {
        Prey,
        Predator
    };
    
    public interface Prey
    {
        void Graze(Terrain t);//takes a plant in parameter
        void Flee(Terrain t, int dmg);
    }
    
    public interface Predator
    {
        void Hunt(Terrain t); //follow and kill a prey
        void EatPrey(Animal p); //eat a prey
    }
    
    public abstract class Animal
    {
        public int health;
        public AnimalType animalType;
        public int posX;
        public int posY;
        public char symbol;
        public int stamina;
        public int reproductionTimer = 0;
        public int reproductionDelay;
        static public Random rng;

        public int GetStamina()
        {
            return stamina;
        }

        public int GetPosX()
        {
            return posX;
        }

        public int GetPosY()
        {
            return posY;
        }

        public void SetPos(int x, int y)
        {
            posX = x;
            posY = y;
        }

        public AnimalType GetAnimalType()
        {
            return animalType;
        }

        public int GetHealth()
        {
            return health;
        }

        public abstract void Update(Terrain t);
        protected abstract Animal GenerateNewAnimal(int stamina);

        static Animal()
        {
            rng = new Random();
        }

        public void Reproduce(Terrain t, int stamina)
        {
            int[][] neighbours = new int[][]
            {
                new int[] {-1, 0},
                new int[] {1, 0},
                new int[] {0, -1},
                new int[] {0, 1} 
            };

            foreach (int[] p in neighbours)
            {
                int[] tmp = new int[] {p[0] + posX, p[1] + posY};
                Animal a = t.GetAnimal(tmp[0], tmp[1]);
                if (a == null)
                {
                    t.AddAnimal(GenerateNewAnimal(stamina), tmp[0], tmp[1]);
                    break;
                }
            }

            reproductionTimer = 0;
        }

        public void Move(Terrain t, int dirx, int diry)
        {
            int[] dir = new int[] {dirx, diry};

            dir[0] += posX;
            dir[1] += posY;

            if (t.IsTileEmpty(dir[0], dir[1]))
            {
                posX = dir[0];
                posY = dir[1];
            }
        }

        public override string ToString()
        {
            return symbol.ToString();
        }
    }

    public class Sheep : Animal, Prey
    {
        public int[][] eatingZone;
        public int agility = 45;//45% chance of dodging an attack
        
        public Sheep()
        {
            Init();
        }

        public Sheep(int stamina)
        {
            Init();
            this.stamina = stamina;
        }

        private void Init()
        {
            health = 100;
            animalType = AnimalType.Prey;
            symbol = 's';
            stamina = 100;
            reproductionDelay = 50;
            reproductionTimer = 0;
            posX = -1;
            posY = -1;
            
            eatingZone = new int[][]
            {
                new int[] {-1, 0},
                new int[] {1, 0},
                new int[] {0, -1},
                new int[] {0, 1},
                new int[] {0, 0} 
            };
        }

        protected override Animal GenerateNewAnimal(int stamina)
        {
            return new Sheep(stamina);
        }

        public void Graze(Terrain t)
        {
            foreach (int[] p in eatingZone)
            {
                Plant tmp = t.GetPlant(p[0] + posX, p[1] + posY);
                if (tmp != null)
                {
                    stamina += tmp.Feed(t);
                }
            }
        }
        
        public void Flee(Terrain t, int dmg)
        {
            if (rng.Next(100) < agility)
                Move(t, rng.Next(-1, 2), rng.Next(-1, 2));
            else
            {
                health -= dmg;
            }
        }

        public override void Update(Terrain t)
        {
            stamina--;
            
            if (stamina < 20)
                Graze(t);
            
            Move(t, rng.Next(-1, 2), rng.Next(-1, 2));
            //if stamina is <= 0, the animal dies
            if (stamina <= 0 || health <= 0)
                t.DestroyAnimal(this);
            if (reproductionTimer > reproductionDelay)
            {
                stamina /= 2;
                Reproduce(t, stamina / 2);
            }

            reproductionTimer++;
        }
    }

    public class Wolf : Animal, Predator
    {
        public Animal target = null;
        private int atk = 55;
        
        public Wolf()
        {
            Init();
        }

        public Wolf(int stamina) : base()
        {
            Init();
            this.stamina = stamina;
        }

        private void Init()
        {
            health = 100;
            animalType = AnimalType.Predator;
            symbol = 'w';
            stamina = 150;
            reproductionDelay = 200;
            reproductionTimer = 0;
            posX = -1;
            posY = -1;
        }

        protected override Animal GenerateNewAnimal(int stamina)
        {
            return new Wolf(stamina);
        }

        public override void Update(Terrain t)
        {
            stamina--;

            if (stamina < 20)
                Hunt(t);
            
            Move(t, rng.Next(-1, 2), rng.Next(-1, 2));
            if (stamina <= 0)
                t.DestroyAnimal(this);
            if (reproductionTimer > reproductionDelay)
            {
                stamina /= 2;
                Reproduce(t, stamina / 2);
            }

            reproductionTimer++;
        }

        public void Hunt(Terrain t)
        {
            int[][] neighbours = new int[][]
            {
                new int[] {-1, 0},
                new int[] {1, 0},
                new int[] {0, -1},
                new int[] {0, 1}
            };
            foreach (int[] p in neighbours)
            {
                Animal tmp = t.GetAnimal(p[0], p[1]);
                if (tmp != null)
                {
                    target = tmp;
                    return;
                }
            }
            if (target == null)
            {
                Animal p = t.FindPrey(posX, posY);
                if (p != null)
                {
                    target = p;
                }
            }
            else
            {
                int movex = 0;
                int movey = 0;
                if (target.GetPosX() < posX)
                    movex = -1;
                if (target.GetPosX() > posX)
                    movex = 1;
                if (target.GetPosY() < posY)
                    movey = -1;
                if (target.GetPosY() > posY)
                    movey = 1;
                Move(t, movex, movey);
                int dx = Math.Abs(target.GetPosX() - posX);
                int dy = Math.Abs(target.GetPosY() - posY);
                if (dx + dy <= 2)
                {
                    if ((target as Prey) != null)
                        (target as Prey).Flee(t, atk);
                }

                if (target.GetHealth() <= 0)
                    EatPrey(target);
            }
        }

        public void EatPrey(Animal p)
        {
            Console.WriteLine("Eating prey: " + p.stamina);
            stamina += p.GetStamina() + 50;
            target = null;
        }
    }
}