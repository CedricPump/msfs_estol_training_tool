using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using eSTOL_Training_Tool_Core.Core;

namespace eSTOL_Training_Tool_Core.UI
{
    public partial class FormUI : Form
    {
        delegate void SetResultTextCallback(string text);
        delegate void SetStatusTextCallback(string text);
        private readonly Controller controller;

        public FormUI(Controller controller)
        {
            InitializeComponent();
            this.controller = controller;
            textBoxUser.Text = controller.user;
            textBoxStatus.Text = "No Reference Position selected";
            buttonTeleport.Enabled = false;
            textBoxResult.Text = "Welcome\r\nSelect a eSTOL field preset or \"Open World\" mode set custom start.";
        }

        public void setPresets(string[] strings)
        {
            comboBoxPreset.Items.Add("Open World");
            comboBoxPreset.Items.AddRange(strings);
            comboBoxPreset.Text = "Open World";
        }

        public void setState(string stateStr)
        {
            if (this.textBoxStatus.InvokeRequired)
            {
                SetStatusTextCallback d = new SetStatusTextCallback(setState);
                this.Invoke(d, new object[] { stateStr });
                return;
            }
            textBoxStatus.Text = stateStr;
        }

        public void setResult(string resultStr)
        {

            if (this.textBoxResult.InvokeRequired)
            {
                SetResultTextCallback d = new SetResultTextCallback(setResult);
                this.Invoke(d, new object[] { resultStr });
                return;
            }
            textBoxResult.Text = resultStr;
        }

        private void buttonSetRefPos_Click(object sender, EventArgs e)
        {
            controller.SetStartPos();
            buttonTeleport.Enabled = controller.IsStilInit();
        }

        private void buttonApplyPreset_Click(object sender, EventArgs e)
        {
            string presetStr = comboBoxPreset.Text;
            controller.SetPreset(presetStr);
            if (presetStr != "Open World")
            {
                buttonTeleport.Enabled = true;
                buttonSetRefPos.Enabled = false;
                textBoxResult.Text = $"Preset selected: {comboBoxPreset.Text}.\nTeleport to reference line?";
            }
            else
            {
                textBoxResult.Text = $"\"Open World\" Mode selected.";
                buttonTeleport.Enabled = controller.IsStilInit();
                buttonSetRefPos.Enabled = true;
            }
        }

        private void buttonTeleport_Click(object sender, EventArgs e)
        {
            controller.TeleportToReferenceLine();
            textBoxResult.Text = "";
        }

        private void textBoxUser_TextChanged(object sender, EventArgs e)
        {
            // controller.SetUser(textBoxUser.Text);
        }

        private void textBoxUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                controller.SetUser(textBoxUser.Text);
                textBoxResult.Text = $"User {textBoxUser.Text} saved";
            }
        }

        private void buttonCreatePreset_Click(object sender, EventArgs e)
        {
            textBoxResult.Text = controller.createPreset();
        }
    }
}
