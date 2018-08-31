using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniversalysWorldGenerator
{
    static class Program
    {

        public static string filePath = @"C:\Users\34011-10-02\Documents\UniversalysWorldGenerator\UniversalysWorldGenerator\map\";
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());



        }
    }
}
