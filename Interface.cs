using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zoo
{
    public static class Screen
    {
        public static int Wide = 80;
        public static int High = 40;
    }

    public class Interface
    {
        DateTime now = DateTime.Now;

        protected SCREEN_ELEMENT[,] screen;
        
        protected SCREEN_ELEMENT[,] CreateScreenBuffer()
        {
            SCREEN_ELEMENT[,] screen = new SCREEN_ELEMENT[Screen.High, Screen.Wide];            
            return screen;
        }
       
        public Interface()
        {
            screen = CreateScreenBuffer();                        
        }

        public void Write(String theString)
        {
            Console.Write(theString);
        }

        public void WriteLine(String theString)
        {
            Console.WriteLine(theString);
        }

        public void DisplayBanner(Zoo theZoo)
        {
            DisplayBanner(theZoo.name.ToUpper());
        }

        public void DisplayStats(Zoo theZoo)
        {
            Console.Write("* " + "£" + theZoo.money.ToString().PadLeft(5) + "                                                               " + (DateTime.Now.ToString("h:mm:ss").PadLeft(7) + " *"));
            WriteSeparator();
        }

        public void WriteSeparator()
        {
            Console.Write("********************************************************************************");
        }

        public void WriteBorder()
        {
            Console.Write("*                                                                              *");
        }

        private void DisplayScreen(SCREEN_ELEMENT[,] theScreen, Zoo theZoo)
        {
            Console.SetCursorPosition(0,0);

            for (int i = 0; i < Screen.High; i++)
            {
                for (int j = 0; j < Screen.Wide; j++)
                {                                       
                    Console.ForegroundColor = theScreen[i, j].foreground;
                    Console.BackgroundColor = theScreen[i, j].background;
                  
                    Console.Write(theScreen[i, j].value);
                }
            }
        }
        
        public void DisplayBanner(String theCaption)
        {
            int marginLeft = ((Screen.Wide - theCaption.Length) / 2);
            int marginRight = (Screen.Wide - marginLeft) - theCaption.Length;

            WriteSeparator();
            WriteBorder();            
            Console.Write("*".PadRight(marginLeft) + theCaption + "*".PadLeft(marginRight));
            WriteBorder(); 
            WriteSeparator();
        }
       
        public void ClearScreen()
        {
            Console.Clear();
        }

        public void ReportAnimals(Zoo theZoo)
        {
            Console.WriteLine("[Z]ebras: " + theZoo.CountZebras());
            Console.WriteLine("[L]ions : " + theZoo.CountLions());
        }

        public void DisplayPrompt()
        {
            Console.Write("> ");
        }

        public void DisplayStatus(Zoo theZoo)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            ClearScreen();
            DisplayBanner(theZoo);
            DisplayStats(theZoo);
            ReportAnimals(theZoo);
            ReportMessage(theZoo);
            DisplayPrompt();
        }
        
        public void DisplayMap(Zoo theZoo)
        {
            theZoo.map.RenderMap(screen, theZoo);
            DisplayScreen(screen, theZoo);          
        }

        public void ReportMessage(Zoo theZoo)
        {
            if (theZoo.lastMessage != "")
            {
                Console.WriteLine(theZoo.lastMessage);
            }
        }
    }
}