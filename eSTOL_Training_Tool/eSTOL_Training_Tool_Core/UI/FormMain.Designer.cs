namespace eSTOL_Training_Tool_Core.UI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            comboBoxSelectPreset = new System.Windows.Forms.ComboBox();
            buttonApplyPreset = new System.Windows.Forms.Button();
            buttonTeleport = new System.Windows.Forms.Button();
            textBoxResult = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            textState = new System.Windows.Forms.TextBox();
            buttonSetRefPos = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // comboBoxSelectPreset
            // 
            comboBoxSelectPreset.FormattingEnabled = true;
            comboBoxSelectPreset.Location = new System.Drawing.Point(626, 56);
            comboBoxSelectPreset.Name = "comboBoxSelectPreset";
            comboBoxSelectPreset.Size = new System.Drawing.Size(162, 23);
            comboBoxSelectPreset.TabIndex = 0;
            comboBoxSelectPreset.SelectedIndexChanged += comboBoxSelectPreset_SelectedIndexChanged;
            // 
            // buttonApplyPreset
            // 
            buttonApplyPreset.Location = new System.Drawing.Point(626, 85);
            buttonApplyPreset.Name = "buttonApplyPreset";
            buttonApplyPreset.Size = new System.Drawing.Size(162, 23);
            buttonApplyPreset.TabIndex = 1;
            buttonApplyPreset.Text = "Apply";
            buttonApplyPreset.Click += buttonApplyPreset_Click;
            // 
            // buttonTeleport
            // 
            buttonTeleport.Location = new System.Drawing.Point(626, 415);
            buttonTeleport.Name = "buttonTeleport";
            buttonTeleport.Size = new System.Drawing.Size(162, 23);
            buttonTeleport.TabIndex = 2;
            buttonTeleport.Text = "Teleport";
            buttonTeleport.Click += buttonTeleport_Click;
            // 
            // textBoxResult
            // 
            textBoxResult.Location = new System.Drawing.Point(12, 56);
            textBoxResult.Multiline = true;
            textBoxResult.Name = "textBoxResult";
            textBoxResult.Size = new System.Drawing.Size(608, 23);
            textBoxResult.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 38);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(39, 15);
            label1.TabIndex = 4;
            label1.Text = "Result";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(749, 38);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(39, 15);
            label2.TabIndex = 5;
            label2.Text = "Preset";
            // 
            // textBoxState
            // 
            textState.Enabled = false;
            textState.Location = new System.Drawing.Point(12, 12);
            textState.Name = "textBoxState";
            textState.Size = new System.Drawing.Size(776, 23);
            textState.TabIndex = 6;
            textState.Text = "State";
            textState.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonSetRefPos
            // 
            buttonSetRefPos.AllowDrop = true;
            buttonSetRefPos.Location = new System.Drawing.Point(626, 386);
            buttonSetRefPos.Name = "buttonSetRefPos";
            buttonSetRefPos.Size = new System.Drawing.Size(162, 23);
            buttonSetRefPos.TabIndex = 7;
            buttonSetRefPos.Text = "Set Start";
            buttonSetRefPos.Click += buttonSetRefPos_Click;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(buttonSetRefPos);
            Controls.Add(textState);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBoxResult);
            Controls.Add(buttonTeleport);
            Controls.Add(buttonApplyPreset);
            Controls.Add(comboBoxSelectPreset);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "FormMain";
            Text = "eSTOL Training Tool";
            Load += FormMain_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ComboBox comboBoxSelectPreset;
        private System.Windows.Forms.Button buttonApplyPreset;
        private System.Windows.Forms.Button buttonTeleport;
        private System.Windows.Forms.TextBox textBoxResult;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textState;
        private System.Windows.Forms.Button buttonSetRefPos;
    }
}