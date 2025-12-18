namespace JA.UI
{
    partial class FeatherstoneForm
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
            if (disposing&&( components!=null ))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FeatherstoneForm));
            this.kryptonToolStrip1 = new Krypton.Toolkit.KryptonToolStrip();
            this.kryptonStatusStrip1 = new Krypton.Toolkit.KryptonStatusStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.kryptonDrawControl1 = new JA.UI.KryptonDrawControl();
            this.kryptonToolStrip1.SuspendLayout();
            this.kryptonStatusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonDrawControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonToolStrip1
            // 
            this.kryptonToolStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.kryptonToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2});
            this.kryptonToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.kryptonToolStrip1.Name = "kryptonToolStrip1";
            this.kryptonToolStrip1.Size = new System.Drawing.Size(934, 25);
            this.kryptonToolStrip1.TabIndex = 0;
            this.kryptonToolStrip1.Text = "kryptonToolStrip1";
            // 
            // kryptonStatusStrip1
            // 
            this.kryptonStatusStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.kryptonStatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.kryptonStatusStrip1.Location = new System.Drawing.Point(0, 674);
            this.kryptonStatusStrip1.Name = "kryptonStatusStrip1";
            this.kryptonStatusStrip1.ProgressBars = null;
            this.kryptonStatusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.kryptonStatusStrip1.Size = new System.Drawing.Size(934, 22);
            this.kryptonStatusStrip1.TabIndex = 1;
            this.kryptonStatusStrip1.Text = "kryptonStatusStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(48, 22);
            this.toolStripButton1.Text = "Run";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(55, 22);
            this.toolStripButton2.Text = "Reset";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(42, 17);
            this.toolStripStatusLabel1.Text = "Status:";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // kryptonDrawControl1
            // 
            this.kryptonDrawControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.kryptonDrawControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonDrawControl1.Location = new System.Drawing.Point(0, 25);
            this.kryptonDrawControl1.Name = "kryptonDrawControl1";
            this.kryptonDrawControl1.Size = new System.Drawing.Size(934, 649);
            this.kryptonDrawControl1.TabIndex = 4;
            this.kryptonDrawControl1.TabStop = false;
            this.kryptonDrawControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.kryptonDrawControl1_Paint);
            // 
            // FeatherstoneForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 696);
            this.Controls.Add(this.kryptonDrawControl1);
            this.Controls.Add(this.kryptonToolStrip1);
            this.Controls.Add(this.kryptonStatusStrip1);
            this.Name = "FeatherstoneForm";
            this.Text = "FeatherstoneForm";
            this.TextExtra = "JA";
            this.kryptonToolStrip1.ResumeLayout(false);
            this.kryptonToolStrip1.PerformLayout();
            this.kryptonStatusStrip1.ResumeLayout(false);
            this.kryptonStatusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonDrawControl1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Krypton.Toolkit.KryptonToolStrip kryptonToolStrip1;
        private Krypton.Toolkit.KryptonStatusStrip kryptonStatusStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private KryptonDrawControl kryptonDrawControl1;
    }
}