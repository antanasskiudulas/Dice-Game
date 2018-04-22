using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiceGame
{
    /// <summary>
    /// Overall class to define a single die and it's desired functionalities
    /// </summary>
        public class Die
        {
            private static readonly Random rng = new Random();
            private static readonly object sync = new object();
            private int numberOnTop;
            //accessors & mutators for the class
            public int NumberOnTop
            {
                get { return numberOnTop; }
            }
            private string colour;
            public string Colour
            {
                get { return colour; }
                set { colour = value; }
            }
            private int numberOfSides;
            public int NumberOfSides
            {
                get { return numberOfSides; }
                set { numberOfSides = value; }
            }
            //default constructor for the die
            public Die(int inputNoOfSides, string inputColour)
            {
                numberOfSides = inputNoOfSides;
                Colour = inputColour;
            }

            /// <summary>
            /// rolls between desired random numbers. Lock ensures that when the method is called, its
            /// seed value for the random is retrieved from different threads (avoids duplicates)
            /// </summary>
            /// <returns>a random number between two integer points</returns>
            public int roll()
            {
                lock (sync)
                {
                    return numberOnTop = rng.Next(1, numberOfSides + 1);
                }
            }

        //when the object is called directly, its desired parameters are returned in a string form
        public override string ToString()
        {
            return string.Format($"{NumberOnTop} {Colour}");
        }
    }

    /// <summary>
    /// Class contains the definition for multiple dice and reflects the desired functionality
    /// for the specification brief
    /// </summary>
    public class DieCollection
    {
        //List to store all of our dice
        public List<Die> dieList = new List<Die>();
        //default colour list
        private string[] colourSelection = { "RED", "GREEN", "BLUE", "BLACK", "GOLD" };

        //constructor to intialize the dice and determine whether there's enough colours for dice in the array
        public DieCollection(int numOfDice, int numOfSides)
        {
            //check to see if the number of dice is more than available colours
            if (numOfDice > colourSelection.Count())
                //calls a helper method to deal with the problem
                OutOfColor(numOfDice, numOfSides);
            else
            {
                for (int i = 0; i < numOfDice; i++)
                {
                    dieList.Add(new Die(numOfSides,colourSelection[i]));
                }
            }
        }
        //only to be called in the constructor. Requests user to input colours if the noOfDice exceeds the colour array
        protected void OutOfColor(int numOfDice, int numOfSides)
        {
            int difference = numOfDice - colourSelection.Count();
            for (int i = 0; i < difference; i++)
            {
                Console.WriteLine($"You have selected more dice than the default amount.\nPlease insert a color for extra {i+5}th");
                string extraColour = Console.ReadLine().ToUpper();
                if(colourSelection.Contains(extraColour))
                {
                    Console.WriteLine($"{extraColour} colour already exists in our colour selection");
                    Console.Write("Current colour selection: \n");
                    foreach (string colour in colourSelection)
                    {
                        Console.Write($" {colour} ");
                    }
                    Console.Write("\n");
                }
                else
                    dieList.Add(new Die(numOfSides, extraColour));
            }
        }

        /// <summary>
        /// rolls all the dice in the die array, records the history for the roll
        /// and increments the player turns. Also accepts the move if player is AI
        /// </summary>
        /// <param name="player">A player to which to add the rolls to the history and record rounds</param>
        public void RollAll(Player player)
        {
            if(player.PlayerName.Contains("CPU"))
                Console.WriteLine("\nPress any key to roll!");
            else
            {
                Console.WriteLine("\nPress any key to roll!");
                Console.ReadKey();
            }
            foreach (Die die in dieList)
            {
                die.roll();
            }
            player.PlayerTurns++;
            player.RollHistory[player.PlayerTurns] = dieList.Select(k => k.NumberOnTop).ToList();
            ShowRolls();
            CalcPoints(player);
        }
        /// <summary>
        /// method overloading the previous method in a case it recieves a request
        /// to reroll and/or double points
        /// </summary>
        /// <param name="player">player to append the reroll history</param>
        /// <param name="reroll">indicates whether it's a reroll</param>
        /// <param name="doublepts">indicates whether the points should be doubled for this roll</param>
        public void RollAll(Player player, bool? reroll = null, bool? doublepts = null)
        {
            var toReroll = dieList.GroupBy(x => x.NumberOnTop)
                  .Where(g => g.Count() == 2)
                  .Select(g => g.Key).First();
            foreach (Die die in dieList)
            {
                if (toReroll != die.NumberOnTop)
                {
                    die.roll();
                    player.RollHistory[player.PlayerTurns].Add(die.NumberOnTop);
                }
            }
            player.HasReroll = false;
            ShowRolls();
            CalcPoints(player, true);
        }
        /// <summary>
        /// Method called when there's a two-of-a-kind in the roll
        /// letting player decide whether to reroll or not
        /// </summary>
        /// <param name="player">to check if it's a non-player entity (AI)</param>
        public void DecisionRoll(Player player)
        {
            Console.WriteLine("\nDo you want to reroll the remaining dice to get double the points for successful matches (Y/N):" +
                 "\nNote; You will get NO POINTS if you get two-of-a-kind or less this roll and you will not be able to reroll if you get two-of-a-kind anymore!\n");
            string choice;

            if (player.PlayerName.Contains("CPU"))
                choice = CPU.Decide(player);
            else
                choice = Console.ReadLine().ToUpper().Trim(' ');

            switch (choice)
            {
                case "Y":
                    RollAll(player, true, true);
                    break;
                case "N":
                    Console.WriteLine($"{player.PlayerName} abstained from rerolling the die; 0 points gained");
                    break;
                default:
                    Console.WriteLine("Incorrect input!");
                    break;
            }
        }
        /// <summary>
        /// Calculates and assigns the points to a player, doubles if it's a successful two-of-a-kind reroll
        /// </summary>
        /// <param name="player">Player to which points to be assigned</param>
        /// <param name="doublepts">Is the point double effect applied to this player?</param>
        public void CalcPoints(Player player,bool doublepts = false)
        {
            /*I chose to use lambda statement dictionary as it makes much shorter to describe the outcome for
             all the cases and using the action delegate, you can easily retrieve the value from the dictionary*/
            Dictionary<int, Action<Player, int, bool>> pointAssigner = new Dictionary<int, Action<Player, int, bool>>
            {
                //p = player, i=most common roll, b= should the points be doubled
                [1] = (p, i, b) => {
                    Console.WriteLine($"{p.PlayerName} has rolled 'zero'; player gains no points");
                    Console.WriteLine($"{p.PlayerName} currently has {p.PlayerScore} points");
                    p.PlayerScore += 0;
                },
                [2] = (p, i, b) => {
                    if (!p.HasReroll)
                    {
                        Console.WriteLine($"{p.PlayerName} has rolled 'two-of-a-kind'; player get's no points for the reroll");
                        Console.WriteLine($"{p.PlayerName} currently has {p.PlayerScore} points");
                    }
                    if(p.HasReroll)
                    {
                        Console.WriteLine($"{p.PlayerName} has rolled 'two-of-a-kind'; player get's to choose to reroll the dice for double points!");
                        DecisionRoll(p);
                    }
                },
                [3] = (p, i, b) => {
                    int pts;
                    if(!b)
                        p.PlayerScore += pts = 3;
                    else
                        p.PlayerScore += pts = 6;
                    Console.WriteLine($"{p.PlayerName} has rolled 'three-of-a-kind'; player awarded {pts} points!");
                    Console.WriteLine($"{p.PlayerName} currently has {p.PlayerScore} points");
                },
                [4] = (p, i, b) => {
                    int pts;
                    if (!b)
                        p.PlayerScore += pts = 6;
                    else
                        p.PlayerScore += pts = 12;
                    Console.WriteLine($"{p.PlayerName} has rolled 'four-of-a-kind'; player awarded {pts} points!");
                    Console.WriteLine($"{p.PlayerName} currently has {p.PlayerScore} points");
                },
                [5] = (p, i, b) => {
                    int pts;
                    if (!b)
                        p.PlayerScore += pts = 12;
                    else
                        p.PlayerScore += pts = 24;
                    Console.WriteLine($"{p.PlayerName} has rolled 'five-of-a-kind'; player awarded {pts} points!");
                    Console.WriteLine($"{p.PlayerName} currently has {p.PlayerScore} points");
                },
            };
            //retrieves the most recurring value from the roll list
            var maxReccuringRoll = dieList.GroupBy(x => x.NumberOnTop)
                .Where(g => g.Count() >= 0)
                .Select(g => g.Count()).Max();

            //creating a reference for a delegate to be initialized if the value is retreavable
            Action<Player, int, bool> pointSetter;
            //checking to see if the most reccuring roll is retreavable using the delegate reference
            if (pointAssigner.TryGetValue(maxReccuringRoll, out pointSetter))
            {
                //if so, but the double parameter entered as false in this method
                if(!doublepts)
                    //assign the points to the player
                    pointSetter(player, maxReccuringRoll, false);
                //if the double parameter entered as true
                else
                    pointSetter(player, maxReccuringRoll, true);
            }
            else
                return;
        }

        /// <summary>
        /// class to show rolls per turn
        /// </summary>
        public void ShowRolls()
        {
            Console.Write("Rolls: ".PadLeft(2));
            foreach (Die die in dieList)
            {
                Console.Write($" {die.Colour} {die.NumberOnTop} |");
            }
            Console.Write("\n");
        }
    }
}
