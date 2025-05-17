using System;
using System.Drawing;
using System.Windows.Forms;

namespace AirStrike1.BL
{
    internal class Enemy : GameObjectBL
    {
        private Form gameForm;
        private int moveSpeed = 5;

        public Enemy(Image image, int height, int width, int x, int y, Form form)
            : base(image, height, width, x, y)
        {
            gameForm = form;
            GetPictureBox().BackColor = Color.Transparent;
        }

        public void Move(Keys key)
        {
            var enemyBox = GetPictureBox();
            if (enemyBox == null) return;

            enemyBox.Left -= moveSpeed;

            if (enemyBox.Right < 0)
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
            catch { }
        }
    }
}