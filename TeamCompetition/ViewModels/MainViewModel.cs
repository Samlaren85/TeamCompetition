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
            if (Team.Result != "" || Team.PeneltySummery<=UserSettings.DISQUALIFYING_LIMIT)
            {
                int placement = MaleTeams.Count;
                foreach (Team opponent in MaleTeams)
                {
                    if (opponent.Result != null)
                    {
                        if (opponent != Team && Team.Compare(opponent.Result))
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
            else Team.Placement = "DSQ";
        }

        private ICommand calculateResult = null!;
        /// <summary>
        /// Calculatets the results for each group in the competition.
        /// </summary>
        public ICommand CalculateResult =>
            calculateResult ??= calculateResult = new RelayCommand(() =>
            {
                if (SelectedTab.Contains("Herrar")) 
                {
                    foreach (Team t in MaleTeams) t.AddingResults();
                    foreach (Team t in MaleTeams) CalculatePlacement(t);
                }
                if (SelectedTab.Contains("Damer"))
                {
                    foreach (Team t in FemaleTeams) t.AddingResults();
                    foreach (Team t in FemaleTeams) CalculatePlacement(t);
                }
                if (SelectedTab.Contains("Herrar"))
                {
                    foreach (Team t in MixTeams) t.AddingResults();
                    foreach (Team t in MixTeams) CalculatePlacement(t);
                }
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
                if (SelectedTab.Contains("Herrar"))
                {
                    MaleTeams.Clear();
                }
                if (SelectedTab.Contains("Damer"))
                {
                    FemaleTeams.Clear();
                }
                if (SelectedTab.Contains("Mix"))
                {
                    MixTeams.Clear();
                }
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
                    using (StreamWriter writer = new("tävling.csv", false))
                    {
                        foreach (Team team in MaleTeams)
                        {
                            writer.WriteLine(team.ToString());
                        }
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
                    using (StreamReader reader = new("tävling.csv"))
                    {
                        MaleTeams.Clear();
                        string line;
                        
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] data = line.Split(';');
                            if (data.Length < 7)
                            {
                                string[] fillData = new string[7];
                                int i = 0;
                                foreach (string str in data)
                                {
                                    fillData[i] = str;
                                    i++;
                                }
                                data = fillData;
                            }
                            Team lag = new() { Name = data[0], Backstroke = data[1], Breaststroke = data[2], Butterfly = data[3], Crawl = data[4], Result = data[5], Placement = data[6] };
                            MaleTeams.Add(lag);
                        }
                    }
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
