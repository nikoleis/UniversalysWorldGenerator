using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalysWorldGenerator
{
    /*
     * Le courant est la classe mère des courants {RIVIERES}{MARINS}{VENTS}
     * RIVIERE : 
     * la classe rivière doit avoir : [une ressource d'eau potable] / [une ressource de pierre précieuse(precious)] / 
     * [ID de la région dans la quelle est commence ainsi que toutes les régions ou elle passe]
     * 
     * La classe rivière doit modifier : [le dictionnaire de la région] / [le taux de ressource d'eau potable par région]  / [le taux de ressource de pierre précieuse(precious)] /
     * 
     */
    class Stream : Geosphere
    {
        protected List<int> stream = new List<int>();


        public Stream(int id)
        {
            stream.Add(id);
        }
    }
}

