using System;
using System.Drawing;
using System.Windows.Forms;

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

            if (PictureBox == null)
                throw new Exception("Enemy picture box not initialized");

            PictureBox.BackColor = Color.Transparent;
            gameForm.Controls.Add(PictureBox);
        }

        public override void Move(Keys key)
        {
            try
            {
                if (PictureBox == null) return;

                // Move enemy left
                PictureBox.Left -= moveSpeed;

                // Check bullet collision
                if (bulletBox != null && bulletBox.Visible &&
                    PictureBox.Bounds.IntersectsWith(bulletBox.Bounds))
                {
                    bulletBox.Visible = false;
                    CleanUp();
                    return;
                }

                // Check helicopter collision
                if (helicopterBox != null && !helicopterBox.IsDisposed &&
                    PictureBox.Bounds.IntersectsWith(helicopterBox.Bounds))
                {
                    gameForm.Invoke((MethodInvoker)delegate {
                        gameForm.Controls.Remove(helicopterBox);
                        helicopterBox.Dispose();
                        CleanUp();
                        MessageBox.Show("Game Over!");
                        Application.Exit();
                    });
                    return;
                }

                // Remove when off-screen
                if (PictureBox.Right < 0)
                {
                    CleanUp();
                }
            }
            catch (Exception ex)
            {
                // Silent fail for enemy movement to prevent game crash
                CleanUp();
            }
        }

        private void CleanUp()
        {
            try
            {
                if (PictureBox != null && !PictureBox.IsDisposed)
                {
                    gameForm.Controls.Remove(PictureBox);
                    PictureBox.Dispose();
                }
                IsAlive = false;
            }
            catch { /* Ensure cleanup doesn't throw */ }
        }
    }
}