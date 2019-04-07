using System;
using System.Drawing;
using System.Windows.Forms;

namespace Do_An_1
{
    class CustomPictureBox : PictureBox
    {
        private bool isHighlight = false;
        private bool isDown = false;
        private Point MousePoint;
        public EventHandler MouseUpCheckWin;
        public EventHandler MouseUpAutoLocation;

        public void SetHighlight(bool value)
        {
            isHighlight = value;
            Invalidate();
        }
        public CustomPictureBox(Bitmap _Img)
        {
            Image = _Img;
            Size = _Img.Size;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            if (isHighlight)
            {
                pe.Graphics.DrawRectangle(new Pen(Color.Red, 6), 0, 0, Width, Height);
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            isDown = true;
            isHighlight = true;
            MousePoint = e.Location;
            BringToFront();
            Invalidate();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isDown)
            {
                Top += e.Y - MousePoint.Y;
                Left += e.X - MousePoint.X;
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (isDown)
            {
                MouseUpAutoLocation?.Invoke(this, e);
            }
            if (isHighlight)
            {
                Invalidate();
            }
            isDown = false;
            isHighlight = false;
            MouseUpCheckWin?.Invoke(this, e);
        }
    }
}
