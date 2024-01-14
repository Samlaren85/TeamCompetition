using System.Windows;
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
    }
}
