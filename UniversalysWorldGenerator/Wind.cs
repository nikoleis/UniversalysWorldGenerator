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

        /// <summary>
        /// Creating a wind with some basic strength
        /// </summary>
        /// <param name="region"></param>
        public Wind(Region region) : base(region)
        {
            stream.Add(region);
            region.waterCurrent = dice.Next(15, 25);
            region.winds.Add(this);
            windID++;
        }

        /// <summary>
        /// The fuction is called so to push the river forward
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public bool GenerateWind(Region region)
        {
            int rand;

            rand = dice.Next(region.humidity / 2, region.humidity * 2) / 100;
            region.waterCurrent = rand + stream.Last().windFlow;

            if (region.rivers.Count == 0)
            {
                region.winds.Add(this);
                stream.Add(region);
                return false;
            }
            else
            {
                region.winds.Add(this);
                AddWindFlow(region, stream.Last().windFlow);
                stream.Add(region);
                return true;
            }
        }

        /// <summary>
        /// Checking if a region meets another river and adds the flow to the other
        /// </summary>
        /// <param name="region"></param>
        /// <param name="flow"></param>
        public void AddWindFlow(Region region, int flow)
        {
            bool findRiver = false;
            int i = 0;
            Wind mainWind = region.winds.First();

            while (!findRiver)
            {
                if (region == mainWind.stream[i])
                {
                    findRiver = true;
                }
                else
                {
                    i++;
                }
            }

            while (i < mainWind.stream.Count)
            {
                mainWind.stream[i].windFlow += flow;
                i++;
            }
        }
    }
}
