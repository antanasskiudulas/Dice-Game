using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiceGame
{
    class Program
    {
        static void Main(string[] args)
        {
            bool loopFlag = false;
            //run for as long as an exception hasn't occured
            do
            {
                Console.WriteLine("Welcome to the 'three-or-more' die game by Antanas Skiudulas");
                //testing for invalid inputs and exceptions within the called methods
                try
                {
                    Console.WriteLine("Set the point limit for the game");
                    int pointCap = int.Parse(Console.ReadLine());

                    Console.WriteLine("Do you want to play vs Player(s) or A.I (1 = PVP / 2 = A.I)");
                    int choice = int.Parse(Console.ReadLine());
                    Game newGame;
                    if (choice == 1)
                    {
                        Console.WriteLine("How many players there will be playing?");
                        int opponent = int.Parse(Console.ReadLine());
                        newGame = new Game(5, 6, pointCap, opponent, false);
                    }
                    else
                    {
                        Console.WriteLine("You will be playing against A.I called 'CPU'");
                        newGame = new Game(5, 6, pointCap, 1, true);
                    }
                    newGame.StartGame();
                    loopFlag = true; //end the loop if the application reached this point without exceptions

            }
                catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } while (!loopFlag);
        }
    }
}