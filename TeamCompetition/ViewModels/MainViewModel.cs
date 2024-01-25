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
        private ObservableCollection<Lag> teamsHerrar = new ObservableCollection<Lag>();
        public ObservableCollection<Lag> TeamsHerrar
        { 
            get { return teamsHerrar; } 
            set { 
                teamsHerrar = value;
                OnPropertyChanged();
                isSaved = false;
            }
        }

        private ObservableCollection<Lag> teamsDamer = new ObservableCollection<Lag>();
        public ObservableCollection<Lag> TeamsDamer
        {
            get { return teamsDamer; }
            set
            {
                teamsDamer = value;
                OnPropertyChanged();
                isSaved = false;
            }
        }

        private ObservableCollection<Lag> teamsMix = new ObservableCollection<Lag>();
        public ObservableCollection<Lag> TeamsMix
        {
            get { return teamsMix; }
            set
            {
                teamsMix = value;
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

        private Lag selectedItem;
        public Lag SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
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

        private void RäknaUtPlacering(Lag Team)
        {
            if (Team.Summering != "")
            {
                int placering = TeamsHerrar.Count;
                foreach (Lag motståndare in TeamsHerrar)
                {
                    if (motståndare.Summering != null)
                    {
                        if (motståndare != Team && Team.Compare(motståndare.Summering))
                        {
                            placering--;
                        }
                    }
                    else
                    {
                        placering--;
                    }
                }
                if (UserSettings.FLERAHEAT || placering <= 6) Team.Placering = placering.ToString();
                else Team.Placering = "DNQ";
            }
            else Team.Placering = "DNQ";
        }

        private ICommand summera = null!;
        public ICommand Summera =>
            summera ??= summera = new RelayCommand(() =>
            {
                if (SelectedTab.Contains("Herrar")) 
                {
                    foreach (Lag t in TeamsHerrar) t.SummeraResultat();
                    foreach (Lag t in TeamsHerrar) RäknaUtPlacering(t);
                }
                if (SelectedTab.Contains("Damer"))
                {
                    foreach (Lag t in TeamsDamer) t.SummeraResultat();
                    foreach (Lag t in TeamsDamer) RäknaUtPlacering(t);
                }
                if (SelectedTab.Contains("Herrar"))
                {
                    foreach (Lag t in TeamsMix) t.SummeraResultat();
                    foreach (Lag t in TeamsMix) RäknaUtPlacering(t);
                }
                OnPropertyChanged();
            });

        private ICommand adderaTillägg = null!;
        public ICommand AdderaTillägg =>
            adderaTillägg ??= adderaTillägg = new RelayCommand(() =>
            {
                if (SelectedColumn == 1) SelectedItem.TilläggRygg++;
                else if (SelectedColumn == 3) SelectedItem.TilläggBröst++;
                else if (SelectedColumn == 5) SelectedItem.TilläggFjäril++;
                else if (SelectedColumn == 7) SelectedItem.TilläggFrisim++;
                OnPropertyChanged();
            });

        private ICommand tabortTillägg = null!;
        public ICommand TabortTillägg =>
            tabortTillägg ??= tabortTillägg = new RelayCommand(() =>
            {
                if (SelectedColumn == 1) SelectedItem.TilläggRygg--;
                else if (SelectedColumn == 3) SelectedItem.TilläggBröst--;
                else if (SelectedColumn == 5) SelectedItem.TilläggFjäril--;
                else if (SelectedColumn == 7) SelectedItem.TilläggFrisim--;
                OnPropertyChanged();
            });

        private ICommand rensa = null!;
        public ICommand Rensa =>
            rensa ??= rensa = new RelayCommand(() =>
            {
                if (SelectedTab.Contains("Herrar"))
                {
                    TeamsHerrar.Clear();
                }
                if (SelectedTab.Contains("Damer"))
                {
                    TeamsDamer.Clear();
                }
                if (SelectedTab.Contains("Mix"))
                {
                    TeamsMix.Clear();
                }
                OnPropertyChanged();
            });

        private ICommand save = null!;
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
                        foreach (Lag lag in TeamsHerrar)
                        {
                            writer.WriteLine(lag.ToString());
                        }
                    }
                    MessageBox.Show("Data sparad!", "Sparad");
                    isSaved = true;
                }
            });

        private ICommand load = null!;
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
                        TeamsHerrar.Clear();
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
                            Lag lag = new() { Namn = data[0], Ryggsim = data[1], Bröstsim = data[2], Fjärilssim = data[3], Frisim = data[4], Summering = data[5], Placering = data[6] };
                            TeamsHerrar.Add(lag);
                        }
                    }
                }
            });
        
        public MessageBoxResult result = MessageBoxResult.None;

        private ICommand exit = null!;
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
