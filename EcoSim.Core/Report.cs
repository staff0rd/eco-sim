using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcoSim.Core
{
    public class Report
    {
        Simulation _simulation;

        public Report(Simulation simulation)
        {
            _simulation = simulation;
        }

        public string[][] GetData()
        {
            var result = new List<string[]>();
            result.Add(ToRow(null, _simulation.Commodities.Select(p => p.Name)));
            var data = _simulation.Commodities.Select(p => p.Price.ToString("N2"));
            result.Add(ToRow("Base Price", data));
            return result.ToArray();
        }

        private static string[] ToRow(string title, IEnumerable<string> data)
        {
            return Enumerable.Repeat<string>(title, 1).Concat(data).ToArray();
        }
    }
}
