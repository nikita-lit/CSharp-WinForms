using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace WinForms
{
    public class PictureView : PictureBox
    {
        private bool _isPixelated = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPixelated
        {
            get { return _isPixelated; }
            set
            {
                _isPixelated = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (IsPixelated)
            {
                pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            }
            else
            {
                pe.Graphics.InterpolationMode = InterpolationMode.Default;
                pe.Graphics.PixelOffsetMode = PixelOffsetMode.Default;
            }

            base.OnPaint(pe);
        }
    }
}
