namespace SecretGarden
{
    public abstract class Plant
    {
        public int growth_speed;
        public int health;
        public double foodPerHP;
        public int reproductionThreshold;
        public char symbol;
        public int posX;
        public int posY;

        public int GetX()
        {
            return posX;
        }

        public int GetY()
        {
            return posY;
        }

        public void SetPos(int x, int y)
        {
            posX = x;
            posY = y;
        }

        public abstract void Reproduce(Terrain t);
        
        public void Grow(Terrain t)
        {
            health += growth_speed;
            if (health >= reproductionThreshold)
                Reproduce(t);
        }

        public int Feed(Terrain t)
        {
            int feedQuantity = (int)((health) * foodPerHP);

            t.DestroyPlant(posX, posY);

            return feedQuantity;
        }

        public abstract void Update(Terrain t);

        public override string ToString()
        {
            return symbol.ToString();
        }
    }

    public class Grass : Plant
    {
        public Grass()
        {
            growth_speed = 10;
            health = 10;
            foodPerHP = 0.05;
            reproductionThreshold = 100;
            symbol = 'G';
        }
        
        public override void Update(Terrain t)
        {
            Grow(t);
        }

        public override void Reproduce(Terrain t)
        {
            int[][] neighbours =
            {
                new int[] {-1, 0},
                new int[] {1, 0},
                new int[] {0, -1},
                new int[] {0, 1} 
            };

            foreach (int[] a in neighbours)
            {
                Plant p = new Grass();
                t.AddPlant(p, a[0] + posX, a[1] + posY);
            }

            this.health /= 5;
        }
    }
}