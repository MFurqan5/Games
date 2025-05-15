using System;
using System.Drawing;
using System.Windows.Forms;

namespace AirStrike1.BL
{
    internal class PlayerBL : GameObjectBL
    {
        private const int MaxX = 1450;
        private const int MaxY = 1000;

        public PlayerBL(int height = 200, int width = 200, int x = 200, int y = 100)
           : base(height, width, x, y)
        {
            Object.Image = Image.FromFile("D:\\visual studio\\gameimage\\flying_helicopter.gif");
            health = 100;
        }

        public BulletBL Fire()
        {
            int bulletX = this.GetPictureBox().Right - 54;
            int bulletY = this.GetPictureBox().Top + (this.GetPictureBox().Height / 2) + 20;
            return new BulletBL(bulletX, bulletY, Direction.Right);
        }

        public void Move(Keys key)
        {
            Point newLocation = Object.Location;
            int moveDistance = moveSpeed;
            int objectWidth = GetPictureBox().Width;
            int objectHeight = GetPictureBox().Height;

            switch (key)
            {
                case Keys.Up:
                    newLocation.Y = Math.Max(-50, (newLocation.Y - moveDistance));
                    direction = Direction.Up;
                    break;
                case Keys.Down:
                    newLocation.Y = Math.Min(MaxY - objectHeight, newLocation.Y + moveDistance);
                    direction = Direction.Down;
                    break;
                case Keys.Left:
                    newLocation.X = Math.Max(0, newLocation.X - moveDistance);
                    direction = Direction.Left;
                    break;
                case Keys.Right:
                    newLocation.X = Math.Min(MaxX - objectWidth, newLocation.X + moveDistance);
                    direction = Direction.Right;
                    break;
            }

            // Final boundary check
            newLocation.X = Clamp(newLocation.X, -50, MaxX - objectWidth);
            newLocation.Y = Clamp(newLocation.Y, -20, MaxY - objectHeight);

            Object.Location = newLocation;
        }

        private int Clamp(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        public void IncreaseScore(int points)
        {
            //Score += points;
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                GetPictureBox().Visible = false;
            }
        }
    }
}