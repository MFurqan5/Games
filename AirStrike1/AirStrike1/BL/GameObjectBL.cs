using System.Drawing;
using System.Windows.Forms;

namespace AirStrike1.BL
{
    internal class GameObjectBL
    {
        protected PictureBox Object;

        public int health;
        public bool IsAlive { get; set; } = true;
        protected Direction direction { get; set; }

        protected int moveSpeed = 3;

        public GameObjectBL()
        {
        }

        public GameObjectBL(Image image, int height, int width, int x, int y)
        {
            Object = new PictureBox();
            Object.Image = image;
            Object.SizeMode = PictureBoxSizeMode.StretchImage;
            Object.Height = height;
            Object.Width = width;
            Object.Location = new Point(x, y);
            Object.BackColor = Color.Transparent;
        }

        public GameObjectBL(int height, int width, int x, int y)
        {
            Object = new PictureBox();
            Object.SizeMode = PictureBoxSizeMode.StretchImage;
            Object.Height = height;
            Object.Width = width;
            Object.Location = new Point(x, y);
            Object.BackColor = Color.Transparent;
        }

        public PictureBox GetPictureBox()
        {
            return Object;
        }

        public void setIsAlive(bool isAlive)
        {
            IsAlive = isAlive;
        }
    }

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down,
        None
    }
}