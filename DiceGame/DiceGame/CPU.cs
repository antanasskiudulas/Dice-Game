using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiceGame
{
    /// <summary>
    /// Class which handles AI interaction and inherits player properties
    /// </summary>
    public class CPU : Player
    {
        /// <summary>
        /// should have a constant name and inherit player 
        /// </summary>
        public CPU () : base("CPU"){}

        public static string Decide(Player AI)
        {
            //stores most common rolls per each key in the rollHistory
            List<double> probabilities = new List<double>();

            //retrieving the most common rolls per each entry in the roll history and adding it to the probabilities
            foreach (KeyValuePair<int, List<int>> entry in AI.RollHistory)
            {
                int query = (from val in entry.Value
                             group val by val into grp
                             orderby grp.Count() descending
                             select grp.Key).First();
                probabilities.Add(query);
            }


            /*if the overall average of the probabilities (the average of all the most common rolls per each entry)
             *  is less than 3 and there's enough data in the probabilities, then roll*/
            if (probabilities.Average() > 3 && probabilities.Count() > 5)
                return "Y";
            else
                /*do not roll if there's not enough data in the probabilities as it's too small of a basis to decide
                  and if the overall average is less than 3*/
                return "N";
        }
    }
}
