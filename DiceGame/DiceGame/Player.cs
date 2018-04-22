using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiceGame
{
    /// <summary>
    /// definition for a player
    /// </summary>
    public class Player
    {
        private string playerName;
        public string PlayerName
        {
            get { return playerName; }
            set { playerName = value; }
        }
        private int playerTurns;
        public int PlayerTurns
        {
            get { return playerTurns; }
            set { playerTurns = value; }
        }
        //contains the history for the rolls for every round
        private Dictionary<int, List<int>> rollHistory = new Dictionary<int, List<int>>();
        public Dictionary<int, List<int>> RollHistory
        {
            get { return rollHistory; }
            set { rollHistory = value; }
        }

        private int playerScore;
        public int PlayerScore
        {
            get { return playerScore; }
            set { playerScore = value; }
        }

        private bool hasReroll = true;
        public bool HasReroll
        {
            get { return hasReroll; }
            set { hasReroll = value; }
        }

        public Player(string inputName)
        {
            playerName = inputName;
        }

        public override string ToString()
        {
            return string.Format($"Name: {PlayerName} Turn: {PlayerTurns} Score: {PlayerScore}");
        }

        //to display all the rolls for this players turn
        public void ShowHistory()
        {
            Console.Write($"Roll History:\n Player '{playerName}' at turn {playerTurns}; ");
            foreach (int i in rollHistory[playerTurns])
            {
                Console.Write($"{i} ");
            }
        }

        public void ShowStats()
        {
            int allTimeNumOfRolls = 0;
            int AllTimeTotal = 0;
            foreach (KeyValuePair<int,List<int>> entry in rollHistory)
            {
                allTimeNumOfRolls += entry.Value.Count;
                AllTimeTotal += entry.Value.Aggregate((x,y)=> x+=y);
            }
            decimal allTimeAverage = AllTimeTotal / allTimeNumOfRolls;

            int totalForThrow = RollHistory[PlayerTurns].Aggregate((x, y) => x += y);
            int totalDiceCount = RollHistory[PlayerTurns].Count;
            decimal averageforThrow = totalForThrow / totalDiceCount;

            Console.WriteLine($"\n{PlayerName} Statistics:" +
                $"\nDie total in this turn: {totalForThrow}" +
                $"\nTotal dice thrown this turn: {totalDiceCount}" +
                $"\nAverage in this turn: {averageforThrow:N1}" +
                $"\nTotal average for all turns: {allTimeAverage:N1}");
        }
    }
    /// <summary>
    /// description for the collection of players and the desired methods
    /// </summary>
    public class PlayerCollection
    {
        //list to store all players
        private List<Player> players = new List<Player>();
        public List<Player> Players
        {
            get { return players; }
            set { players = value; }
        }
        /// <summary>
        /// Initializes the desired number of players in the player list
        /// </summary>
        /// <param name="noOfPlayers">number of players to be initialized</param>
        public PlayerCollection(int noOfPlayers)
        {
            for (int i = 0; i < noOfPlayers; i++)
            {
                Console.WriteLine($"Input a name for player #{i}");
                string input = Console.ReadLine().ToUpper().Trim(' ');
                Players.Add(new Player(input));
            }
        }
    }
}
