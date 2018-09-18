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

        /// <summary>
        /// Creating a river sets a minimum flow
        /// </summary>
        /// <param name="region"></param>
        public River(Region region) : base(region)
        {
            //stream.Add(region);
            region.riverStream = dice.Next(3, 6);
            region.rivers.Add(this);
            riverID++;
        }

        /// <summary>
        /// The fuction is called so to push the river forward
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public bool GenerateRiver(Region region)
        {
            int rand;
            
            rand = dice.Next(region.humidity / 2, region.humidity * 2) / 100;
            region.riverStream = rand + stream.Last().riverStream;
            
            if(region.rivers.Count == 0)
            {
                region.rivers.Add(this);
                stream.Add(region);
                return false;
            }
            else
            {
                region.rivers.Add(this);
                AddRiverCurrent(region, stream.Last().riverStream);
                stream.Add(region);
                return true;
            }
        }

        /// <summary>
        /// Checking if a region meets another river and adds the flow to the other
        /// </summary>
        /// <param name="region"></param>
        /// <param name="flow"></param>
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
                mainRiver.stream[i].riverStream += flow;
                i++;
            }
        }
    }
}
