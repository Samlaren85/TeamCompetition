using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCompetition.Models
{
    public enum Simsätten
    {
        frisim, ryggsim, bröstsim, fjärilsim
    }
    public static class UserSettings
    {
        public static int TILLÄGGSSEKUNDER = 5;
        public static bool FLERAHEAT = true;
        public static bool DELTAGANDE_HERRAR = true;
        public static bool DELTAGANDE_DAMER = true;
        public static bool DELTAGANDE_MIX = true;
    }
}
