using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using eSTOL_Training_Tool_Core.Core;
using static System.Net.Mime.MediaTypeNames;

namespace eSTOL_Training_Tool_Core.UI
{
    public partial class FormMain : Form
    {
        delegate void SetTextCallback(string text);
        public Controller controller;

        public FormMain()
        {
            InitializeComponent();
        }

        private void buttonApplyPreset_Click(object sender, EventArgs e)
        {

        }

        private void buttonTeleport_Click(object sender, EventArgs e)
        {

        }

        public void setPresets(string[] strings)
        {
            comboBoxSelectPreset.Items.Add("Open World");
            comboBoxSelectPreset.Items.AddRange(strings);
            comboBoxSelectPreset.Text = "Open World";
        }

        public void setState(string stateStr)
        {
            if (this.textState.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setState);
                this.Invoke(d, new object[] { stateStr });
                return;
            }
            textState.Text = stateStr;
        }

        public void setResult(string resultStr)
        {
            
            if (this.textBoxResult.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setState);
                this.Invoke(d, new object[] { resultStr });
                return;
            }
            textBoxResult.Text = resultStr;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private void comboBoxSelectPreset_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonSetRefPos_Click(object sender, EventArgs e)
        {

        }
    }
}
