namespace WindowsFormsApplication1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.trackBar3 = new System.Windows.Forms.TrackBar();
            this.planeDirXLabel = new System.Windows.Forms.Label();
            this.planeDirYLabel = new System.Windows.Forms.Label();
            this.planeDirZLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rayDirYTrackBar = new System.Windows.Forms.TrackBar();
            this.rayDirXLabel = new System.Windows.Forms.Label();
            this.rayDirXTrackBar = new System.Windows.Forms.TrackBar();
            this.rayDirYLabel = new System.Windows.Forms.Label();
            this.rayDirZLabel = new System.Windows.Forms.Label();
            this.rayDirZTrackBar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.intersectionPointLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rayDirYTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayDirXTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayDirZTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(26, 29);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Minimum = -100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(187, 45);
            this.trackBar1.TabIndex = 1;
            this.trackBar1.Value = 100;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // trackBar2
            // 
            this.trackBar2.Location = new System.Drawing.Point(26, 70);
            this.trackBar2.Maximum = 100;
            this.trackBar2.Minimum = -100;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(187, 45);
            this.trackBar2.TabIndex = 2;
            this.trackBar2.Value = 100;
            this.trackBar2.Scroll += new System.EventHandler(this.trackBar2_Scroll);
            // 
            // trackBar3
            // 
            this.trackBar3.Location = new System.Drawing.Point(26, 121);
            this.trackBar3.Maximum = 100;
            this.trackBar3.Minimum = -100;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new System.Drawing.Size(187, 45);
            this.trackBar3.TabIndex = 3;
            this.trackBar3.Value = 100;
            this.trackBar3.Scroll += new System.EventHandler(this.trackBar3_Scroll);
            // 
            // planeDirXLabel
            // 
            this.planeDirXLabel.AutoSize = true;
            this.planeDirXLabel.Location = new System.Drawing.Point(230, 29);
            this.planeDirXLabel.Name = "planeDirXLabel";
            this.planeDirXLabel.Size = new System.Drawing.Size(35, 13);
            this.planeDirXLabel.TabIndex = 5;
            this.planeDirXLabel.Text = "label1";
            // 
            // planeDirYLabel
            // 
            this.planeDirYLabel.AutoSize = true;
            this.planeDirYLabel.Location = new System.Drawing.Point(230, 70);
            this.planeDirYLabel.Name = "planeDirYLabel";
            this.planeDirYLabel.Size = new System.Drawing.Size(35, 13);
            this.planeDirYLabel.TabIndex = 6;
            this.planeDirYLabel.Text = "label1";
            // 
            // planeDirZLabel
            // 
            this.planeDirZLabel.AutoSize = true;
            this.planeDirZLabel.Location = new System.Drawing.Point(230, 121);
            this.planeDirZLabel.Name = "planeDirZLabel";
            this.planeDirZLabel.Size = new System.Drawing.Size(35, 13);
            this.planeDirZLabel.TabIndex = 7;
            this.planeDirZLabel.Text = "label1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.trackBar2);
            this.groupBox1.Controls.Add(this.planeDirXLabel);
            this.groupBox1.Controls.Add(this.trackBar1);
            this.groupBox1.Controls.Add(this.planeDirYLabel);
            this.groupBox1.Controls.Add(this.planeDirZLabel);
            this.groupBox1.Controls.Add(this.trackBar3);
            this.groupBox1.Location = new System.Drawing.Point(12, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(270, 161);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Plane Direction";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.rayDirYTrackBar);
            this.groupBox2.Controls.Add(this.rayDirXLabel);
            this.groupBox2.Controls.Add(this.rayDirXTrackBar);
            this.groupBox2.Controls.Add(this.rayDirYLabel);
            this.groupBox2.Controls.Add(this.rayDirZLabel);
            this.groupBox2.Controls.Add(this.rayDirZTrackBar);
            this.groupBox2.Location = new System.Drawing.Point(12, 228);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(270, 161);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Ray Direction";
            // 
            // rayDirYTrackBar
            // 
            this.rayDirYTrackBar.Location = new System.Drawing.Point(26, 70);
            this.rayDirYTrackBar.Maximum = 100;
            this.rayDirYTrackBar.Minimum = -100;
            this.rayDirYTrackBar.Name = "rayDirYTrackBar";
            this.rayDirYTrackBar.Size = new System.Drawing.Size(187, 45);
            this.rayDirYTrackBar.TabIndex = 2;
            this.rayDirYTrackBar.Scroll += new System.EventHandler(this.rayDirYTrackBar_Scroll);
            // 
            // rayDirXLabel
            // 
            this.rayDirXLabel.AutoSize = true;
            this.rayDirXLabel.Location = new System.Drawing.Point(230, 29);
            this.rayDirXLabel.Name = "rayDirXLabel";
            this.rayDirXLabel.Size = new System.Drawing.Size(35, 13);
            this.rayDirXLabel.TabIndex = 5;
            this.rayDirXLabel.Text = "label1";
            // 
            // rayDirXTrackBar
            // 
            this.rayDirXTrackBar.Location = new System.Drawing.Point(26, 29);
            this.rayDirXTrackBar.Maximum = 100;
            this.rayDirXTrackBar.Minimum = -100;
            this.rayDirXTrackBar.Name = "rayDirXTrackBar";
            this.rayDirXTrackBar.Size = new System.Drawing.Size(187, 45);
            this.rayDirXTrackBar.TabIndex = 1;
            this.rayDirXTrackBar.Scroll += new System.EventHandler(this.rayDirXTrackBar_Scroll);
            // 
            // rayDirYLabel
            // 
            this.rayDirYLabel.AutoSize = true;
            this.rayDirYLabel.Location = new System.Drawing.Point(230, 70);
            this.rayDirYLabel.Name = "rayDirYLabel";
            this.rayDirYLabel.Size = new System.Drawing.Size(35, 13);
            this.rayDirYLabel.TabIndex = 6;
            this.rayDirYLabel.Text = "label1";
            // 
            // rayDirZLabel
            // 
            this.rayDirZLabel.AutoSize = true;
            this.rayDirZLabel.Location = new System.Drawing.Point(230, 121);
            this.rayDirZLabel.Name = "rayDirZLabel";
            this.rayDirZLabel.Size = new System.Drawing.Size(35, 13);
            this.rayDirZLabel.TabIndex = 7;
            this.rayDirZLabel.Text = "label1";
            // 
            // rayDirZTrackBar
            // 
            this.rayDirZTrackBar.Location = new System.Drawing.Point(26, 121);
            this.rayDirZTrackBar.Maximum = 100;
            this.rayDirZTrackBar.Minimum = -100;
            this.rayDirZTrackBar.Name = "rayDirZTrackBar";
            this.rayDirZTrackBar.Size = new System.Drawing.Size(187, 45);
            this.rayDirZTrackBar.TabIndex = 3;
            this.rayDirZTrackBar.Scroll += new System.EventHandler(this.rayDirZTrackBar_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 428);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Intersection Point";
            // 
            // intersectionPointLabel
            // 
            this.intersectionPointLabel.AutoSize = true;
            this.intersectionPointLabel.Location = new System.Drawing.Point(142, 428);
            this.intersectionPointLabel.Name = "intersectionPointLabel";
            this.intersectionPointLabel.Size = new System.Drawing.Size(35, 13);
            this.intersectionPointLabel.TabIndex = 12;
            this.intersectionPointLabel.Text = "label2";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 29);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(19, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "0";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1152, 740);
            this.Controls.Add(this.intersectionPointLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rayDirYTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayDirXTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rayDirZTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.TrackBar trackBar2;
        private System.Windows.Forms.TrackBar trackBar3;
        private System.Windows.Forms.Label planeDirXLabel;
        private System.Windows.Forms.Label planeDirYLabel;
        private System.Windows.Forms.Label planeDirZLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TrackBar rayDirYTrackBar;
        private System.Windows.Forms.Label rayDirXLabel;
        private System.Windows.Forms.TrackBar rayDirXTrackBar;
        private System.Windows.Forms.Label rayDirYLabel;
        private System.Windows.Forms.Label rayDirZLabel;
        private System.Windows.Forms.TrackBar rayDirZTrackBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label intersectionPointLabel;
        private System.Windows.Forms.Button button1;
    }
}

