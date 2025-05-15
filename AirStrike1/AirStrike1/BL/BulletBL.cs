using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AirStrike1.BL
{
    internal class BulletBL:GameObjectBL
    {
        public BulletBL(int x, int y, Direction direction)
           : base(Image.FromFile("D:\\visual studio\\gameimage\\bullet.png"),
                  height: 10,
                  width: 20,
                  x: x,
                  y: y)
        {
            this.moveSpeed = 20;
            this.direction = direction;
            this.GetPictureBox().BackColor = Color.Transparent;
        }

        public void Move(Keys key = Keys.None)
        {
            if (!IsAlive) return;

            Point newPos = GetPictureBox().Location;

            switch (direction)
            {
                case Direction.Right:
                    newPos.X += moveSpeed;
                    break;
                case Direction.Left:
                    newPos.X -= moveSpeed;
                    break;
                case Direction.Up:
                    newPos.Y -= moveSpeed;
                    break;
                case Direction.Down:
                    newPos.Y += moveSpeed;
                    break;
            }

            GetPictureBox().Location = newPos;
        }
    }
}

