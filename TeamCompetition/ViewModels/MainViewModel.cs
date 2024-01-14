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
        private ObservableCollection<Lag> teams = new ObservableCollection<Lag>();
        public ObservableCollection<Lag> Teams
        { 
            get { return teams; } 
            set { 
                teams = value;
                OnPropertyChanged();
                isSaved = false;
            }
        }

        protected bool isSaved;

        private void RäknaUtPlacering()
        {
            foreach(Lag lag in Teams)
            {
                if (lag.Summering != "")
                {
                    int placering = Teams.Count;
                    foreach (Lag motståndare in Teams)
                    {
                        if (motståndare.Summering != null)
                        {
                            if (motståndare != lag && lag.Compare(motståndare.Summering))
                            {
                                placering--;
                            }
                        }
                        else
                        {
                            placering--;
                        }
                    }
                    if (placering <= 6) lag.Placering = placering.ToString();
                    else lag.Placering = "DNQ";
                }
                else lag.Placering = "DNQ";
            }
        }

        private ICommand summera = null!;
        public ICommand Summera =>
            summera ??= summera = new RelayCommand(() =>
            {
                foreach (Lag t in teams) t.SummeraResultat();
                RäknaUtPlacering();
                OnPropertyChanged();
            });

        private ICommand rensa = null!;
        public ICommand Rensa =>
            rensa ??= rensa = new RelayCommand(() =>
            {
                teams.Clear();
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
                        foreach (Lag lag in Teams)
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
                        Teams.Clear();
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
                            Teams.Add(lag);
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
