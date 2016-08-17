namespace VISSIM_MOVES_CONV {
    partial class MainWindow {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.ButtonClose = new System.Windows.Forms.Button();
            this.ButtonRun = new System.Windows.Forms.Button();
            this.FileDialogOpen = new System.Windows.Forms.OpenFileDialog();
            this.ButtonLoadVissim = new System.Windows.Forms.Button();
            this.PathBoxLoad = new System.Windows.Forms.TextBox();
            this.ButtonSaveOutput = new System.Windows.Forms.Button();
            this.PathBoxSave = new System.Windows.Forms.TextBox();
            this.FileDialogSave = new System.Windows.Forms.FolderBrowserDialog();
            this.ConsoleBox = new System.Windows.Forms.RichTextBox();
            this.PathBoxOffset = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.PathBoxMovesZone = new System.Windows.Forms.TextBox();
            this.PathBoxRoadType = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.PathBoxKMeans = new System.Windows.Forms.TextBox();
            this.TextBoxName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radio_sub_no = new System.Windows.Forms.RadioButton();
            this.radio_sub_link = new System.Windows.Forms.RadioButton();
            this.radio_sub_od = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radio_empty_err = new System.Windows.Forms.RadioButton();
            this.radio_empty_w = new System.Windows.Forms.RadioButton();
            this.CBClusterEnable = new System.Windows.Forms.CheckBox();
            this.GBCluster = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.GBCluster.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonClose
            // 
            this.ButtonClose.Location = new System.Drawing.Point(502, 571);
            this.ButtonClose.Name = "ButtonClose";
            this.ButtonClose.Size = new System.Drawing.Size(75, 23);
            this.ButtonClose.TabIndex = 0;
            this.ButtonClose.Text = "Close";
            this.ButtonClose.UseVisualStyleBackColor = true;
            this.ButtonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // ButtonRun
            // 
            this.ButtonRun.Location = new System.Drawing.Point(421, 571);
            this.ButtonRun.Name = "ButtonRun";
            this.ButtonRun.Size = new System.Drawing.Size(75, 23);
            this.ButtonRun.TabIndex = 1;
            this.ButtonRun.Text = "Run";
            this.ButtonRun.UseVisualStyleBackColor = true;
            this.ButtonRun.Click += new System.EventHandler(this.ButtonRun_Click);
            // 
            // FileDialogOpen
            // 
            this.FileDialogOpen.Filter = "VISSIM Trajectories|*.fzp;*.txt|All Files|*.*";
            this.FileDialogOpen.Title = "Load a Vissim Vehicle Record";
            // 
            // ButtonLoadVissim
            // 
            this.ButtonLoadVissim.Location = new System.Drawing.Point(12, 12);
            this.ButtonLoadVissim.Name = "ButtonLoadVissim";
            this.ButtonLoadVissim.Size = new System.Drawing.Size(100, 23);
            this.ButtonLoadVissim.TabIndex = 2;
            this.ButtonLoadVissim.Text = "Vissim Trajectory";
            this.ButtonLoadVissim.UseVisualStyleBackColor = true;
            this.ButtonLoadVissim.Click += new System.EventHandler(this.ButtonLoadVissim_Click);
            // 
            // PathBoxLoad
            // 
            this.PathBoxLoad.Location = new System.Drawing.Point(118, 14);
            this.PathBoxLoad.Name = "PathBoxLoad";
            this.PathBoxLoad.Size = new System.Drawing.Size(459, 20);
            this.PathBoxLoad.TabIndex = 3;
            // 
            // ButtonSaveOutput
            // 
            this.ButtonSaveOutput.Location = new System.Drawing.Point(12, 41);
            this.ButtonSaveOutput.Name = "ButtonSaveOutput";
            this.ButtonSaveOutput.Size = new System.Drawing.Size(100, 23);
            this.ButtonSaveOutput.TabIndex = 4;
            this.ButtonSaveOutput.Text = "Output Folder";
            this.ButtonSaveOutput.UseVisualStyleBackColor = true;
            this.ButtonSaveOutput.Click += new System.EventHandler(this.ButtonSaveOutput_Click);
            // 
            // PathBoxSave
            // 
            this.PathBoxSave.Location = new System.Drawing.Point(118, 43);
            this.PathBoxSave.Name = "PathBoxSave";
            this.PathBoxSave.Size = new System.Drawing.Size(459, 20);
            this.PathBoxSave.TabIndex = 5;
            // 
            // FileDialogSave
            // 
            this.FileDialogSave.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // ConsoleBox
            // 
            this.ConsoleBox.Location = new System.Drawing.Point(12, 358);
            this.ConsoleBox.Name = "ConsoleBox";
            this.ConsoleBox.Size = new System.Drawing.Size(565, 207);
            this.ConsoleBox.TabIndex = 6;
            this.ConsoleBox.Text = resources.GetString("ConsoleBox.Text");
            this.ConsoleBox.TextChanged += new System.EventHandler(this.ConsoleBox_TextChanged);
            // 
            // PathBoxOffset
            // 
            this.PathBoxOffset.Location = new System.Drawing.Point(12, 101);
            this.PathBoxOffset.Name = "PathBoxOffset";
            this.PathBoxOffset.Size = new System.Drawing.Size(97, 20);
            this.PathBoxOffset.TabIndex = 7;
            this.PathBoxOffset.Text = "200";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(115, 104);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(348, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Time Offset (VISSIM time stamp in seconds of the first trajectory element)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(115, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(384, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "MOVES CountyID (Refer to MOVES Manual for a list. Default = Erie County, NY)";
            // 
            // PathBoxMovesZone
            // 
            this.PathBoxMovesZone.Location = new System.Drawing.Point(12, 127);
            this.PathBoxMovesZone.Name = "PathBoxMovesZone";
            this.PathBoxMovesZone.Size = new System.Drawing.Size(97, 20);
            this.PathBoxMovesZone.TabIndex = 10;
            this.PathBoxMovesZone.Text = "36029";
            // 
            // PathBoxRoadType
            // 
            this.PathBoxRoadType.Location = new System.Drawing.Point(12, 153);
            this.PathBoxRoadType.Name = "PathBoxRoadType";
            this.PathBoxRoadType.Size = new System.Drawing.Size(97, 20);
            this.PathBoxRoadType.TabIndex = 11;
            this.PathBoxRoadType.Text = "5";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(115, 156);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(447, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Road Type. This is not currently analysed from VISSIM. Default is Urban Unrestric" +
    "ted Access.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(109, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(379, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Total k-means clusters to generate (over all OD pairs, or maximum for each link)";
            // 
            // PathBoxKMeans
            // 
            this.PathBoxKMeans.Location = new System.Drawing.Point(6, 19);
            this.PathBoxKMeans.Name = "PathBoxKMeans";
            this.PathBoxKMeans.Size = new System.Drawing.Size(97, 20);
            this.PathBoxKMeans.TabIndex = 13;
            this.PathBoxKMeans.Text = "10";
            // 
            // TextBoxName
            // 
            this.TextBoxName.Location = new System.Drawing.Point(118, 71);
            this.TextBoxName.Name = "TextBoxName";
            this.TextBoxName.Size = new System.Drawing.Size(459, 20);
            this.TextBoxName.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(41, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Project Name";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radio_sub_no);
            this.groupBox1.Controls.Add(this.radio_sub_link);
            this.groupBox1.Controls.Add(this.radio_sub_od);
            this.groupBox1.Location = new System.Drawing.Point(6, 45);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(547, 45);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Subdivide Cluster Count by...";
            // 
            // radio_sub_no
            // 
            this.radio_sub_no.AutoSize = true;
            this.radio_sub_no.Enabled = false;
            this.radio_sub_no.Location = new System.Drawing.Point(142, 19);
            this.radio_sub_no.Name = "radio_sub_no";
            this.radio_sub_no.Size = new System.Drawing.Size(96, 17);
            this.radio_sub_no.TabIndex = 21;
            this.radio_sub_no.Text = "No Subdivision";
            this.radio_sub_no.UseVisualStyleBackColor = true;
            // 
            // radio_sub_link
            // 
            this.radio_sub_link.AutoSize = true;
            this.radio_sub_link.Checked = true;
            this.radio_sub_link.Location = new System.Drawing.Point(86, 19);
            this.radio_sub_link.Name = "radio_sub_link";
            this.radio_sub_link.Size = new System.Drawing.Size(50, 17);
            this.radio_sub_link.TabIndex = 20;
            this.radio_sub_link.TabStop = true;
            this.radio_sub_link.Text = "Links";
            this.radio_sub_link.UseVisualStyleBackColor = true;
            // 
            // radio_sub_od
            // 
            this.radio_sub_od.AutoSize = true;
            this.radio_sub_od.Location = new System.Drawing.Point(6, 19);
            this.radio_sub_od.Name = "radio_sub_od";
            this.radio_sub_od.Size = new System.Drawing.Size(74, 17);
            this.radio_sub_od.TabIndex = 19;
            this.radio_sub_od.Text = "O-D Paths";
            this.radio_sub_od.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radio_empty_err);
            this.groupBox2.Controls.Add(this.radio_empty_w);
            this.groupBox2.Location = new System.Drawing.Point(6, 96);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(547, 45);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Final Empty Cluster Resolution";
            // 
            // radio_empty_err
            // 
            this.radio_empty_err.AutoSize = true;
            this.radio_empty_err.Location = new System.Drawing.Point(142, 19);
            this.radio_empty_err.Name = "radio_empty_err";
            this.radio_empty_err.Size = new System.Drawing.Size(80, 17);
            this.radio_empty_err.TabIndex = 20;
            this.radio_empty_err.Text = "Throw Error";
            this.radio_empty_err.UseVisualStyleBackColor = true;
            // 
            // radio_empty_w
            // 
            this.radio_empty_w.AutoSize = true;
            this.radio_empty_w.Checked = true;
            this.radio_empty_w.Location = new System.Drawing.Point(6, 19);
            this.radio_empty_w.Name = "radio_empty_w";
            this.radio_empty_w.Size = new System.Drawing.Size(115, 17);
            this.radio_empty_w.TabIndex = 19;
            this.radio_empty_w.TabStop = true;
            this.radio_empty_w.Text = "Remove and Warn";
            this.radio_empty_w.UseVisualStyleBackColor = true;
            // 
            // CBClusterEnable
            // 
            this.CBClusterEnable.AutoSize = true;
            this.CBClusterEnable.Checked = true;
            this.CBClusterEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CBClusterEnable.Location = new System.Drawing.Point(12, 179);
            this.CBClusterEnable.Name = "CBClusterEnable";
            this.CBClusterEnable.Size = new System.Drawing.Size(211, 17);
            this.CBClusterEnable.TabIndex = 23;
            this.CBClusterEnable.Text = "Enable Clustering for Complex Analyses";
            this.CBClusterEnable.UseVisualStyleBackColor = true;
            this.CBClusterEnable.CheckedChanged += new System.EventHandler(this.ClusterEnableCB_CheckedChanged);
            // 
            // GBCluster
            // 
            this.GBCluster.Controls.Add(this.groupBox2);
            this.GBCluster.Controls.Add(this.groupBox1);
            this.GBCluster.Controls.Add(this.label4);
            this.GBCluster.Controls.Add(this.PathBoxKMeans);
            this.GBCluster.Location = new System.Drawing.Point(35, 202);
            this.GBCluster.Name = "GBCluster";
            this.GBCluster.Size = new System.Drawing.Size(542, 150);
            this.GBCluster.TabIndex = 24;
            this.GBCluster.TabStop = false;
            this.GBCluster.Text = "Clustering Options";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 606);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.GBCluster);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.CBClusterEnable);
            this.Controls.Add(this.PathBoxOffset);
            this.Controls.Add(this.PathBoxMovesZone);
            this.Controls.Add(this.TextBoxName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ConsoleBox);
            this.Controls.Add(this.PathBoxRoadType);
            this.Controls.Add(this.PathBoxSave);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ButtonSaveOutput);
            this.Controls.Add(this.PathBoxLoad);
            this.Controls.Add(this.ButtonLoadVissim);
            this.Controls.Add(this.ButtonRun);
            this.Controls.Add(this.ButtonClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.Text = "VISSIM MOVES Integrator";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.GBCluster.ResumeLayout(false);
            this.GBCluster.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonClose;
        private System.Windows.Forms.Button ButtonRun;
        private System.Windows.Forms.OpenFileDialog FileDialogOpen;
        private System.Windows.Forms.Button ButtonLoadVissim;
        private System.Windows.Forms.TextBox PathBoxLoad;
        private System.Windows.Forms.Button ButtonSaveOutput;
        private System.Windows.Forms.TextBox PathBoxSave;
        private System.Windows.Forms.FolderBrowserDialog FileDialogSave;
        private System.Windows.Forms.RichTextBox ConsoleBox;
        private System.Windows.Forms.TextBox PathBoxOffset;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox PathBoxMovesZone;
        private System.Windows.Forms.TextBox PathBoxRoadType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PathBoxKMeans;
        private System.Windows.Forms.TextBox TextBoxName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radio_sub_od;
        private System.Windows.Forms.RadioButton radio_sub_no;
        private System.Windows.Forms.RadioButton radio_sub_link;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radio_empty_err;
        private System.Windows.Forms.RadioButton radio_empty_w;
        private System.Windows.Forms.CheckBox CBClusterEnable;
        private System.Windows.Forms.GroupBox GBCluster;
    }
}

