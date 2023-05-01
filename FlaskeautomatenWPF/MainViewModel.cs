using System.ComponentModel;
using Flaskautomaten;
using System.Threading;
using Flaskeautomaten_WPF;
using System.Threading.Tasks;

namespace Flaskeautomaten_WPFVM
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int _sodaCount;
        private int _beerCount;
        private Program _program;

        public MainViewModel()
        {
            _program = new Program();
        }
        public int SodaCount
        {
            get => _sodaCount;
            set
            {
                _sodaCount = value;
                OnPropertyChanged(nameof(SodaCount));
            }
        }

        public int BeerCount
        {
            get => _beerCount;
            set
            {
                _beerCount = value;
                OnPropertyChanged(nameof(BeerCount));
            }
        }

        public async Task StartSimulation()
        {
            await _program.StartSimulation((message) =>
            {
                if (message.Contains("Soda"))
                {
                    SodaCount++;
                }
                else if (message.Contains("Beer"))
                {
                    BeerCount++;
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
