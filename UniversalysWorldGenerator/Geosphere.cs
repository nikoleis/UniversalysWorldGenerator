using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalysWorldGenerator
{
    class Geosphere
    {
        public string type = "";
        public Dictionary<string, int> resources = new Dictionary<string, int>();
        protected static Random dice = new Random();

        public Geosphere()
        {
            resources.Add("grain", 0);
            resources.Add("fruit", 0);
            resources.Add("meat", 0);
            resources.Add("protein", 0);
            resources.Add("sugar", 0);
            resources.Add("spice", 0);
            resources.Add("salt", 0);
            resources.Add("alcohol", 0);
            resources.Add("medical", 0);
            resources.Add("freshWater", 0);
            resources.Add("wood", 0);
            resources.Add("stone", 0);
            resources.Add("deposit", 0);
            resources.Add("metal", 0);
            resources.Add("rareMetal", 0);
            resources.Add("preciousMetal", 0);
            resources.Add("radioactiveOre", 0);
            resources.Add("gems", 0);
            resources.Add("reagent", 0);
            resources.Add("fabrics", 0);
            resources.Add("dyes", 0);
            resources.Add("leather", 0);
            resources.Add("fuel", 0);
            resources.Add("appearance", 0);
        }

        //public int mana;
        //public int magicMetal;
        //public int monster;
    }
}
