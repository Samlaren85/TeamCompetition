using System.Windows;
using System.Xml.Serialization;
using TeamCompetition.Models;
using TeamCompetition.ViewModels;

namespace TeamCompetition.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainViewModel mainViewModel = (MainViewModel)DataContext;
            if (mainViewModel.Exit.CanExecute(null))
                mainViewModel.Exit.Execute(null);
            e.Cancel = true;
        }

        private void Window_Activated(object sender, System.EventArgs e)
        {
            //Visar flikarna för de aktiva deltagarna
            if (UserSettings.PARTISAPATING_MALE) tabHerrar.Visibility = Visibility.Visible;
            else tabHerrar.Visibility = Visibility.Collapsed;
            if (UserSettings.PARTISAPATING_FEMALE) tabDamer.Visibility = Visibility.Visible;
            else tabDamer.Visibility = Visibility.Collapsed;
            if (UserSettings.PARTISAPATING_MIX) tabMix.Visibility = Visibility.Visible;
            else tabMix.Visibility = Visibility.Collapsed;
        }
    }
}
