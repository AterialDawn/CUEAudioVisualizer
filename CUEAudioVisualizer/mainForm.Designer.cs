namespace CUEAudioVisualizer
{
    partial class mainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
            this.mainNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.mainMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.colorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.primaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.primaryColorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.secondaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.secondaryColorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deviceSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visualizerModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smoothingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sensitivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.mainMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainNotifyIcon
            // 
            this.mainNotifyIcon.ContextMenuStrip = this.mainMenuStrip;
            this.mainNotifyIcon.Text = "CUE Audio Visualizer";
            this.mainNotifyIcon.Visible = true;
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.colorToolStripMenuItem,
            this.deviceSelectionToolStripMenuItem,
            this.visualizerModeToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(161, 120);
            // 
            // colorToolStripMenuItem
            // 
            this.colorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.primaryToolStripMenuItem,
            this.secondaryToolStripMenuItem});
            this.colorToolStripMenuItem.Name = "colorToolStripMenuItem";
            this.colorToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.colorToolStripMenuItem.Text = "Colors";
            // 
            // primaryToolStripMenuItem
            // 
            this.primaryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.primaryColorMenuItem});
            this.primaryToolStripMenuItem.Name = "primaryToolStripMenuItem";
            this.primaryToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.primaryToolStripMenuItem.Text = "Primary";
            // 
            // primaryColorMenuItem
            // 
            this.primaryColorMenuItem.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.primaryColorMenuItem.Name = "primaryColorMenuItem";
            this.primaryColorMenuItem.Size = new System.Drawing.Size(77, 22);
            this.primaryColorMenuItem.Text = " ";
            this.primaryColorMenuItem.Click += new System.EventHandler(this.primaryColorMenuItem_Click);
            // 
            // secondaryToolStripMenuItem
            // 
            this.secondaryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.secondaryColorMenuItem});
            this.secondaryToolStripMenuItem.Name = "secondaryToolStripMenuItem";
            this.secondaryToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.secondaryToolStripMenuItem.Text = "Secondary";
            // 
            // secondaryColorMenuItem
            // 
            this.secondaryColorMenuItem.BackColor = System.Drawing.SystemColors.Highlight;
            this.secondaryColorMenuItem.Name = "secondaryColorMenuItem";
            this.secondaryColorMenuItem.Size = new System.Drawing.Size(80, 22);
            this.secondaryColorMenuItem.Text = "  ";
            this.secondaryColorMenuItem.Click += new System.EventHandler(this.secondaryColorMenuItem_Click);
            // 
            // deviceSelectionToolStripMenuItem
            // 
            this.deviceSelectionToolStripMenuItem.Name = "deviceSelectionToolStripMenuItem";
            this.deviceSelectionToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.deviceSelectionToolStripMenuItem.Text = "Device Selection";
            // 
            // visualizerModeToolStripMenuItem
            // 
            this.visualizerModeToolStripMenuItem.Name = "visualizerModeToolStripMenuItem";
            this.visualizerModeToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.visualizerModeToolStripMenuItem.Text = "Visualizer Mode";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.smoothingToolStripMenuItem,
            this.sensitivityToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // smoothingToolStripMenuItem
            // 
            this.smoothingToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.smoothingToolStripMenuItem.Name = "smoothingToolStripMenuItem";
            this.smoothingToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.smoothingToolStripMenuItem.Text = "Smoothing...";
            // 
            // sensitivityToolStripMenuItem
            // 
            this.sensitivityToolStripMenuItem.Name = "sensitivityToolStripMenuItem";
            this.sensitivityToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.sensitivityToolStripMenuItem.Text = "Sensitivity...";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // visUpdateTimer
            // 
            this.visUpdateTimer.Enabled = true;
            this.visUpdateTimer.Interval = 10;
            this.visUpdateTimer.Tick += new System.EventHandler(this.visUpdateTimer_Tick);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 182);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "mainForm";
            this.Text = "CUEAudioVisualizerForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.mainForm_FormClosing);
            this.mainMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon mainNotifyIcon;
        private System.Windows.Forms.ContextMenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Timer visUpdateTimer;
        private System.Windows.Forms.ToolStripMenuItem colorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem primaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem primaryColorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem secondaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deviceSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem visualizerModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem secondaryColorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem smoothingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sensitivityToolStripMenuItem;
    }
}