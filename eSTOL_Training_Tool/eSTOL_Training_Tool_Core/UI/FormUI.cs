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
using eSTOL_Training_Tool;
using eSTOL_Training_Tool_Core.Core;

namespace eSTOL_Training_Tool_Core.UI
{
    public partial class FormUI : Form
    {
        delegate void SetResultTextCallback(string text);
        delegate void SetStatusTextCallback(string text);
        private readonly Controller controller;
        private STOLResult? result = null;

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

        public void DrawResult(STOLResult result)
        {
            this.result = result;
            panel.Invalidate(); // Redraw the panel
            panel.BackColor = Color.FromArgb(0, 128, 0); // Dark Green
        }

        private void panel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.FromArgb(0, 128, 0));
            Graphics g = e.Graphics;

            int fieldStart = 5;
            int fieldWidth = 250;
            int fieldLength = 600;

            // Create a pen (color, thickness)
            Pen whitePen = new Pen(Color.White, 3);
            Pen redPen = new Pen(Color.Red, 3);
            Pen BluePen = new Pen(Color.Blue, 3);
            Pen orangePen = new Pen(Color.Orange, 3);
            Pen yellowPen = new Pen(Color.Yellow, 3);
            Pen blackPen = new Pen(Color.Black, 5);

            whitePen = new Pen(Color.White, 3);
            g.DrawLine(whitePen, 5, 25, 255, 25);
            g.DrawLine(whitePen, 5, 45, 255, 45);
            g.DrawLine(whitePen, 5, 65, 255, 65);
            g.DrawLine(whitePen, 5, 85, 255, 85);
            g.DrawLine(redPen, 5, 105, 255, 105);
            g.DrawLine(whitePen, 5, 125, 255, 125);
            g.DrawLine(whitePen, 5, 145, 255, 145);
            g.DrawLine(whitePen, 5, 165, 255, 165);
            g.DrawLine(whitePen, 5, 185, 255, 185);
            g.DrawLine(redPen, 5, 205, 255, 205);
            g.DrawLine(whitePen, 5, 225, 255, 225);
            g.DrawLine(whitePen, 5, 245, 255, 245);
            g.DrawLine(whitePen, 5, 265, 255, 265);
            g.DrawLine(whitePen, 5, 285, 255, 285);
            g.DrawLine(redPen, 5, 305, 255, 305);
            g.DrawLine(whitePen, 5, 325, 255, 325);
            g.DrawLine(whitePen, 5, 345, 255, 345);
            g.DrawLine(whitePen, 5, 365, 255, 365);
            g.DrawLine(whitePen, 5, 385, 255, 385);
            g.DrawLine(redPen, 5, 405, 255, 405);
            g.DrawLine(whitePen, 5, 425, 255, 425);
            g.DrawLine(whitePen, 5, 445, 255, 445);
            g.DrawLine(whitePen, 5, 465, 255, 465);
            g.DrawLine(whitePen, 5, 485, 255, 485);
            g.DrawLine(redPen, 5, 505, 255, 505);
            g.DrawLine(whitePen, 5, 525, 255, 525);
            g.DrawLine(whitePen, 5, 545, 255, 545);
            g.DrawLine(whitePen, 5, 565, 255, 565);
            g.DrawLine(whitePen, 5, 585, 255, 585);

            if(result != null) 
            {
                var toDist = (int) Math.Round(result.Takeoffdist);
                var tdDist = (int)Math.Round(result.Touchdowndist);
                var stopDist = (int)Math.Round(result.Landingdist);

                // Takeoff
                g.DrawLine(BluePen, fieldStart + fieldWidth / 2 - 3, fieldStart + fieldLength, fieldStart + fieldWidth / 2 - 3, fieldStart + fieldLength - toDist);

                // touchdown
                g.DrawLine(yellowPen, fieldStart + fieldWidth / 2 + 3, fieldStart + fieldLength, fieldStart + fieldWidth / 2 + 3, fieldStart + fieldLength - tdDist);

                // stop
                g.DrawLine(orangePen, fieldStart + fieldWidth / 2 + 3, fieldStart + fieldLength - tdDist, fieldStart + fieldWidth / 2 + 3, fieldStart + fieldLength - stopDist);

                g.DrawEllipse(blackPen, fieldStart + fieldWidth / 2 - 3, fieldStart + fieldLength - toDist, 1, 1);
                g.DrawEllipse(blackPen, fieldStart + fieldWidth / 2 + 3, fieldStart + fieldLength - tdDist, 1, 1);
                g.DrawEllipse(blackPen, fieldStart + fieldWidth / 2 + 3, fieldStart + fieldLength - stopDist, 1, 1);




                // Create a font and brush
                Font drawFont = new Font("Arial", 9, FontStyle.Bold);
                Brush drawBrush = new SolidBrush(Color.Black);


                PointF drawPoint = new PointF(fieldStart + fieldWidth / 2 - 50, fieldStart + fieldLength - (toDist+5));
                e.Graphics.DrawString("Takeoff", drawFont, drawBrush, drawPoint);
                drawPoint = new PointF(fieldStart + fieldWidth / 2 + 10, fieldStart + fieldLength - (tdDist + 5));
                e.Graphics.DrawString("Tochdown", drawFont, drawBrush, drawPoint);
                drawPoint = new PointF(fieldStart + fieldWidth / 2 + 10, fieldStart + fieldLength - (stopDist + 5));
                e.Graphics.DrawString("Stop", drawFont, drawBrush, drawPoint);
            }

            // Draw borders
            whitePen = new Pen(Color.White, 5);
            g.DrawRectangle(whitePen, fieldStart, fieldStart, fieldWidth, fieldLength);

            // Dispose of the pen to free resources
            whitePen.Dispose();
            redPen.Dispose();
        }
    }
}
