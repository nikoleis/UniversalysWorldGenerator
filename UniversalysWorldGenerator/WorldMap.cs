using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Drawing;

namespace UniversalysWorldGenerator
{
    class WorldMap
    {
        public static int LATITUDE = 800, LONGITUDE = 1200, REGION = 7000, MASS = 32;
        public static int OCEAN, SEA, ISLAND, VALLEY, COASTMOUNTAIN, MOUNTAIN;
        public Bitmap mapPaint = new Bitmap(LONGITUDE, LATITUDE);
        // TODO : STOCK MAPMASK IN DATABASE
        public int[,] mapMask = new int[LONGITUDE, LATITUDE];
        public Region[] regionList = new Region[REGION];
        Random dice = new Random();
        public List<Region> notTreatedRegions = new List<Region>();
        public List<River> riverList = new List<River>();
        public List<Wind> windList = new List<Wind>();
        public List<WaterCurrent> waterCurrentList = new List<WaterCurrent>();
        int generationOrder = 0;

        public WorldMap()
        {
        }

        #region Regions
        /// <summary>
        /// Generate the region mask
        /// </summary>
        public void GenerateRegions()
        {

            int i = 0, j = 0, rand;
            Point current = new Point();
            Point next;
            Point neighbor;
            Region region;
            List<Point> iterativePoint = new List<Point>();
            int numNeighbors = 0;
            int[] regionNeighbors = new int[4];
            int[] regionNeighborsSize = new int[4];
            bool alreadyAdded;
            int maxSizeNeighbors;

            // Initializing the mapMask to -1 as we want to use 0 for the first regionID
            for (int k = 0; k < LONGITUDE; k++)
            {
                for (int l = 0; l < LATITUDE; l++)
                {
                    mapMask[k, l] = -1;
                }
            }

            while (i < REGION)
            {
                // These two conditions are testing to get more region in the center band, to compensate for map distorsion due to the flat representation.
                // It is very basic and only skew probabilities, not force these out
                current.X = dice.Next(0, LONGITUDE);
                current.Y = dice.Next(((i / REGION) * LATITUDE) / 5, LATITUDE - ((i / REGION) * LATITUDE) / 5);

                // Filling the map with the various starting points. The neighbor points are also locked out to allow for some minimal distance between points.
                // Note that if a point lands on a blocked area, it won't generate a region, which allows to slightly vary their final number from map to map
                // We make sure to not get out of the map, however. Also we cannot add an element that already is part of the queue there
                // One note: no region is cycling around the poles, as we consider our map finishing a little before these. This simplify some calculations!
                if (mapMask[current.X, current.Y] == -1)
                {
                    mapMask[current.X, current.Y] = i;

                    if (current.X > 0)
                    {
                        mapMask[current.X - 1, current.Y] = -2;
                        neighbor = new Point(current.X - 1, current.Y);
                        iterativePoint.Add(neighbor);
                    }
                    else
                    {
                        mapMask[LONGITUDE - 1, current.Y] = -2;
                        neighbor = new Point(LONGITUDE - 1, current.Y);
                        iterativePoint.Add(neighbor);
                    }
                    if (current.X < LONGITUDE - 1)
                    {
                        mapMask[current.X + 1, current.Y] = -2;
                        neighbor = new Point(current.X + 1, current.Y);
                        iterativePoint.Add(neighbor);
                    }
                    else
                    {
                        mapMask[0, current.Y] = -2;
                        neighbor = new Point(0, current.Y);
                        iterativePoint.Add(neighbor);
                    }
                    if (current.Y > 0)
                    {
                        mapMask[current.X, current.Y - 1] = -2;
                        neighbor = new Point(current.X, current.Y - 1);
                        iterativePoint.Add(neighbor);
                    }
                    if (current.Y < LATITUDE - 1)
                    {
                        mapMask[current.X, current.Y + 1] = -2;
                        neighbor = new Point(current.X, current.Y + 1);
                        iterativePoint.Add(neighbor);
                    }
                    // We populate the region list here, with the coordinate of its center in (X, Y) and attribute it an ID
                    region = new Region(current.X, current.Y, i);
                    regionList[i] = region;
                    i++;
                }
            }

            // Now for the filling of the void. On each member of iterativePoint, we test the neighbors and have a probability to convert it to a neighboring region.
            // If not converted, we add it again at the end of the queue, and we use this to ensure our regions aren't too regular that way.
            // Due to how we fill the map, no space shall be forgotten. We however have to check there for duplicates in the queue this time.
            while (iterativePoint.Count > 0)
            {
                numNeighbors = 0;
                rand = dice.Next(0, iterativePoint.Count);
                current = iterativePoint[rand];
                iterativePoint.RemoveAt(rand);

                i = 0;
                while (i < 4)
                {
                    regionNeighbors[i] = -1;
                    i++;
                }

                // We first create a small array of neighboring pixel regions, if those exist
                // If not we add the empty neighbor to a list of points to fill IF we fill this point
                if (current.X > 0)
                {
                    if (mapMask[current.X - 1, current.Y] >= 0)
                    {
                        regionNeighbors[numNeighbors] = mapMask[current.X - 1, current.Y];
                        numNeighbors++;
                    }
                    else if (mapMask[current.X - 1, current.Y] == -1)
                    {
                        mapMask[current.X - 1, current.Y] = -2;
                        next = new Point(current.X - 1, current.Y);
                        iterativePoint.Add(next);
                    }
                }
                else
                {
                    if (mapMask[LONGITUDE - 1, current.Y] >= 0)
                    {
                        regionNeighbors[numNeighbors] = mapMask[LONGITUDE - 1, current.Y];
                        numNeighbors++;
                    }
                    else if (mapMask[LONGITUDE - 1, current.Y] == -1)
                    {
                        mapMask[LONGITUDE - 1, current.Y] = -2;
                        next = new Point(LONGITUDE - 1, current.Y);
                        iterativePoint.Add(next);
                    }
                }
                if (current.X < LONGITUDE - 1)
                {
                    if (mapMask[current.X + 1, current.Y] >= 0)
                    {
                        regionNeighbors[numNeighbors] = mapMask[current.X + 1, current.Y];
                        numNeighbors++;
                    }
                    else if (mapMask[current.X + 1, current.Y] == -1)
                    {
                        mapMask[current.X + 1, current.Y] = -2;
                        next = new Point(current.X + 1, current.Y);
                        iterativePoint.Add(next);
                    }
                }
                else
                {
                    if (mapMask[0, current.Y] >= 0)
                    {
                        regionNeighbors[numNeighbors] = mapMask[0, current.Y];
                        numNeighbors++;
                    }
                    else if (mapMask[0, current.Y] == -1)
                    {
                        mapMask[0, current.Y] = -2;
                        next = new Point(0, current.Y);
                        iterativePoint.Add(next);
                    }
                }
                if (current.Y > 0)
                {
                    if (mapMask[current.X, current.Y - 1] >= 0)
                    {
                        regionNeighbors[numNeighbors] = mapMask[current.X, current.Y - 1];
                        numNeighbors++;
                    }
                    else if (mapMask[current.X, current.Y - 1] == -1)
                    {
                        mapMask[current.X, current.Y - 1] = -2;
                        next = new Point(current.X, current.Y - 1);
                        iterativePoint.Add(next);
                    }
                }
                if (current.Y < LATITUDE - 1)
                {
                    if (mapMask[current.X, current.Y + 1] >= 0)
                    {
                        regionNeighbors[numNeighbors] = mapMask[current.X, current.Y + 1];
                        numNeighbors++;
                    }
                    else if (mapMask[current.X, current.Y + 1] == -1)
                    {
                        mapMask[current.X, current.Y + 1] = -2;
                        next = new Point(current.X, current.Y + 1);
                        iterativePoint.Add(next);
                    }
                }


                // Once done, we fill it. For this we use the size of the neighboring regions and randoml select among these taking the smallest in priority
                /*
                 00000   00000   00003   01033   11333  
                 00000   00003   01133   11133   11133
                 00103 > 01113 > 01113 > 11113 > 11113   Example of what we would see in execution
                 00000   00120   01123   11123   11123
                 00020   00222   02222   22222   22222

                 00000   00000   00103   01133   11133  
                 00000   00103   01113   11113   11113
                 00103 > 01133 > 11133 > 11133 > 11133   Another example with the same starting point
                 00000   00023   01223   01223   21223
                 00020   00220   02223   22223   22223    */

                // We treat the pixel, mapMask[X,Y] takes the value of the region it now belongs
                // The first part looks for the largest surrounding region
                i = 0;
                maxSizeNeighbors = 0;
                while (i <= numNeighbors - 1)
                {
                    if (maxSizeNeighbors < regionList[regionNeighbors[i]].pixelSize)
                    {
                        maxSizeNeighbors = regionList[regionNeighbors[i]].pixelSize;
                    }
                    i++;
                }
                // Then we set the probability range for each neighbor to be used to extend the region map
                i = 0;
                maxSizeNeighbors *= 2;
                while (i <= numNeighbors - 1)
                {
                    if (i != 0)
                    {
                        j = 0;
                        alreadyAdded = false;
                        while (j < i)
                        {
                            if (regionNeighbors[i] == regionNeighbors[j])
                            {
                                alreadyAdded = true;
                            }
                            j++;
                        }
                        if (!alreadyAdded)
                        {
                            regionNeighborsSize[i] = maxSizeNeighbors - regionList[regionNeighbors[i]].pixelSize + regionNeighborsSize[i - 1];
                        }
                        else
                        {
                            regionNeighborsSize[i] = maxSizeNeighbors - regionList[regionNeighbors[i]].pixelSize * 2 + regionNeighborsSize[i - 1];
                        }
                    }
                    else
                    {
                        regionNeighborsSize[i] = 5000 - regionList[regionNeighbors[i]].pixelSize;
                    }
                    i++;
                }
                // Now that we have those probabilities, we can properly select the region. This allows for more uniform region size overall
                rand = dice.Next(0, regionNeighborsSize[i - 1]);
                i = 0;
                while (rand > regionNeighborsSize[i])
                {
                    i++;
                }
                mapMask[current.X, current.Y] = regionNeighbors[i];
                regionList[regionNeighbors[i]].pixelSize++;
            }
            // Building the neighbor list for each region from there, allowing for knowing the link between regions
            GenerateNeighbors();
        }

        /// <summary>
        /// Allows the calculation of which province is neighboring a given region
        /// </summary>
        public void GenerateNeighbors()
        {
            for (int i = 0; i < LONGITUDE; i++)
            {
                for (int j = 0; j < LATITUDE; j++)
                {
                    if (i != 0)
                    {
                        if (mapMask[i, j] != mapMask[i - 1, j])
                        {
                            if (!regionList[mapMask[i, j]].neighbors.Contains(regionList[mapMask[i - 1, j]]))
                            {
                                regionList[mapMask[i, j]].neighbors.Add(regionList[mapMask[i - 1, j]]);
                            }
                        }
                    }
                    else
                    {
                        if (mapMask[i, j] != mapMask[LONGITUDE - 1, j])
                        {
                            if (!regionList[mapMask[i, j]].neighbors.Contains(regionList[mapMask[LONGITUDE - 1, j]]))
                            {
                                regionList[mapMask[i, j]].neighbors.Add(regionList[mapMask[LONGITUDE - 1, j]]);
                            }
                        }
                    }
                    if (i != LONGITUDE - 1)
                    {
                        if (mapMask[i, j] != mapMask[i + 1, j])
                        {
                            if (!regionList[mapMask[i, j]].neighbors.Contains(regionList[mapMask[i + 1, j]]))
                            {
                                regionList[mapMask[i, j]].neighbors.Add(regionList[mapMask[i + 1, j]]);
                            }
                        }
                    }
                    else
                    {
                        if (mapMask[i, j] != mapMask[0, j])
                        {
                            if (!regionList[mapMask[i, j]].neighbors.Contains(regionList[mapMask[0, j]]))
                            {
                                regionList[mapMask[i, j]].neighbors.Add(regionList[mapMask[0, j]]);
                            }
                        }
                    }
                    if (j != 0)
                    {
                        if (mapMask[i, j] != mapMask[i, j - 1])
                        {
                            if (!regionList[mapMask[i, j]].neighbors.Contains(regionList[mapMask[i, j - 1]]))
                            {
                                regionList[mapMask[i, j]].neighbors.Add(regionList[mapMask[i, j - 1]]);
                            }
                        }
                    }

                    if (j != LATITUDE - 1)
                    {
                        if (mapMask[i, j] != mapMask[i, j + 1])
                        {
                            if (!regionList[mapMask[i, j]].neighbors.Contains(regionList[mapMask[i, j + 1]]))
                            {
                                regionList[mapMask[i, j]].neighbors.Add(regionList[mapMask[i, j + 1]]);
                            }
                        }
                    }

                }
            }
        }

        #endregion

        #region Height

        /// <summary>
        /// Generates a valley, with access to sea and hills
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public int CreateValley(Region region)
        {
            // Valleys are relatively large structures, around 1.3 time the Mass value in number of regions
            int size = dice.Next(5 * MASS / 6, 9 * MASS / 6);
            int regionNumber = 0;
            bool nextToSea = false;
            bool nextToHill = false;
            List<Region> neighborRegions = new List<Region>();
            List<Region> valley = new List<Region>();
            List<Region> sea = new List<Region>();
            List<Region> hill = new List<Region>();
            // First, the creation of a low altitude valley
            do
            {
                region.height = dice.Next(1, 5) * dice.Next(1, 4) + dice.Next(1, 6);
                region.generationOrder = generationOrder;
                notTreatedRegions.Remove(region);
                neighborRegions.Remove(region);
                valley.Add(region);
                regionNumber++;
                foreach (Region neighbor in region.neighbors)
                {
                    if (!neighborRegions.Contains(neighbor) && notTreatedRegions.Contains(neighbor))
                    {
                        neighborRegions.Add(neighbor);
                    }
                }
                if (neighborRegions.Count == 0)
                {
                    goto beach;
                }
                // We choose to take a semi random region in the list to keep it not entirely random, but a little blobby in shape, while not being a round structure
                region = neighborRegions[dice.Next(0, neighborRegions.Count / 2)];

                size--;
            } while (size > 0);

            // Place then a sea region as a starting point for where the sea extends
            region.height = -dice.Next(1, 5) * dice.Next(1, 5) - dice.Next(1, 6);
            region.generationOrder = generationOrder;
            notTreatedRegions.Remove(region);
            neighborRegions.Remove(region);
            sea.Add(region);
            regionNumber++;
            foreach (Region neighbor in region.neighbors)
            {
                if (!neighborRegions.Contains(neighbor) && notTreatedRegions.Contains(neighbor))
                {
                    neighborRegions.Add(neighbor);
                }
            }
            if (neighborRegions.Count == 0)
            {
                goto beach;
            }
            region = neighborRegions[dice.Next(0, neighborRegions.Count)];

            // Place two hills regions, made for rivers to appear
            size = 2;
            do
            {
                region.height = dice.Next(3, 7) * dice.Next(4, 9) + dice.Next(9, 18);
                region.generationOrder = generationOrder;
                notTreatedRegions.Remove(region);
                neighborRegions.Remove(region);
                hill.Add(region);
                regionNumber++;
                foreach (Region neighbor in region.neighbors)
                {
                    if (!neighborRegions.Contains(neighbor) && notTreatedRegions.Contains(neighbor))
                    {
                        neighborRegions.Add(neighbor);
                    }
                }
                if (neighborRegions.Count == 0)
                {
                    goto beach;
                }

                size--;
            } while (size > 0);
            // And now to extend the structure on around 1.2 Mass. This leads to the entire structure taking 2.5 Mass
            size = dice.Next(5 * MASS / 6, 9 * MASS / 6);
            do
            {
                // Here we test the presence of the sea nearby first, then of hill. If none of those, it is a valley
                nextToSea = false;
                nextToHill = false;
                // Now we try to keep to a circular extension
                region = neighborRegions[0];
                region.generationOrder = generationOrder;
                foreach (Region lookout in region.neighbors)
                {
                    if (sea.Contains(lookout))
                    {
                        nextToSea = true;
                    }
                    else if (hill.Contains(lookout))
                    {
                        nextToHill = true;
                    }
                }
                // Sea has the priority for extension, then hills, and finally the valley
                if (nextToSea)
                {
                    region.height = -dice.Next(1, 5) * dice.Next(1, 5) - dice.Next(1, 6);
                    sea.Add(region);
                }
                else if (nextToHill)
                {
                    region.height = dice.Next(3, 7) * dice.Next(4, 9) + dice.Next(9, 18);
                    hill.Add(region);
                }
                else
                {
                    region.height = dice.Next(1, 5) * dice.Next(1, 4) + dice.Next(1, 6);
                    valley.Add(region);
                }
                notTreatedRegions.Remove(region);
                neighborRegions.Remove(region);
                // Building the list of neighboring regions as usual to continue the extension
                foreach (Region neighbor in region.neighbors)
                {
                    if (!neighborRegions.Contains(neighbor) && notTreatedRegions.Contains(neighbor))
                    {
                        neighborRegions.Add(neighbor);
                    }
                }
                if (neighborRegions.Count == 0)
                {
                    goto beach;
                }
                size--;
            } while (size > 0);
            // And now, we lower the altitude of valleys next to the sea and increase it slightly next to hills, to create a natural place to flow for water
            beach:
            foreach (Region valleyRegion in valley)
            {
                nextToSea = false;
                nextToHill = false;
                foreach (Region lookout in valleyRegion.neighbors)
                {
                    if (sea.Contains(lookout))
                    {
                        nextToSea = true;
                    }
                    else if (hill.Contains(lookout))
                    {
                        nextToHill = true;
                    }
                }
                if (nextToSea && !nextToHill)
                {
                    valleyRegion.height /= 2;
                }
                else if (!nextToSea && nextToHill)
                {
                    valleyRegion.height *= 3;
                    valleyRegion.height /= 2;
                }
            }
            return regionNumber;
        }

        /// <summary>
        /// Places an ocean with small islands across it
        /// </summary>
        /// <param name="region"></param>
        public int CreateOcean(Region region)
        {
            // Oceans are truly massive, set to cover enough space to separate continents, taking around 6-9 Mass worth of regions
            int size = dice.Next(14 * MASS / 3, 31 * MASS / 3);
            int regionNumber = 0;

            List<Region> neighborRegions = new List<Region>();
            do
            {
                // Low island density in a sea, but those do exist
                if (dice.Next(1, 151) == 1)
                {
                    region.height = dice.Next(1, 3) + dice.Next(2, 6);
                    region.isSmallIsland = true;
                    regionNumber++;
                    if (dice.Next(1, 3) == 1)
                    {
                        region.height += dice.Next(3, 8);
                        region.isVolcano = true;
                    }
                }
                // Oceans are deep, this ensure they tend to bring the surrounding terrain under sea level
                else
                {
                    region.height = -dice.Next(40, 60) - dice.Next(1, 8) - dice.Next(1, 6);
                    regionNumber++;
                }

                region.generationOrder = generationOrder;
                notTreatedRegions.Remove(region);
                neighborRegions.Remove(region);
                foreach (Region neighbor in region.neighbors)
                {
                    if (!neighborRegions.Contains(neighbor) && notTreatedRegions.Contains(neighbor))
                    {
                        neighborRegions.Add(neighbor);
                    }
                }
                if (neighborRegions.Count == 0)
                {
                    goto hotspot;
                }
                region = neighborRegions[dice.Next(0, neighborRegions.Count / 2)];
                size--;
            } while (size > 0);

            hotspot:;
            return regionNumber;
        }

        /// <summary>
        /// Places a low depth sea filled with islands
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public int CreateSea(Region region)
        {
            // We limit ourselves to a given size a little above 1 Mass
            int size = dice.Next(6 * MASS / 7, 11 * MASS / 7);
            int regionNumber = 0;
            List<Region> neighborRegions = new List<Region>();
            // And we operate until we reach it, or have no room to extend ourselves
            do
            {
                // A sea has a little more islands than an ocean, and less of these are volcanic in origin than in an ocean
                if (dice.Next(1, 81) == 1)
                {
                    region.height = dice.Next(1, 3) + dice.Next(2, 6);
                    region.isSmallIsland = true;
                    regionNumber++;
                    if (dice.Next(1, 6) == 1)
                    {
                        region.height += dice.Next(3, 8);
                        region.isVolcano = true;
                    }
                }
                // Most areas are however made of continental shelves
                else
                {
                    region.height = -dice.Next(5, 20) - dice.Next(1, 5) - dice.Next(1, 4);
                    regionNumber++;
                }

                region.generationOrder = generationOrder;
                notTreatedRegions.Remove(region);
                neighborRegions.Remove(region);

                // We move to one of the neighbors
                foreach (Region neighbor in region.neighbors)
                {
                    if (!neighborRegions.Contains(neighbor) && notTreatedRegions.Contains(neighbor))
                    {
                        neighborRegions.Add(neighbor);
                    }
                }
                if (neighborRegions.Count == 0)
                {
                    goto hotspot;
                }
                region = neighborRegions[dice.Next(0, neighborRegions.Count)];
                size--;
            } while (size > 0);
            hotspot:;
            return regionNumber;
        }

        /// <summary>
        /// Places a low depth sea filled with islands
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public int CreateIsland(Region region)
        {
            // Island chains are relativel small area made of land and sea alike, around 1 Mass big
            int size = dice.Next(2 * MASS / 3, 4 * MASS / 3);
            int regionNumber = 0;
            int islandChance;
            bool island;
            int rand;
            List<Region> neighborRegions = new List<Region>();
            List<Region> islandRegion = new List<Region>();
            do
            {
                // There are larger islands part of those systems. If we start from a larger island, we have larger probability to prolong it
                island = false;
                if (islandRegion.Contains(region))
                {
                    islandChance = 11;
                }
                else
                {
                    islandChance = 6;
                }
                rand = dice.Next(1, 15);
                // ... But also a lot of smaller ones are present around
                if (rand <= 4)
                {
                    region.height = dice.Next(1, 3) + dice.Next(2, 6);
                    region.isSmallIsland = true;
                    regionNumber++;
                }
                // Large islands seems to be tall, but that's to ensure their survivability when blurring the height of regions
                else if (rand < islandChance)
                {
                    region.height = dice.Next(15, 25) + dice.Next(0, 16);
                    regionNumber++;
                    island = true;
                }
                else
                {
                    region.height = -dice.Next(5, 20) - dice.Next(1, 5) - dice.Next(1, 4);
                    regionNumber++;
                }

                region.generationOrder = generationOrder;
                notTreatedRegions.Remove(region);
                neighborRegions.Remove(region);

                // Usual extension to neighbors by now
                foreach (Region neighbor in region.neighbors)
                {
                    if (!neighborRegions.Contains(neighbor) && notTreatedRegions.Contains(neighbor))
                    {
                        neighborRegions.Add(neighbor);
                        if (island)
                        {
                            islandRegion.Add(neighbor);
                        }
                    }
                }
                if (neighborRegions.Count == 0)
                {
                    goto hotspot;
                }
                region = neighborRegions[dice.Next(0, neighborRegions.Count)];
                size--;
            } while (size > 0);
            hotspot:;
            return regionNumber;
        }

        /// <summary>
        /// Procedural coastal mountains generation, akin to the Andean structures
        /// </summary>
        /// <param name="region"></param>
        public int CreateCoastalMountains(Region region)
        {
            // Mountains are long structures that are rectilign in a sense
            int size = dice.Next(3 * MASS / 7, 11 * MASS / 7);
            int baseHeight = dice.Next(5, 9) * dice.Next(3, 7); // This gives homogeneity to the mountain range's height
            double change = 0;
            int regionNumber = 0;
            double[] direction = new double[2];
            double chainX = region.X, chainY = region.Y;
            bool endChain = false;

            List<Region> hillNeighborRegions = new List<Region>();
            List<Region> seaNeighborRegions = new List<Region>();
            List<Region> mountain = new List<Region>();

            // Initialize the mountain's original direction
            direction[0] = dice.NextDouble();
            direction[1] = 1 - direction[0];
            if (dice.Next(1, 3) == 1)
            {
                direction[0] *= -1;
            }
            if (dice.Next(1, 3) == 1)
            {
                direction[1] *= -1;
            }

            do
            {
                foreach (Region neighbor in region.neighbors)
                {
                    if (!seaNeighborRegions.Contains(neighbor) && !hillNeighborRegions.Contains(neighbor) && !mountain.Contains(neighbor))
                    {
                        // Mountain going south east, sea placed north east. Then, we rotate from that
                        if ((direction[0] >= 0 && direction[1] >= 0 && (neighbor.IsEastFrom(region) && neighbor.IsNorthFrom(region))) ||
                            (direction[0] <= 0 && direction[1] >= 0 && (neighbor.IsEastFrom(region) && neighbor.IsSouthFrom(region))) ||
                            (direction[0] <= 0 && direction[1] <= 0 && (neighbor.IsWestFrom(region) && neighbor.IsSouthFrom(region))) ||
                            (direction[0] >= 0 && direction[1] <= 0 && (neighbor.IsWestFrom(region) && neighbor.IsNorthFrom(region))))
                        {
                            seaNeighborRegions.Add(neighbor);
                        }
                        else
                        {
                            hillNeighborRegions.Add(neighbor);
                        }
                    }
                }

                // Loop seeking for next region
                do
                {
                    chainX += direction[0];
                    chainY += direction[1];
                    chainX = Region.CalculateX(chainX);
                    if (chainY < 0 || chainY > LATITUDE - 1 || !notTreatedRegions.Contains(regionList[mapMask[(int)chainX, (int)chainY]]))
                    {
                        endChain = true;
                    }
                } while (!endChain && mapMask[(int)chainX, (int)chainY] == region.ID);


                // finish the treatment of the region, then get to the new one. If that region is part of the hill and sea found before, of course, remove it from the list
                size--;
                region.height = baseHeight + dice.Next(0, 40);
                notTreatedRegions.Remove(region);
                regionNumber++;
                mountain.Add(region);
                if (dice.Next(1, 5) == 1)
                {
                    region.isVolcano = true;
                }
                region.generationOrder = generationOrder;

                if (!endChain)
                {
                    region = regionList[mapMask[(int)chainX, (int)chainY]];
                    seaNeighborRegions.Remove(region);
                    hillNeighborRegions.Remove(region);

                    // apply a slight change in the direction to not have too much of a linear range
                    change = dice.Next(-12, 13);
                    direction = Region.LineRotation(direction, change);
                }

            } while (size > 0 && !endChain);

            // There, we generate the trench areas and the ocean around it
            foreach (Region seaRegion in seaNeighborRegions)
            {
                seaRegion.height = -dice.Next(75, 90);
                seaRegion.isOceanTrench = true;
                regionNumber++;
                notTreatedRegions.Remove(seaRegion);
                region.generationOrder = generationOrder;
                foreach (Region seaLayerRegion in seaRegion.neighbors)
                {
                    if (notTreatedRegions.Contains(seaLayerRegion) && !seaNeighborRegions.Contains(seaLayerRegion) && !hillNeighborRegions.Contains(seaLayerRegion) && !mountain.Contains(seaLayerRegion))
                    {
                        seaLayerRegion.height = -dice.Next(40, 60) - dice.Next(1, 8) - dice.Next(1, 6);
                        regionNumber++;
                        notTreatedRegions.Remove(seaLayerRegion);

                    }
                }

            }
            // The rest is made of hills, around a third the height of a mountain range, followed by plateau or plains depending of the base height
            foreach (Region hillRegion in hillNeighborRegions)
            {
                hillRegion.height = (baseHeight + dice.Next(0, 15)) / 2;
                regionNumber++;
                notTreatedRegions.Remove(hillRegion);
                region.generationOrder = generationOrder;
                foreach (Region landLayerRegion in hillRegion.neighbors)
                {
                    if (notTreatedRegions.Contains(landLayerRegion) && !seaNeighborRegions.Contains(landLayerRegion) && !hillNeighborRegions.Contains(landLayerRegion) && !mountain.Contains(landLayerRegion))
                    {
                        landLayerRegion.height = (baseHeight + dice.Next(2, 6) * dice.Next(3, 6)) / 2;
                        regionNumber++;
                        notTreatedRegions.Remove(landLayerRegion);

                    }
                }
            }
            return regionNumber;
        }

        /// <summary>
        /// Inner mountain range generation
        /// </summary>
        /// <param name="region"></param>
        public int CreateContinentalMountains(Region region)
        {
            // Continental ountains are slightly shorter than their coastal counterparts
            int size = dice.Next(3 * MASS / 7, 8 * MASS / 7);
            int baseHeight = dice.Next(5, 9) * dice.Next(3, 7); // This gives homogeneity to the mountain range's height
            int change = 0;
            int regionNumber = 0;
            double[] direction = new double[2];
            double chainX = region.X, chainY = region.Y;
            bool endChain = false;

            List<Region> hillNeighborRegions = new List<Region>();
            List<Region> mountain = new List<Region>();

            // Initialize the mountain's original direction
            direction[0] = dice.NextDouble();
            direction[1] = 1 - direction[0];
            if (dice.Next(1, 3) == 1)
            {
                direction[0] *= -1;
            }
            if (dice.Next(1, 3) == 1)
            {
                direction[1] *= -1;
            }

            do
            {
                foreach (Region neighbor in region.neighbors)
                {
                    if (!hillNeighborRegions.Contains(neighbor) && !mountain.Contains(neighbor))
                    {
                        hillNeighborRegions.Add(neighbor);
                    }
                }

                // Loop seeking for next region
                do
                {
                    chainX += direction[0];
                    chainY += direction[1];
                    chainX = Region.CalculateX(chainX);
                    if (chainY < 0 || chainY > LATITUDE - 1 || !notTreatedRegions.Contains(regionList[mapMask[(int)chainX, (int)chainY]]))
                    {
                        endChain = true;
                    }
                } while (!endChain && mapMask[(int)chainX, (int)chainY] == region.ID);


                // finish the treatment of the region, then get to the new one. If that region is part of the hill and sea found before, of course, remove it from the list
                size--;
                region.height = baseHeight + dice.Next(0, 40);
                notTreatedRegions.Remove(region);
                regionNumber++;
                mountain.Add(region);
                if (dice.Next(1, 5) == 1)
                {
                    region.isVolcano = true;
                }
                region.generationOrder = generationOrder;

                if (!endChain)
                {
                    region = regionList[mapMask[(int)chainX, (int)chainY]];
                    hillNeighborRegions.Remove(region);

                    // apply a slight change in the direction to not have too much of a linear range
                    change = dice.Next(-12, 13);
                    Region.LineRotation(direction, change);
                }

            } while (size > 0 && !endChain);

            // The rest is made of hills, around half the height of a mountain range, followed by plateau or plains
            foreach (Region hillRegion in hillNeighborRegions)
            {
                hillRegion.height = (baseHeight + dice.Next(0, 15)) / 2;
                regionNumber++;
                notTreatedRegions.Remove(hillRegion);
                region.generationOrder = generationOrder;
                foreach (Region landLayerRegion in hillRegion.neighbors)
                {
                    if (notTreatedRegions.Contains(landLayerRegion) && !hillNeighborRegions.Contains(landLayerRegion) && !mountain.Contains(landLayerRegion))
                    {
                        landLayerRegion.height = (baseHeight + dice.Next(2, 6) * dice.Next(3, 6)) / 2;
                        regionNumber++;
                        notTreatedRegions.Remove(landLayerRegion);

                    }
                }
            }
            return regionNumber;
        }

        /// <summary>
        /// Continues extending an ocean
        /// </summary>
        /// <param name="region"></param>
        public void ExtendOcean(Region region)
        {
            // Ocean extension is maybe the largest one, but even that one barely pass the Mass size
            int size = dice.Next(4 * MASS  / 9, 11 * MASS / 9);
            List<Region> neighborRegions = new List<Region>();
            do
            {
                // We have still a low number of islands
                if (dice.Next(1, 151) == 1)
                {
                    region.height = dice.Next(1, 3) + dice.Next(2, 6);
                    region.isSmallIsland = true;
                    if (dice.Next(1, 3) == 1)
                    {
                        region.height += dice.Next(3, 8);
                        region.isVolcano = true;
                    }
                }
                else
                {
                    region.height = -dice.Next(40, 60) - dice.Next(1, 8) - dice.Next(1, 6);
                }

                region.generationOrder = generationOrder;
                notTreatedRegions.Remove(region);
                neighborRegions.Remove(region);
                foreach (Region neighbor in region.neighbors)
                {
                    if (!neighborRegions.Contains(neighbor) && notTreatedRegions.Contains(neighbor))
                    {
                        neighborRegions.Add(neighbor);
                    }
                }
                if (neighborRegions.Count > 0)
                {
                    region = neighborRegions[0];
                }
                size--;
            } while (size > 0);
        }

        /// <summary>
        /// Ocean filled with islands
        /// </summary>
        /// <param name="region"></param>
        public void ExtendOceanArchipelago(Region region)
        {
            // Archipelagoes are the logical prolongation of an island chains, or a smaller one simply
            int size = dice.Next(2 * MASS / 9, 5 * MASS / 9);
            int rand;
            List<Region> neighborRegions = new List<Region>();
            do
            {
                rand = dice.Next(1, 26);
                if (rand < 6)
                {
                    region.height = dice.Next(1, 3) + dice.Next(2, 6);
                    region.isSmallIsland = true;
                    if (dice.Next(1, 3) == 1)
                    {
                        region.height += dice.Next(3, 8);
                        region.isVolcano = true;
                    }
                }
                else if (rand < 8)
                {
                    region.height = dice.Next(1, 10) + dice.Next(3, 15);
                    if (dice.Next(1, 5) == 1)
                    {
                        region.height += dice.Next(4, 12);
                        region.isVolcano = true;
                    }
                }
                else
                {
                    region.height = -dice.Next(40, 60) - dice.Next(1, 8) - dice.Next(1, 6);
                }

                region.generationOrder = generationOrder;
                notTreatedRegions.Remove(region);
                neighborRegions.Remove(region);
                foreach (Region neighbor in region.neighbors)
                {
                    if (!neighborRegions.Contains(neighbor) && notTreatedRegions.Contains(neighbor))
                    {
                        neighborRegions.Add(neighbor);
                    }
                }
                if (neighborRegions.Count > 0)
                {
                    region = neighborRegions[0];
                }
                size--;
            } while (size > 0);
        }

        /// <summary>
        /// Brings a low depth aquatic area
        /// </summary>
        /// <param name="region"></param>
        public void ExtendSea(Region region)
        {
            // Extending sea on around half a mass
            int size = dice.Next(3 * MASS / 10, 6 * MASS / 10);
            int rand;
            List<Region> neighborRegions = new List<Region>();
            do
            {
                rand = dice.Next(1, 101);
                if (rand < 6)
                {
                    region.height = dice.Next(1, 3) + dice.Next(2, 6);
                    region.isSmallIsland = true;
                    if (dice.Next(1, 3) == 1)
                    {
                        region.height += dice.Next(3, 8);
                        region.isVolcano = true;
                    }
                }
                // A small chance of generating larger islands
                else if (rand < 12)
                {
                    region.height = dice.Next(1, 15) + dice.Next(15, 35);
                    if (dice.Next(1, 5) == 1)
                    {
                        region.height += dice.Next(7, 20);
                        region.isVolcano = true;
                    }
                }
                // Depth is limited in those areas
                else
                {
                    region.height = -dice.Next(1, 5) * dice.Next(1, 5) - dice.Next(1, 6);
                }

                region.generationOrder = generationOrder;
                notTreatedRegions.Remove(region);
                neighborRegions.Remove(region);
                foreach (Region neighbor in region.neighbors)
                {
                    if (!neighborRegions.Contains(neighbor) && notTreatedRegions.Contains(neighbor))
                    {
                        neighborRegions.Add(neighbor);
                    }
                }
                if (neighborRegions.Count > 0)
                {
                    region = neighborRegions[0];
                }
                size--;
            } while (size > 0);
        }

        /// <summary>
        /// Relatively flat area, perfect for rivers to flow into
        /// </summary>
        /// <param name="region"></param>
        public void ExtendPlain(Region region)
        {
            // Plains are relatively vast, but a little below a Mass still
            int size = dice.Next(4 * MASS / 9, 8 * MASS / 9);
            int rand;
            List<Region> neighborRegions = new List<Region>();
            do
            {
                rand = dice.Next(1, 18);
                // There is a limited chance to build higher regions, soon to be smoothed down
                if (rand < 6)
                {
                    region.height = dice.Next(3, 7) * dice.Next(4, 9) + dice.Next(9, 18);
                }
                else
                {
                    region.height = dice.Next(1, 7) * dice.Next(1, 6) + dice.Next(1, 10);
                }

                region.generationOrder = generationOrder;
                notTreatedRegions.Remove(region);
                neighborRegions.Remove(region);
                foreach (Region neighbor in region.neighbors)
                {
                    if (!neighborRegions.Contains(neighbor) && notTreatedRegions.Contains(neighbor))
                    {
                        neighborRegions.Add(neighbor);
                    }
                }
                if (neighborRegions.Count > 0)
                {
                    region = neighborRegions[0];
                }
                size--;
            } while (size > 0);
        }

        /// <summary>
        /// Hill filled area
        /// </summary>
        /// <param name="region"></param>
        public void ExtendLand(Region region)
        {
            // Land areas are usually higher than plains and have more chance to start the generation of rivers
            int size = dice.Next(3 * MASS / 11, 7 * MASS / 11);
            int rand;
            List<Region> neighborRegions = new List<Region>();
            do
            {
                rand = dice.Next(1, 18);
                // Height tends to be higher than plains
                if (rand < 11)
                {
                    region.height = dice.Next(3, 7) * dice.Next(4, 9) + dice.Next(12, 21);
                }
                else
                {
                    region.height = dice.Next(1, 7) * dice.Next(2, 6) + dice.Next(10, 21);
                }

                region.generationOrder = generationOrder;
                notTreatedRegions.Remove(region);
                neighborRegions.Remove(region);
                foreach (Region neighbor in region.neighbors)
                {
                    if (!neighborRegions.Contains(neighbor) && notTreatedRegions.Contains(neighbor))
                    {
                        neighborRegions.Add(neighbor);
                    }
                }
                if (neighborRegions.Count > 0)
                {
                    region = neighborRegions[0];
                }
                size--;
            } while (size > 0);
        }

        /// <summary>
        /// Main function to create the height map
        /// </summary>
        public void GenerateLandmass()
        {
            int rand;
            int regionChange;
            Region randRegion;

            notTreatedRegions.AddRange(regionList);

            // This defines the general repartition of land generation on the world map
            OCEAN = 55 * REGION / 100;
            SEA = 59 * REGION / 100;
            ISLAND = 66 * REGION / 100;
            VALLEY = 84 * REGION / 100;
            COASTMOUNTAIN = 93 * REGION / 100;
            MOUNTAIN = REGION;


            // Each time we generate a type of structure, we lower the chance of having more structures of the same type, so to keep the ratio going
            // and somewhat ensure to have a variety of structures to build onto
            while (notTreatedRegions.Count > (REGION / 3))
            {
                rand = dice.Next(0, (OCEAN + SEA + ISLAND + VALLEY + COASTMOUNTAIN + MOUNTAIN));
                if (rand < OCEAN)
                {
                    regionChange = CreateOcean(notTreatedRegions[dice.Next(0, notTreatedRegions.Count)]);
                    OCEAN -= regionChange;
                    SEA -= regionChange;
                    ISLAND -= regionChange;
                    VALLEY -= regionChange;
                    COASTMOUNTAIN -= regionChange;
                    MOUNTAIN -= regionChange;
                }
                else if (rand < OCEAN + SEA)
                {
                    regionChange = CreateSea(notTreatedRegions[dice.Next(0, notTreatedRegions.Count)]);
                    SEA -= regionChange;
                    ISLAND -= regionChange;
                    VALLEY -= regionChange;
                    COASTMOUNTAIN -= regionChange;
                    MOUNTAIN -= regionChange;
                }
                else if (rand < OCEAN + SEA + ISLAND)
                {
                    regionChange = CreateSea(notTreatedRegions[dice.Next(0, notTreatedRegions.Count)]);
                    ISLAND -= regionChange;
                    VALLEY -= regionChange;
                    COASTMOUNTAIN -= regionChange;
                    MOUNTAIN -= regionChange;
                }
                else if (rand < OCEAN + SEA + ISLAND + VALLEY)
                {
                    regionChange = CreateValley(notTreatedRegions[dice.Next(0, notTreatedRegions.Count)]);
                    VALLEY -= regionChange;
                    COASTMOUNTAIN -= regionChange;
                    MOUNTAIN -= regionChange;
                }
                else if (rand < OCEAN + SEA + ISLAND + VALLEY + COASTMOUNTAIN)
                {
                    regionChange = CreateCoastalMountains(notTreatedRegions[dice.Next(0, notTreatedRegions.Count)]);
                    COASTMOUNTAIN -= regionChange;
                    MOUNTAIN -= regionChange;
                }
                else
                {
                    regionChange = CreateContinentalMountains(notTreatedRegions[dice.Next(0, notTreatedRegions.Count)]);
                    MOUNTAIN -= regionChange;
                }
                generationOrder++;
            }
            // Then, we extend the regions once generated
            while (notTreatedRegions.Count > 0)
            {
                // We select a region next to an already treated region
                do
                {
                    randRegion = notTreatedRegions[dice.Next(0, notTreatedRegions.Count)];
                } while (randRegion.IsIsolated());
                // From places of high altitude, we get only land
                if (randRegion.MeanHeight() > 30)
                {
                    rand = dice.Next(1, 21);
                    if (rand < 7)
                    {
                        ExtendPlain(randRegion);
                    }
                    else
                    {
                        ExtendLand(randRegion);
                    }
                }
                // Lower areas can prolong plains from themselves more easily, but also give way for sea
                else if (randRegion.MeanHeight() > 0)
                {
                    rand = dice.Next(1, 21);
                    if (rand < 11)
                    {
                        ExtendPlain(randRegion);
                    }
                    else if (rand < 18)
                    {
                        ExtendLand(randRegion);
                    }
                    else
                    {
                        ExtendSea(randRegion);
                    }
                }
                // Regions close to sea level will prolong sea and ocean alike, but also more archipelago
                else if (randRegion.MeanHeight() > - 15)
                {
                    rand = dice.Next(1, 21);
                    if (rand < 14)
                    {
                        ExtendOcean(randRegion);
                    }
                    else if (rand < 17)
                    {
                        ExtendSea(randRegion);
                    }
                    else
                    {
                        ExtendOceanArchipelago(randRegion);
                    }
                }
                // Lower level regions gets to generate oceans mostly
                else
                {
                    rand = dice.Next(1, 21);
                    if (rand < 19)
                    {
                        ExtendOcean(randRegion);
                    }
                    else
                    {
                        ExtendOceanArchipelago(randRegion);
                    }
                }
                generationOrder++;
            }
        }

        /// <summary>
        /// Cleaning the world from many inner oceans and harmonizing height. Places continental masses
        /// </summary>
        public void CleanLandmass()
        {
            foreach (Region region in regionList)
            {
                // Small holes we remove by putting these at height = 0
                if (region.IsLoneHole())
                {
                    region.height = 0;
                    region.height = region.MeanHeight();
                    region.isOceanTrench = false;
                }
                // And then we rebalance the height of each region thanks to its neighbors
                region.calculateHeight = (region.height + region.MeanHeight()) / 2;
            }

            foreach (Region region in regionList)
            {
                // Now we define the type of each reagion thanks to height. We stored the calculated height because we didn't want changes made in cascade
                region.height = region.calculateHeight;
                
                if (region.height < -40)
                {
                    region.isOcean = true;
                }
                else if (region.height <= 0)
                {
                    // We do not want regions with a height of 0 in the end
                    region.isSea = true;
                    if (region.height == 0)
                    {
                        region.height = -1;
                    }
                }
                else if (region.height <= 16)
                {
                    region.isValley = true;
                }
                else if (region.height <= 40)
                {
                    region.isHilly = true;
                }
                else
                {
                    region.isMountain = true;
                }
                if (region.isSmallIsland)
                {
                    // The averaging lowered the height of small islands, we restore it to a minimum there
                    region.height = dice.Next(1, 3) + dice.Next(0, 8);
                }
            }

        }

        #endregion

        #region Temperature
        /// <summary>
        /// Generating temperature worldwide
        /// </summary>
        public void GenerateTemperature()
        {
            int rand;

            for (int i = 0; i < REGION; i++)
            {
                rand = dice.Next(0, 11);

                // temperature depends on Latitude
                regionList[i].temperature = 75 + rand - 150 * (Math.Abs(regionList[i].Y - (LATITUDE / 2))) / (LATITUDE / 2);

                // Sea based regions tends to be warmer in all case
                if (regionList[i].isSea)
                {
                    rand = dice.Next(10, 21);
                    regionList[i].temperature += rand;
                }
                // Oceans tends toward 0
                if (regionList[i].isOcean)
                {
                    rand = dice.Next(5, 11);
                    if (regionList[i].temperature < 0)
                    {
                        regionList[i].temperature += rand;
                    }
                    else
                    {
                        regionList[i].temperature -= rand;
                    }

                }
                // Lands lost in the middle of a continent tends toward the extremes
                if (regionList[i].IsLandlocked())
                {
                    rand = dice.Next(5, 21);
                    if (regionList[i].temperature < 0)
                    {
                        regionList[i].temperature -= rand;
                    }
                    else
                    {
                        regionList[i].temperature += rand;
                    }
                }
                // Altitude has an impact on temperature
                if (regionList[i].IsLand())
                {
                    regionList[i].temperature -= regionList[i].height / 5;
                }
            }
        }

        #endregion

        #region Humidity
        public void GenerateHumidity()
        {
            int pole, temperate, desert;

            // The climatic bands are slightly randomized for variety's sake. There are four areas types:
            // - Poles are humid and uniform
            // - Temperate areas see their humidity come from the east
            // - Desert are dry, especially the continental areas
            // - Tropical areas are wet, see humidity come from the west and even the seas are more humid, leading to wind blowing rain around
            pole = dice.Next(LATITUDE / 8, LATITUDE / 6);
            temperate = pole + dice.Next(LATITUDE / 8, LATITUDE / 6);
            desert = temperate + dice.Next(LATITUDE / 8, LATITUDE / 6);

            // We call the respective function depending of the region's center
            foreach (Region region in regionList)
            {
                if (region.Y < pole || region.Y > LATITUDE - pole)
                {
                    SetPolarHumidity(region);
                }
                else if ((region.Y < temperate && region.Y >= pole) || (region.Y > LATITUDE - temperate && region.Y <= LATITUDE - pole))
                {
                    SetTemperateHumidity(region);
                }
                else if (region.Y < desert || region.Y > LATITUDE - desert)
                {
                    SetDesertHumidity(region);
                }
                else
                {
                    SetTropicalHumidity(region);
                }
                // Make sure we do not get a negative humidity!
                if (region.humidity < 0)
                {
                    region.humidity = 0;
                }
                // Or too high an humidity, too
                if (region.humidity > 100)
                {
                    region.humidity = 100;
                }
            }
        }

        public void SetPolarHumidity(Region region)
        {
            region.isPolar = true;
            if (region.isOcean)
            {
                region.humidity = dice.Next(60, 70);
            }
            if (region.isSea)
            {
                region.humidity = dice.Next(70, 80);
            }
            // Land near water is more humid
            if (region.IsLand())
            {
                region.humidity = dice.Next(32, 45) + dice.Next(1, 8);
                if (region.IsLandlocked())
                {
                    region.humidity -= dice.Next(3, 7);
                }
            }
            // Higher altitude blocks rainfall! Note that we look at the mean height
            if (region.MeanHeight() > 30)
            {
                region.humidity += (30 - region.MeanHeight()) / 5;
            }
        }

        public void SetTemperateHumidity(Region region)
        {
            region.isTemperate = true;
            if (region.isOcean)
            {
                region.humidity = dice.Next(58, 68);
            }
            if (region.isSea)
            {
                region.humidity = dice.Next(60, 68);
            }
            //Temperate lands are drier if there's a mountain to the west, and wetter if there is a mountain on the east. Presence of water always increase humidity, more if it is a sea
            // Islands are more humid by definition too
            if (region.IsLand())
            {
                region.humidity = dice.Next(31, 45) + dice.Next(1, 8);
                if (region.HasWesternMountain())
                {
                    region.humidity -= dice.Next(11, 16);
                }
                if (region.HasEasternMountain())
                {
                    region.humidity += dice.Next(7, 12);
                }
                if (region.HasEasternWater())
                {
                    region.humidity += dice.Next(3, 7);
                }
                if (region.HasWesternWater())
                {
                    region.humidity += dice.Next(5, 10);
                }
                if (region.isLargeIsland)
                {
                    region.humidity += dice.Next(3, 6);
                }
            }
            // As usual, high altitude means low humidity
            if (region.MeanHeight() > 30)
            {
                region.humidity += (30 - region.MeanHeight()) / 5;
            }

        }

        public void SetDesertHumidity(Region region)
        {
            region.isDesert = true;

            if (region.isOcean)
            {
                region.humidity = dice.Next(54, 64);
            }
            if (region.isSea)
            {
                region.humidity = dice.Next(64, 74);
            }
            // Desert lands that are not coastal are dry, but then follows the same ideas as temperate area, just less pronouced
            if (region.IsLand())
            {
                region.humidity = dice.Next(32, 40) + dice.Next(1, 8);
                if (region.HasWesternMountain())
                {
                    region.humidity -= dice.Next(7, 13);
                }
                if (region.HasEasternMountain())
                {
                    region.humidity += dice.Next(5, 9);
                }
                if (region.HasEasternWater())
                {
                    region.humidity += dice.Next(1, 5);
                }
                if (region.HasWesternWater())
                {
                    region.humidity += dice.Next(9, 13);
                }
                if (region.IsLandlocked())
                {
                    region.humidity -= dice.Next(9, 15);
                }
                if (region.isLargeIsland)
                {
                    region.humidity += dice.Next(4, 9);
                }
            }
            // As usual, high altitude means low humidity
            if (region.MeanHeight() > 30)
            {
                region.humidity += (30 - region.MeanHeight()) / 5;
            }
        }

        public void SetTropicalHumidity(Region region)
        {

            region.isTropical = true;
            if (region.isOcean)
            {
                region.humidity = dice.Next(61, 71);
            }
            if (region.isSea)
            {
                region.humidity = dice.Next(69, 79);
            }
            if (region.IsLand())
            {
                region.humidity = dice.Next(38, 54) + dice.Next(1, 8);
                if (region.HasWesternMountain())
                {
                    region.humidity += dice.Next(7, 17);
                }
                if (region.HasEasternMountain())
                {
                    region.humidity -= dice.Next(10, 17);
                }
                if (region.HasEasternWater())
                {
                    region.humidity += dice.Next(8, 15);
                }
                if (region.HasWesternWater())
                {
                    region.humidity += dice.Next(2, 5);
                }
                if (region.isLargeIsland)
                {
                    region.humidity += dice.Next(3, 8);
                }
            }
            // As usual, high altitude means low humidity
            if (region.MeanHeight() > 30)
            {
                region.humidity += (30 - region.MeanHeight()) / 5;
            }
        }
        #endregion

        #region Currents

        /// <summary>
        /// Places rivers on the map in specific regions, and starts their propagation
        /// </summary>
        public void PlaceRivers()
        {
            int rand;
            Region region;
            River river;
            int numberRiver;

            numberRiver = dice.Next(REGION / 32, REGION / 25);

            while (numberRiver != 0)
            {
                // We're looking for somewhat high altitude regions not located in the polar area and not coastal. Deserts areas are limited to moutains due to how dry they are. And no rivers on top of each other!
                do
                {
                    rand = dice.Next(0, REGION);
                    region = regionList[rand];
                } while (!(region.isHilly && (region.isTemperate || (region.isDesert && region.isMountain) || region.isTropical) && region.IsLandlocked()) && region.rivers.Count == 0);

                river = new River(region);
                riverList.Add(river);
                PropagateRiver(river);

                numberRiver--;
            }
        }

        /// <summary>
        /// Function to allow a river to flow down toward the sea
        /// </summary>
        /// <param name="river"></param>
        public void PropagateRiver(River river)
        {
            Region region = river.stream.First();
            Region nextRegion = region.neighbors[0];
            bool otherRiverReached = false;
            int meanHeight, lowestMeanHeight;

            // We advance the river until we reach the sea level or find another river
            while (region.height > 0 && otherRiverReached == false)
            {
                // ... And to do so we are looking for the lowest regions neighboring the one we are now, pondered with the general area
                lowestMeanHeight = 200;
                foreach (Region neighbor in region.neighbors)
                {
                    // We ensure to go down if a neighboring region is under the sea level
                    if (neighbor.IsWater())
                    {
                        meanHeight = neighbor.height - 100;
                    }
                    else
                    {
                        meanHeight = neighbor.MeanHeight();
                    }

                    // Even if those regions are higher naturally, we want to ensure we avoid hilly and moutaneous regions if we can
                    if (neighbor.isHilly || neighbor.isMountain)
                    {
                        meanHeight += 15;
                    }

                    //Adding a little random element here to potentially have a plain have two concurrent rivers on a couple regions
                    meanHeight += dice.Next(0, 16);

                    if (meanHeight < lowestMeanHeight && !river.stream.Contains(neighbor))
                    {
                        lowestMeanHeight = meanHeight;
                        nextRegion = neighbor;
                    }
                }

                // And we fill the region, testing if we join a second river or not. If we find that this river is nothing else than itself, we build an inner sea region. In any case
                // erosion will do its work, digging slightly a valley and increase the local humidity somewhat
                otherRiverReached = river.GenerateRiver(nextRegion);
                if (!otherRiverReached)
                {
                    region = nextRegion;
                    nextRegion = region.neighbors[0];
                    region.humidity += dice.Next(10 - region.humidity / 20, 20 - region.humidity / 10);
                    if (region.height > 0)
                    {
                        region.height -= dice.Next((region.height) / 50, (region.height) / 25);
                    }
                    if (region.height == 0)
                    {
                        region.height = 1;
                    }
                }
                // In case the river loops back on itself, create the sea
                else
                {
                    if (river.stream.Count > 2)
                    {
                        if (river.stream[river.stream.Count - 1] == river.stream[river.stream.Count - 3])
                        {
                            region.height = dice.Next(1, 8) + dice.Next(1, 4) - 12;
                            region.isContinent = false;
                            region.isHilly = false;
                            region.isMountain = false;
                            region.isLargeIsland = false;
                            region.isSmallIsland = false;
                            region.isSea = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create deposits along rivers
        /// </summary>
        public void FlowingRiver()
        {
            int i, j = riverList.Count - 1;
            River river;
            while (j >= 0)
            {
                river = riverList[j];
                i = 0;
                while (i < river.stream.Count)
                {
                    // We make sure that every region sans the last one gets the water corresponding to the river size
                    if (i < river.stream.Count - 1)
                    {
                        river.stream[i].geology.Add(new Geology("River", river.stream[i].riverStream));
                    }
                    // Minerals are carried over from neighboring regions, making these possible to exploit even outside of hills
                    if (i > 0)
                    {
                        if (river.stream[i - 1].naturalResource.resources["salt"] > 0)
                        {
                            river.stream[i].geology.Add(new Geology("Flowing salt", river.stream[i - 1].naturalResource.resources["salt"]));
                        }
                        if (river.stream[i - 1].naturalResource.resources["metal"] > 0)
                        {
                            river.stream[i].geology.Add(new Geology("Flowing metal", river.stream[i - 1].naturalResource.resources["metal"]));
                        }
                        if (river.stream[i - 1].naturalResource.resources["rareMetal"] > 0)
                        {
                            river.stream[i].geology.Add(new Geology("Flowing rare metal", river.stream[i - 1].naturalResource.resources["rareMetal"]));
                        }
                        if (river.stream[i - 1].naturalResource.resources["preciousMetal"] > 0)
                        {
                            river.stream[i].geology.Add(new Geology("Flowing precious metal", river.stream[i - 1].naturalResource.resources["preciousMetal"]));
                        }
                        if (river.stream[i - 1].naturalResource.resources["radioactiveOre"] > 0)
                        {
                            river.stream[i].geology.Add(new Geology("Flowing radioactive ore", river.stream[i - 1].naturalResource.resources["radioactiveOre"]));
                        }
                        if (river.stream[i - 1].naturalResource.resources["gems"] > 0)
                        {
                            river.stream[i].geology.Add(new Geology("Flowing gems", river.stream[i - 1].naturalResource.resources["gems"]));
                        }
                        if (river.stream[i - 1].naturalResource.resources["reagent"] > 0)
                        {
                            river.stream[i].geology.Add(new Geology("Flowing reagent", river.stream[i - 1].naturalResource.resources["reagent"]));
                        }
                    }
                    i++;
                }
                j--;
            }
        }

        /// <summary>
        /// Places the starting point of winds and begins their blowing
        /// </summary>
        public void PlaceWinds()
        {
            int rand;
            Region region;
            Wind wind;
            int numberWind;
            bool cold;
            bool north;


            numberWind = dice.Next(8 * MASS / 9, 10 * MASS / 9);


            while (numberWind != 0)
            {
                // Winds in our case are arcing toward the pole or equator. And not created in a region with wind already!
                do
                {
                    rand = dice.Next(0, REGION);
                    region = regionList[rand];
                } while (region.winds.Count != 0 || region.isMountain);

                // Setting wind parameters
                if (region.Y < LATITUDE / 2)
                {
                    north = true;
                }
                else
                {
                    north = false;
                }

                if (region.isTropical || region.isDesert)
                {
                    cold = false;
                }
                else
                {
                    cold = true;
                }

                wind = new Wind(region, cold, north);
                windList.Add(wind);
                // Creating entire wind flow, with a limited distance it can run
                rand = dice.Next(6 * MASS / 9, 10 * MASS / 9);
                PropagateWind(wind, rand);

                numberWind--;
            }
        }


        //////// TODO //////////

        /// <summary>
        /// Function dedicated to create the entire flow of wind from a starting region
        /// </summary>
        /// <param name="wind"></param>
        public void PropagateWind(Wind wind, int length)
        {
            Region region = wind.stream.First();
            int windX = region.X, windY = region.Y;
            bool stepCheck = true; // True = diagonal step
            int direction = 1; // 1: SE, 2: SW, 3: NW, 4: NE
            int previousHumidity = region.humidity;
            int previousTemperature = region.temperature;
            bool stopWind = false;

            // Winds are moving until they reach either a pole or the equator
            while (windY > 0 && windY < LATITUDE - 1 && length > 0)
            {
                length -= REGION / (MASS * 10);
                // Dividing along the various cases for creating the wind movement. note that all movement are symetrical along the equatorial axis
                if (stepCheck)
                {
                    if (wind.northern)
                    {
                        if (wind.coldWind && (region.isPolar || region.isTemperate))
                        {
                            windX++;
                            windY++;
                            direction = 1;
                        }
                        else if (wind.coldWind && (region.isTropical || region.isDesert))
                        {
                            windX--;
                            windY++;
                            direction = 2;
                        }
                        else if (!wind.coldWind && (region.isTropical || region.isDesert))
                        {
                            windX--;
                            windY--;
                            direction = 3;
                        }
                        else
                        {
                            windX++;
                            windY--;
                            direction = 4;
                        }
                    }
                    else
                    {
                        if (wind.coldWind && (region.isPolar || region.isTemperate))
                        {
                            windX++;
                            windY--;
                            direction = 4;
                        }
                        else if (wind.coldWind && (region.isTropical || region.isDesert))
                        {
                            windX--;
                            windY--;
                            direction = 3;
                        }
                        else if (!wind.coldWind && (region.isTropical || region.isDesert))
                        {
                            windX--;
                            windY++;
                            direction = 2;
                        }
                        else
                        {
                            windX++;
                            windY++;
                            direction = 1;
                        }
                    }
                    stepCheck = false;
                }
                else
                {
                    if (wind.northern)
                    {
                        if (region.isPolar)
                        {
                            windX++;
                        }
                        else if (region.isTropical)
                        {
                            windX--;
                        }
                        else if (wind.coldWind)
                        {
                            windY++;
                        }
                        else
                        {
                            windY--;
                        }
                    }
                    else
                    {
                        if (region.isPolar)
                        {
                            windX++;
                        }
                        else if (region.isTropical)
                        {
                            windX--;
                        }
                        else if (wind.coldWind)
                        {
                            windY--;
                        }
                        else
                        {
                            windY++;
                        }
                    }
                    stepCheck = true;
                }
                // Adjusting the detection coordinate for these to not exit the map's boundaries
                if (windX < 0)
                {
                    windX += LONGITUDE;
                }
                if (windX >= LONGITUDE)
                {
                    windX -= LONGITUDE;
                }
                // Slight optimization, we only do the following if we get in a new region
                if (region.ID != mapMask[windX, windY])
                {
                    // In the case we meet a mountain, the wind is stopped, but a new one is generated a little on the side.
                    // If the new starting area meets another mountain right at generation, it stops, else, the wind continue its course normally
                    if (regionList[mapMask[windX, windY]].isMountain)
                    {
                        switch (direction)
                        {
                            case 1:
                                if (wind.northern)
                                {
                                    if (windY < 5)
                                    {
                                        stopWind = true;
                                    }
                                }
                                else
                                {
                                    if (windY < (LATITUDE / 2) - 5)
                                    {
                                        wind.northern = true;
                                        direction = 3;
                                    }
                                }
                                windX += 5;
                                windY -= 5;
                                if (windX >= LONGITUDE)
                                {
                                    windX -= LONGITUDE;
                                }
                                if (windY < 0)
                                {
                                    windY = 0;
                                }
                                if (regionList[mapMask[windX, windY]].isMountain)
                                {
                                    stopWind = true;
                                }
                                else
                                {
                                    length -= 7 * MASS;
                                }
                                break;
                            case 2:
                                if (!wind.northern)
                                {
                                    if (windY > LATITUDE - 6)
                                    {
                                        stopWind = true;
                                    }
                                }
                                else
                                {
                                    if (windY > (LATITUDE / 2) - 5)
                                    {
                                        wind.northern = false;
                                        direction = 2;
                                    }
                                }
                                windX += 5;
                                windY += 5;
                                if (windX >= LONGITUDE)
                                {
                                    windX -= LONGITUDE;
                                }
                                if (windY >= LATITUDE)
                                {
                                    windY = LATITUDE - 1;
                                }
                                if (regionList[mapMask[windX, windY]].isMountain)
                                {
                                    stopWind = true;
                                }
                                else
                                {
                                    length -= 7 * MASS;
                                }
                                break;
                            case 3:
                                if (!wind.northern)
                                {
                                    if (windY > LATITUDE - 6)
                                    {
                                        stopWind = true;
                                    }
                                }
                                else
                                {
                                    if (windY > (LATITUDE / 2) - 5)
                                    {
                                        wind.northern = false;
                                        direction = 1;
                                    }
                                }
                                windX -= 5;
                                windY += 5;
                                if (windX < 0)
                                {
                                    windX += LONGITUDE;
                                }
                                if (windY >= LATITUDE)
                                {
                                    windY = LATITUDE - 1;
                                }
                                if (regionList[mapMask[windX, windY]].isMountain)
                                {
                                    stopWind = true;
                                }
                                else
                                {
                                    length -= 7 * MASS;
                                }
                                break;
                            case 4:
                                if (wind.northern)
                                {
                                    if (windY < 5)
                                    {
                                        stopWind = true;
                                    }
                                }
                                else
                                {
                                    if (windY < (LATITUDE / 2) - 5)
                                    {
                                        wind.northern = true;
                                        direction = 4;
                                    }
                                }
                                windX -= 5;
                                windY -= 5;
                                if (windX < 0)
                                {
                                    windX += LONGITUDE;
                                }
                                if (windY < 0)
                                {
                                    windY = 0;
                                }
                                if (regionList[mapMask[windX, windY]].isMountain)
                                {
                                    stopWind = true;
                                }
                                else
                                {
                                    length -= 7 * MASS;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    if (!stopWind)
                    {
                        previousHumidity = region.humidity;
                        previousTemperature = region.temperature;
                        region = regionList[mapMask[windX, windY]];
                        // Not adding a wind that is already present
                        if (!wind.stream.Contains(region))
                        {
                            wind.stream.Add(region);
                            region.winds.Add(wind);
                            // We modify the temperature and humidity of the region but also its neighbors
                            if (wind.coldWind)
                            {
                                region.temperature -= dice.Next(6, 10) * (100 - Math.Abs(region.temperature)) / 100;
                            }
                            else
                            {
                                region.temperature += dice.Next(6, 10) * (100 - Math.Abs(region.temperature)) / 100;
                            }
                            region.humidity = (region.humidity * 2 + previousHumidity) / 3;
                            region.temperature = (region.temperature * 2 + previousTemperature) / 3;
                            foreach (Region neighbor in region.neighbors)
                            {
                                neighbor.humidity = (neighbor.humidity * 3 + previousHumidity) / 4;
                                neighbor.temperature = (neighbor.temperature * 3 + previousTemperature) / 4;
                            }
                        }
                    }
                }

            }

        }

        /// <summary>
        /// Places the starting point of water currents and begins their flowing
        /// </summary>
        public void PlaceWaterCurrents()
        {
            int rand;
            Region region;
            WaterCurrent waterCurrent;
            int numberWaterCurrent;
            bool cold = true;

            numberWaterCurrent = dice.Next(REGION / 150, REGION / 120);

            while (numberWaterCurrent != 0)
            {
                // Water currents are limited to water regions, obviously, and starts in polar or tropical areas
                do
                {
                    rand = dice.Next(0, REGION);
                    region = regionList[rand];
                } while (region.waterCurrents.Count != 0 || region.IsLand() || region.isTemperate || region.isDesert);

                // Setting water current parameters
                if (region.isTropical)
                {
                    cold = false;
                }
                else if (region.isPolar)
                {
                    cold = true;
                }

                waterCurrent = new WaterCurrent(region, cold);
                waterCurrentList.Add(waterCurrent);
                // Creating the water current flow
                if (waterCurrent.coldWater)
                {
                    PropagateColdWaterCurrent(waterCurrent);
                }
                else
                {
                    PropagateWarmWaterCurrent(waterCurrent);
                }

                numberWaterCurrent--;
            }
        }

        /// <summary>
        /// Used to create a warm current
        /// </summary>
        /// <param name="waterCurrent"></param>
        public void PropagateWarmWaterCurrent(WaterCurrent waterCurrent)
        {
            int minTemperature;
            int compareTemperature = 100;
            Region nextRegion = null;
            bool continueCurrent = true;
            while (continueCurrent)
            {
                minTemperature = 100;
                nextRegion = null;
                // Warm currents are coming from the tropics and slides to the poles. They tend to go east slightly
                foreach (Region region in waterCurrent.stream.Last().neighbors)
                {
                    if (region.IsWater())
                    {
                        if (region.IsEastFrom(waterCurrent.stream.Last()))
                        {
                            compareTemperature = region.temperature - 20;
                        }
                        else
                        {
                            compareTemperature = region.temperature - 10;
                        }
                        if (compareTemperature < minTemperature)
                        {
                            nextRegion = region;
                            minTemperature = compareTemperature;
                        }
                    }
                }
                // if we found a valid region, we extend the water current
                if (nextRegion != null && !waterCurrent.stream.Contains(nextRegion) && compareTemperature < waterCurrent.stream.Last().temperature)
                {
                    foreach (Region region in waterCurrent.stream.Last().neighbors)
                    {
                        region.temperature += dice.Next(4, 11) * (100 - Math.Abs(region.temperature)) / 100;
                        region.humidity += dice.Next(4, 11) * (100 - region.humidity) / 100;
                    }
                    waterCurrent.stream.Add(nextRegion);
                    nextRegion.waterCurrents.Add(waterCurrent);
                }
                else
                {
                    continueCurrent = false;
                }
            }

        }

        /// <summary>
        /// Used to create a cold current
        /// </summary>
        /// <param name="waterCurrent"></param>
        public void PropagateColdWaterCurrent(WaterCurrent waterCurrent)
        {
            int maxTemperature = -100;
            int compareTemperature = -100;
            Region nextRegion = null;
            bool continueCurrent = true;
            while (continueCurrent)
            {
                maxTemperature = -100;
                nextRegion = null;
                // Cold currents are coming from the poles and slides to the equator. They tend to go west slightly
                foreach (Region region in waterCurrent.stream.Last().neighbors)
                {
                    if (region.IsWater())
                    {
                        if (region.IsWestFrom(waterCurrent.stream.Last()))
                        {
                            compareTemperature = region.temperature + 20;
                        }
                        else
                        {
                            compareTemperature = region.temperature + 10;
                        }
                        if (compareTemperature > maxTemperature)
                        {
                            nextRegion = region;
                            maxTemperature = compareTemperature;
                        }
                    }
                }
                // if we found a valid region, we extend the water current
                if (nextRegion != null && !waterCurrent.stream.Contains(nextRegion) && compareTemperature > waterCurrent.stream.Last().temperature)
                {
                    foreach (Region region in waterCurrent.stream.Last().neighbors)
                    {
                        region.temperature -= dice.Next(4, 11) * (100 - Math.Abs(region.temperature)) / 100;
                        region.humidity += dice.Next(4, 11) * (100 - region.humidity) / 100;
                    }
                    waterCurrent.stream.Add(nextRegion);
                    nextRegion.waterCurrents.Add(waterCurrent);
                }
                else
                {
                    continueCurrent = false;
                }
            }

        }

        #endregion

        #region Geology

        public void GenerateDeposit()
        {
            int rand;
            foreach (Region region in regionList)
            {

                // For each and every type of geological structure, calculation of its probabilities of happening per region
                int canyon, clay, waterfall, undergroundRiver, lake, marsh, minorRiver;
                int iron, copper, lead, aluminum, quicksilver, coal, salt, stone, sand, oil, radioactiveOre, otherMetal, minorDeposit;
                int gold, silver, platinum, semiPrecious, precious, geods, fossils, otherPrecious;
                int explosiveVolcano, effusiveVolcano, caverns, glacier, cliffs, rockFormation, richSoils, coloredSoils, stoneArch, sinkhole, crater;

                if (region.IsLand())
                {
                    canyon = 0;
                    clay = 3;
                    waterfall = 0;
                    undergroundRiver = 3;
                    lake = 7;
                    marsh = 1;
                    minorRiver = 0;
                    iron = 4;
                    copper = 4;
                    lead = 9;
                    aluminum = 8;
                    quicksilver = 7;
                    coal = 9;
                    salt = 2;
                    stone = 21;
                    sand = 4;
                    oil = 7;
                    radioactiveOre = 7;
                    otherMetal = 12;
                    minorDeposit = 100;
                    gold = 1;
                    silver = 2;
                    platinum = 1;
                    semiPrecious = 6;
                    precious = 2;
                    geods = 3;
                    fossils = 8;
                    otherPrecious = 11;
                    explosiveVolcano = 0;
                    effusiveVolcano = 0;
                    caverns = 6;
                    glacier = 0;
                    cliffs = 2;
                    rockFormation = 8;
                    richSoils = 18;
                    coloredSoils = 2;
                    stoneArch = 3;
                    sinkhole = 4;
                    crater = 5;

                    if (region.isHilly)
                    {
                        canyon += 3;
                        undergroundRiver += 7;
                        lake += 5;
                        iron += 20;
                        copper += 7;
                        lead += 5;
                        aluminum += 15;
                        quicksilver += 8;
                        coal += 6;
                        salt += 3;
                        stone += 8;
                        radioactiveOre += 4;
                        otherMetal += 5;
                        gold += 9;
                        silver += 10;
                        platinum += 4;
                        semiPrecious += 15;
                        precious += 11;
                        geods += 7;
                        fossils += 17;
                        otherPrecious += 5;
                        explosiveVolcano += 6;
                        effusiveVolcano += 14;
                        caverns += 11;
                        glacier += 1;
                        cliffs += 15;
                        rockFormation += 11;
                        richSoils -= 5;
                        stoneArch += 4;
                        sinkhole -= 1;
                        crater -= 5;
                    }
                    if (region.isMountain)
                    {
                        undergroundRiver += 9;
                        iron += 6;
                        copper += 12;
                        lead += 3;
                        aluminum += 2;
                        quicksilver += 2;
                        oil -= 15;
                        otherMetal += 3;
                        gold += 7;
                        silver += 9;
                        platinum += 4;
                        semiPrecious += 6;
                        precious += 5;
                        geods += 11;
                        fossils += 2;
                        otherPrecious += 4;
                        explosiveVolcano += 3;
                        effusiveVolcano += 5;
                        caverns += 2;
                        glacier += 3;
                        richSoils -= 18;
                        stoneArch += 1;
                    }
                    if (region.isPolar)
                    {
                        undergroundRiver -= 20;
                        lake -= 100;
                        oil += 9;
                        fossils += 3;
                        glacier += 40;
                    }
                    if (region.isTemperate)
                    {
                        marsh += 5;
                        clay += 6;
                        minorRiver += 30;
                        coal += 8;
                        glacier += 20;
                        richSoils += 17;
                        coloredSoils += 7;
                        crater += 1;
                    }
                    if (region.isDesert)
                    {
                        lake -= 10;
                        quicksilver += 7;
                        coal += 3;
                        salt += 10;
                        sand += 8;
                        oil += 13;
                        gold += 3;
                        semiPrecious += 3;
                        fossils += 3;
                        coloredSoils += 12;
                        crater += 2;
                    }
                    if (region.isTropical)
                    {
                        marsh += 13;
                        clay += 6;
                        minorRiver += 55;
                        coal += 8;
                        sand += 3;
                        semiPrecious += 2;
                        precious += 2;
                        richSoils += 21;
                        coloredSoils += 14;
                        crater += 3;
                    }
                    if (region.humidity < 20)
                    {
                        salt += 25 - region.humidity;
                        cliffs += 15 - region.humidity / 2;
                        rockFormation += 15 - region.humidity / 2;
                    }
                    if (region.humidity > 40)
                    {
                        lake += (region.humidity / 4) - 40;
                        marsh += (region.humidity / 5) - 40;
                        minorRiver += (region.humidity / 5) - 40;
                        richSoils += (region.humidity / 4) - 40;
                    }
                    if (region.height < 20)
                    {
                        marsh += 15 - (region.height / 2);
                        minorRiver += 10;
                        sand += 25 - region.height;
                        glacier -= 25 - region.height;
                        sinkhole += 3;
                    }
                    if (region.temperature < 0)
                    {
                        glacier -= region.temperature / 5;
                    }
                    if (region.rivers.Count == 0)
                    {
                        clay += 31;
                        waterfall += region.height;
                        undergroundRiver += 11;
                        lake += 12;
                        marsh += 6;
                        minorRiver += 4;
                        sand += 7;
                        canyon += 3 + region.height / 2;
                        semiPrecious += 3;
                        caverns += 7;
                        cliffs += 6;
                        richSoils += 8;
                        stoneArch += 3;
                    }
                    if (region.isLargeIsland)
                    {
                        explosiveVolcano += 18;
                        effusiveVolcano += 10;
                        richSoils += 4;
                        coloredSoils += 4;
                        sinkhole += 4;
                        crater -= 2;
                    }


                    rand = dice.Next(1, 101);
                    if (rand <= canyon)
                    {
                        region.geology.Add(new Geology("Canyon"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= clay)
                    {
                        region.geology.Add(new Geology("Clay"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= waterfall)
                    {
                        region.geology.Add(new Geology("Waterfall"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= undergroundRiver)
                    {
                        region.geology.Add(new Geology("UndergroundRiver"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= lake)
                    {
                        region.geology.Add(new Geology("Lake"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= marsh)
                    {
                        region.geology.Add(new Geology("Marsh"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= minorRiver)
                    {
                        region.geology.Add(new Geology("MinorRiver"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= iron)
                    {
                        region.geology.Add(new Geology("Iron"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= copper)
                    {
                        region.geology.Add(new Geology("Copper"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= lead)
                    {
                        region.geology.Add(new Geology("Lead"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= aluminum)
                    {
                        region.geology.Add(new Geology("Aluminum"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= aluminum)
                    {
                        region.geology.Add(new Geology("Quicksilver"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= coal)
                    {
                        region.geology.Add(new Geology("Coal"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= salt)
                    {
                        region.geology.Add(new Geology("Salt"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= stone)
                    {
                        region.geology.Add(new Geology("Stone"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= sand)
                    {
                        region.geology.Add(new Geology("Sand"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= oil)
                    {
                        region.geology.Add(new Geology("Oil"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= radioactiveOre)
                    {
                        region.geology.Add(new Geology("RadioactiveOre"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= otherMetal)
                    {
                        region.geology.Add(new Geology("OtherMetal"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= minorDeposit)
                    {
                        region.geology.Add(new Geology("MinorDeposit"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= gold)
                    {
                        region.geology.Add(new Geology("Gold"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= silver)
                    {
                        region.geology.Add(new Geology("Silver"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= platinum)
                    {
                        region.geology.Add(new Geology("Platinum"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= semiPrecious)
                    {
                        region.geology.Add(new Geology("SemiPrecious"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= precious)
                    {
                        region.geology.Add(new Geology("Precious"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= geods)
                    {
                        region.geology.Add(new Geology("Geods"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= fossils)
                    {
                        region.geology.Add(new Geology("Fossils"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= otherPrecious)
                    {
                        region.geology.Add(new Geology("OtherPrecious"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= explosiveVolcano)
                    {
                        region.geology.Add(new Geology("ExplosiveVolcano"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= effusiveVolcano)
                    {
                        region.geology.Add(new Geology("EffusiveVolcano"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= caverns)
                    {
                        region.geology.Add(new Geology("Caverns"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= glacier)
                    {
                        region.geology.Add(new Geology("Glacier"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= cliffs)
                    {
                        region.geology.Add(new Geology("Cliffs"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= rockFormation)
                    {
                        region.geology.Add(new Geology("RockFormation"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= richSoils)
                    {
                        region.geology.Add(new Geology("RichSoils"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= coloredSoils)
                    {
                        region.geology.Add(new Geology("ColoredSoils"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= stoneArch)
                    {
                        region.geology.Add(new Geology("StoneArch"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= sinkhole)
                    {
                        region.geology.Add(new Geology("Sinkhole"));
                    }
                    rand = dice.Next(1, 101);
                    if (rand <= crater)
                    {
                        region.geology.Add(new Geology("Crater"));
                    }
                }

            }


        }

        #endregion

        #region Plants
        // Generate plants

        // Propagate plants

        // Check if a new pass is required to loop on

        // Calculate resources
        #endregion


        #region Display

        /// <summary>
        /// A function made so we only update the 
        /// </summary>
        public void UpdateGeology()
        {
            foreach (Region region in regionList)
            {

                foreach (var geology in region.geology)
                {
                    foreach (string res in geology.resources.Keys)
                    {
                        foreach (string key in region.naturalResource.resources.Keys)
                        {
                            if (key == res && geology.resources[res] != 0)
                            {
                                region.naturalResource.resources[key] += geology.resources[res];
                                goto EndDictionaryLoop;
                            }
                        }
                        EndDictionaryLoop:;
                    }
                }
            }
        }

        /// <summary>
        /// Fills the info box when clicking on a region
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public string RegionInfo(int x, int y)
        {
            string result = "";
            int IDregion = mapMask[x, y];

            result += "ID REGION : " + regionList[IDregion].ID.ToString() + Environment.NewLine;

            result += "CONTINENT ID : " + regionList[IDregion].generationOrder.ToString() + Environment.NewLine;

            result += "REGION HEIGHT : " + regionList[IDregion].height.ToString() + Environment.NewLine;

            result += "TEMPERATURE : " + regionList[IDregion].temperature.ToString() + Environment.NewLine;

            result += "HUMIDITY : " + regionList[IDregion].humidity.ToString() + Environment.NewLine;

            #region Boolean
            if (regionList[IDregion].isSea)
            {
                result += "Sea : " + regionList[IDregion].isSea.ToString() + Environment.NewLine;
            }
            if (regionList[IDregion].isMountain)
            {
                result += "Mountain Range : " + regionList[IDregion].isMountain.ToString() + Environment.NewLine;
            }
            if (regionList[IDregion].isOcean)
            {
                result += "Ocean : " + regionList[IDregion].isOcean.ToString() + Environment.NewLine;
            }
            if (regionList[IDregion].isHilly)
            {
                result += "Hill : " + regionList[IDregion].isHilly.ToString() + Environment.NewLine;
            }
            if (regionList[IDregion].isLargeIsland)
            {
                result += "Large Island : " + regionList[IDregion].isLargeIsland.ToString() + Environment.NewLine;
            }
            if (regionList[IDregion].isOceanTrench)
            {
                result += "Deep Ocean Trench : " + regionList[IDregion].isOceanTrench.ToString() + Environment.NewLine;
            }
            if (regionList[IDregion].isContinent)
            {
                result += "Continent : " + regionList[IDregion].isContinent.ToString() + Environment.NewLine;
            }
            if (regionList[IDregion].isSmallIsland)
            {
                result += "Small island : " + regionList[IDregion].isSmallIsland.ToString() + Environment.NewLine;
            }

            if (regionList[IDregion].isPolar)
            {
                result += "Polar : " + regionList[IDregion].isPolar.ToString() + Environment.NewLine;
            }
            if (regionList[IDregion].isTemperate)
            {
                result += "Temperate : " + regionList[IDregion].isTemperate.ToString() + Environment.NewLine;
            }
            if (regionList[IDregion].isDesert)
            {
                result += "Desert : " + regionList[IDregion].isDesert.ToString() + Environment.NewLine;
            }
            if (regionList[IDregion].isTropical)
            {
                result += "Tropical : " + regionList[IDregion].isTropical.ToString() + Environment.NewLine;
            }
            if (regionList[IDregion].rivers.Count > 0)
            {
                result += "River : " + regionList[IDregion].rivers.Count.ToString() + Environment.NewLine;
            }
            if (regionList[IDregion].winds.Count > 0)
            {
                result += "Wind : " + regionList[IDregion].winds.Count.ToString() + Environment.NewLine;
            }
            //if (regionList[IDregion].hasMarineStream)
            //{
            //    result += "Marine Stream : " + regionList[IDregion].hasMarineStream.ToString() + Environment.NewLine;
            //}
            #endregion
            foreach (var item in regionList[IDregion].naturalResource.resources)
            {
                if (item.Value > 0)
                {
                    result += item.Key + " : " + item.Value + Environment.NewLine;
                }
            }

            return result;
        }

        /// <summary>
        /// Draws a simple map with high contrast showing the shape of regions
        /// </summary>
        public void DrawRegionMap()
        {
            int i = 0, j = 0, colorRed = 0, colorGreen = 0, colorBlue = 0;

            // We simply assignate one color per region and paint it that way
            while (i < LONGITUDE)
            {
                j = 0;
                while (j < LATITUDE)
                {
                    // Painting the map, brown to white for continents, green blue to deep blue for water
                    if (mapMask[i, j] >= 0)
                    {
                        colorRed = 25 * regionList[mapMask[i, j]].ID % 255;
                        colorGreen = 25 * (regionList[mapMask[i, j]].ID / 5) % 255;
                        colorBlue = 25 * (regionList[mapMask[i, j]].ID / 20) % 255;
                    }
                    else
                    {
                        colorRed = 255;
                        colorGreen = 255;
                        colorBlue = 255;
                    }
                    mapPaint.SetPixel(i, j, Color.FromArgb(colorRed, colorGreen, colorBlue));
                    j++;
                }
                i++;
            }
            mapPaint.Save(Program.filePath + "map.png");
        }

        /// <summary>
        /// Draws the general altitude of a region, white is higher
        /// </summary>
        public void DrawHeightMap()
        {
            int i = 0, j = 0, colorRed = 0, colorGreen = 0, colorBlue = 0;
            while (i < LONGITUDE)
            {
                j = 0;
                while (j < LATITUDE)
                {
                    // Painting the map, brown to white for continents, green blue to deep blue for water
                    if (regionList[mapMask[i, j]].isSmallIsland)
                    {
                        colorRed = 120;
                        colorGreen = 170;
                        colorBlue = 60;
                    }
                    else if (regionList[mapMask[i, j]].height < 0)
                    {
                        colorRed = 0;
                        colorGreen = 60 + regionList[mapMask[i, j]].height / 2;
                        colorBlue = 150 - regionList[mapMask[i, j]].height / 4;
                    }
                    else if (regionList[mapMask[i, j]].height > 0)
                    {
                        colorRed = 100 + regionList[mapMask[i, j]].height * 5 / 4;
                        colorGreen = 80 + (regionList[mapMask[i, j]].height) * 3 / 2;
                        colorBlue = 40 + (regionList[mapMask[i, j]].height) * 2;
                    }
                    else
                    {
                        colorRed = 0;
                        colorGreen = 0;
                        colorBlue = 0;
                    }
                    mapPaint.SetPixel(i, j, Color.FromArgb(colorRed, colorGreen, colorBlue));
                    j++;
                }
                i++;
            }
            mapPaint.Save(Program.filePath + "mapHeight.png");
        }

        /// <summary>
        /// Temperature gradient, with red being hot and blue being cold
        /// </summary>
        public void DrawTemperatureMap()
        {
            int i = 0, j = 0, colorRed = 0, colorGreen = 0, colorBlue = 0;
            while (i < LONGITUDE)
            {
                j = 0;
                while (j < LATITUDE)
                {

                    colorRed = 120 + regionList[mapMask[i, j]].temperature;
                    colorGreen = 220 - Math.Abs(regionList[mapMask[i, j]].temperature) * 2;
                    colorBlue = 120 - regionList[mapMask[i, j]].temperature;

                    mapPaint.SetPixel(i, j, Color.FromArgb(colorRed, colorGreen, colorBlue));
                    j++;
                }
                i++;
            }
            mapPaint.Save(Program.filePath + "mapTemperature.png");
        }

        /// <summary>
        /// Draws the humidity, the greener the more humid
        /// </summary>
        public void DrawHumidityMap()
        {
            int i = 0, j = 0, colorRed = 0, colorGreen = 0, colorBlue = 0;
            while (i < LONGITUDE)
            {
                j = 0;
                while (j < LATITUDE)
                {

                    colorRed = 0;
                    colorGreen = 20 + regionList[mapMask[i, j]].humidity * 2;
                    colorBlue = 20;

                    mapPaint.SetPixel(i, j, Color.FromArgb(colorRed, colorGreen, colorBlue));
                    j++;
                }
                i++;
            }
            mapPaint.Save(Program.filePath + "mapHumidity.png");
        }

        /// <summary>
        /// Shows where mountains and hills are present
        /// </summary>
        public void DrawMountainMap()
        {
            int i = 0, j = 0, colorRed = 0, colorGreen = 0, colorBlue = 0;
            while (i < LONGITUDE)
            {
                j = 0;
                while (j < LATITUDE)
                {
                    // A technical map to show where mountains and hills are located
                    if (regionList[mapMask[i, j]].isHilly && regionList[mapMask[i, j]].isMountain)
                    {
                        colorRed = 250;
                        colorGreen = 200;
                        colorBlue = 200;
                    }
                    else if (!regionList[mapMask[i, j]].isMountain && regionList[mapMask[i, j]].isHilly)
                    {
                        colorRed = 200;
                        colorGreen = 80;
                        colorBlue = 80;
                    }
                    else if (!regionList[mapMask[i, j]].isHilly && regionList[mapMask[i, j]].isMountain)
                    {
                        colorRed = 250;
                        colorGreen = 100;
                        colorBlue = 100;
                    }
                    else if (regionList[mapMask[i, j]].IsLand())
                    {
                        colorRed = 120;
                        colorGreen = 150;
                        colorBlue = 40;
                    }
                    else if (regionList[mapMask[i, j]].IsWater())
                    {
                        colorRed = 10;
                        colorGreen = 10;
                        colorBlue = 250;
                    }
                    mapPaint.SetPixel(i, j, Color.FromArgb(colorRed, colorGreen, colorBlue));
                    j++;
                }
                i++;
            }
            mapPaint.Save(Program.filePath + "mapMountain.png");
        }

        /// <summary>
        /// Presents the regions filled with a river
        /// </summary>
        public void DrawRiverMap()
        {
            int i = 0, j = 0, colorRed = 0, colorGreen = 0, colorBlue = 0;
            while (i < LONGITUDE)
            {
                j = 0;
                while (j < LATITUDE)
                {
                    // A technical map to show rivers
                    if (regionList[mapMask[i, j]].rivers.Count > 0 && regionList[mapMask[i, j]].IsLand())
                    {
                        colorRed = 50;
                        colorGreen = 100 + regionList[mapMask[i, j]].riverStream;
                        colorBlue = 50;
                        if (colorGreen > 255)
                        {
                            colorGreen = 255;
                        }
                    }
                    else if (regionList[mapMask[i, j]].rivers.Count > 0 && regionList[mapMask[i, j]].IsWater())
                    {
                        colorRed = 10;
                        colorGreen = 40;
                        colorBlue = 200;
                    }
                    else if (regionList[mapMask[i, j]].IsLand())
                    {
                        colorRed = 120;
                        colorGreen = 150;
                        colorBlue = 40;
                    }
                    else if (regionList[mapMask[i, j]].IsWater())
                    {
                        colorRed = 10;
                        colorGreen = 10;
                        colorBlue = 250;
                    }
                    mapPaint.SetPixel(i, j, Color.FromArgb(colorRed, colorGreen, colorBlue));
                    j++;
                }
                i++;
            }
            mapPaint.Save(Program.filePath + "mapRiver.png");
        }

        /// <summary>
        /// Presents the regions filled with winds
        /// </summary>
        public void DrawWindMap()
        {
            int i = 0, j = 0, colorRed = 0, colorGreen = 0, colorBlue = 0;
            while (i < LONGITUDE)
            {
                j = 0;
                while (j < LATITUDE)
                {
                    // A technical map to show rivers
                    if (regionList[mapMask[i, j]].winds.Count > 0 && regionList[mapMask[i, j]].IsLand())
                    {
                        colorRed = 120;
                        colorGreen = 220;
                        colorBlue = 40;
                    }
                    else if (regionList[mapMask[i, j]].winds.Count > 0 && regionList[mapMask[i, j]].IsWater())
                    {
                        colorRed = 10;
                        colorGreen = 220;
                        colorBlue = 250;
                    }
                    else if (regionList[mapMask[i, j]].IsLand() && regionList[mapMask[i, j]].isPolar)
                    {
                        colorRed = 80;
                        colorGreen = 150;
                        colorBlue = 120;
                    }
                    else if (regionList[mapMask[i, j]].IsLand() && regionList[mapMask[i, j]].isTemperate)
                    {
                        colorRed = 80;
                        colorGreen = 180;
                        colorBlue = 60;
                    }
                    else if (regionList[mapMask[i, j]].IsLand() && regionList[mapMask[i, j]].isDesert)
                    {
                        colorRed = 120;
                        colorGreen = 150;
                        colorBlue = 40;
                    }
                    else if (regionList[mapMask[i, j]].IsLand() && regionList[mapMask[i, j]].isTropical)
                    {
                        colorRed = 180;
                        colorGreen = 250;
                        colorBlue = 120;
                    }
                    else if (regionList[mapMask[i, j]].IsWater())
                    {
                        colorRed = 10;
                        colorGreen = 10;
                        colorBlue = 250;
                    }
                    mapPaint.SetPixel(i, j, Color.FromArgb(colorRed, colorGreen, colorBlue));
                    j++;
                }
                i++;
            }
            mapPaint.Save(Program.filePath + "mapWind.png");
        }

        /// <summary>
        /// Presents the regions filled with water currents
        /// </summary>
        public void DrawWaterCurrentMap()
        {
            int i = 0, j = 0, colorRed = 0, colorGreen = 0, colorBlue = 0;
            while (i < LONGITUDE)
            {
                j = 0;
                while (j < LATITUDE)
                {
                    // A technical map to show water currents
                    if (regionList[mapMask[i, j]].IsLand())
                    {
                        colorRed = 120;
                        colorGreen = 150;
                        colorBlue = 40;
                    }
                    else if (regionList[mapMask[i, j]].waterCurrents.Count > 0 && regionList[mapMask[i, j]].IsWater())
                    {
                        colorRed = 10;
                        colorGreen = 220;
                        colorBlue = 250;
                    }
                    else if (regionList[mapMask[i, j]].IsWater())
                    {
                        colorRed = 10;
                        colorGreen = 10;
                        colorBlue = 250;
                    }
                    mapPaint.SetPixel(i, j, Color.FromArgb(colorRed, colorGreen, colorBlue));
                    j++;
                }
                i++;
            }
            mapPaint.Save(Program.filePath + "mapWaterCurrent.png");
        }

        /// <summary>
        /// Shows the regions generated by the landmass creation
        /// </summary>
        public void DrawContinentMap()
        {
            int i = 0, j = 0, colorRed = 0, colorGreen = 0, colorBlue = 0;
            while (i < LONGITUDE)
            {
                j = 0;
                while (j < LATITUDE)
                {
                    // Painting the map, brown to white for continents, green blue to deep blue for water

                    colorRed = (25 * regionList[mapMask[i, j]].generationOrder) % 255;
                    colorGreen = 50 * (regionList[mapMask[i, j]].generationOrder / 10) % 255;
                    colorBlue = 0;


                    mapPaint.SetPixel(i, j, Color.FromArgb(colorRed, colorGreen, colorBlue));
                    j++;
                }
                i++;
            }
            mapPaint.Save(Program.filePath + "mapContinent.png");
        }

        /// <summary>
        /// A map combining height and temperature
        /// </summary>
        public void DrawClimateMap()
        {
            int i = 0, j = 0, colorRed = 0, colorGreen = 0, colorBlue = 0;
            //This map combines temperature and height, but only on land
            while (i < LONGITUDE)
            {
                j = 0;
                while (j < LATITUDE)
                {
                    if (regionList[mapMask[i, j]].height <= 0)
                    {
                        colorRed = 0;
                        colorGreen = 25;
                        colorBlue = 200;
                    }
                    else
                    {
                        colorRed = ((100 + regionList[mapMask[i, j]].height * 5 / 4) * (120 + regionList[mapMask[i, j]].temperature)) / 255;
                        colorGreen = ((80 + (regionList[mapMask[i, j]].height) * 3 / 2) * (220 - Math.Abs(regionList[mapMask[i, j]].temperature) * 2)) / 255;
                        colorBlue = ((40 + (regionList[mapMask[i, j]].height) * 2) * (120 - regionList[mapMask[i, j]].temperature)) / 255;
                    }

                    mapPaint.SetPixel(i, j, Color.FromArgb(colorRed, colorGreen, colorBlue));
                    j++;
                }
                i++;
            }
            mapPaint.Save(Program.filePath + "mapClimate.png");
        }
        #endregion
    }
}
