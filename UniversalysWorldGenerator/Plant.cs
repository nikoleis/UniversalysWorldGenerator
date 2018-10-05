using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalysWorldGenerator
{
    class Plant : Biosphere
    {
        private static int Count = 0;
        public int size; //
        public int favoredTemp, favoredHeight, favoredHumid;
        public List<Trait> traitsList;
        public string description = "";

        public Plant()
        {
            Count++;
        }

        /* Plant traits */


    }
}
