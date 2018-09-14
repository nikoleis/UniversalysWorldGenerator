using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

        //TODO : STOCK ALL GEOLOGIES IN DATABASE
namespace UniversalysWorldGenerator
{
    class Region
    {
        public int X, Y, ID;
        public int height = 0, temperature = 0, humidity = 0;
        public int continentID;
        public List<Region> neighbors = new List<Region>();
        public bool isMountainRange = false, isOcean = false, isContinent = false, isHilly = false, isSea = false, isLargeIsland = false, isOceanTrench = false, isSmallIsland = false;
        public bool isPolar = false, isTropical = false, isTemperate = false, isDesert = false;
        public List<Geology> geology = new List<Geology>();
        public List<River> rivers = new List<River>();
        public List<Wind> winds = new List<Wind>();
        public int waterCurrent = 0;
        public Geosphere naturalResource = new Geosphere();

        public Region(int x, int y, int id)
        {
            X = x;
            Y = y;
            ID = id;
        }

        public bool IsWater()
        {
            return (height < 0);
        }

        public bool IsLand()
        {
            return (height > 0);
        }

        public bool IsLandlocked()
        {
            int i = 0;

            while(i < neighbors.Count)
            {
                if (neighbors[i].IsWater())
                {
                    return false;
                }
                i++;
            }
            return true;
        }

        int CalculateX(int regionXToCorrect, int regionXToCompare, int LONGITUDE)
        {
            int correctedX = regionXToCorrect;

            if ((regionXToCompare < LONGITUDE / 5) && (regionXToCorrect > 4 * LONGITUDE / 5))
            {
                correctedX -= LONGITUDE;
            }
            else if ((regionXToCompare > 4 * LONGITUDE / 5) && (regionXToCorrect < LONGITUDE / 5))
            {
                correctedX += LONGITUDE;
            }

            return correctedX;
        }

        public bool HasWesternMountain(int LONGITUDE)
        {

            foreach (Region neighbor in neighbors)
            {
                int correctedX;
                int i = 0;
                
                while(i < neighbors.Count)
                {
                    correctedX = CalculateX(neighbors[i].X, X, LONGITUDE);
                    if(correctedX < X && neighbors[i].isMountainRange)
                    {
                        return true;
                    }
                    i++;
                }
            }
            return false;
        }

        public bool HasEasternMountain(int LONGITUDE)
        {

            foreach (Region neighbor in neighbors)
            {
                int correctedX;
                int i = 0;

                while (i < neighbors.Count)
                {
                    correctedX = CalculateX(neighbors[i].X, X, LONGITUDE);
                    if (correctedX > X && neighbors[i].isMountainRange)
                    {
                        return true;
                    }
                    i++;
                }
            }
            return false;
        }

        public bool HasWesternWater(int LONGITUDE)
        {

            foreach (Region neighbor in neighbors)
            {
                int correctedX;
                int i = 0;

                while (i < neighbors.Count)
                {
                    correctedX = CalculateX(neighbors[i].X, X, LONGITUDE);
                    if (correctedX < X && neighbors[i].IsWater())
                    {
                        return true;
                    }
                    i++;
                }
            }
            return false;
        }

        public bool HasEasternWater(int LONGITUDE)
        {

            foreach (Region neighbor in neighbors)
            {
                int correctedX;
                int i = 0;

                while (i < neighbors.Count)
                {
                    correctedX = CalculateX(neighbors[i].X, X, LONGITUDE);
                    if (correctedX > X && neighbors[i].IsWater())
                    {
                        return true;
                    }
                    i++;
                }
            }
            return false;
        }

        public int MeanHeight()
        {
            // This fuction calculates the mean between a region's height and the sum of its surrounding regions, allowing us to get a sense of direction in water flow
            int neighborHeight = 0;

            foreach (Region neighbor in neighbors)
            {
                neighborHeight += neighbor.height;
            }

            return (height + (neighborHeight / neighbors.Count)) / 2;
        }

 

    }
}
