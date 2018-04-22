using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiceGame
{
    /// <summary>
    /// deals with the game rules and handles players turns, declares victors and initializes, both players and dice
    /// </summary>
    public class Game
    {
        //maximum point value to which the game continues
        private int pointCap;
        public int PointCap
        {
            get { return pointCap; }
            set { pointCap = value; }
        }

        //intializes player and die collection classes.
        DieCollection dice;
        PlayerCollection playerList;

        public Game(int numOfDices, int numOfSides, int pointCap, int numOfPlayers = 0, bool CPU = false)
        {
            if (numOfPlayers != 0)
                playerList = new PlayerCollection(numOfPlayers);
            if (CPU)
                playerList.Players.Add(new Player("CPU"));

            dice = new DieCollection(numOfDices, numOfSides);
            this.pointCap = pointCap;
        }
        /// <summary>
        /// template for declaring a winner
        /// </summary>
        /// <param name="player">winner to be declared</param>
        protected void declareWinner(Player player)
        {
            Console.WriteLine($"\nCongratulations to {player.PlayerName} for reaching {player.PlayerScore} at turn {player.PlayerTurns}, winning the game!");
        }

        /// <summary>
        /// Begins the game when called
        /// </summary>
        public void StartGame()
        {
            Console.WriteLine("Rules:"+
                "\n1. Each player takes turns to roll 5 dice"+
                "\n2. if you match; 1:0P, 2:Reroll or 0P, 3:3, 4:6P, 5:12P"+
                "\n3. if you reroll and get two-of-a-kind or less, you get 0 points (everything else doubles) but you won't be able to reroll anymore"+
                $"\n4. first to reach {pointCap} is declared the winner"+
                "\n5. A.I information will always be displayed in Cyan");
            //contains the currently highest player score
            int currentHighestFlag = 0;

            //Runes whilst no one has reached the point cap
            while (currentHighestFlag < pointCap)
            {
                //looping through each player
                foreach (Player player in playerList.Players)
                {
                    //checking the player to determine in which coulour to be displayed
                    if (player.PlayerName.Contains("CPU"))
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    else
                        Console.ForegroundColor = ConsoleColor.White;

                    Console.WriteLine($"\n\nPlayer {player.PlayerName}'s turn to roll the dice!");
                    dice.RollAll(player);
                    player.ShowHistory();
                    player.ShowStats();
                    //updates the highest value after the turn
                    currentHighestFlag = playerList.Players.GroupBy(y => y.PlayerScore).Select(g => g.Key).Max();
                    //checks again the cap and if it's exceeded or matched, declare the winner
                    if (currentHighestFlag >= pointCap)
                        declareWinner(player);

                    //sleep after each roll so the user can track the flow of the game
                    Thread.Sleep(150);
                }
            }
        }
    }
}
