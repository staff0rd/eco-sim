using EcoSim.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace EcoSim.WpfApp
{
    public class MainViewModel : ViewModel
    {
        public ICommand AdvanceCommand { get; private set; }
        public ICommand BenchmarkCommand { get; private set; }
        public ICommand RestartCommand { get; set; }
        public ICommand RestartBenchmarkCommand { get; set; }
        public ICommand AddSimulationCommand { get; private set; }

        public int BenchmarkRounds { get; set; }
        public ObservableCollection<SimulationViewModel> Simulations { get; private set; }

        public MainViewModel()
        {
            Simulations = new ObservableCollection<SimulationViewModel>();
            BenchmarkRounds = 50000;
            InitCommands();
            Restart();
            OnPropertyChanged("BenchmarkRounds");
            OnPropertyChanged("Simulations");
        }

        private void Restart()
        {
            Simulations.ToList().ForEach(p => p.Restart());
        }

        private void InitCommands()
        {
            AdvanceCommand = new RelayCommand(() => Advance());
            BenchmarkCommand = new RelayCommand(() => Benchmark());
            RestartCommand = new RelayCommand(() => Restart());
            RestartBenchmarkCommand = new RelayCommand(() => RestartAndBenchmark());
            AddSimulationCommand = new RelayCommand(() => AddSimulation());
        }

        private void AddSimulation()
        {
            Simulations.Add(new SimulationViewModel(Simulations.Count));
        }

        private void Benchmark()
        {
            Simulations.ToList().ForEach(p => p.Benchmark(BenchmarkRounds));
        }

        private void RestartAndBenchmark()
        {
            Simulations.ToList().ForEach(p => p.Restart());
            Benchmark();
        }

        private void Advance()
        {
            Simulations.ToList().ForEach(p => p.Advance());
        }
    }
}
