using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalysWorldGenerator
{
    class River : Stream
    {

        static int riverID = 0;


        public River(Region region) : base(region)
        {
            stream.Add(region);
            region.waterCurrent = dice.Next(3, 6);
            region.rivers.Add(this);
            riverID++;
        }

        public bool GenerateRiver(Region region)
        {
            int rand;
            
            rand = dice.Next(region.humidity / 3, region.humidity * 3) / 100;
            region.waterCurrent = rand + stream.Last().waterCurrent;
            
            if(region.rivers.Count == 0)
            {
                region.rivers.Add(this);
                stream.Add(region);
                return false;
            }
            else
            {
                region.rivers.Add(this);
                AddRiverCurrent(region, stream.Last().waterCurrent);
                stream.Add(region);
                return true;
            }
        }


        public void AddRiverCurrent(Region region, int flow)
        {
            bool findRiver = false;
            int i = 0;
            River mainRiver = region.rivers.First();

            while (!findRiver)
            {
                if(region == mainRiver.stream[i])
                {
                    findRiver = true;
                }
                else
                {
                    i++;
                }
            }

            while (i < mainRiver.stream.Count)
            {
                mainRiver.stream[i].waterCurrent += flow;
                i++;
            }
        }
    }
}
