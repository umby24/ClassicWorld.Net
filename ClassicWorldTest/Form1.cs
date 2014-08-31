using System;
using System.Windows.Forms;
using System.Diagnostics;
using ClassicWorld.NET;

namespace ClassicWorldTest {
    public partial class Form1 : Form {
        bool _loaded;
        private Classicworld _map;

        public Form1() {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e) {
            var result = openMap.ShowDialog();

            if (result == DialogResult.OK) {
                var timer = new Stopwatch();
                timer.Start();

                _map = new Classicworld(openMap.FileName);
                _map.Load();

                timer.Stop();
                lblTime.Text = "Time: " + timer.Elapsed.TotalSeconds;
                _loaded = true;
            }
        }

        private void btnSave_Click(object sender, EventArgs e) {
            var result = saveMap.ShowDialog();

            if (result != DialogResult.OK) 
                return;

            if (_loaded) {
                var timer = new Stopwatch();
                timer.Start();

                _map.Save(saveMap.FileName);

                timer.Stop();

                lblTime.Text = "Time: " + timer.Elapsed.TotalSeconds;
            } else {
                // -- Create a map and save it.
                var timer = new Stopwatch();
                timer.Start();

                _map = new Classicworld(128, 128, 128) {MapName = "New Map!"};
                _map.Save(saveMap.FileName);

                timer.Stop();

                lblTime.Text = "Time: " + timer.Elapsed.TotalSeconds;
            }
        }
    }
}
