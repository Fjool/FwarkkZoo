using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Zoo
{
    public struct SCREEN_ELEMENT
    {
        public char value;
        public ConsoleColor foreground;
        public ConsoleColor background;
    }

    public class Grass
    {   
        static Random random = new Random();
  
        public int foodStock  = 100;
        public int waterLevel =   0;

        public ConsoleColor foreground{ get{ if (waterLevel > 0) { return ConsoleColor.Blue; } else { return ConsoleColor.Green;      }}}
        public ConsoleColor background{ get{ if (waterLevel > 0) { return ConsoleColor.Green;} else { return ConsoleColor.DarkYellow; }}}

        public char Value()
        {
            char symbol;

            if (waterLevel > 0)
            {
                symbol = ChooseSymbol(waterLevel);
            }
            else
            {
                symbol = ChooseSymbol(foodStock);
            }

            return symbol;
        }

        private char ChooseSymbol(int Value)
        {
            char symbol;

                 if (Value >= 100) { symbol = '█'; }
            else if (Value >=  75) { symbol = '▓'; }
            else if (Value >=  50) { symbol = '▒'; }
            else if (Value >=  25) { symbol = '░'; }
            else                   { symbol = ' '; }

            return symbol;
        }
    }

    public class Map
    {
        static Random random = new Random();

        int randomWaterPositionXAxis = random.Next(1, 39);
        int randomWaterPositionYAxis = random.Next(1, 79);

        public Grass[,] terrain;
        public double[] height;
        
        double scale = 5.0;

        private double waterLevelValue = -0.5;

        public double waterLevel{ get { return waterLevelValue; } set { waterLevelValue = value; }}

        double frand()
        {
            return (1-random.NextDouble()*2);
        }

        public Map()
        {
            terrain = new Grass[Screen.High, Screen.Wide];
           
            int featuresize = 16;
            int samplesize = featuresize;
 
            InitialiseHeightMap(featuresize, samplesize);

            FillTerrainWithGrass();
        }

        private void InitialiseHeightMap(int featuresize, int samplesize)
        {
            height = new double[Screen.Wide * Screen.High];

            for (int y = 0; y < Screen.High; y += featuresize)
                for (int x = 0; x < Screen.Wide; x += featuresize)
                {
                    setSample(x, y, frand());
                }

            while (samplesize > 1)
            {
                DiamondSquare(samplesize, scale);

                samplesize /= 2;
                scale /= 2.0;
            }
        }   
        
        public double sample(int x, int y)
        {
            int wrapX = Math.Abs(x % Screen.Wide);
            int wrapY = Math.Abs(y % Screen.High);
            
            return height[(wrapX + (wrapY * Screen.Wide))];
        }
 
        public void setSample(int x, int y, double value)
        {
            int wrapX = Math.Abs(x % Screen.Wide);
            int wrapY = Math.Abs(y % Screen.High);

            height[(wrapX + (wrapY * Screen.Wide))] = value;
        }

        public void sampleSquare(int x, int y, int size, double value)
        {
            int hs = size / 2;
 
            // a     b 
            //
            //    x
            //
            // c     d
 
            double a = sample(x - hs, y - hs);
            double b = sample(x + hs, y - hs);
            double c = sample(x - hs, y + hs);
            double d = sample(x + hs, y + hs);
 
            setSample(x, y, ((a + b + c + d) / 4.0) + value);
 
        }
 
        public void sampleDiamond(int x, int y, int size, double value)
        {
            int hs = size / 2;
 
            //   c
            //
            //a  x  b
            //
            //   d
 
            double a = sample(x - hs, y);
            double b = sample(x + hs, y);
            double c = sample(x, y - hs);
            double d = sample(x, y + hs);
 
            setSample(x, y, ((a + b + c + d) / 4.0) + value);
        }

        void DiamondSquare(int stepsize, double scale)
        {
            int halfstep = stepsize / 2;
 
            for (int y = halfstep; y < Screen.High + halfstep; y += stepsize)
            {
                for (int x = halfstep; x < Screen.Wide + halfstep; x += stepsize)
                {
                    sampleSquare(x, y, stepsize, frand() * scale);
                }
            }
 
            for (int y = 0; y < Screen.High; y += stepsize)
            {
                for (int x = 0; x < Screen.Wide; x += stepsize)
                {
                    sampleDiamond(x + halfstep, y, stepsize, frand() * scale);
                    sampleDiamond(x, y + halfstep, stepsize, frand() * scale);
                }
            }
        }

        private void FillTerrainWithGrass()
        {
            for (int i = 0; i < Screen.High; i++)
            {
                for (int j = 0; j < Screen.Wide; j++)
                {
                    terrain[i,j] = new Grass();

                    // height ranges from -scale to scale, so normalize it to between 0 and 100
                    terrain[i,j].waterLevel = (int)(((height[(i*Screen.Wide) + j]+scale)/2)*100);
                    terrain[i,j].foodStock = 100;
                }
            }
        }
        
        private void CopyTerrainToScreenBuffer(SCREEN_ELEMENT[,] screen)
        {
            for (int i = 0; i < Screen.High; i++)
            {
                for (int j = 0; j < Screen.Wide; j++)
                {
                    screen[i,j].value      = terrain[i,j].Value();
                    screen[i,j].foreground = terrain[i,j].foreground;
                    screen[i,j].background = terrain[i,j].background;
                }
            }
        }
        
        private static void RenderAnimals(SCREEN_ELEMENT[,] screen, Zoo theZoo)
        {
            foreach (Animal animal in theZoo.GetAnimalsOfType<Animal>())
            {
                screen[animal.locY, animal.locX].foreground = animal.foreground;
                screen[animal.locY, animal.locX].background = animal.background;
                screen[animal.locY, animal.locX].value      = animal.symbol;
            }
        }

        public void RenderMap(SCREEN_ELEMENT[,] screen, Zoo theZoo)
        {
            CopyTerrainToScreenBuffer(screen);
            RenderAnimals(screen, theZoo);
        }
        
        public void Update()
        {
            for (int i = 0; i < Screen.High; i++)
            {
                for (int j = 0; j < Screen.Wide; j++)
                {
                    terrain[i,j].foodStock += 1;
                    terrain[i,j].foodStock = Math.Min(terrain[i,j].foodStock, 100);
                }
            }
        }
    }
}