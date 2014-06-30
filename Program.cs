using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zoo
{    
    static class Messages
    {
        public const String NONE = "";       
        public const String CANNOT_AFFORD = "You cannot afford that.";       
    }

    class Program
    {
        enum GameState { GS_STATUS, GS_MAP };
       
        /*
        Interface theInterface;
        Zoo theZoo;

        long lastTime;
        long elapsedTime;
        */
        static void Main(string[] args)
        {
            int CYCLE_TIME = 100;
            
            Interface theInterface = new Interface();
            
            Zoo theZoo = new Zoo(theInterface);
            Boolean isGameAlive = true;

            DateTime lastTime = DateTime.Now;
            double elapsedTime;

            GameState State = GameState.GS_STATUS;

            Console.WindowHeight = Screen.High+1;
            Console.WindowWidth  = Screen.Wide;
            Console.BufferHeight = Screen.High+1;
            Console.BufferWidth  = Screen.Wide;
            
            /*
            theInterface.DisplayBanner("Welcome to Zoo Game!");
            theInterface.Write("What is your zoo called?");

            String tempName = Console.ReadLine();
            if (tempName != "") 
            { theZoo.name = tempName;
            }
            */
            
            lastTime = DateTime.Now;
                        
            State = GameState.GS_MAP;

            //theZoo.AddLion();
            for (int i = 0; i<5; i++)
            {            theZoo.AddZebra();
            }

            //Lion l1 = theZoo.GetAnimalsOfType<Lion>().First<Lion>();
            //Zebra z1 = theZoo.GetAnimalsOfType<Zebra>().First<Zebra>();

            //l1.MoveTo(40,20);
            //z1.MoveTo(41,21);

            while (isGameAlive)
            {
                elapsedTime = DateTime.Now.Subtract(lastTime).TotalMilliseconds;
                
                if (Console.KeyAvailable)
                {
                    // check for command
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    switch(key.Key)
                    {
                        // take action
                        case ConsoleKey.Q:
                            isGameAlive = false;
                            break;

                        case ConsoleKey.Z:
                            theZoo.AddZebra();
                            break;

                        case ConsoleKey.L:
                            theZoo.AddLion();
                            break;

                        case ConsoleKey.M:
                            if (State == GameState.GS_STATUS)
                                State = GameState.GS_MAP;
                            else if (State == GameState.GS_MAP)
                                State = GameState.GS_STATUS;
                                break;

                        case ConsoleKey.O:
                            theZoo.map.waterLevel += 0.1;
                            break;

                        case ConsoleKey.P:
                            theZoo.map.waterLevel -= 0.1;
                            break;
                    }
                }

                if (elapsedTime > CYCLE_TIME)
                {
                    theZoo.Update(elapsedTime);
                    if (State == GameState.GS_STATUS)
                    {
                        theInterface.DisplayStatus(theZoo);
                    }
                    else
                    {
                        theInterface.DisplayMap(theZoo);
                    }

                    lastTime = DateTime.Now;
                }
            }
        }
    }

    public class Zoo
    {
        static Int32 zebraPrice = 100;
        public Int32 lionPrice = 500;

        public Int32 money = 100000;
        
        List<Animal> animalList = new List<Animal>();
        public Map map = new Map();

        public String name = "Fwarkk's Zoo";

        public String lastMessage;

        Interface theInterface;

        public Zoo(Interface theInterface)
        {
            this.theInterface = theInterface;
        }

        public List<T> GetAnimalsOfType<T>()
        {
            return (List<T>)animalList.OfType<T>().ToList<T>();
        }

        public Int32 CountZebras()
        {
            return GetAnimalsOfType<Zebra>().Count();
        }

        public Int32 CountLions()
        {
            return GetAnimalsOfType<Lion>().Count();
        }

        public T AddAnimal<T>() where T : Animal, new()
        {
            T newAnimal = new T();
            newAnimal.theZoo = this;

            animalList.Add(newAnimal);
            lastMessage = newAnimal.species + " added.";            

            return newAnimal;
        }

        public void AddZebra()
        {
            if (money >= zebraPrice)
            {   AddAnimal<Zebra>();
                money -= zebraPrice;            
            }
            else
            {
                lastMessage = Messages.CANNOT_AFFORD;
            }
        }

        public void AddLion()
        {
            if (money >= lionPrice)
            {   AddAnimal<Lion>();
                money -= lionPrice;            
            }
            else
            {
                lastMessage = Messages.CANNOT_AFFORD;
            }           
        }


        public void Update(double elapsedTime)
        {
            List<Animal> deadAnimals = new List<Animal>();

            foreach (Animal animal in animalList)
            {
                animal.Update(elapsedTime);
            }

            PurgeDeadAnimals(deadAnimals);

            map.Update();
        }

        private void PurgeDeadAnimals(List<Animal> deadAnimals)
        {
            foreach (Animal animal in animalList)
            {
                if (animal.isDead)
                {
                    deadAnimals.Add(animal);
                }
            }

            foreach (Animal animal in deadAnimals)
            {
                animalList.Remove(animal);
            }
        }
    }
}
