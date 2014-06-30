using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Zoo
{
    public enum ANIMAL_STATE{ SLEEPING, AWAKE };

    public abstract class Animal
    {       
        public Zoo theZoo;

        public int maxFood       = 1000;
        public float currentFood = 1000;
        public int hungerPerTick =   10;

        static Random random = new Random();

        private int y = (random.Next(0, Screen.High - 1));
        private int x = (random.Next(0, Screen.Wide - 1));

        public int locX { get { return x; } }
        public int locY { get { return y; } }

        public Boolean isDead = false;

        public DateTime Birthday;

        public char symbol = '?';
        public String species = "<undefined>";

        protected double accumulatedTime = 0;

        public ANIMAL_STATE state = ANIMAL_STATE.AWAKE;

        public ConsoleColor foreground = ConsoleColor.White;
        public ConsoleColor background = ConsoleColor.Black;

        //--------------------------------------------------------------------------------
        protected virtual void Initialise()
        {
            Birthday = DateTime.Now;
        }

        //--------------------------------------------------------------------------------
        public Animal()
        {
            Initialise();
        }
            
        //--------------------------------------------------------------------------------
        protected void MoveRandomly()
        {
            int newX = 0, newY = 0;
            Boolean hasMoved = false;
            
            if (state == ANIMAL_STATE.AWAKE)
            {
                int numberOfTries=0;

                while (!hasMoved && numberOfTries < 5)
                {                       
                    newX = x + (1 - (random.Next(0, 3)));
                    newY = y + (1 - (random.Next(0, 3)));

                    if (newX != x || newY != y)
                    {
                        hasMoved = MoveTo(newX, newY);
                        numberOfTries++;
                    }
                }
            }                               
        }
  
        //--------------------------------------------------------------------------------
        public virtual void Update(double elapsedTime)
        {
            accumulatedTime += elapsedTime;

            Move();
            CheckHunger();
        }

        //--------------------------------------------------------------------------------
        protected virtual void CheckHunger()
        {
            // animal loses 10 food per second
            currentFood -= hungerPerTick;

            // check animal status and complain if hungry, die if dead
            if (currentFood < 10)
            {
                theZoo.lastMessage = this.species + " has died.";
                isDead = true;
            }
            else if (isHungry())
            {
                theZoo.lastMessage = this.species + " is hungry.";
                this.foreground = ConsoleColor.Red;
            }
        }

        //--------------------------------------------------------------------------------
        public virtual void Move()
        {
            MoveRandomly();
            currentFood--;
        }

        //--------------------------------------------------------------------------------
        protected A FindClosest<A>() where A:Animal
        {
            List<A> allA = theZoo.GetAnimalsOfType<A>().Where<A>(z => z != this).ToList<A>();
            allA.Sort(CompareByDistance<A>);

            if (allA.Count() > 0)
            {   return allA[0];
            }
            else
            {   return null;
            }
        }
        
        //--------------------------------------------------------------------------------
        int CompareByDistance<A>(A x, A y) where A:Animal
        {
            int distanceFromX = this.DistanceFrom(x);
            int distanceFromY = this.DistanceFrom(y);

            if (distanceFromX < distanceFromY)
            {   return 1;
            }
            else
            {   return 0;
            }
        }

        //--------------------------------------------------------------------------------
        protected A FindAdjacentAnimal<A>() where A:Animal
        {
            List<A> allAnimals = theZoo.GetAnimalsOfType<A>();
                    
            A animal, foundAnimal = null;
            int animalNum = 0;

            while ((foundAnimal == null) && (animalNum < allAnimals.Count()))
            {
                animal = allAnimals[animalNum];

                if ((animal.y + 1 == y && animal.x + 1 == x) ||
                    (animal.y     == y && animal.x + 1 == x) ||
                    (animal.y - 1 == y && animal.x + 1 == x) ||
                    (animal.y + 1 == y && animal.x     == x) ||
                    (animal.y     == y && animal.x     == x) ||
                    (animal.y - 1 == y && animal.x     == x) ||
                    (animal.y + 1 == y && animal.x - 1 == x) ||
                    (animal.y     == y && animal.x - 1 == x) ||
                    (animal.y - 1 == y && animal.x - 1 == x))
                {
                    foundAnimal = animal;                    
                }
                
                animalNum++;
            }
            return foundAnimal;
        }

        //--------------------------------------------------------------------------------
        public int DistanceFrom(Animal anotherAnimal)
        {
            int diffX = Math.Abs(anotherAnimal.locX - locX);
            int diffY = Math.Abs(anotherAnimal.locY - locY);

            double distance = Math.Sqrt(diffX*diffX + diffY*diffY);
            
            return (int)Math.Round(distance,0);
        }

        //--------------------------------------------------------------------------------
        public int DistanceFrom(int x, int y)
        {
            int diffX = Math.Abs(x - locX);
            int diffY = Math.Abs(y - locY);

            double distance = Math.Sqrt(diffX*diffX + diffY*diffY);
            
            return (int)Math.Round(distance,0);
        }

        //--------------------------------------------------------------------------------      
        public Boolean MoveRelativeTo(Animal anotherAnimal, int direction)
        {
            int newX = x, newY = y;

            if (anotherAnimal.x < x) { newX = x-direction; } else if (anotherAnimal.x > x) { newX = x+direction; }
            if (anotherAnimal.y < y) { newY = y-direction; } else if (anotherAnimal.y > y) { newY = y+direction; }

            return MoveTo(newX, newY); 
        }
        

        //--------------------------------------------------------------------------------      
        public Boolean MoveToward(Animal anotherAnimal)
        {
            return MoveRelativeTo(anotherAnimal, 1);
        }

        //--------------------------------------------------------------------------------      
        public Boolean MoveAwayFrom(Animal anotherAnimal)
        {
           return MoveRelativeTo(anotherAnimal, -1);                                  
        }

        //--------------------------------------------------------------------------------      
        protected Boolean isHungry()
        {
            return currentFood < (maxFood / 2);
        }

        //--------------------------------------------------------------------------------      
        protected Boolean isSqaureOccupied(int tryX, int tryY)
        {            
            return theZoo.GetAnimalsOfType<Animal>().Exists(a => ((a.x == tryX) && (a.y == tryY)));
        }

        //--------------------------------------------------------------------------------      
        public Boolean MoveTo(int newX, int newY)
        {                      
            if (newY < 0 || newX < 0 || newX >= Screen.Wide || newY >= Screen.High)
            {
                theZoo.lastMessage = "Attempt to move outsize zoo.";
                return false;
            }
            else if (isSqaureOccupied(newX, newY))
            {
                theZoo.lastMessage = "Can't move to occupied space: (" + newX.ToString() + "," + newY.ToString() + ")";
                return false;
            }
                
            x = newX;
            y = newY;  

            currentFood -= 10;

            return true;  
        }
    }

    //--------------------------------------------------------------------------------
    public class Zebra : Animal
    {
        Random rand;

        protected override void Initialise()
        {
            base.Initialise();
            symbol = 'Z';
            species = "Zebra";
            rand = new Random((int)DateTime.Now.Ticks);
        }
        /*
        protected Boolean GroupWithOtherZebras()
        {
            Boolean hasMoved = false;

            Zebra closestZebra = FindClosest<Zebra>();
            
            if (closestZebra != null)
            {   if (DistanceFrom(closestZebra) < 3)
                {
                    hasMoved = MoveAwayFrom(closestZebra);
                }
                else
                {
                    hasMoved = MoveToward(closestZebra);
                }
            }

            return hasMoved;
        }
        */
        protected void MoveToMostAttractiveSquare()
        {
            // evaluate all surrounding squares and move to the nicest one
            int newX = locX, newY = locY;
            double currentBest = 0;
           
            for(int i = -1; i <= 1; i++)
            {   for(int j = -1; j <= 1; j++)
                {                    
                    if (locX+i >= 0 && locX + i < Screen.Wide && locY+j >= 0 && locY+j < Screen.High)
                    {
                        double checkSquare = EvaluateSquare(locX+i, locY+j) + rand.Next(0,10);

                        if (checkSquare > currentBest)
                        {
                            newX = locX+i;
                            newY = locY+j;

                            currentBest = checkSquare;
                        }
                    }
                }
            }

            MoveTo(newX, newY);
        }

        protected double EvaluateSquare(int x, int y)
        {
            double attraction = theZoo.map.terrain[y,x].foodStock;

            // look at the given square and decide how attractive it is
            // grass - proximity to lions + proximity to other zebras
            foreach (Lion lion in theZoo.GetAnimalsOfType<Lion>())
            {
                double distance = lion.DistanceFrom(x, y)+1;                
                attraction -= (255/(distance*distance));
            }

            // maybe move closer to nearest zebra
            Zebra zebra = FindClosest<Zebra>();
            if (this.DistanceFrom(zebra) > zebra.DistanceFrom(x, y))
            {
                attraction += 1;
            }
            else
            {   attraction -= 1;
            }

            // avoid swimming into very deep water
            if (theZoo.map.terrain[y,x].waterLevel > 50)
            {   attraction -= 100;
            }

            return attraction;
        }

        protected Boolean LionIsClose()
        {
            return theZoo.GetAnimalsOfType<Lion>().Exists(l => l.DistanceFrom(this) < 10);
        }

        public override void Update(double elapsedTime)
        {   
            if (LionIsClose())
            {
                MoveToMostAttractiveSquare();
            }
            else
            {   if (isHungry())
                {
                    // otherwise, if we are at all hungry, eat from the current square (if it has food)
                    if (theZoo.map.terrain[locY,locX].foodStock > 50)
                    {   
                        currentFood += 2f;
                    }
                    else if (theZoo.map.terrain[locY,locX].foodStock > 20)
                    {   
                        currentFood += 1f;
                    }
                    else if (theZoo.map.terrain[locY,locX].foodStock > 0)
                    {   
                        currentFood += 0.5f;
                    }

                    currentFood = Math.Min(maxFood, currentFood);
                    theZoo.map.terrain[locY,locX].foodStock -= 50;
                    theZoo.map.terrain[locY,locX].foodStock = Math.Max(0, theZoo.map.terrain[locY,locX].foodStock);
                }
                else 
                {   MoveToMostAttractiveSquare();                     
                }
            }
        }
/*
        protected Boolean RunFromLions()
        {
            Boolean hasMoved = false;
            Lion lion = FindClosest<Lion>();

            if (lion != null && lion.DistanceFrom(this) < 10)
            {   hasMoved = MoveAwayFrom(lion);                    
            }
                        
            return hasMoved;
        }

        public override void Move()
        {
            if (!RunFromLions())
            {
                base.Move();
            }            
        }
 */
    }

    //--------------------------------------------------------------------------------
    class Lion : Animal
    {
        protected override void Initialise()
        {
            base.Initialise();
            currentFood = maxFood;
            symbol = '☺';
            species = "Lion";
            foreground = ConsoleColor.Yellow;
            hungerPerTick = 10;
        }

        public override void Move()
        {
            if (!ChaseZebras())
            {
                base.Move();
            }
        }

        protected Boolean ChaseZebras()
        {
            Boolean hasMoved = false;

            if (isHungry())
            {
                Zebra zebra = FindClosest<Zebra>();

                if (zebra != null)
                {   hasMoved = MoveToward(zebra);                                                
                }
            }
                        
            return hasMoved;
        }

        protected override void CheckHunger()
        {
            base.CheckHunger();

            if (isHungry())
            {
                theZoo.lastMessage = "Lion is hungry, be weary zebras...";
                foreground = ConsoleColor.Red;

                if (theZoo.GetAnimalsOfType<Zebra>().Count() > 0)
                {
                    Zebra zebra = FindAdjacentAnimal<Zebra>();
        
                    if (zebra == null)
                    {
                        theZoo.lastMessage = "Hungry lion can't find a zebra.... :(";
                    }
                    else
                    {
                        zebra.isDead = true;
                        this.currentFood = 1000;
                        theZoo.lastMessage = "Lion ate a zebra. Yum!";                        
                    }
                }
            }
            else
            {   foreground = ConsoleColor.Yellow;
            }
        }
    }
}
