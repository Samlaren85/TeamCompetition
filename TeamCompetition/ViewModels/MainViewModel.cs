using TeamCompetition.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TeamCompetition.Models;
using TeamCompetition.Views;
using System.IO;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Text.Json.Nodes;
using Newtonsoft.Json;

namespace TeamCompetition.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private ObservableCollection<Team> maleTeams = new ObservableCollection<Team>();
        public ObservableCollection<Team> MaleTeams
        { 
            get { return maleTeams; } 
            set { 
                maleTeams = value;
                OnPropertyChanged();
                isSaved = false;
            }
        }

        private ObservableCollection<Team> femaleTeams = new ObservableCollection<Team>();
        public ObservableCollection<Team> FemaleTeams
        {
            get { return femaleTeams; }
            set
            {
                femaleTeams = value;
                OnPropertyChanged();
                isSaved = false;
            }
        }

        private ObservableCollection<Team> mixTeams = new ObservableCollection<Team>();
        public ObservableCollection<Team> MixTeams
        {
            get { return mixTeams; }
            set
            {
                mixTeams = value;
                OnPropertyChanged();
                isSaved = false;
            }
        }

        public ObservableCollection <Team> CurrentTeams
        {
            get 
            {
                if (SelectedTab.Contains("Herrar"))
                {
                    return MaleTeams;
                }
                if (SelectedTab.Contains("Damer"))
                {
                    return FemaleTeams;
                }
                if (SelectedTab.Contains("Mix"))
                {
                    return MixTeams;
                }
                return null;
            }
        }


        private string selectedTab;
        public string SelectedTab
        {
            get => selectedTab;
            set
            {
                selectedTab = value;
                
                OnPropertyChanged();
            }
        }

        private Team selectedTeam;
        public Team SelectedTeam
        {
            get => selectedTeam;
            set
            {
                selectedTeam = value;
                OnPropertyChanged();
            }
        }

        private int selectedColumn;
        public int SelectedColumn
        {
            get => selectedColumn;
            set
            {
                selectedColumn = value;
                OnPropertyChanged();
            }
        }

        protected bool isSaved;

        /// <summary>
        /// Calculate the teams placement in the final.
        /// Based on total accumulated time and added penelty time.
        /// </summary>
        /// <param name="Team"></param>
        private void CalculatePlacement(Team Team)
        {
            if (Team.Result.Equals("") || Team.PeneltySummery>UserSettings.DISQUALIFYING_LIMIT) Team.Placement = "DSQ";
            else 
            {
                int placement = CurrentTeams.Count;
                
                foreach (Team opponent in CurrentTeams)
                {
                    if (!opponent.Result.Equals(""))
                    {
                        if (opponent != Team && Team.Compare(opponent))
                        {
                            placement--;
                        }
                    }
                    else
                    {
                        placement--;
                    }
                }
                if (UserSettings.MULTIPLEHEATS || placement <= 6) Team.Placement = placement.ToString();
                else Team.Placement = "DNQ";
            }
            
        }

        private ICommand calculateResult = null!;
        /// <summary>
        /// Clears and calculatets the results for each group in the competition.
        /// </summary>
        public ICommand CalculateResult =>
            calculateResult ??= calculateResult = new RelayCommand(() =>
            {
                foreach (Team t in CurrentTeams) t.Result = "";
                foreach (Team t in CurrentTeams) t.AddingResults();
                foreach (Team t in CurrentTeams) CalculatePlacement(t);
                
                OnPropertyChanged();
            });

        private ICommand addPenelty = null!;
        /// <summary>
        /// Adds penelty time to a team at the specified event
        /// </summary>
        public ICommand AddPenelty =>
            addPenelty ??= addPenelty = new RelayCommand(() =>
            {
                if (SelectedColumn == 1) SelectedTeam.PeneltyBackstroke++;
                else if (SelectedColumn == 3) SelectedTeam.PeneltyBreaststroke++;
                else if (SelectedColumn == 5) SelectedTeam.PeneltyButterfly++;
                else if (SelectedColumn == 7) SelectedTeam.PeneltyCrawl++;
                OnPropertyChanged();
            });

        private ICommand removePenelty = null!;
        /// <summary>
        /// Removes penelty time to a team at the specified event
        /// </summary>
        public ICommand RemovePenelty =>
            removePenelty ??= removePenelty = new RelayCommand(() =>
            {
                if (SelectedColumn == 1) SelectedTeam.PeneltyBackstroke--;
                else if (SelectedColumn == 3) SelectedTeam.PeneltyBreaststroke--;
                else if (SelectedColumn == 5) SelectedTeam.PeneltyButterfly--;
                else if (SelectedColumn == 7) SelectedTeam.PeneltyCrawl--;
                OnPropertyChanged();
            });

        private ICommand clearTable = null!;
        /// <summary>
        /// Clears the selected table from all data
        /// </summary>
        public ICommand ClearTable =>
            clearTable ??= clearTable = new RelayCommand(() =>
            {
                CurrentTeams.Clear();
                OnPropertyChanged();
            });

        private ICommand save = null!;
        /// <summary>
        /// Saves the data to a txt file in the application directory
        /// </summary>
        public ICommand Save =>
            save ??= save = new RelayCommand(() =>
            {
                string messageBoxText = "Vill du spara datan?";
                string caption = "Spara";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                if (result == MessageBoxResult.Yes)
                {
                    SaveData saveData = new SaveData(MaleTeams.ToList(), FemaleTeams.ToList(), MixTeams.ToList());
                    
                    using (StreamWriter writer = new("tävling.json", false))
                    {
                        writer.Write(JsonConvert.SerializeObject(saveData));
                    }
                    using (StreamWriter writer = new("settings.dat", false))
                    {
                        writer.Write(UserSettings.ToString());
                    }
                    MessageBox.Show("Data sparad!", "Sparad");
                    isSaved = true;
                }
            });

        private ICommand load = null!;
        /// <summary>
        /// Loads the data from a txt file in the application directory
        /// </summary>
        public ICommand Load =>
            load ??= load = new RelayCommand(() =>
            {
                string messageBoxText = "Vill du ladda tidigare data?";
                string caption = "Ladda";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                if (result == MessageBoxResult.Yes)
                {
                    MaleTeams.Clear();
                    FemaleTeams.Clear();
                    MixTeams.Clear();
                    SaveData loadData;
                    string[] Settings = new string[6];
                    using (StreamReader reader = new("tävling.json"))
                    {
                        loadData = JsonConvert.DeserializeObject<SaveData>(reader.ReadToEnd());
                    }
                    using (StreamReader reader = new("settings.dat"))
                    {
                        Settings = reader.ReadToEnd().Split(';');
                    }
                    foreach (Team team in loadData.SavedMaleTeams) MaleTeams.Add(team);
                    foreach (Team team in loadData.SavedFemaleTeams) FemaleTeams.Add(team);
                    foreach (Team team in loadData.SavedMixedTeams) MixTeams.Add(team);
                    Int32.TryParse(Settings[0], out UserSettings.PENELTYSECONDS);
                    Int32.TryParse(Settings[1], out UserSettings.DISQUALIFYING_LIMIT);
                    bool.TryParse(Settings[2], out UserSettings.MULTIPLEHEATS);
                    bool.TryParse(Settings[3], out UserSettings.PARTISAPATING_MALE);
                    bool.TryParse(Settings[4], out UserSettings.PARTISAPATING_FEMALE);
                    bool.TryParse(Settings[5], out UserSettings.PARTISAPATING_MIX);
                }
            });
        
        public MessageBoxResult result = MessageBoxResult.None;

        private ICommand exit = null!;
        /// <summary>
        /// Checks if the data has been saved and prompts the user if not so.
        /// Then exits the application if user chooses it.
        /// </summary>
        public ICommand Exit =>
            exit ??= exit = new RelayCommand(() =>
            {
                if (result == MessageBoxResult.None)
                {
                    if (!isSaved)
                    {
                        string messageBoxText = "Vill du verkligen avsluta utan att spara?";
                        string caption = "Avsluta";
                        MessageBoxButton button = MessageBoxButton.YesNo;
                        MessageBoxImage icon = MessageBoxImage.Warning;

                        result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                    }
                    else result = MessageBoxResult.Yes;

                    if (result == MessageBoxResult.Yes)
                    {
                        Application.Current.Shutdown();
                    }
                    else result = MessageBoxResult.None;
                }
            });
        
    }
}
