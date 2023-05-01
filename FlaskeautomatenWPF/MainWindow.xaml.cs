using Flaskautomaten;
using System;
using System.Windows;
using Flaskeautomaten_WPFVM;
using System.Threading.Tasks;

namespace Flaskeautomaten_WPF
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;
        private Program _program;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            _program = new Program();
        }

        private void StartSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                _program.StartSimulation(OutputCallback);
            });
        }


        private void OutputCallback(string message)
        {
            // Use Dispatcher to update UI from a non-UI thread
            Dispatcher.Invoke(() =>
            {
                // Parse the message and update the ViewModel accordingly
                if (message.Contains("SodaConsumer har consumeret"))
                {
                    _viewModel.SodaCount++;
                }
                else if (message.Contains("BeerConsumer har consumeret"))
                {
                    _viewModel.BeerCount++;
                }
            });
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
