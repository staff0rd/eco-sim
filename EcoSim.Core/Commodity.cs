using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcoSim.Core
{
    public class Commodity
    {
        public string Name;

        public float Price;

        public float BasePrice;

        public float Supply = 100;

        public float Demand = 100;

        public float Production;

        public float Bounds;

        public float MaxPrice
        {
            get { return BasePrice + BasePrice * Bounds; }
        }

        public float MinPrice
        {
            get { return BasePrice - BasePrice * Bounds; }
        }
    }
}
