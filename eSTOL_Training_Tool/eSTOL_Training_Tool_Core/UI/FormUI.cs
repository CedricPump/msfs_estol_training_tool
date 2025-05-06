using System;
using System.Device.Location;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reactive;
using System.Windows.Forms;
using eSTOL_Training_Tool;
using eSTOL_Training_Tool_Core.Core;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace eSTOL_Training_Tool_Core.UI
{
    public partial class FormUI : Form
    {
        delegate void SetResultTextCallback(string text);
        delegate void SetStatusTextCallback(string text);
        private readonly Controller controller;
        private STOLResult? result = null;
        private TimeSpan StopwatchOffset = TimeSpan.Zero;
        System.Diagnostics.Stopwatch stopwatch;
        private bool alwaysontop = false;
        private int stopwatchOffsetSeconds = 30;
        private bool debug = false;
        private object drawing = new object();

        // stol ref
        private GeoCoordinate InitailPos = null;
        private GeoCoordinate PlanePos = null;
        private double InitialHeading = 0.0;

        private double WindDir = 0.0;
        public double Wind = 0.0;

        private int fieldSizeFull = 600;
        private int fieldSizeZoom = 220;
        private int selectedFieldSize = 600;

        public FormUI(Controller controller)
        {
            InitializeComponent();
            var config = Config.GetInstance();
            alwaysontop = config.alwaysOnTop;
            this.checkBoxOntop.Checked = alwaysontop;
            this.TopMost = alwaysontop;

            this.Text = "eSTOL Training Tool " + VersionHelper.GetVersion();

            this.controller = controller;
            textBoxUser.Text = controller.user;
            textBoxStatus.Text = "No Reference Position selected";
            buttonTeleport.Enabled = false;
            textBoxResult.Text = "Welcome\r\nSelect a eSTOL field preset or \"Open World\" mode set custom start.";

            comboBoxUnit.Items.Add("feet");
            comboBoxUnit.Items.Add("meters");
            comboBoxUnit.Items.Add("yard");
            comboBoxUnit.Text = config.Unit;
            // comboBoxUnit.Items.Add("yard");

            this.stopwatch = new System.Diagnostics.Stopwatch();
            this.progressBarStopwatch.Minimum = 0;
            this.progressBarStopwatch.Maximum = 180;

            this.checkBoxResult.Checked = config.isSendResults;
            this.checkBoxTelemetry.Checked = config.isSendTelemetry;

            this.numericUpDownStopwatchOffest.Value = stopwatchOffsetSeconds;
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

        private void panel_Resize(object sender, EventArgs e)
        {
            panel.Invalidate(); // Triggers Paint event
        }

        private void panel_Paint(object sender, PaintEventArgs e)
        {
            lock (drawing)
            {
                Graphics g = e.Graphics;
                g.Clear(Color.Transparent); // Background green
                g.DrawImage(panel.BackgroundImage, new Rectangle(0, 0, this.Width, this.Height));

                // Canvas size
                int canvasWidth = e.ClipRectangle.Width;
                int canvasHeight = e.ClipRectangle.Height;

                // Logical runway size
                int logicalFieldWidth = 140;
                int logicalFieldLength = selectedFieldSize;

                // Independent scaling
                float scaleX = (float)canvasWidth / logicalFieldWidth;
                float scaleY = (float)canvasHeight / logicalFieldLength;

                // Pens
                using Pen whitePen = new Pen(Color.White, 1);
                using Pen redPen = new Pen(Color.Red, 2);
                using Pen bluePen = new Pen(Color.Blue, 3);
                using Pen orangePen = new Pen(Color.Orange, 3);
                using Pen yellowPen = new Pen(Color.Yellow, 3);
                using Pen blackPen = new Pen(Color.Black, 5);

                // Draw horizontal marker lines every 10feet
                for (int i = 10; i <= logicalFieldLength; i += 10)
                {
                    float y = canvasHeight - i * scaleY;
                    Pen pen = (i % 100 == 0) ? redPen : whitePen;
                    g.DrawLine(pen, 0, y, canvasWidth, y);
                }

                if (this.PlanePos != null)
                {
                    (double planeDist_y, double planeOffset_x) = GeoUtils.GetDistanceAlongAxis(this.InitailPos, this.PlanePos, this.InitialHeading);
                    float planeDist = (float)planeDist_y * scaleY * 3.28084f;
                    float planeOff = (float)planeOffset_x * scaleX * 3.28084f;

                    Console.WriteLine(planeOff);

                    g.DrawEllipse(blackPen, canvasWidth / 2f + planeOff, canvasHeight - planeDist, 1, 1);
                }

                // Result-dependent lines
                if (result != null)
                {
                    // lets make the distance lines more precise
                    // result.InitialPosition; // start position on bottom field Line
                    // result.TakeoffPosition;
                    // result.TouchdownPosition;
                    // result.StopPosition;

                    (double toDist_y, double toOffset_x) = GeoUtils.GetDistanceAlongAxis(result.InitialPosition, result.TakeoffPosition, result.InitialHeading);
                    (double tdDist_y, double tdOffset_x) = GeoUtils.GetDistanceAlongAxis(result.InitialPosition, result.TouchdownPosition, result.InitialHeading);
                    (double stopDist_y, double stopOffset_x) = GeoUtils.GetDistanceAlongAxis(result.InitialPosition, result.StopPosition, result.InitialHeading);

                    float toDist = (float)toDist_y * scaleY * 3.28084f;
                    float toOff = (float)toOffset_x * scaleX * 3.28084f;

                    float tdDist = (float)tdDist_y * scaleY * 3.28084f;
                    float tdOff = (float)tdOffset_x * scaleX * 3.28084f;

                    float stopDist = (float)stopDist_y * scaleY * 3.28084f;
                    float stopOff = (float)stopOffset_x * scaleX * 3.28084f;

                    // Takeoff line
                    g.DrawLine(bluePen, canvasWidth / 2f + toOff, canvasHeight, canvasWidth / 2f + toOff, canvasHeight - toDist);
                    // Touchdown line
                    g.DrawLine(yellowPen, canvasWidth / 2f + tdOff, canvasHeight, canvasWidth / 2f + tdOff, canvasHeight - tdDist);
                    // Stop line
                    g.DrawLine(orangePen, canvasWidth / 2f + tdOff, canvasHeight - tdDist, canvasWidth / 2f + stopOff, canvasHeight - stopDist);

                    // Dots
                    g.DrawEllipse(blackPen, canvasWidth / 2f + toOff, canvasHeight - toDist, 1, 1);
                    g.DrawEllipse(blackPen, canvasWidth / 2f + tdOff, canvasHeight - tdDist, 1, 1);
                    g.DrawEllipse(blackPen, canvasWidth / 2f + stopOff, canvasHeight - stopDist, 1, 1);

                    // Labels
                    using Font drawFont = new Font("Arial", 9, FontStyle.Bold);
                    using Brush drawBrush = new SolidBrush(Color.Black);

                    g.DrawString("Takeoff", drawFont, drawBrush, canvasWidth / 2f + toOff - 50, canvasHeight - toDist - 15);
                    g.DrawString("Touchdown", drawFont, drawBrush, canvasWidth / 2f + tdOff + 5, canvasHeight - tdDist - 15);
                    g.DrawString("Stop", drawFont, drawBrush, canvasWidth / 2f + stopOff + 5, canvasHeight - stopDist - 15);
                }

                // Runway border stretched to full canvas
                using Pen borderPen = new Pen(Color.LightGray, 5);
                g.DrawRectangle(borderPen, 0, 0, canvasWidth, canvasHeight);
            }
        }

        void pannel_DoubleClick( object sender, EventArgs e) 
        {
            if (selectedFieldSize == fieldSizeFull) selectedFieldSize = fieldSizeZoom;
            else if (selectedFieldSize == fieldSizeZoom) selectedFieldSize = fieldSizeFull;
            panel.Invalidate();
        }

        private void comboBoxUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            string unit = comboBoxUnit.Text;
            var config = Config.GetInstance();
            config.Unit = unit;
            config.Save();
            controller.setUnit(unit);
        }

        private void checkBoxResult_CheckedChanged(object sender, EventArgs e)
        {
            var config = Config.GetInstance();
            config.isSendResults = checkBoxResult.Checked;
            if (config.isSendResults) MessageBox.Show("By enabeling, you agree that your landing result data will be temporarily stored for up to 30 days and may be shown on a public dashboard.\n" +
                "For more information, see the privacy policy: https://github.com/CedricPump/msfs_estol_training_tool/blob/main/doc/Privacy_Policy.md");
            config.Save();
        }

        private void checkBoxTelemetry_CheckedChanged(object sender, EventArgs e)
        {
            var config = Config.GetInstance();
            config.isSendTelemetry = checkBoxTelemetry.Checked;
            if (config.isSendTelemetry) MessageBox.Show("By enabeling, you agree that your ingame telemetry data will be temporarily stored for up to 30 days and may be shown on a public dashboard.\n" +
                "For more information, see the privacy policy: https://github.com/CedricPump/msfs_estol_training_tool/blob/main/doc/Privacy_Policy.md");
            config.Save();
        }

        private void Timer(object sender, EventArgs e)
        {
            // just to check it periodically
            this.TopMost = alwaysontop;

            TimeSpan elapsed = this.stopwatch.Elapsed + StopwatchOffset;
            string minus = elapsed.TotalSeconds < 0 ? "-" : " ";
            labelStopwatch.Text = string.Format("{0}{1:00}:{2:00}", minus, elapsed.Minutes, Math.Abs(elapsed.Seconds));
            if (elapsed.TotalSeconds <= 180)
            {
                if (elapsed.TotalSeconds >= 0)
                {
                    this.progressBarStopwatch.Value = (int)elapsed.TotalSeconds;
                }
                else
                {
                    this.progressBarStopwatch.Value = 0;
                }
            }

        }

        public void StartStopWatch()
        {
            if (!this.stopwatch.IsRunning)
            {
                this.stopwatch.Reset();
                this.StopwatchOffset = TimeSpan.Zero;
                this.stopwatch.Start();
            }
        }

        public void StopStopWatch()
        {
            this.stopwatch.Stop();
        }

        public void ResetStopWatch()
        {
            this.stopwatch.Reset();
        }

        private void buttonStartStopwatch_Click(object sender, EventArgs e)
        {
            this.StopwatchOffset = TimeSpan.FromSeconds(-stopwatchOffsetSeconds);
            this.stopwatch.Restart();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            this.StopwatchOffset = TimeSpan.Zero;
            this.stopwatch.Restart();
        }

        private void checkBoxOntop_CheckedChanged(object sender, EventArgs e)
        {
            this.alwaysontop = checkBoxOntop.Checked;
            this.TopMost = this.alwaysontop;
            var config = Config.GetInstance();
            config.alwaysOnTop = this.alwaysontop;
            config.Save();

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.stopwatchOffsetSeconds = (int)numericUpDownStopwatchOffest.Value;
        }

        private void labelPreset_Click(object sender, EventArgs e)
        {
            if (debug)
            {
                this.controller.reloadPreset();
            }
        }

        private void checkBoxDebugging_CheckedChanged(object sender, EventArgs e)
        {
            this.debug = checkBoxDebugging.Checked;
            Config.GetInstance().debug = this.debug;
        }

        internal void setPlanePos(GeoCoordinate initialPosition, double initialHeading, GeoCoordinate position)
        {
            this.InitailPos = initialPosition;
            this.InitialHeading = initialHeading;
            this.PlanePos = position;
            panel.Invalidate();
        }

        public void setWind(double winddir, double wind)
        {
            this.WindDir = winddir;
            this.Wind = wind;
            panel1.Invalidate();
        }

        private void panelWind_Paint(object sender, PaintEventArgs e)
        {
            labelWind.Text = $"{this.Wind,4:0.0}";

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.WhiteSmoke);

            AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 10); // width, height
            Pen arrowPen = new Pen(Color.Black, 2)
            {
                CustomEndCap = bigArrow
            };

            // Draw wind compass circle
            g.DrawEllipse(Pens.Black, 2, 2, 60, 60);

            // Convert to canvas angle (0° = up, rotate clockwise)
            double windDirTo = (WindDir + 180) % 360;
            double angleRad = (windDirTo - 90) * Math.PI / 180.0;

            float centerX = 32;
            float centerY = 32;
            float length = 25;

            // Calculate start and end points so arrow pivots around center
            float dx = (float)(Math.Cos(angleRad) * length);
            float dy = (float)(Math.Sin(angleRad) * length);

            PointF start = new PointF(centerX + dx, centerY - dy); // tail
            PointF end = new PointF(centerX - dx, centerY + dy);   // arrowhead

            g.DrawLine(arrowPen,  start, end);
        }


        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
