using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalysWorldGenerator
{
    class Wind : Stream
    {

        static int windID = 0;
        public bool coldWind; // True is cold, false is hot
        public bool northern; // True is north hemisphere, false is south

        /// <summary>
        /// Creating a wind with some basic strength
        /// </summary>
        /// <param name="region"></param>
        public Wind(Region region, bool cold, bool north) : base(region)
        {
            //stream.Add(region);
            coldWind = cold;
            northern = north;
            region.winds.Add(this);
            windID++;
        }
    }
}
