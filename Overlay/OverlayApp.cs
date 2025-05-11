using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OverlayApp
{
    public partial class OverlayForm : Form
    {
        private Timer animationTimer;
        private List<Rectangle> boxes;
        private int boxSpeed = 3;   // Speed of boxes (pixels per tick)
        private int boxSize = 25;   // Size of each box
        private int spacing = 40;   // Distance between boxes
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;


        public OverlayForm()
        {
            InitializeComponent();
            SetupOverlay();
            SetupAnimation();
            this.MouseDown += OverlayForm_MouseDown;
            this.MouseMove += OverlayForm_MouseMove;
            this.MouseUp += OverlayForm_MouseUp;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.BackColor = Color.DarkGray;
            this.TransparencyKey = Color.DarkGray;
            this.DoubleBuffered = true; // reduce flicker

            this.ResumeLayout(false);
        }

        private void SetupOverlay()
        {
            // Check if there is a second monitor
            if (Screen.AllScreens.Length > 1)
            {
                Screen secondScreen = Screen.AllScreens[1];

                int overlayWidth = 300;  // Only small overlay width
                int overlayHeight = 35;  // Small height

                this.Size = new Size(overlayWidth, overlayHeight);
                this.Location = new Point(
                    secondScreen.WorkingArea.Right - overlayWidth,
                    secondScreen.WorkingArea.Bottom - overlayHeight
                );
            }
            else
            {
                // Fallback to primary monitor if only one exists
                Screen primaryScreen = Screen.PrimaryScreen;

                int overlayWidth = 300;
                int overlayHeight = 35;

                this.Size = new Size(overlayWidth, overlayHeight);
                this.Location = new Point(
                    primaryScreen.WorkingArea.Right - overlayWidth,
                    primaryScreen.WorkingArea.Bottom - overlayHeight
                );
            }
        }



        private void SetupAnimation()
        {
            // Initialize boxes
            boxes = new List<Rectangle>();
            int numberOfBoxes = (this.Width / spacing) + 2;

            for (int i = 0; i < numberOfBoxes; i++)
            {
                boxes.Add(new Rectangle(i * spacing, (this.Height - boxSize) / 2, boxSize, boxSize));
            }

            // Setup animation timer
            animationTimer = new Timer();
            animationTimer.Interval = 30; // ~33 frames per second
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < boxes.Count; i++)
            {
                boxes[i] = new Rectangle(boxes[i].X + boxSpeed, boxes[i].Y, boxes[i].Width, boxes[i].Height);

                // Reset box to left when out of view
                if (boxes[i].X > this.Width)
                {
                    boxes[i] = new Rectangle(-boxSize, boxes[i].Y, boxes[i].Width, boxes[i].Height);
                }
            }

            this.Invalidate(); // Redraw
        }

        private void OverlayForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = true;
                dragCursorPoint = Cursor.Position;
                dragFormPoint = this.Location;
            }
        }

        private void OverlayForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(diff));
            }
        }

        private void OverlayForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = false;
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (Brush brush = new SolidBrush(Color.FromArgb(150, Color.Gray))) // semi-transparent gray boxes
            {
                foreach (var box in boxes)
                {
                    e.Graphics.FillRectangle(brush, box);
                }
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new OverlayForm());
        }
    }
}
