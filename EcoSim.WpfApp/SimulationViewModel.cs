using EcoSim.Core;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoSim.WpfApp
{
    public class SimulationViewModel : ViewModel
    {
        Simulation _simulation;

        public string[][] Report { get; set; }
        public PlotModel PricePlot { get; private set; }
        public PlotModel PriceLimitedPlot { get; private set; }
        public int Seed { get; set; }

        public SimulationViewModel(int seed)
        {
            Seed = seed;
            Restart();
        }

        public void Benchmark(int benchmarkRounds)
        {
            for (int i = 0; i < benchmarkRounds; i++)
                _simulation.Simulate();
            Plot();
        }

        public void Restart()
        {
            _simulation = new Simulation(Seed, new HashSet<Commodity>
            {
                new Commodity { Name = "food", Price = 5 },
                new Commodity { Name = "wood", Price = 5 },
                new Commodity { Name = "metal", Price = 10 },
                new Commodity { Name = "ore", Price = 15 },
                new Commodity { Name = "tools", Price = 20 }
            });

            Plot();
        }

        public void Advance()
        {
            _simulation.Simulate();
            Plot();
        }

        private void Plot()
        {
            Report = new Report(_simulation).GetData();
            PricePlot = GetPlot("Price", _simulation.PriceHistory.ToDictionary(p => p.Key.Name, p => p.Value));
            PriceLimitedPlot = GetPlot("Price Last 20", _simulation.PriceHistory.ToDictionary(p => p.Key.Name, p => p.Value), 20);

            OnPropertyChanged("Report");
            OnPropertyChanged("PricePlot");
            OnPropertyChanged("PriceLimitedPlot");
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
    }
}
