using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VISSIM_MOVES_CONV {
    public partial class MainWindow : Form {
        public MainWindow() {
            InitializeComponent();
        }

        private void ButtonClose_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void ButtonLoadVissim_Click(object sender, EventArgs e) {
            DialogResult result = this.FileDialogOpen.ShowDialog();
            if (result == DialogResult.OK) {
                string file = this.FileDialogOpen.FileName;
                try {
                    this.PathBoxLoad.Text = file;
                }
                catch {
                }
            }
        }

        private void ButtonSaveOutput_Click(object sender, EventArgs e) {
            DialogResult result = this.FileDialogSave.ShowDialog();
            if (result == DialogResult.OK) {
                string path = this.FileDialogSave.SelectedPath;
                try {
                    this.PathBoxSave.Text = path;
                }
                catch {
                }
            }
        }

        private void ButtonRun_Click(object sender, EventArgs e) {
            string cluster_operation_mode = "NONE";
            string empty_resolution_mode = "NONE";
            if (this.radio_sub_od.Checked == true) cluster_operation_mode = "OD";
            else if (this.radio_sub_link.Checked == true) cluster_operation_mode = "LN";

            if (this.radio_empty_w.Checked == true) empty_resolution_mode = "RW";
            else if (this.radio_empty_err.Checked == true) empty_resolution_mode = "ER";

            if (this.CBClusterEnable.Checked == false) cluster_operation_mode = "NO"; //overwrite the cluster mode if disabled.

            ProgramDelegator ProgramDelegate = new ProgramDelegator(
                this.PathBoxLoad.Text, 
                this.PathBoxSave.Text, 
                Convert.ToInt32(this.PathBoxOffset.Text),
                this.PathBoxMovesZone.Text,
                (this.PathBoxMovesZone.Text + "0"),
                this.PathBoxRoadType.Text,
                this.TextBoxName.Text,
                Convert.ToInt32(this.PathBoxKMeans.Text),
                cluster_operation_mode,
                empty_resolution_mode);
            ProgramDelegate.Run(this.ConsoleBox);
        }

        private void ConsoleBox_TextChanged(object sender, EventArgs e) {
            ConsoleBox.SelectionStart = ConsoleBox.TextLength;
            ConsoleBox.ScrollToCaret();
        }

        private void MainRun_DoWork(object sender, DoWorkEventArgs e) {

        }

        private void ClusterEnableCB_CheckedChanged(object sender, EventArgs e)
        {
            if (this.GBCluster.Enabled) this.GBCluster.Enabled = false;
            else this.GBCluster.Enabled = true;
        }
    }
}
