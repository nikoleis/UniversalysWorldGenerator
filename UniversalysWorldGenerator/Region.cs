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
        public int pixelSize = 1;
        public List<Region> neighbors = new List<Region>();
        public bool isMountain = false, isValley = false, isHilly = false, isVolcano;
        public bool isSea = false, isOceanTrench = false, isOcean = false;
        public bool isContinent = false, isLargeIsland = false, isSmallIsland = false;
        public bool isPolar = false, isTropical = false, isTemperate = false, isDesert = false;
        public List<Geology> geology = new List<Geology>();
        public List<River> rivers = new List<River>();
        public List<Wind> winds = new List<Wind>();
        public List<WaterCurrent> waterCurrents = new List<WaterCurrent>();
        public int riverStream = 0;
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

        int CalculateX(int regionXToCorrect, int regionXToCompare)
        {
            int correctedX = regionXToCorrect;

            if ((regionXToCompare < WorldMap.LONGITUDE / 5) && (regionXToCorrect > 4 * WorldMap.LONGITUDE / 5))
            {
                correctedX -= WorldMap.LONGITUDE;
            }
            else if ((regionXToCompare > 4 * WorldMap.LONGITUDE / 5) && (regionXToCorrect < WorldMap.LONGITUDE / 5))
            {
                correctedX += WorldMap.LONGITUDE;
            }

            return correctedX;
        }

        public bool HasWesternMountain()
        {

            foreach (Region neighbor in neighbors)
            {
                int correctedX;
                int i = 0;
                
                while(i < neighbors.Count)
                {
                    correctedX = CalculateX(neighbors[i].X, X);
                    if(correctedX < X && neighbors[i].isMountain)
                    {
                        return true;
                    }
                    i++;
                }
            }
            return false;
        }

        public bool HasEasternMountain()
        {

            foreach (Region neighbor in neighbors)
            {
                int correctedX;
                int i = 0;

                while (i < neighbors.Count)
                {
                    correctedX = CalculateX(neighbors[i].X, X);
                    if (correctedX > X && neighbors[i].isMountain)
                    {
                        return true;
                    }
                    i++;
                }
            }
            return false;
        }

        public bool HasWesternWater()
        {

            foreach (Region neighbor in neighbors)
            {
                int correctedX;
                int i = 0;

                while (i < neighbors.Count)
                {
                    correctedX = CalculateX(neighbors[i].X, X);
                    if (correctedX < X && neighbors[i].IsWater())
                    {
                        return true;
                    }
                    i++;
                }
            }
            return false;
        }

        public bool HasEasternWater()
        {

            foreach (Region neighbor in neighbors)
            {
                int correctedX;
                int i = 0;

                while (i < neighbors.Count)
                {
                    correctedX = CalculateX(neighbors[i].X, X);
                    if (correctedX > X && neighbors[i].IsWater())
                    {
                        return true;
                    }
                    i++;
                }
            }
            return false;
        }

        public bool IsEastFrom(Region region)
        {
            int correctedX = CalculateX(region.X, X);

            if (correctedX < X)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsWestFrom(Region region)
        {
            int correctedX = CalculateX(region.X, X);

            if (correctedX > X)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsNorthFrom(Region region)
        {
            if (region.Y < Y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsSouthFrom(Region region)
        {
            if (region.Y < Y)
            {
                return true;
            }
            else
            {
                return false;
            }
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
