using EcoSim.Core;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace EcoSim.WpfApp
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ICommand AdvanceCommand { get; private set; }
        public ICommand BenchmarkCommand { get; private set; }
        public ICommand RestartCommand { get; set; }
        public ICommand RestartBenchmarkCommand { get; set; }

        public int Seed { get; set; }
        public int BenchmarkRounds { get; set; }

        public string[][] Report { get; set; }
        public PlotModel PricePlot { get; private set; }
        public PlotModel PriceLimitedPlot { get; private set; }
        public PlotModel DemandPlot { get; private set; }
        public PlotModel VariancePlot { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        Simulation _simulation;

        public ViewModel()
        {
            BenchmarkRounds = 50000;
            OnPropertyChanged("BenchmarkRounds");

            InitCommands();

            Restart();
        }

        private void InitCommands()
        {
            AdvanceCommand = new RelayCommand(() => Advance());
            BenchmarkCommand = new RelayCommand(() => Benchmark());
            RestartCommand = new RelayCommand(() => Restart());
            RestartBenchmarkCommand = new RelayCommand(() => RestartAndBenchmark());
        }

        private void RestartAndBenchmark()
        {
            Restart();
            Benchmark();
        }

        private void Benchmark()
        {
            for (int i = 0; i < BenchmarkRounds; i++)
                _simulation.Simulate();
            Plot();
        }

        private void Restart()
        {
            _simulation = new Simulation(Seed, new HashSet<Commodity>
            {
                new Commodity { Name = "food", Price = 5 },
                new Commodity { Name = "wood", Price = 5 },
                new Commodity { Name = "metal", Price = 15 },
                new Commodity { Name = "ore", Price = 10 },
                new Commodity { Name = "tools", Price = 20 }
            });

            Plot();
        }

        private void Advance()
        {
            _simulation.Simulate();
            Plot();
        }

        private void Plot()
        {
            Report = new Report(_simulation).GetData();
            PricePlot = GetPlot("Price", _simulation.PriceHistory.ToDictionary(p => p.Key.Name, p => p.Value));
            PriceLimitedPlot = GetPlot("Price Last 20", _simulation.PriceHistory.ToDictionary(p => p.Key.Name, p => p.Value), 20);
            //DemandPlot = GetPlot("Demand", _simulation.DemandHistory.ToDictionary(p => p.Key.Name, p => p.Value));
            //VariancePlot = GetPlot("Variance", _simulation.VarianceHistory.ToDictionary(p => p.Key.Name, p => p.Value));

            OnPropertyChanged("Report");
            OnPropertyChanged("PricePlot");
            OnPropertyChanged("PriceLimitedPlot");
            OnPropertyChanged("DemandPlot");
            OnPropertyChanged("VariancePlot");
        }

        private static PlotModel GetPlot(string title, Dictionary<string, List<float>> dictionary, int limit = 99999999)
        {
            var plot = new PlotModel { Title = title };
            foreach (var key in dictionary.Keys)
            {
                var list = dictionary[key];
                var series = new LineSeries { Title = key };
                var skip = dictionary[key].Count - limit;
                if (skip > 0)
                    list = list.Skip(skip).Take(limit).ToList();
                for (int i = 0; i < list.Count; i++)
                    series.Points.Add(new DataPoint(i, list[i]));
                plot.Series.Add(series);
            }
            return plot;
        }

        protected void OnPropertyChanged(string property)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
