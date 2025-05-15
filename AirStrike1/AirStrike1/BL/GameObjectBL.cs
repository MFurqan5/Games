using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Helicopter
{
    internal class GameObjectBL
    {

        protected PictureBox PictureBox;

        public int health;
        public bool IsAlive { get; set; } = true;
        public string move_direction { get; set; }

        protected int moveSpeed = 3;

        public GameObjectBL()
        {

        }
        public GameObjectBL(Image image, int height, int width, int x, int y)
        {
            PictureBox = new PictureBox();
            PictureBox.Image = image;
            PictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            PictureBox.Height = height;
            PictureBox.Width = width;
            PictureBox.Location = new Point(x, y);
            PictureBox.BackColor = Color.Transparent;

        }
        public PictureBox GetPictureBox()
        {
            return PictureBox;
        }

        public virtual void Move(Keys key)
        {
            MessageBox.Show("Move method in ObjectBL called. This should be overridden in derived classes.");
        }


    }
}