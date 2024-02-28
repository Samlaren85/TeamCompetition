using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCompetition.Models
{
    public enum SwimStyles
    {
        crawl, backstroke, breaststroke, butterfly
    }
    public static class UserSettings
    {
        public static int PENELTYSECONDS = 5;
        public static int DISQUALIFYING_LIMIT = 2;
        public static bool MULTIPLEHEATS = true;
        public static bool PARTISAPATING_MALE = true;
        public static bool PARTISAPATING_FEMALE = true;
        public static bool PARTISAPATING_MIX = true;

        public static string ToString()
        {
            return $"{PENELTYSECONDS};{DISQUALIFYING_LIMIT};{MULTIPLEHEATS};{PARTISAPATING_MALE};{PARTISAPATING_FEMALE};{PARTISAPATING_MIX}";
        }
    }
}