using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Drawing;

namespace UniversalysWorldGenerator
{
    class WorldMap
    {
        public static int LATITUDE = 800, LONGITUDE = 1200, REGION = 3000, RANDOMSTEP = 4;
        public Bitmap mapPaint = new Bitmap(LONGITUDE, LATITUDE);
        // TODO : STOCK MAPMASK IN DATABASE
        public int[,] mapMask = new int[LONGITUDE, LATITUDE];
        public Region[] regionList = new Region[REGION];
        Random dice = new Random();
        public List<Region> notTreatedRegions = new List<Region>();
        public List<River> riverList = new List<River>();
        public List<Wind> windList = new List<Wind>();
        public List<WaterCurrent> waterCurrentList = new List<WaterCurrent>();
        int continentID = 0;

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
                current.Y = dice.Next(((2 * i / REGION) * LATITUDE) / 5, LATITUDE - ((2 * i / REGION) * LATITUDE) / 5);

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
        /// Main function for generating the landmass
        /// </summary>
        /// <param name="type"></param>
        /// <param name="size"></param>
        public void CreateLandmass(int type, int size)
        {

            Region region;
            List<Region> neighbors;
            Region currentRegion;

            int i, rand;
            List<Region> treatedEmptyRegions = new List<Region>();
            int startingSize = size;

            // 1) Choose the center to spread from
            rand = dice.Next(0, notTreatedRegions.Count);
            currentRegion = notTreatedRegions[rand];
            region = regionList[currentRegion.ID];
            /* 
             * 1 : Ocean
             * 2 : Continent
             * 3 : Hill
             * 4 : Archipelago 
             * J'ai ajouté l'assignation à chaque région. Maintenant les régions seront accessibles grâce à leur appartenance. On peut désigner tous les continents par exemple est augmenter leur température. 
             * Je pense que nous devrions revoir le fait de créer des collines comme ça car il y a des collines en plein milieu de l'océan. ou alors les remplacer par des îles?
             * I added assignment to each region. Now we can access to every regions by there belonging. by example we can designate every continents and increase their temperatures.
             * I think we should revise the fact that we create hills because they are generated right in the middle of the oceans, or maybe replace their names by island?
            */
            // Added: 5 for inner seas, 6 for plains, 7 for plateau and 8 for island chains
            switch (type)
            {
                // Those are oceans, with deep water abound
                case 1:
                    region.height = dice.Next(0, 10) + dice.Next(0, 10) - 85;
                    region.isOcean = true;
                    if (region.height < -80)
                    {
                        region.isOceanTrench = true;
                    }
                    region.continentID = continentID;
                    break;
                // Those are continental masses, these tends to keep at a low altitude. For now
                case 2:
                    region.height = dice.Next(0, 5) + dice.Next(0, 16) + 5;
                    region.isContinent = true;
                    region.continentID = continentID;
                    break;
                // Those are "shield" type terrains that grows more upward than most continents, represents highlands, mesas and other former volcanic structures 
                case 3:
                    region.height = dice.Next(0, 24) + dice.Next(0, 8) + 12;
                    region.isHilly = true;
                    //Si une région est collé a un continent alors elle appartient à ce continent 
                    //If a region is neighbors with a continent so she belong to this.
                    foreach (Region item in region.neighbors)
                    {
                        if (item.isContinent)
                        {
                            region.isContinent = true;
                            goto ContinentDetect;
                        }
                    }
                    ContinentDetect:
                    region.continentID = continentID;
                    break;
                // Those are archipelagoes. It designate areas with a higher sea level surrounded by small landmasses. Examples includes all of South East Asia, the North sea, the Carribeans...
                case 4:
                    region.height = dice.Next(0, 40) + dice.Next(0, 15) - 60;
                    region.isLargeIsland = true;
                    if (region.height < 0)
                    {
                        region.isLargeIsland = false;
                        region.isSea = true;
                    }
                    region.continentID = continentID;
                    break;
                // Small seas with a slightly lower level than archipelago
                case 5:
                    region.height = dice.Next(0, 10) + dice.Next(0, 10) - 35;
                    region.isSea = true;

                    region.continentID = continentID;
                    break;
                // Plains, with a lower level than most areas, perfect for rivers to flow into
                case 6:
                    region.height = dice.Next(0, 7) + dice.Next(0, 8) + 2;
                    region.isContinent = true;

                    region.continentID = continentID;
                    break;
                // Highlands standing alone in some areas
                case 7:
                    region.height = dice.Next(0, 14) + dice.Next(0, 10) + 22;
                    region.isContinent = true;
                    region.isHilly = true;
                    region.continentID = continentID;
                    break;
                // Small islands and shallow seas
                case 8:
                    region.height = dice.Next(0, 12) + dice.Next(0, 12);
                    region.isSmallIsland = true;
                    if (region.height <= 0)
                    {
                        if (region.height == 0)
                        {
                            region.height = -1;
                        }
                        region.isSea = true;
                        region.isContinent = false;
                    }
                    else
                    {
                        region.isLargeIsland = true;
                    }

                    region.continentID = continentID;
                    break;
            }
            neighbors = new List<Region>(region.neighbors);
            notTreatedRegions.Remove(currentRegion);

            // 2) Use the neighbors to potentially spread the area type. When reaching the optimal size, the RANDOMSTEP value will ensure some irrgularity to avoid massive blobs
            while (neighbors.Count > 0 && size >= 0)
            {
                rand = dice.Next(0, neighbors.Count * 2) * dice.Next(0, neighbors.Count / 2) / neighbors.Count;
                currentRegion = neighbors[rand];
                neighbors.RemoveAt(rand);
                if (notTreatedRegions.Contains(currentRegion) && !treatedEmptyRegions.Contains(currentRegion))
                {
                    region = regionList[currentRegion.ID];
                    region.continentID = continentID;

                    // When the area remaining to cover lowers, we do not always fill the region to to not create a too uniform structure
                    if (size > dice.Next(1, 101 + RANDOMSTEP * 2))
                    {
                        switch (type)
                        {
                            case 1:
                                // Oceans tends to create deep trenches on the borders
                                region.height = dice.Next(0, 10) + dice.Next(0, 10) - 85 - (startingSize - size) / (startingSize / 12);
                                region.isOcean = true;
                                if (region.height < -70)
                                {
                                    region.isOceanTrench = true;
                                }

                                break;
                            case 2:
                                // Continents are overall of an unified, flat land
                                region.height = dice.Next(0, 5) + dice.Next(0, 16) + 5; ;
                                region.isContinent = true;
                                break;
                            case 3:
                                // Hilly regions have a slightly higher altitude, and are more random in their distribution
                                region.height = dice.Next(0, 24) + dice.Next(0, 8) + 12;
                                region.isHilly = true;
                                break;
                            case 4:
                                // Archipelagoes tends to create small pieces of land on the borders
                                region.height = dice.Next(0, 40) + dice.Next(0, 15) - 60 + (startingSize - size) / (startingSize / 10);
                                region.isLargeIsland = true;
                                if (region.height < 0)
                                {
                                    region.isLargeIsland = false;
                                    region.isSea = true;
                                }
                                break;
                            case 5:
                                // The smaller seas tends to be a little deeper in the center
                                region.height = dice.Next(0, 10) + dice.Next(0, 10) - 35 + (startingSize - size) / (startingSize / 10);
                                region.isSea = true;
                                break;
                            case 6:
                                // Plain are plain and simple as befit for these
                                region.height = dice.Next(0, 7) + dice.Next(0, 8) + 2;
                                region.isContinent = true;
                                break;
                            case 7:
                                // Highlands are relatively chaotic, but not as much as real hills
                                region.height = dice.Next(0, 14) + dice.Next(0, 10) + 22;
                                region.isContinent = true;
                                region.isHilly = true;
                                break;
                            case 8:
                                // The small island areas  tends to focus these in the center
                                region.height = dice.Next(0, 12) + dice.Next(0, 12) - (startingSize - size) / (startingSize / 10);
                                region.isSmallIsland = true;
                                if (region.height <= 0)
                                {
                                    if (region.height == 0)
                                    {
                                        region.height = -1;
                                    }
                                    region.isSea = true;
                                    region.isContinent = false;
                                }
                                else
                                {
                                    region.isLargeIsland = true;
                                }
                                break;
                        }

                        //We eliminate height = 0 cases here, turning these into water
                        if (region.height == 0)
                        {
                            region.height = -1;
                            region.isLargeIsland = false;
                            region.isSea = true;
                        }
                        i = 0;
                        while (i < region.neighbors.Count)
                        {
                            neighbors.Add(region.neighbors[i]);
                            i++;
                        }
                        notTreatedRegions.Remove(currentRegion);
                    }
                    else
                    {
                        treatedEmptyRegions.Add(currentRegion);
                    }
                    //A higher RANDOMSTEP allows for smaller regions without changing the global size of a region
                    size -= RANDOMSTEP;

                }
            }

        }

        public void CreateValley(Region region)
        {
            int size = dice.Next(12 - RANDOMSTEP / 2, 30 - RANDOMSTEP);
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
                region.isValley = true;
                region.continentID = continentID;
                notTreatedRegions.Remove(region);
                neighborRegions.Remove(region);
                valley.Add(region);
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
                region = neighborRegions[dice.Next(0, neighborRegions.Count / 2)];
                size--;
            } while (size > 0);

            // Place then a sea region as a starting point
            region.height = -dice.Next(1, 5) * dice.Next(1, 5) - dice.Next(1, 6);
            region.isSea = true;
            region.continentID = continentID;
            notTreatedRegions.Remove(region);
            neighborRegions.Remove(region);
            sea.Add(region);
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

            // Place two hills region

            size = 2;
            do
            {
                region.height = dice.Next(3, 7) * dice.Next(3, 6) + dice.Next(7, 14);
                region.isHilly = true;
                region.continentID = continentID;
                notTreatedRegions.Remove(region);
                neighborRegions.Remove(region);
                hill.Add(region);
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
            // And now to extend the structure
            size = dice.Next(24 - RANDOMSTEP / 2, 45 - RANDOMSTEP);
            do
            {
                // Here we test the presence of the sea nearby first, then of hill. If none of those, it is a valley
                nextToSea = false;
                nextToHill = false;

                region = neighborRegions[dice.Next(0, neighborRegions.Count / 2)];
                region.continentID = continentID;
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
                    region.isSea = true;
                    region.height = -dice.Next(1, 5) * dice.Next(1, 5) - dice.Next(1, 6);
                    sea.Add(region);
                }
                else if (nextToHill)
                {
                    region.isHilly = true;
                    region.height = dice.Next(3, 7) * dice.Next(3, 6) + dice.Next(7, 14);
                    hill.Add(region);
                }
                else
                {
                    region.isValley = true;
                    region.height = dice.Next(1, 5) * dice.Next(1, 4) + dice.Next(1, 6);
                    valley.Add(region);
                }
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
        }

        public void CreateOcean(Region region)
        {
            int size = dice.Next(45 - RANDOMSTEP, 120 - RANDOMSTEP * 2);

            List<Region> neighborRegions = new List<Region>();
            do
            {
                if (dice.Next(1, 40) == 1)
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
                    region.isOcean = true;
                }
                
                region.continentID = continentID;
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
        }

        public void CreateCoastalMountains(Region region)
        {
            int size = dice.Next(12 - RANDOMSTEP / 2, 22 - RANDOMSTEP);
            int baseHeight = dice.Next(5, 9) * dice.Next(3, 7);
            bool north = false, east = false; // Allows to choose which side is the sea

            List<Region> neighborRegions = new List<Region>();
            List<Region> hillNeighborRegions = new List<Region>();
            List<Region> seaNeighborRegions = new List<Region>();
            List<Region> mountain = new List<Region>();

            if (dice.Next(1, 3) == 1)
            {
                north = true;
            }
            if (dice.Next(1, 3) == 1)
            {
                east = true;
            }

            do
            {
                region.height = baseHeight + dice.Next(10, 40);
                region.isMountain = true;
                if (dice.Next(1,5) == 1)
                {
                    region.isVolcano = true;
                }

                region.continentID = continentID;
                notTreatedRegions.Remove(region);
                neighborRegions.Remove(region);
                foreach (Region neighbor in region.neighbors)
                {
                    if (neighborRegions.Contains(neighbor))
                    {
                        if ((neighbor.IsNorthFrom(region) && north) || (neighbor.IsSouthFrom(region) && !north))
                        {
                            if ((neighbor.IsEastFrom(region) && east) || (neighbor.IsWestFrom(region) && !east))
                            {
                                seaNeighborRegions.Add(neighbor);
                            }
                            else
                            {
                                hillNeighborRegions.Add(neighbor);
                            }
                        }
                        else
                        {
                            if ((neighbor.IsEastFrom(region) && east) || (neighbor.IsWestFrom(region) && !east))
                            {
                                hillNeighborRegions.Add(neighbor);
                            }
                            else
                            {
                                seaNeighborRegions.Add(neighbor);
                            }
                        }
                        neighborRegions.Remove(neighbor);
                    }
                    else if (notTreatedRegions.Contains(neighbor) && !(seaNeighborRegions.Contains(neighbor) || hillNeighborRegions.Contains(neighbor)))
                    {
                        neighborRegions.Add(neighbor);
                    }
                }
                if (neighborRegions.Count == 0)
                {
                    goto finish;
                }
                region = neighborRegions[dice.Next(0, neighborRegions.Count)];
                size--;
            } while (size > 0);
            
            finish:;
        }

        public void GenerateLandmass()
        {
            int rand;

            notTreatedRegions.AddRange(regionList);
            Region region;

            while (notTreatedRegions.Count != 0)
            {
                region = notTreatedRegions[dice.Next(0, notTreatedRegions.Count)];
                CreateOcean(region);
            }

            //while (notTreatedRegions.Count > (2 * REGION / 5))
            //{
            //    // Choosing what kind of landmass to build
            //    rand = dice.Next(0, 12);
            //    switch (rand)
            //    {
            //        case 0:
            //            // Very large ocean
            //            CreateLandmass(1, 3800 / RANDOMSTEP);
            //            break;
            //        case 1:
            //            // Large ocean
            //            CreateLandmass(1, 3400 / RANDOMSTEP);
            //            break;
            //        case 2:
            //            // Ocean
            //            CreateLandmass(1, 2800 / RANDOMSTEP);
            //            break;
            //        case 3:
            //            // Small Ocean
            //            CreateLandmass(1, 2500 / RANDOMSTEP);
            //            break;
            //        case 4:
            //            // Smaller Ocean
            //            CreateLandmass(1, 2000 / RANDOMSTEP);
            //            break;
            //        case 5:
            //            // Large Continent
            //            CreateLandmass(2, 2000 / RANDOMSTEP);
            //            break;
            //        case 6:
            //            // Continent
            //            CreateLandmass(2, 1700 / RANDOMSTEP);
            //            break;
            //        case 7:
            //            // Small Continent
            //            CreateLandmass(2, 1500 / RANDOMSTEP);
            //            break;
            //        case 8:
            //            // Little Continent
            //            CreateLandmass(2, 1100 / RANDOMSTEP);
            //            break;
            //        case 9:
            //            // Big Hilly
            //            CreateLandmass(3, 1250 / RANDOMSTEP);
            //            break;
            //        case 10:
            //            // Small Hilly
            //            CreateLandmass(3, 1000 / RANDOMSTEP);
            //            break;
            //        case 11:
            //            // Archipelago
            //            CreateLandmass(4, 1400 / RANDOMSTEP);
            //            break;

            //    }
            //    continentID++;
            //}
            //while (notTreatedRegions.Count > 0)
            //{
            //    // Choosing what kind of landmass to build
            //    rand = dice.Next(0, 10);
            //    switch (rand)
            //    {
            //        case 0:
            //            // Inner sea
            //            CreateLandmass(5, 1100 / RANDOMSTEP);
            //            break;
            //        case 1:
            //            // Small inner sea
            //            CreateLandmass(5, 850 / RANDOMSTEP);
            //            break;
            //        case 2:
            //            // Massive Plains
            //            CreateLandmass(6, 1000 / RANDOMSTEP);
            //            break;
            //        case 3:
            //            // Plains
            //            CreateLandmass(6, 800 / RANDOMSTEP);
            //            break;
            //        case 4:
            //            // Small plains
            //            CreateLandmass(6, 650 / RANDOMSTEP);
            //            break;
            //        case 5:
            //            // Plateau
            //            CreateLandmass(7, 600 / RANDOMSTEP);
            //            break;
            //        case 6:
            //            // Small plateau
            //            CreateLandmass(7, 400 / RANDOMSTEP);
            //            break;
            //        case 7:
            //            // Small hills
            //            CreateLandmass(7, 250 / RANDOMSTEP);
            //            break;
            //        case 8:
            //            // Island chains
            //            CreateLandmass(8, 450 / RANDOMSTEP);
            //            break;
            //        case 9:
            //            // Small island chain
            //            CreateLandmass(8, 300 / RANDOMSTEP);
            //            break;
            //    }
            //    continentID++;
            //}
            //// Creating a random amount of mountain ranges
            //int j = 0;
            //rand = dice.Next(REGION / 200, REGION / 160);
            //while (j < rand)
            //{
            //    CreateMountainRange();
            //    j++;
            //}
        }

        public void CreateMountainRange()
        {
            Region region;

            int i;
            int rand;
            int globalHeight;
            List<Region> hillyRegions = new List<Region>();
            List<Region> globalNeighbors = new List<Region>();
            // 1) Choose the starting point to spread from

            do
            {
                rand = dice.Next(0, REGION);
            } while (regionList[rand].IsWater() || regionList[rand].isMountain);
            region = regionList[rand];
            globalHeight = dice.Next(35, 60);
            region.height = globalHeight + dice.Next(0, 20) + dice.Next(0, 20);
            region.isMountain = true;

            //Add region Neighbors to global neighbors list

            hillyRegions.AddRange(region.neighbors);

            i = dice.Next(8, 35 - RANDOMSTEP);


            // 2) Use the neighbors to spread
            while (i > 0)
            {

                rand = dice.Next(0, hillyRegions.Count);

                region = hillyRegions[rand];
                if (region.height > 0)
                {
                    region.height = globalHeight + dice.Next(0, 20) + dice.Next(0, 20);
                    region.isMountain = true;
                }
                else if (region.height < -40)
                {
                    region.height = -dice.Next(40, 80) - 10;
                    if (region.height < -70)
                    {
                        region.isOceanTrench = true;
                    }
                }
                hillyRegions.Remove(region);

                globalNeighbors.AddRange(hillyRegions);
                hillyRegions.Clear();
                hillyRegions.AddRange(region.neighbors);

                i--;
            }
            globalNeighbors = globalNeighbors.Distinct().ToList();
            //If in the global neighbors list the current evalued item is not a MoutainRange change it to Hill.
            i = 0;
            foreach (Region global in globalNeighbors)
            {
                if (!global.isMountain)
                {
                    if (global.height < -70)
                    {
                        global.isOceanTrench = true;
                    }
                    else if (global.height < -40)
                    {
                        global.isOcean = true;
                    }
                    else if (global.height < 0 && !(global.height < -40))
                    {
                        global.isSea = true;
                    }
                    else
                    {
                        global.isHilly = true;
                        global.height += dice.Next(10 - region.height / 20, 20 - region.height / 10);
                    }
                }
            }

        }

        /// <summary>
        /// Cleaning the world from many inner oceans
        /// </summary>
        public void CleanLandmass()
        {
            int rand;
            foreach (Region region in regionList)
            {
                rand = dice.Next(1, 9);
                if (region.height < 0 && region.IsLandlocked() && rand != 1)
                {
                    region.height = 0;
                    region.height = region.MeanHeight();
                    region.isSea = false;
                    region.isOcean = false;
                    region.isOceanTrench = false;
                    region.isContinent = true;
                }
            }
        }

        #endregion

        #region Temperature
        public void GenerateTemperature()
        {
            int rand;

            for (int i = 0; i < REGION; i++)
            {
                rand = dice.Next(0, 14);

                // temperature depends on Latitude
                regionList[i].temperature = 65 + rand - 140 * (Math.Abs(regionList[i].Y - (LATITUDE / 2))) / (LATITUDE / 2);

                // Sea based regions tends to be warmer
                if (regionList[i].isSea)
                {
                    rand = dice.Next(3, 10);
                    regionList[i].temperature += rand;
                }
                // Oceans tends toward 0
                if (regionList[i].isOcean)
                {
                    rand = dice.Next(4, 12);
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
                    rand = dice.Next(6, 16);
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
                    regionList[i].temperature -= regionList[i].height / 10;
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

            numberRiver = dice.Next(REGION / 27, REGION / 20);

            while (numberRiver != 0)
            {
                // We're looking for somewhat high altitude regions not located in the polar area and not coastal. Deserts areas are limited to moutains due to how dry they are. And no rivers on top of each other!
                do
                {
                    rand = dice.Next(0, REGION);
                    region = regionList[rand];
                } while (!(region.height > 15 && (region.isTemperate || (region.isDesert && region.isMountain) || region.isTropical) && region.IsLandlocked()) && region.rivers.Count == 0);

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
                // erosion will do its workdigging slightly a valley and increase the local humidity somewhat
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

            numberWind = dice.Next(REGION / 120, REGION / 100);


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
                rand = dice.Next(300, 600) + dice.Next(3, 30) * dice.Next(2, 20);
                PropagateWind(wind, rand);

                numberWind--;
            }
        }

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
                length -= RANDOMSTEP;
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
                                    length -= 7 * RANDOMSTEP;
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
                                    length -= 7 * RANDOMSTEP;
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
                                    length -= 7 * RANDOMSTEP;
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
                                    length -= 7 * RANDOMSTEP;
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

            result += "CONTINENT ID : " + regionList[IDregion].continentID.ToString() + Environment.NewLine;

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
                        colorRed = 130;
                        colorGreen = 140;
                        colorBlue = 80;
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

                    colorRed = (25 * regionList[mapMask[i, j]].continentID) % 255;
                    colorGreen = 50 * (regionList[mapMask[i, j]].continentID / 10) % 255;
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
