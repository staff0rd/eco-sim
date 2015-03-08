using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcoSim.Core
{
    public class Simulation
    {
        public HashSet<Commodity> Commodities { get; private set; }

        public Dictionary<Commodity, List<float>> PriceHistory;
        public Dictionary<Commodity, List<float>> SupplyHistory;
        public Dictionary<Commodity, List<float>> DemandHistory;
        public Dictionary<Commodity, List<float>> VarianceHistory;

        public int Rounds = 0;
        public Random RNG;
        public float MinimumPrice = 9;
        public int Seed { get; private set; }

        public Simulation(int seed, HashSet<Commodity> commodities)
        {
            Seed = seed;
            RNG = new Random(Seed);
            Commodities = commodities;
            Init();
        }

        private void Init()
        {
            PriceHistory = new Dictionary<Commodity,List<float>>();
            SupplyHistory = new Dictionary<Commodity, List<float>>();
            DemandHistory = new Dictionary<Commodity, List<float>>();
            VarianceHistory = new Dictionary<Commodity, List<float>>();
            foreach (var commodity in Commodities)
            {
                PriceHistory.Add(commodity, new List<float> { commodity.Price });
                SupplyHistory.Add(commodity, new List<float> { 0 });
                DemandHistory.Add(commodity, new List<float> { 0 });
                VarianceHistory.Add(commodity, new List<float> { 0 });
            }
        }

        public void Simulate()
        {
            Rounds++;
            foreach (var commodity in Commodities)
                Simulate(commodity);
        }

        public void Simulate(int rounds)
        {
            for (int i = 0; i < rounds; i++)
                Simulate();
        }

        private void Simulate(Commodity commodity)
        {
            var supply = (float)RNG.Next(0, (int)commodity.Supply);
            var demand = (float)RNG.Next(0, (int)commodity.Demand);
            var variance = supply - demand;
            SupplyHistory[commodity].Add(supply);
            DemandHistory[commodity].Add(demand);
            VarianceHistory[commodity].Add(variance);

            //ByRatio(commodity, variance);
            VaryByIncrement(commodity, variance);

            if (commodity.Price < MinimumPrice)
                commodity.Price = MinimumPrice;

            PriceHistory[commodity].Add(commodity.Price);
        }

        private void VaryByIncrement(Commodity commodity, float variance)
        {
            if (variance > 0)
                commodity.Price--;

            if (variance < 0)
                commodity.Price++;
        }

        private static void VaryByRatio(Commodity commodity, float variance)
        {
            if (variance > 0)
                commodity.Price += commodity.Price * (variance / commodity.Supply);

            if (variance < 0)
                commodity.Price -= commodity.Price * (Math.Abs(variance) / commodity.Demand);
        }
    }
}
