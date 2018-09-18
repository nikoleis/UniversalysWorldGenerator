using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalysWorldGenerator
{


    class WaterCurrent : Stream
    {
        static int waterCurrentID = 0;
        public bool coldWater; // True is cold, false is hot

        public WaterCurrent(Region region, bool cold) : base(region)
        {
            //stream.Add(region);
            coldWater = cold;
            region.waterCurrents.Add(this);
            waterCurrentID++;
        }
    }
}
