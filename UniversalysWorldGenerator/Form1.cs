using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.IO;
namespace UniversalysWorldGenerator
{
    public partial class Form1 : Form
    {

        
        WorldMap map = new WorldMap();

        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new string[] { "Height", "Temperature", "Humidity", "Climate" , "Mountain", "Continent", "River" });

        }

        private void TestButton_Click(object sender, EventArgs e)
        {

            InformationDisplay.Text = "";


            map.GenerateRegions();
            map.GenerateLandmass();
            map.CleanLandmass();
            map.GenerateTemperature();
            map.GenerateHumidity();
            map.PlaceRivers();
            map.GenerateDeposit();
            map.FlowingRiver();
            map.UpdateGeology();


            map.DrawRegionMap();
            map.DrawHeightMap();
            map.DrawClimateMap();
            map.DrawTemperatureMap();
            map.DrawHumidityMap();
            map.DrawRiverMap();

            map.DrawMountainMap();
            map.DrawContinentMap();

            MessageBox.Show("Done !");
            // Pour ne plus avoir besoin de relancer le générateur a chaque fois
            // To not relaunch the generator every time we need a new map 
            Image img;

            using (var bmpTemp = new Bitmap(Program.filePath + "mapHeight.png"))
            {
                img = new Bitmap(bmpTemp);
                pictureBox1.Image = img;
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            var relativePoint = this.PointToClient(Cursor.Position);
            InformationDisplay.Text += "{X = " + relativePoint.X + " : Y = " + relativePoint.Y + "}" + Environment.NewLine;
            InformationDisplay.Text += map.RegionInfo(relativePoint.X, relativePoint.Y);
            InformationDisplay.Text += Environment.NewLine;
            InformationDisplay.SelectionStart = InformationDisplay.Text.Length;
            InformationDisplay.ScrollToCaret();

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            InformationDisplay.Text = "";
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            Image img;
            if (comboBox1.SelectedIndex == 0)
                using (var bmpTemp = new Bitmap(Program.filePath + "mapHeight.png"))
                {
                    img = new Bitmap(bmpTemp);
                    pictureBox1.Image = img;
                }
            if (comboBox1.SelectedIndex == 1)
                using (var bmpTemp = new Bitmap(Program.filePath + "mapTemperature.png"))
                {
                    img = new Bitmap(bmpTemp);
                    pictureBox1.Image = img;
                }
            if (comboBox1.SelectedIndex == 2)
                using (var bmpTemp = new Bitmap(Program.filePath + "mapHumidity.png"))
                {
                    img = new Bitmap(bmpTemp);
                    pictureBox1.Image = img;
                }
            if (comboBox1.SelectedIndex == 3)
                using (var bmpTemp = new Bitmap(Program.filePath + "mapClimate.png"))
                {
                    img = new Bitmap(bmpTemp);
                    pictureBox1.Image = img;
                }
            if (comboBox1.SelectedIndex == 4)
                using (var bmpTemp = new Bitmap(Program.filePath + "mapMountain.png"))
                {
                    img = new Bitmap(bmpTemp);
                    pictureBox1.Image = img;
                }
            if (comboBox1.SelectedIndex == 5)
                using (var bmpTemp = new Bitmap(Program.filePath + "mapContinent.png"))
                {
                    img = new Bitmap(bmpTemp);
                    pictureBox1.Image = img;
                }
            if (comboBox1.SelectedIndex == 6)
                using (var bmpTemp = new Bitmap(Program.filePath + "mapRiver.png"))
                {
                    img = new Bitmap(bmpTemp);
                    pictureBox1.Image = img;
                }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
