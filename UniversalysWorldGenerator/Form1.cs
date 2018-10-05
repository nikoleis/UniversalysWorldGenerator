using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace UniversalysWorldGenerator
{
    public partial class Form1 : Form
    {

        static Image img;
        WorldMap map = new WorldMap();
        bool mapLoaded = false;
        int generationStep = 0;

        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new string[] { "Height", "Temperature", "Humidity", "Climate", "Mountain", "Landmass", "Continent", "Rivers", "Winds", "Water currents" });

            if (!Directory.Exists(Program.filePath))
            {
                Directory.CreateDirectory(Program.filePath);
            }

        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            mapLoaded = true;
            InformationDisplay.Text = "";

            map.GenerateRegions();
            map.GenerateLandmass();
            map.CleanLandmass();
            map.AssignContinent();
            map.GenerateTemperature();
            map.GenerateHumidity();
            map.PlaceWinds();
            map.PlaceWaterCurrents();
            map.ApplyMapChange();
            // We apply winds and water current BEFORE generating rivers for the incoming erosion can alter potential river starting points 
            map.PlaceRivers();
            map.ApplyMapChange();
            map.GenerateDeposit();
            map.FlowingRiver();
            map.UpdateGeology();
            

            map.DrawRegionMap();
            map.DrawHeightMap();
            map.DrawClimateMap();
            map.DrawTemperatureMap();
            map.DrawHumidityMap();
            map.DrawRiverMap();
            map.DrawWindMap();
            map.DrawWaterCurrentMap();

            map.DrawMountainMap();
            map.DrawLandmassMap();
            map.DrawContinentMap();

            InformationDisplay.Text += "Done !";

            using (var bmpTemp = new Bitmap(Program.filePath + "mapHeight.png"))
            {
                img = new Bitmap(bmpTemp);
                pictureBox1.Image = img;
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (mapLoaded)
            {
                var relativePoint = PointToClient(Cursor.Position);
                InformationDisplay.Text += "{X = " + relativePoint.X + " : Y = " + relativePoint.Y + "}" + Environment.NewLine;
                InformationDisplay.Text += map.RegionInfo(relativePoint.X, relativePoint.Y);
                InformationDisplay.Text += Environment.NewLine;
                InformationDisplay.SelectionStart = InformationDisplay.Text.Length;
                InformationDisplay.ScrollToCaret();

            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            InformationDisplay.Text = "";
        }

        //public static void LoadMap()
        //{

        //    var img = Image.FromFile(Program.filePath + "map.png");
        //    pictureBox1.Image = img;


        //}

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
                using (var bmpTemp = new Bitmap(Program.filePath + "mapLandmass.png"))
                {
                    img = new Bitmap(bmpTemp);
                    pictureBox1.Image = img;
                }
            if (comboBox1.SelectedIndex == 6)
                using (var bmpTemp = new Bitmap(Program.filePath + "mapContinent.png"))
                {
                    img = new Bitmap(bmpTemp);
                    pictureBox1.Image = img;
                }
            if (comboBox1.SelectedIndex == 7)
                using (var bmpTemp = new Bitmap(Program.filePath + "mapRiver.png"))
                {
                    img = new Bitmap(bmpTemp);
                    pictureBox1.Image = img;
                }
            if (comboBox1.SelectedIndex == 8)
                using (var bmpTemp = new Bitmap(Program.filePath + "mapWind.png"))
                {
                    img = new Bitmap(bmpTemp);
                    pictureBox1.Image = img;
                }
            if (comboBox1.SelectedIndex == 9)
                using (var bmpTemp = new Bitmap(Program.filePath + "mapWaterCurrent.png"))
                {
                    img = new Bitmap(bmpTemp);
                    pictureBox1.Image = img;
                }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            mapLoaded = true;
            InformationDisplay.Text = "";

            switch (generationStep)
            {
                case 0:
                    map.GenerateRegions();
                    map.DrawRegionMap();
                    generationStep++;
                    btnStep.Text = "Create landmass";
                    break;
                case 1:
                    map.GenerateLandmass();
                    map.CleanLandmass();
                    map.DrawHeightMap();
                    map.DrawContinentMap();
                    map.DrawMountainMap();
                    generationStep++;
                    btnStep.Text = "Set temperature";
                    break;
                case 2:
                    map.GenerateTemperature();
                    map.DrawTemperatureMap();
                    generationStep++;
                    btnStep.Text = "Set humidity";
                    break;
                case 3:
                    map.GenerateHumidity();
                    map.DrawHumidityMap();
                    generationStep++;
                    btnStep.Text = "Create flows";
                    break;
                case 4:
                    map.PlaceRivers();
                    map.PlaceWinds();
                    map.DrawHeightMap();
                    map.DrawTemperatureMap();
                    map.DrawClimateMap();
                    map.DrawHumidityMap();
                    generationStep++;
                    btnStep.Text = "Set geology";
                    break;
                case 5:
                    map.GenerateDeposit();
                    map.FlowingRiver();
                    map.UpdateGeology();
                    map.DrawRiverMap();
                    map.DrawWindMap();
                    map.DrawWaterCurrentMap();
                    generationStep = 100;
                    btnStep.Text = "Settle plants";
                    break;
                case 100:
                    MessageBox.Show("Not yet implemented");
                    break;
                default:
                    break;

            }
        }
    }
}
