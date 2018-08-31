using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalysWorldGenerator
{
    class River : Stream
    {

        List<int> waterCurrent = new List<int>();

        public River(int id) : base(id)
        {
            stream.Add(id);
            waterCurrent.Add(dice.Next(3, 6));
        }

        public void ProlongRiver(Region region)
        {
            int rand;

            stream.Add(region.ID);
            rand = dice.Next(region.humidity / 2, region.humidity * 2);
            waterCurrent.Add(dice.Next(0, 2) + waterCurrent.Last());
        }
    }
}
