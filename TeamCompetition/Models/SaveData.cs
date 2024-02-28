using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCompetition.Models
{
    public class SaveData
    {
        public List<Team> SavedMaleTeams;
        public List<Team> SavedFemaleTeams;
        public List<Team> SavedMixedTeams;

        public SaveData(List<Team> male, List<Team> female, List<Team> mixed)
        {
            SavedMaleTeams = male;
            SavedFemaleTeams = female;
            SavedMixedTeams = mixed;
        }
    }
}
