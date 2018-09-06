using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalysWorldGenerator
{
    class Geology : Geosphere
    {


        public Geology(string ore)
        {
            switch (ore)
            {
                case "Canyon":
                    Canyon();
                    break;
                case "Clay":
                    Clay();
                    break;
                case "Waterfall":
                    Waterfall();
                    break;
                case "UndergroundRiver":
                    UndergroundRiver();
                    break;
                case "Lake":
                    Lake();
                    break;
                case "Marsh":
                    Marsh();
                    break;
                case "MinorRiver":
                    MinorRiver();
                    break;
                case "Iron":
                    Iron();
                    break;
                case "Copper":
                    Copper();
                    break;
                case "Lead":
                    Lead();
                    break;
                case "Aluminum":
                    Aluminum();
                    break;
                case "Quicksilver":
                    Quicksilver();
                    break;
                case "Coal":
                    Coal();
                    break;
                case "Salt":
                    Salt();
                    break;
                case "Stone":
                    Stone();
                    break;
                case "Sand":
                    Sand();
                    break;
                case "Oil":
                    Oil();
                    break;
                case "RadioactiveOre":
                    RadioactiveOre();
                    break;
                case "OtherMetal":
                    OtherMetal();
                    break;
                case "MinorDeposit":
                    MinorDeposit();
                    break;
                case "Gold":
                    Gold();
                    break;
                case "Silver":
                    Silver();
                    break;
                case "Platinum":
                    Platinum();
                    break;
                case "SemiPrecious":
                    SemiPrecious();
                    break;
                case "Geods":
                    Geods();
                    break;
                case "Fossils":
                    Fossils();
                    break;
                case "OtherPrecious":
                    OtherPrecious();
                    break;
                case "ExplosiveVolcano":
                    ExplosiveVolcano();
                    break;
                case "EffusiveVolcano":
                    EffusiveVolcano();
                    break;
                case "Caverns":
                    Caverns();
                    break;
                case "Glacier":
                    Glacier();
                    break;
                case "Cliffs":
                    Cliffs();
                    break;
                case "RockFormation":
                    RockFormation();
                    break;
                case "RichSoils":
                    RichSoils();
                    break;
                case "ColoredSoils":
                    ColoredSoils();
                    break;
                case "StoneArch":
                    StoneArch();
                    break;
                case "Sinkhole":
                    Sinkhole();
                    break;
                case "Crater":
                    Crater();
                    break;
            }
        }


        public Geology (string ore, int amount)
        {
            switch (ore)
            {
                case "River":
                    River(amount);
                    break;
                case "Flowing salt":
                    FlowingSalt(amount);
                    break;
                case "Flowing metal":
                    FlowingMetal(amount);
                    break;
                case "Flowing rare metal":
                    FlowingRareMetal(amount);
                    break;
                case "Flowing precious metal":
                    FlowingPreciousMetal(amount);
                    break;
                case "Flowing radioactive ore":
                    FlowingRadioactiveOre(amount);
                    break;
                case "Flowing gems":
                    FlowingGems(amount);
                    break;
                case "Flowing reagent":
                    FlowingReagent(amount);
                    break;
                default:
                    break;
            }
        }

        #region Rivers
        public void River(int amount)
        {
            // Allows the exploitation of a river's freshwater
            type = "River";
            int i = 0;
            int rand;
            resources["freshWater"] = amount;
            for (i = 0; i < amount; i++)
            {
                rand = dice.Next(1, 5);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
        }

        public void FlowingSalt(int amount)
        {
            // Salt being carried by a river
            type = "Flowing salt";
            int i = 0;
            int rand;
            for (i = 0; i < amount; i++)
            {
                rand = dice.Next(1, 5);
                if (rand == 1)
                {
                    resources["salt"] += 1;
                }
            }
        }

        public void FlowingMetal(int amount)
        {
            // Metal being carried by a river
            type = "Flowing metal";
            int i = 0;
            int rand;
            for (i = 0; i < amount; i++)
            {
                rand = dice.Next(1, 5);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
        }

        public void FlowingRareMetal(int amount)
        {
            // Rare metal being carried by a river
            type = "Flowing rare metal";
            int i = 0;
            int rand;
            for (i = 0; i < amount; i++)
            {
                rand = dice.Next(1, 5);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
        }

        public void FlowingPreciousMetal(int amount)
        {
            // Precious metal being carried by a river
            type = "Flowing precious metal";
            int i = 0;
            int rand;
            for (i = 0; i < amount; i++)
            {
                rand = dice.Next(1, 5);
                if (rand == 1)
                {
                    resources["preciousMetal"] += 1;
                }
            }
        }

        public void FlowingRadioactiveOre(int amount)
        {
            // Radioactive ores being carried by a river
            type = "Flowing radioactive ore";
            int i = 0;
            int rand;
            for (i = 0; i < amount; i++)
            {
                rand = dice.Next(1, 5);
                if (rand == 1)
                {
                    resources["radioactiveOre"] += 1;
                }
            }
        }

        public void FlowingGems(int amount)
        {
            // Gems being carried by a river
            type = "Flowing gems";
            int i = 0;
            int rand;
            for (i = 0; i < amount; i++)
            {
                rand = dice.Next(1, 5);
                if (rand == 1)
                {
                    resources["gems"] += 1;
                }
            }
        }

        public void FlowingReagent(int amount)
        {
            // Reagent being carried by a river
            type = "Flowing reagent";
            int i = 0;
            int rand;
            for (i = 0; i < amount; i++)
            {
                rand = dice.Next(1, 5);
                if (rand == 1)
                {
                    resources["reagent"] += 1;
                }
            }
        }

        public void Canyon()
        {
            // Canyons offers an easy access to alluvial deposits and some good mining prospect. They also tend to have fossils exposed and usually are memorable structures
            type = "Canyon";
            int rand;
            int i;
            for (i = 0; i < 6; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["salt"] += 1;
                }
            }
            for (i = 0; i < 5; i++)
            {
                rand = dice.Next(1, 7);
                if (rand == 1)
                {
                    resources["gems"] += 1;
                }
            }
            for (i = 0; i < 4; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["appearance"] += 1;
                }
            }
            for (i = 0; i < 6; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
        }

        public void Clay()
        {
            // Clay deposits are only found in river crossed areas and offers a good source of deposit of various forms
            type = "Clay";
            int i = 0;
            int rand;
            for (i = 0; i < 12; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
            for (i = 0; i < 4; i++)
            {
                rand = dice.Next(1, 5);
                if (rand == 1)
                {
                    resources["fuel"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 5);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
        }

        public void Waterfall()
        {
            // Waterfalls happens along rivers exclusively. Those do not bring much in term of resources, but are major landmarks
            type = "Waterfall";
            int rand;
            int i;
            for (i = 0; i < 8; i++)
            {
                rand = dice.Next(1, 5);
                if (rand == 1)
                {
                    resources["appearance"] += 1;
                }
            }
        }

        public void UndergroundRiver()
        {
            // Underground rivers are a technical geology layer where troglodytes biomass can live exclusively, and brings nothing by themselves
            type = "Underground River";
        }

        public void Lake()
        {
            // Lakes are major freshwater source, of course, but also allows for aquatic creatures to thrive and creates a local subregion
            type = "Lake";
            int i = 0;
            int rand;
            for (i = 0; i < 12; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["freshWater"] += 1;
                }
            }
        }

        public void Marsh()
        {
            // Marshes (which also covers mangroves) are very humid areas with non drinkable water but a buch of other resources
            type = "Marsh";
            int i = 0;
            int rand;
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["freshWater"] += 1;
                }
            }
            for (i = 0; i < 4; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["fuel"] += 1;
                }
            }
            for (i = 0; i < 6; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
        }

        public void MinorRiver()
        {
            // Smaller rivers can also bring their own resources
            type = "MinorRiver";
            int i = 0;
            int rand;
            for (i = 0; i < 8; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["freshWater"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
        }

        #endregion

        #region ores
        public void Iron()
        {
            // The most common metal in use in many eras
            type = "Iron";
            int i = 0;
            int rand;
            for (i = 0; i < 15; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
        }

        public void Copper()
        {
            // Copper is another common metal that is softer, but useful
            type = "Copper";
            int i = 0;
            int rand;
            for (i = 0; i < 15; i++)
            {
                rand = dice.Next(1, 6);
                if (rand == 1 || rand == 2)
                {
                    resources["metal"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
        }

        public void Lead()
        {
            // Lead is an useful metal for many different purpose 
            type = "Lead";
            int i = 0;
            int rand;
            for (i = 0; i < 10; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
        }

        public void Aluminum()
        {
            // Lighter than iron but harder to harvest, aluminum is found in clay filled areas
            type = "Aluminum";
            int i = 0;
            int rand;
            for (i = 0; i < 10; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 5; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
        }

        public void Quicksilver()
        {
            // A classic reageant that is as toxic as it is useful
            type = "Quicksilver";
            int i = 0;
            int rand;
            for (i = 0; i < 12; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["reagent"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
        }

        public void Coal()
        {
            // The common natural fuel found under the surface
            type = "Coal";
            int i = 0;
            int rand;
            for (i = 0; i < 12; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["fuel"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
            for (i = 0; i < 4; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
        }

        public void Salt()
        {
            // Salt is used for conservation across the ages and is commonly found under the surface
            type = "Salt mine";
            int i = 0;
            int rand;
            for (i = 0; i < 16; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["salt"] += 1;
                }
            }
            for (i = 0; i < 4; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
        }

        public void Stone()
        {
            // Construction stone are a common sight across the world
            type = "Quarry stone";
            int i = 0;
            int rand;
            for (i = 0; i < 12; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
        }

        public void Sand()
        {
            // Glass and concrete are very demanding in sand
            type = "Sand bank";
            int i = 0;
            int rand;
            for (i = 0; i < 12; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
        }

        public void Oil()
        {
            // Oil is difficult to reach, but have a high potential
            type = "Oil spill";
            int i = 0;
            int rand;
            for (i = 0; i < 15; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["fuel"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
        }

        public void RadioactiveOre()
        {
            // Uranium, plutonium... the radioactive metals have many properties sought for by humanity
            type = "Radioactive ore";
            int i = 0;
            int rand;
            for (i = 0; i < 15; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["radioactiveOre"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
        }

        public void OtherMetal()
        {
            // There are a variety of other deposits around the world
            type = "Varied metals";
            int i = 0;
            int rand;
            for (i = 0; i < 8; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
            for (i = 0; i < 10; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
        }

        public void MinorDeposit()
        {
            // Many smaller deposits are found all around the world
            type = "Minor deposit";
            int i = 0;
            int rand;
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 4);
                if (rand == 1)
                {
                    resources["salt"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 5);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 7);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 11);
                if (rand == 1)
                {
                    resources["radioactiveOre"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 4);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 4);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 6);
                if (rand == 1)
                {
                    resources["gems"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 5);
                if (rand == 1)
                {
                    resources["reagent"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 4);
                if (rand == 1)
                {
                    resources["fuel"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["appearance"] += 1;
                }
            }
        }
        #endregion

        #region preciousOres
        public void Gold()
        {
            // The most common metal in use in many eras
            type = "Gold";
            int i = 0;
            int rand;
            for (i = 0; i < 15; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["preciousMetal"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
        }

        public void Silver()
        {
            // A precious and valuable metal
            type = "Silver";
            int i = 0;
            int rand;
            for (i = 0; i < 11; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["preciousMetal"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
        }

        public void Platinum()
        {
            // Rare and extremely valuable
            type = "Platinum";
            int i = 0;
            int rand;
            for (i = 0; i < 18; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["preciousMetal"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
        }

        public void SemiPrecious()
        {
            // From Quartz to Beryl, all are still valuable
            type = "Semi precious stones";
            int i = 0;
            int rand;
            for (i = 0; i < 10; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["gems"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
        }

        public void Precious()
        {
            // A precious and valuable stone
            type = "Precious stones";
            int i = 0;
            int rand;
            for (i = 0; i < 15; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["gems"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
        }

        public void Geods()
        {
            // Giant crystalline structure
            type = "Geods";
            int i = 0;
            int rand;
            for (i = 0; i < 12; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["gems"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["preciousMetal"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
            for (i = 0; i < 5; i++)
            {
                rand = dice.Next(1, 5);
                if (rand == 1)
                {
                    resources["appearance"] += 1;
                }
            }
        }

        public void Fossils()
        {
            // remains from a time before time
            type = "Fossils";
            int i = 0;
            int rand;
            for (i = 0; i < 8; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["gems"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
            for (i = 0; i < 5; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 4);
                if (rand == 1)
                {
                    resources["appearance"] += 1;
                }
            }
        }

        public void OtherPrecious()
        {
            // A variety of smaller veins
            type = "Other precious minerals";
            int i = 0;
            int rand;
            for (i = 0; i < 6; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["gems"] += 1;
                }
            }
            for (i = 0; i < 6; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["preciousMetal"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
            for (i = 0; i < 5; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
        }
        #endregion

        #region NaturalStructures
        public void ExplosiveVolcano()
        {
            // Mighty mountains that brings as much as they destroy
            type = "Explosive Volcano";
            int i = 0;
            int rand;
            for (i = 0; i < 7; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["reagent"] += 1;
                }
            }
            for (i = 0; i < 5; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
            for (i = 0; i < 4; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["gems"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 10; i++)
            {
                rand = dice.Next(1, 5);
                if (rand == 1)
                {
                    resources["appearance"] += 1;
                }
            }
        }

        public void EffusiveVolcano()
        {
            // Less dangerous than its explosive counterpart, but still provide resources in variety
            type = "Effusive Volcano";
            int i = 0;
            int rand;
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["reagent"] += 1;
                }
            }
            for (i = 0; i < 6; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
            for (i = 0; i < 5; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["gems"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 8; i++)
            {
                rand = dice.Next(1, 5);
                if (rand == 1)
                {
                    resources["appearance"] += 1;
                }
            }
        }

        public void Caverns()
        {
            // Caves network are a practical way to move around and hosts troglodyte creatures, on top of having a lot of varied deposits being easier to reach
            type = "Caverns";
            int i = 0;
            int rand;
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["reagent"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["gems"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["preciousMetal"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["salt"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 4);
                if (rand == 1)
                {
                    resources["appearance"] += 1;
                }
            }
        }

        public void Glacier()
        {
            // Glaciers are merely ice filling a valley
            type = "Glacier";
            int i = 0;
            int rand;
            for (i = 0; i < 7; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["freshWater"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
            for (i = 0; i < 5; i++)
            {
                rand = dice.Next(1, 4);
                if (rand == 1)
                {
                    resources["appearance"] += 1;
                }
            }
        }

        public void Cliffs()
        {
            // Cliffs are obstacle that provides a good vision of the geological story of a region
            type = "Cliffs";
            int i = 0;
            int rand;
            for (i = 0; i < 10; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 4);
                if (rand == 1)
                {
                    resources["appearance"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["gems"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["preciousMetal"] += 1;
                }
            }
        }

        public void RockFormation()
        {
            // There are a variety of oddly shaped formation all around the world, those are the most obvious
            type = "Rock formation";
            int i = 0;
            int rand;
            for (i = 0; i < 5; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
            for (i = 0; i < 6; i++)
            {
                rand = dice.Next(1, 4);
                if (rand == 1)
                {
                    resources["appearance"] += 1;
                }
            }
            for (i = 0; i < 3; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["gems"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
            for (i = 0; i < 1; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["preciousMetal"] += 1;
                }
            }
        }

        public void RichSoils()
        {
            // Plants and animals can find the perfect conditions to thrive in this area
            type = "Rich soils";
            int i = 0;
            int rand;
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["grain"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["fruit"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["meat"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["protein"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["freshWater"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["wood"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["fabrics"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["leather"] += 1;
                }
            }
        }

        public void ColoredSoils()
        {
            // Some dyes are found in the very dirt below one's feet
            type = "Colored soils";
            int i = 0;
            int rand;
            for (i = 0; i < 9; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["dyes"] += 1;
                }
            }
            for (i = 0; i < 4; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
            for (i = 0; i < 5; i++)
            {
                rand = dice.Next(1, 5);
                if (rand == 1)
                {
                    resources["appearance"] += 1;
                }
            }
        }

        public void StoneArch()
        {
            // A nice looking hole in a rock
            type = "Stone arch";
            int i = 0;
            int rand;
            for (i = 0; i < 6; i++)
            {
                rand = dice.Next(1, 4);
                if (rand == 1)
                {
                    resources["appearance"] += 1;
                }
            }
        }

        public void Sinkhole()
        {
            // A less nice looking hole in the ground
            type = "Sink hole";
            int i = 0;
            int rand;
            for (i = 0; i < 5; i++)
            {
                rand = dice.Next(1, 4);
                if (rand == 1)
                {
                    resources["appearance"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["freshWater"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
        }

        public void Crater()
        {
            // Not necessarily dinosaur ending
            type = "Crater";
            int i = 0;
            int rand;
            for (i = 0; i < 6; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["metal"] += 1;
                }
            }
            for (i = 0; i < 2; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["stone"] += 1;
                }
            }
            for (i = 0; i < 4; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["deposit"] += 1;
                }
            }
            for (i = 0; i < 4; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["rareMetal"] += 1;
                }
            }
            for (i = 0; i < 4; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["preciousMetal"] += 1;
                }
            }
            for (i = 0; i < 4; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["radioactiveOre"] += 1;
                }
            }
            for (i = 0; i < 5; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["gems"] += 1;
                }
            }
            for (i = 0; i < 4; i++)
            {
                rand = dice.Next(1, 3);
                if (rand == 1)
                {
                    resources["reagent"] += 1;
                }
            }
        }
        #endregion


    }
}
