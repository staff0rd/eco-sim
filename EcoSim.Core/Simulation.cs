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
        public float Bounds { get; private set; }

        public Simulation(int seed, HashSet<Commodity> commodities, float bounds = 0.5f)
        {
            Seed = seed;
            RNG = new Random(Seed);
            Commodities = commodities;
            Init(bounds);
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
            commodity.Production = supply - demand;
            
            SupplyHistory[commodity].Add(supply);
            DemandHistory[commodity].Add(demand);
            VarianceHistory[commodity].Add(commodity.Production);

            //VaryByRatio(commodity, variance);
            VaryByIncrement(commodity);

            if (commodity.Price < MinimumPrice)
                commodity.Price = MinimumPrice;

            PriceHistory[commodity].Add(commodity.Price);
        }

        private void Init(float bounds)
        {
            PriceHistory = new Dictionary<Commodity, List<float>>();
            SupplyHistory = new Dictionary<Commodity, List<float>>();
            DemandHistory = new Dictionary<Commodity, List<float>>();
            VarianceHistory = new Dictionary<Commodity, List<float>>();
            foreach (var commodity in Commodities)
            {
                PriceHistory.Add(commodity, new List<float> { commodity.Price });
                SupplyHistory.Add(commodity, new List<float> { 0 });
                DemandHistory.Add(commodity, new List<float> { 0 });
                VarianceHistory.Add(commodity, new List<float> { 0 });
                commodity.BasePrice = commodity.Price;
                commodity.Bounds = bounds;
            }
        }

        private static void VaryByIncrement(Commodity commodity)
        {
            if (commodity.Production != 0)
                commodity.Price -= (int)(commodity.Production / Math.Abs(commodity.Production));

            Clamp(commodity);
        }

        private static void VaryByRatio(Commodity commodity, float variance)
        {
            if (variance > 0)
                commodity.Price += commodity.Price * (variance / commodity.Supply);

            if (variance < 0)
                commodity.Price -= commodity.Price * (Math.Abs(variance) / commodity.Demand);

            Clamp(commodity);
        }

        private static void Clamp(Commodity commodity)
        {
            if (commodity.Price > commodity.MaxPrice)
                commodity.Price = commodity.MaxPrice;

            if (commodity.Price < commodity.MinPrice)
                commodity.Price = commodity.MinPrice;
        }

        public float GetPrice(string commodityName)
        {
            var commodity = Commodities.SingleOrDefault(p => p.Name == commodityName);
            return commodity != null ? commodity.Price : 0;
        }
    }
}
