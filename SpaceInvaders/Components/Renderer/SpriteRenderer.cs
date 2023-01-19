using SpaceInvaders.DataStructures;
using SpaceInvaders.Resources;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Security.Policy;
using System.Windows.Forms;

namespace SpaceInvaders.Components.Renderer
{
    internal class SpriteRenderer : PictureBox
    {
        public SpriteRenderer(Bitmap image, Vector2 pos) : base()
        {
            SizeMode = PictureBoxSizeMode.StretchImage;
            this.Image = image;
            this.Size = image.Size;
            this.Location = pos.ToPoint();
        }

        public void SetPosition(Vector2 pos)
        {
            this.Location = pos.ToPoint();
        }

        public void SetScale(Vector2 size)
        {
            this.Size = size.ToSize();
        }

        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            this.BackColor = Color.Transparent;
            paintEventArgs.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            paintEventArgs.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            base.OnPaint(paintEventArgs);
        }
    }
}