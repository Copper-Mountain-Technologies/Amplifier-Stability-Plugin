namespace AmplifierStability
{
    partial class FormMain
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
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.readyTimer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelVnaInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelSpacer = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.buttonAutoConfig = new System.Windows.Forms.Button();
            this.traceComboBoxLabel = new System.Windows.Forms.Label();
            this.buttonPlot = new System.Windows.Forms.Button();
            this.comboBoxTrace = new System.Windows.Forms.ComboBox();
            this.labelUserMessage = new System.Windows.Forms.Label();
            this.comboBoxChannel = new System.Windows.Forms.ComboBox();
            this.groupBoxStabilityFactor = new System.Windows.Forms.GroupBox();
            this.radioButtonMu2 = new System.Windows.Forms.RadioButton();
            this.radioButtonMu1 = new System.Windows.Forms.RadioButton();
            this.radioButtonK = new System.Windows.Forms.RadioButton();
            this.chanComboBoxLabel = new System.Windows.Forms.Label();
            this.statusStrip.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.groupBoxStabilityFactor.SuspendLayout();
            this.SuspendLayout();
            // 
            // updateTimer
            // 
            this.updateTimer.Interval = 1000;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // readyTimer
            // 
            this.readyTimer.Interval = 1000;
            this.readyTimer.Tick += new System.EventHandler(this.readyTimer_Tick);
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelVnaInfo,
            this.toolStripStatusLabelSpacer,
            this.toolStripStatusLabelVersion});
            this.statusStrip.Location = new System.Drawing.Point(0, 240);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(284, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 28;
            // 
            // toolStripStatusLabelVnaInfo
            // 
            this.toolStripStatusLabelVnaInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusLabelVnaInfo.Margin = new System.Windows.Forms.Padding(5, 3, 0, 2);
            this.toolStripStatusLabelVnaInfo.Name = "toolStripStatusLabelVnaInfo";
            this.toolStripStatusLabelVnaInfo.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.toolStripStatusLabelVnaInfo.Size = new System.Drawing.Size(27, 17);
            this.toolStripStatusLabelVnaInfo.Text = "     ";
            // 
            // toolStripStatusLabelSpacer
            // 
            this.toolStripStatusLabelSpacer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusLabelSpacer.Name = "toolStripStatusLabelSpacer";
            this.toolStripStatusLabelSpacer.Size = new System.Drawing.Size(206, 17);
            this.toolStripStatusLabelSpacer.Spring = true;
            // 
            // toolStripStatusLabelVersion
            // 
            this.toolStripStatusLabelVersion.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripStatusLabelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusLabelVersion.ForeColor = System.Drawing.SystemColors.GrayText;
            this.toolStripStatusLabelVersion.Margin = new System.Windows.Forms.Padding(5, 3, 0, 2);
            this.toolStripStatusLabelVersion.Name = "toolStripStatusLabelVersion";
            this.toolStripStatusLabelVersion.Size = new System.Drawing.Size(26, 17);
            this.toolStripStatusLabelVersion.Text = "v ---";
            this.toolStripStatusLabelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.buttonAutoConfig);
            this.panelMain.Controls.Add(this.traceComboBoxLabel);
            this.panelMain.Controls.Add(this.buttonPlot);
            this.panelMain.Controls.Add(this.comboBoxTrace);
            this.panelMain.Controls.Add(this.labelUserMessage);
            this.panelMain.Controls.Add(this.comboBoxChannel);
            this.panelMain.Controls.Add(this.groupBoxStabilityFactor);
            this.panelMain.Controls.Add(this.chanComboBoxLabel);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(284, 262);
            this.panelMain.TabIndex = 35;
            // 
            // buttonAutoConfig
            // 
            this.buttonAutoConfig.Location = new System.Drawing.Point(195, 127);
            this.buttonAutoConfig.Name = "buttonAutoConfig";
            this.buttonAutoConfig.Size = new System.Drawing.Size(77, 23);
            this.buttonAutoConfig.TabIndex = 4;
            this.buttonAutoConfig.Text = "&Auto Config";
            this.buttonAutoConfig.UseVisualStyleBackColor = true;
            this.buttonAutoConfig.Click += new System.EventHandler(this.autoConfigButton_Click);
            // 
            // traceComboBoxLabel
            // 
            this.traceComboBoxLabel.AutoSize = true;
            this.traceComboBoxLabel.Location = new System.Drawing.Point(11, 113);
            this.traceComboBoxLabel.Name = "traceComboBoxLabel";
            this.traceComboBoxLabel.Size = new System.Drawing.Size(117, 13);
            this.traceComboBoxLabel.TabIndex = 3;
            this.traceComboBoxLabel.Text = "Plot Results into &Trace:";
            // 
            // buttonPlot
            // 
            this.buttonPlot.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPlot.Location = new System.Drawing.Point(12, 194);
            this.buttonPlot.Name = "buttonPlot";
            this.buttonPlot.Size = new System.Drawing.Size(260, 35);
            this.buttonPlot.TabIndex = 5;
            this.buttonPlot.Text = "&Plot Amplifier Stability";
            this.buttonPlot.UseVisualStyleBackColor = true;
            this.buttonPlot.Click += new System.EventHandler(this.plotButton_Click);
            // 
            // comboBoxTrace
            // 
            this.comboBoxTrace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTrace.FormattingEnabled = true;
            this.comboBoxTrace.Location = new System.Drawing.Point(12, 129);
            this.comboBoxTrace.Name = "comboBoxTrace";
            this.comboBoxTrace.Size = new System.Drawing.Size(177, 21);
            this.comboBoxTrace.TabIndex = 3;
            // 
            // labelUserMessage
            // 
            this.labelUserMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUserMessage.ForeColor = System.Drawing.Color.DarkBlue;
            this.labelUserMessage.Location = new System.Drawing.Point(12, 154);
            this.labelUserMessage.Name = "labelUserMessage";
            this.labelUserMessage.Size = new System.Drawing.Size(260, 37);
            this.labelUserMessage.TabIndex = 0;
            this.labelUserMessage.Text = "< user message goes here >";
            this.labelUserMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // comboBoxChannel
            // 
            this.comboBoxChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxChannel.FormattingEnabled = true;
            this.comboBoxChannel.Location = new System.Drawing.Point(12, 83);
            this.comboBoxChannel.Name = "comboBoxChannel";
            this.comboBoxChannel.Size = new System.Drawing.Size(260, 21);
            this.comboBoxChannel.TabIndex = 2;
            this.comboBoxChannel.SelectedIndexChanged += new System.EventHandler(this.chanComboBox_SelectedIndexChanged);
            // 
            // groupBoxStabilityFactor
            // 
            this.groupBoxStabilityFactor.Controls.Add(this.radioButtonMu2);
            this.groupBoxStabilityFactor.Controls.Add(this.radioButtonMu1);
            this.groupBoxStabilityFactor.Controls.Add(this.radioButtonK);
            this.groupBoxStabilityFactor.Location = new System.Drawing.Point(12, 9);
            this.groupBoxStabilityFactor.Name = "groupBoxStabilityFactor";
            this.groupBoxStabilityFactor.Size = new System.Drawing.Size(260, 48);
            this.groupBoxStabilityFactor.TabIndex = 1;
            this.groupBoxStabilityFactor.TabStop = false;
            this.groupBoxStabilityFactor.Text = "Stability Factor";
            // 
            // radioButtonMu2
            // 
            this.radioButtonMu2.AutoSize = true;
            this.radioButtonMu2.Location = new System.Drawing.Point(203, 19);
            this.radioButtonMu2.Name = "radioButtonMu2";
            this.radioButtonMu2.Size = new System.Drawing.Size(46, 17);
            this.radioButtonMu2.TabIndex = 3;
            this.radioButtonMu2.TabStop = true;
            this.radioButtonMu2.Text = "Mu&2";
            this.radioButtonMu2.UseVisualStyleBackColor = true;
            // 
            // radioButtonMu1
            // 
            this.radioButtonMu1.AutoSize = true;
            this.radioButtonMu1.Location = new System.Drawing.Point(100, 19);
            this.radioButtonMu1.Name = "radioButtonMu1";
            this.radioButtonMu1.Size = new System.Drawing.Size(46, 17);
            this.radioButtonMu1.TabIndex = 2;
            this.radioButtonMu1.TabStop = true;
            this.radioButtonMu1.Text = "Mu&1";
            this.radioButtonMu1.UseVisualStyleBackColor = true;
            // 
            // radioButtonK
            // 
            this.radioButtonK.AutoSize = true;
            this.radioButtonK.Location = new System.Drawing.Point(11, 19);
            this.radioButtonK.Name = "radioButtonK";
            this.radioButtonK.Size = new System.Drawing.Size(32, 17);
            this.radioButtonK.TabIndex = 1;
            this.radioButtonK.TabStop = true;
            this.radioButtonK.Text = "&K";
            this.radioButtonK.UseVisualStyleBackColor = true;
            // 
            // chanComboBoxLabel
            // 
            this.chanComboBoxLabel.AutoSize = true;
            this.chanComboBoxLabel.Location = new System.Drawing.Point(11, 67);
            this.chanComboBoxLabel.Name = "chanComboBoxLabel";
            this.chanComboBoxLabel.Size = new System.Drawing.Size(49, 13);
            this.chanComboBoxLabel.TabIndex = 2;
            this.chanComboBoxLabel.Text = "&Channel:";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.panelMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "< application title goes here >";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.groupBoxStabilityFactor.ResumeLayout(false);
            this.groupBoxStabilityFactor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.Timer readyTimer;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelVnaInfo;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelSpacer;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelVersion;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Button buttonPlot;
        private System.Windows.Forms.Label labelUserMessage;
        private System.Windows.Forms.ComboBox comboBoxChannel;
        private System.Windows.Forms.GroupBox groupBoxStabilityFactor;
        private System.Windows.Forms.RadioButton radioButtonMu2;
        private System.Windows.Forms.RadioButton radioButtonMu1;
        private System.Windows.Forms.RadioButton radioButtonK;
        private System.Windows.Forms.Label chanComboBoxLabel;
        private System.Windows.Forms.Button buttonAutoConfig;
        private System.Windows.Forms.Label traceComboBoxLabel;
        private System.Windows.Forms.ComboBox comboBoxTrace;
    }
}

