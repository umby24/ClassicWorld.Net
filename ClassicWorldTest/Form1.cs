using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using ClassicWorld_NET;

namespace ClassicWorldTest {
    public partial class Form1 : Form {
        bool Loaded = false;
        ClassicWorld Map;

        public Form1() {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e) {
            var Result = openMap.ShowDialog();

            if (Result == DialogResult.OK) {
                Stopwatch Timer = new Stopwatch();
                Timer.Start();

                Map = new ClassicWorld(openMap.FileName);
                Map.Load();

                Timer.Stop();
                lblTime.Text = "Time: " + Timer.Elapsed.TotalSeconds.ToString();
                Loaded = true;
            }
        }

        private void btnSave_Click(object sender, EventArgs e) {
            var Result = saveMap.ShowDialog();

            if (Result == DialogResult.OK) { 
                if (Loaded) {
                    Stopwatch Timer = new Stopwatch();
                    Timer.Start();

                    Map.Save(saveMap.FileName);

                    Timer.Stop();

                    lblTime.Text = "Time: " + Timer.Elapsed.TotalSeconds.ToString();
                } else {
                    // -- Create a map and save it.
                    Stopwatch Timer = new Stopwatch();
                    Timer.Start();

                    Map = new ClassicWorld(128, 128, 128);
                    Map.MapName = "New Map!";
                    Map.Save(saveMap.FileName);

                    Timer.Stop();

                    lblTime.Text = "Time: " + Timer.Elapsed.TotalSeconds.ToString();
                }
            }
        }
    }
}
