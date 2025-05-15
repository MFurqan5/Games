using System;
using System.Drawing;
using System.Windows.Forms;
using AirStrike1.BL;

namespace Helicopter
{
    internal class Enemy : GameObjectBL
    {
        private Form gameForm;
        private int moveSpeed = 5;
        private PictureBox helicopterBox;
        private PictureBox bulletBox;

        public Enemy(Image image, int height, int width, int x, int y, Form form, PictureBox helicopter, PictureBox bullet)
            : base(image, height, width, x, y)
        {
            gameForm = form ?? throw new ArgumentNullException(nameof(form));
            helicopterBox = helicopter;
            bulletBox = bullet;

            if (GetPictureBox() == null)
                throw new Exception("Enemy picture box not initialized");

            GetPictureBox().BackColor = Color.Transparent;
            gameForm.Controls.Add(GetPictureBox());
        }
        public Enemy(Image image, int height, int width, int x, int y, Form form)
        {
            // Create the enemy PictureBox
            Object = new PictureBox()
            {
                Image = image,
                Size = new Size(width, height),
                Location = new Point(x, y),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent
            };
            form.Controls.Add(GetPictureBox());
        }
        public void Move(Keys key)
        {
            try
            {
                var enemyBox = GetPictureBox();
                if (enemyBox == null) return;

                // Move enemy left
                enemyBox.Left -= moveSpeed;

                // Check bullet collision
                if (bulletBox != null && bulletBox.Visible &&
                    enemyBox.Bounds.IntersectsWith(bulletBox.Bounds))
                {
                    bulletBox.Visible = false;
                    CleanUp();
                    return;
                }

                // Remove when off-screen
                if (enemyBox.Right < 0)
                {
                    CleanUp();
                }
            }
            catch (Exception)
            {
                CleanUp();
            }
        }

        private void CleanUp()
        {
            try
            {
                var enemyBox = GetPictureBox();
                if (enemyBox != null && !enemyBox.IsDisposed)
                {
                    enemyBox.Dispose();
                }
                setIsAlive(false);
            }
            catch { /* Ensure cleanup doesn't throw */ }
        }
    }
}
