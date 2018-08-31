using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalysWorldGenerator
{
    class Region
    {
        public int X, Y, ID;
        public int height = 0, temperature = 0, humidity = 0;
        public int continentID;
        public List<int> neighbors = new List<int>();
        public bool isMountainRange = false, isOcean = false, isContinent = false, isHilly = false, isSea = false, isLargeIsland = false, isOceanTrench;
        public bool isPolar = false, isTropical = false, isTemperate = false, isDesert = false;
        public bool hasRiver = false, hasMarineStream = false, hasWind = false;
        //TODO : STOCK ALL GEOLOGIES IN DATABASE
        public List<Geology> geology = new List<Geology>();
        public Geosphere naturalResource = new Geosphere();

        public Region(int x, int y)
        {
            X = x;
            Y = y;            
        }

        public Region(int x, int y, int id)
        {
            X = x;
            Y = y;
            ID = id;
        }

        public bool isWater()
        {
            return (height < 0);
        }

        public bool isLand()
        {
            return (height > 0);
        }

    }
}
