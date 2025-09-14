namespace eSTOL_Training_Tool_Core.UI
{
    partial class FormUI
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUI));
            labelResult = new System.Windows.Forms.Label();
            textBoxResult = new System.Windows.Forms.TextBox();
            textBoxUser = new System.Windows.Forms.TextBox();
            labelPreset = new System.Windows.Forms.Label();
            comboBoxPreset = new System.Windows.Forms.ComboBox();
            buttonApplyPreset = new System.Windows.Forms.Button();
            buttonTeleport = new System.Windows.Forms.Button();
            buttonSetRefPos = new System.Windows.Forms.Button();
            textBoxStatus = new System.Windows.Forms.TextBox();
            buttonCreatePreset = new System.Windows.Forms.Button();
            panel = new System.Windows.Forms.Panel();
            comboBoxUnit = new System.Windows.Forms.ComboBox();
            checkBoxResult = new System.Windows.Forms.CheckBox();
            checkBoxTelemetry = new System.Windows.Forms.CheckBox();
            timer1 = new System.Windows.Forms.Timer(components);
            labelStopwatch = new System.Windows.Forms.Label();
            progressBarStopwatch = new System.Windows.Forms.ProgressBar();
            buttonStartStopwatch = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            button1 = new System.Windows.Forms.Button();
            checkBoxOntop = new System.Windows.Forms.CheckBox();
            numericUpDownStopwatchOffest = new System.Windows.Forms.NumericUpDown();
            label2 = new System.Windows.Forms.Label();
            checkBoxDebugging = new System.Windows.Forms.CheckBox();
            panelWind = new System.Windows.Forms.Panel();
            labelWind = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            textBoxAligned = new System.Windows.Forms.TextBox();
            labelName = new System.Windows.Forms.Label();
            labelSession = new System.Windows.Forms.Label();
            textBoxSessionKey = new System.Windows.Forms.TextBox();
            numericUpDownTransparency = new System.Windows.Forms.NumericUpDown();
            checkBoxSaveRecording = new System.Windows.Forms.CheckBox();
            linkLabelRecordings = new System.Windows.Forms.LinkLabel();
            buttonUnFlip = new System.Windows.Forms.Button();
            panelCollisions = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)numericUpDownStopwatchOffest).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTransparency).BeginInit();
            SuspendLayout();
            // 
            // labelResult
            // 
            labelResult.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            labelResult.AutoSize = true;
            labelResult.Location = new System.Drawing.Point(12, 9);
            labelResult.Name = "labelResult";
            labelResult.Size = new System.Drawing.Size(39, 15);
            labelResult.TabIndex = 99;
            labelResult.Text = "Result";
            // 
            // textBoxResult
            // 
            textBoxResult.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            textBoxResult.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            textBoxResult.Location = new System.Drawing.Point(12, 58);
            textBoxResult.Multiline = true;
            textBoxResult.Name = "textBoxResult";
            textBoxResult.ReadOnly = true;
            textBoxResult.Size = new System.Drawing.Size(335, 549);
            textBoxResult.TabIndex = 1;
            // 
            // textBoxUser
            // 
            textBoxUser.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            textBoxUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxUser.Location = new System.Drawing.Point(667, 43);
            textBoxUser.Name = "textBoxUser";
            textBoxUser.Size = new System.Drawing.Size(209, 23);
            textBoxUser.TabIndex = 2;
            textBoxUser.TextChanged += textBoxUser_TextChanged;
            textBoxUser.KeyDown += textBoxUser_KeyDown;
            // 
            // labelPreset
            // 
            labelPreset.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            labelPreset.AutoSize = true;
            labelPreset.Location = new System.Drawing.Point(666, 192);
            labelPreset.Name = "labelPreset";
            labelPreset.Size = new System.Drawing.Size(39, 15);
            labelPreset.TabIndex = 3;
            labelPreset.Text = "Preset";
            labelPreset.Click += labelPreset_Click;
            // 
            // comboBoxPreset
            // 
            comboBoxPreset.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            comboBoxPreset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            comboBoxPreset.FormattingEnabled = true;
            comboBoxPreset.Location = new System.Drawing.Point(667, 210);
            comboBoxPreset.Name = "comboBoxPreset";
            comboBoxPreset.Size = new System.Drawing.Size(211, 23);
            comboBoxPreset.TabIndex = 0;
            // 
            // buttonApplyPreset
            // 
            buttonApplyPreset.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonApplyPreset.AutoSize = true;
            buttonApplyPreset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonApplyPreset.Location = new System.Drawing.Point(667, 239);
            buttonApplyPreset.Name = "buttonApplyPreset";
            buttonApplyPreset.Size = new System.Drawing.Size(211, 27);
            buttonApplyPreset.TabIndex = 5;
            buttonApplyPreset.Text = "Apply";
            buttonApplyPreset.Click += buttonApplyPreset_Click;
            // 
            // buttonTeleport
            // 
            buttonTeleport.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonTeleport.AutoSize = true;
            buttonTeleport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonTeleport.Location = new System.Drawing.Point(667, 330);
            buttonTeleport.Name = "buttonTeleport";
            buttonTeleport.Size = new System.Drawing.Size(211, 27);
            buttonTeleport.TabIndex = 6;
            buttonTeleport.Text = "Teleport";
            buttonTeleport.Click += buttonTeleport_Click;
            // 
            // buttonSetRefPos
            // 
            buttonSetRefPos.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonSetRefPos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonSetRefPos.Location = new System.Drawing.Point(667, 270);
            buttonSetRefPos.Name = "buttonSetRefPos";
            buttonSetRefPos.Size = new System.Drawing.Size(211, 23);
            buttonSetRefPos.TabIndex = 7;
            buttonSetRefPos.Text = "Set Start";
            buttonSetRefPos.Click += buttonSetRefPos_Click;
            // 
            // textBoxStatus
            // 
            textBoxStatus.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxStatus.Location = new System.Drawing.Point(12, 613);
            textBoxStatus.Name = "textBoxStatus";
            textBoxStatus.ReadOnly = true;
            textBoxStatus.Size = new System.Drawing.Size(648, 23);
            textBoxStatus.TabIndex = 8;
            textBoxStatus.Text = "Status";
            textBoxStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonCreatePreset
            // 
            buttonCreatePreset.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonCreatePreset.AutoSize = true;
            buttonCreatePreset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonCreatePreset.Location = new System.Drawing.Point(667, 299);
            buttonCreatePreset.Name = "buttonCreatePreset";
            buttonCreatePreset.Size = new System.Drawing.Size(211, 27);
            buttonCreatePreset.TabIndex = 9;
            buttonCreatePreset.Text = "Create Preset";
            buttonCreatePreset.Click += buttonCreatePreset_Click;
            // 
            // panel
            // 
            panel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            panel.BackgroundImage = (System.Drawing.Image)resources.GetObject("panel.BackgroundImage");
            panel.Location = new System.Drawing.Point(353, 29);
            panel.Name = "panel";
            panel.Size = new System.Drawing.Size(307, 578);
            panel.TabIndex = 10;
            panel.Paint += panel_Paint;
            panel.DoubleClick += pannel_DoubleClick;
            panel.Resize += panel_Resize;
            // 
            // comboBoxUnit
            // 
            comboBoxUnit.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            comboBoxUnit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            comboBoxUnit.FormattingEnabled = true;
            comboBoxUnit.Location = new System.Drawing.Point(667, 166);
            comboBoxUnit.Name = "comboBoxUnit";
            comboBoxUnit.Size = new System.Drawing.Size(211, 23);
            comboBoxUnit.TabIndex = 11;
            comboBoxUnit.SelectedIndexChanged += comboBoxUnit_SelectedIndexChanged;
            // 
            // checkBoxResult
            // 
            checkBoxResult.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            checkBoxResult.AutoSize = true;
            checkBoxResult.Location = new System.Drawing.Point(670, 116);
            checkBoxResult.Name = "checkBoxResult";
            checkBoxResult.Size = new System.Drawing.Size(88, 19);
            checkBoxResult.TabIndex = 12;
            checkBoxResult.Text = "send results";
            checkBoxResult.CheckedChanged += checkBoxResult_CheckedChanged;
            // 
            // checkBoxTelemetry
            // 
            checkBoxTelemetry.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            checkBoxTelemetry.AutoSize = true;
            checkBoxTelemetry.Location = new System.Drawing.Point(669, 141);
            checkBoxTelemetry.Name = "checkBoxTelemetry";
            checkBoxTelemetry.Size = new System.Drawing.Size(104, 19);
            checkBoxTelemetry.TabIndex = 13;
            checkBoxTelemetry.Text = "send telemetry";
            checkBoxTelemetry.CheckedChanged += checkBoxTelemetry_CheckedChanged;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Tick += Timer;
            // 
            // labelStopwatch
            // 
            labelStopwatch.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            labelStopwatch.AutoSize = true;
            labelStopwatch.Font = new System.Drawing.Font("Consolas", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            labelStopwatch.Location = new System.Drawing.Point(668, 558);
            labelStopwatch.Name = "labelStopwatch";
            labelStopwatch.Size = new System.Drawing.Size(132, 41);
            labelStopwatch.TabIndex = 14;
            labelStopwatch.Text = " 00:00";
            labelStopwatch.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // progressBarStopwatch
            // 
            progressBarStopwatch.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            progressBarStopwatch.Location = new System.Drawing.Point(666, 612);
            progressBarStopwatch.Name = "progressBarStopwatch";
            progressBarStopwatch.Size = new System.Drawing.Size(210, 23);
            progressBarStopwatch.TabIndex = 15;
            // 
            // buttonStartStopwatch
            // 
            buttonStartStopwatch.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonStartStopwatch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonStartStopwatch.Location = new System.Drawing.Point(800, 582);
            buttonStartStopwatch.Name = "buttonStartStopwatch";
            buttonStartStopwatch.Size = new System.Drawing.Size(76, 24);
            buttonStartStopwatch.TabIndex = 16;
            buttonStartStopwatch.Text = "T-Offset";
            buttonStartStopwatch.Click += buttonStartStopwatch_Click;
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(747, 494);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(129, 30);
            label1.TabIndex = 17;
            label1.Text = "always listen to Airboss\r\nfor timing instructions";
            // 
            // button1
            // 
            button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button1.Location = new System.Drawing.Point(801, 556);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(75, 23);
            button1.TabIndex = 18;
            button1.Text = "Start";
            button1.Click += buttonStart_Click;
            // 
            // checkBoxOntop
            // 
            checkBoxOntop.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            checkBoxOntop.AutoSize = true;
            checkBoxOntop.Location = new System.Drawing.Point(777, 5);
            checkBoxOntop.Name = "checkBoxOntop";
            checkBoxOntop.Size = new System.Drawing.Size(99, 19);
            checkBoxOntop.TabIndex = 19;
            checkBoxOntop.Text = "always on top";
            checkBoxOntop.CheckedChanged += checkBoxOntop_CheckedChanged;
            // 
            // numericUpDownStopwatchOffest
            // 
            numericUpDownStopwatchOffest.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownStopwatchOffest.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            numericUpDownStopwatchOffest.Location = new System.Drawing.Point(668, 527);
            numericUpDownStopwatchOffest.Name = "numericUpDownStopwatchOffest";
            numericUpDownStopwatchOffest.Size = new System.Drawing.Size(208, 23);
            numericUpDownStopwatchOffest.TabIndex = 20;
            numericUpDownStopwatchOffest.Value = new decimal(new int[] { 30, 0, 0, 0 });
            numericUpDownStopwatchOffest.ValueChanged += numericUpDown1_ValueChanged;
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(668, 509);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(39, 15);
            label2.TabIndex = 21;
            label2.Text = "Offset";
            // 
            // checkBoxDebugging
            // 
            checkBoxDebugging.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            checkBoxDebugging.AutoSize = true;
            checkBoxDebugging.Location = new System.Drawing.Point(711, 5);
            checkBoxDebugging.Name = "checkBoxDebugging";
            checkBoxDebugging.Size = new System.Drawing.Size(60, 19);
            checkBoxDebugging.TabIndex = 22;
            checkBoxDebugging.Text = "debug";
            checkBoxDebugging.CheckedChanged += checkBoxDebugging_CheckedChanged;
            // 
            // panelWind
            // 
            panelWind.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            panelWind.Location = new System.Drawing.Point(667, 399);
            panelWind.Name = "panelWind";
            panelWind.Size = new System.Drawing.Size(69, 68);
            panelWind.TabIndex = 23;
            panelWind.Paint += panelWind_Paint;
            // 
            // labelWind
            // 
            labelWind.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            labelWind.AutoSize = true;
            labelWind.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            labelWind.Location = new System.Drawing.Point(738, 438);
            labelWind.Name = "labelWind";
            labelWind.Size = new System.Drawing.Size(60, 25);
            labelWind.TabIndex = 24;
            labelWind.Text = "--,- ft";
            labelWind.Click += label3_Click;
            // 
            // label3
            // 
            label3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(738, 423);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(35, 15);
            label3.TabIndex = 25;
            label3.Text = "Wind";
            // 
            // textBoxAligned
            // 
            textBoxAligned.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxAligned.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxAligned.Location = new System.Drawing.Point(12, 29);
            textBoxAligned.Name = "textBoxAligned";
            textBoxAligned.ReadOnly = true;
            textBoxAligned.Size = new System.Drawing.Size(335, 23);
            textBoxAligned.TabIndex = 26;
            textBoxAligned.Text = "...";
            textBoxAligned.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            textBoxAligned.TextChanged += textBox1_TextChanged;
            // 
            // labelName
            // 
            labelName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            labelName.AutoSize = true;
            labelName.Font = new System.Drawing.Font("Segoe UI", 6F);
            labelName.Location = new System.Drawing.Point(667, 29);
            labelName.Name = "labelName";
            labelName.Size = new System.Drawing.Size(26, 11);
            labelName.TabIndex = 27;
            labelName.Text = "Name";
            labelName.Click += label4_Click;
            // 
            // labelSession
            // 
            labelSession.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            labelSession.AutoSize = true;
            labelSession.Font = new System.Drawing.Font("Segoe UI", 6F);
            labelSession.Location = new System.Drawing.Point(664, 69);
            labelSession.Name = "labelSession";
            labelSession.Size = new System.Drawing.Size(31, 11);
            labelSession.TabIndex = 28;
            labelSession.Text = "Session";
            labelSession.Click += label4_Click_1;
            // 
            // textBoxSessionKey
            // 
            textBoxSessionKey.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            textBoxSessionKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxSessionKey.Location = new System.Drawing.Point(667, 83);
            textBoxSessionKey.Name = "textBoxSessionKey";
            textBoxSessionKey.Size = new System.Drawing.Size(209, 23);
            textBoxSessionKey.TabIndex = 29;
            textBoxSessionKey.KeyDown += textBoxSessionKey_KeyDown;
            // 
            // numericUpDownTransparency
            // 
            numericUpDownTransparency.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownTransparency.BorderStyle = System.Windows.Forms.BorderStyle.None;
            numericUpDownTransparency.Font = new System.Drawing.Font("Segoe UI", 8F);
            numericUpDownTransparency.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDownTransparency.Location = new System.Drawing.Point(667, 5);
            numericUpDownTransparency.Maximum = new decimal(new int[] { 80, 0, 0, 0 });
            numericUpDownTransparency.Name = "numericUpDownTransparency";
            numericUpDownTransparency.Size = new System.Drawing.Size(35, 18);
            numericUpDownTransparency.TabIndex = 30;
            numericUpDownTransparency.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            numericUpDownTransparency.ValueChanged += numericUpDownTransparency_ValueChanged;
            // 
            // checkBoxSaveRecording
            // 
            checkBoxSaveRecording.AutoSize = true;
            checkBoxSaveRecording.Checked = true;
            checkBoxSaveRecording.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxSaveRecording.Location = new System.Drawing.Point(793, 116);
            checkBoxSaveRecording.Name = "checkBoxSaveRecording";
            checkBoxSaveRecording.Size = new System.Drawing.Size(85, 19);
            checkBoxSaveRecording.TabIndex = 100;
            checkBoxSaveRecording.Text = "record GPX";
            checkBoxSaveRecording.UseVisualStyleBackColor = true;
            checkBoxSaveRecording.CheckedChanged += checkBoxSaveRecording_CheckedChanged;
            // 
            // linkLabelRecordings
            // 
            linkLabelRecordings.AutoSize = true;
            linkLabelRecordings.Location = new System.Drawing.Point(777, 142);
            linkLabelRecordings.Name = "linkLabelRecordings";
            linkLabelRecordings.Size = new System.Drawing.Size(99, 15);
            linkLabelRecordings.TabIndex = 101;
            linkLabelRecordings.TabStop = true;
            linkLabelRecordings.Text = "GPX export folder";
            linkLabelRecordings.LinkClicked += linkLabelRecordings_LinkClicked;
            // 
            // buttonUnFlip
            // 
            buttonUnFlip.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonUnFlip.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonUnFlip.Location = new System.Drawing.Point(666, 363);
            buttonUnFlip.Name = "buttonUnFlip";
            buttonUnFlip.Size = new System.Drawing.Size(212, 23);
            buttonUnFlip.TabIndex = 102;
            buttonUnFlip.Text = "UnFlip";
            buttonUnFlip.UseVisualStyleBackColor = true;
            buttonUnFlip.Click += buttonUnFlip_Click;
            // 
            // panelCollisions
            // 
            panelCollisions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            panelCollisions.BackgroundImage = (System.Drawing.Image)resources.GetObject("panelCollisions.BackgroundImage");
            panelCollisions.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panelCollisions.Location = new System.Drawing.Point(810, 399);
            panelCollisions.Name = "panelCollisions";
            panelCollisions.Size = new System.Drawing.Size(68, 68);
            panelCollisions.TabIndex = 103;
            panelCollisions.Paint += panelCollisions_Paint;
            // 
            // FormUI
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(888, 647);
            Controls.Add(panelCollisions);
            Controls.Add(buttonUnFlip);
            Controls.Add(linkLabelRecordings);
            Controls.Add(checkBoxSaveRecording);
            Controls.Add(numericUpDownTransparency);
            Controls.Add(textBoxSessionKey);
            Controls.Add(labelSession);
            Controls.Add(labelName);
            Controls.Add(textBoxAligned);
            Controls.Add(label3);
            Controls.Add(labelWind);
            Controls.Add(panelWind);
            Controls.Add(checkBoxDebugging);
            Controls.Add(label2);
            Controls.Add(numericUpDownStopwatchOffest);
            Controls.Add(checkBoxOntop);
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(buttonStartStopwatch);
            Controls.Add(progressBarStopwatch);
            Controls.Add(labelStopwatch);
            Controls.Add(checkBoxTelemetry);
            Controls.Add(checkBoxResult);
            Controls.Add(comboBoxUnit);
            Controls.Add(panel);
            Controls.Add(buttonCreatePreset);
            Controls.Add(textBoxStatus);
            Controls.Add(buttonSetRefPos);
            Controls.Add(buttonTeleport);
            Controls.Add(buttonApplyPreset);
            Controls.Add(comboBoxPreset);
            Controls.Add(labelPreset);
            Controls.Add(textBoxUser);
            Controls.Add(textBoxResult);
            Controls.Add(labelResult);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "FormUI";
            Text = "eSTOL Training Tool";
            ((System.ComponentModel.ISupportInitialize)numericUpDownStopwatchOffest).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTransparency).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.TextBox textBoxResult;
        private System.Windows.Forms.TextBox textBoxUser;
        private System.Windows.Forms.Label labelPreset;
        private System.Windows.Forms.ComboBox comboBoxPreset;
        private System.Windows.Forms.Button buttonApplyPreset;
        private System.Windows.Forms.Button buttonTeleport;
        private System.Windows.Forms.Button buttonSetRefPos;
        private System.Windows.Forms.TextBox textBoxStatus;
        private System.Windows.Forms.Button buttonCreatePreset;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.ComboBox comboBoxUnit;
        private System.Windows.Forms.CheckBox checkBoxResult;
        private System.Windows.Forms.CheckBox checkBoxTelemetry;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label labelStopwatch;
        private System.Windows.Forms.ProgressBar progressBarStopwatch;
        private System.Windows.Forms.Button buttonStartStopwatch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBoxOntop;
        private System.Windows.Forms.NumericUpDown numericUpDownStopwatchOffest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxDebugging;
        private System.Windows.Forms.Panel panelWind;
        private System.Windows.Forms.Label labelWind;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxAligned;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelSession;
        private System.Windows.Forms.TextBox textBoxSessionKey;
        private System.Windows.Forms.NumericUpDown numericUpDownTransparency;
        private System.Windows.Forms.CheckBox checkBoxSaveRecording;
        private System.Windows.Forms.LinkLabel linkLabelRecordings;
        private System.Windows.Forms.Button buttonUnFlip;
        private System.Windows.Forms.Panel panelCollisions;
    }
}