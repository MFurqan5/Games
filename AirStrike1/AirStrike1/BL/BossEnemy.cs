using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AirStrike1.BL
{
    internal class BossEnemy : GameObjectBL
    {
        private Form gameForm;
        private int moveSpeed = 3;
        private int health = 5; // Boss health
        private Timer shootTimer;
        private PlayerBL player;
        private List<BulletBL> bossBullets = new List<BulletBL>();
        private Random random = new Random();

        // Expose health property
        public int Health { get { return health; } }

        public BossEnemy(Image image, int height, int width, int x, int y, Form form, PlayerBL player)
            : base(image, height, width, x, y)
        {
            gameForm = form;
            this.player = player;
            GetPictureBox().BackColor = Color.Transparent;

            shootTimer = new Timer();
            shootTimer.Interval = 2000;
            shootTimer.Tick += ShootAtPlayer;
            shootTimer.Start();
        }

        public void Move()
        {
            var bossBox = GetPictureBox();
            if (bossBox == null) return;

            if (bossBox.Top <= 0 || bossBox.Bottom >= gameForm.ClientSize.Height)
            {
                moveSpeed = -moveSpeed;
            }
            bossBox.Top += moveSpeed;
        }

        private void ShootAtPlayer(object sender, EventArgs e)
        {
            if (!IsAlive) return;

            int bulletX = GetPictureBox().Left - 20;
            int bulletY = GetPictureBox().Top + (GetPictureBox().Height / 2);

            BulletBL newBullet = new BulletBL(bulletX, bulletY, Direction.Left);
            bossBullets.Add(newBullet);
            gameForm.Controls.Add(newBullet.GetPictureBox());
            newBullet.GetPictureBox().BringToFront();
        }

        public void UpdateBullets()
        {
            for (int i = bossBullets.Count - 1; i >= 0; i--)
            {
                bossBullets[i].Move();

                if (bossBullets[i].GetPictureBox().Bounds.IntersectsWith(player.GetPictureBox().Bounds))
                {
                    player.TakeDamage(1);
                    bossBullets[i].setIsAlive(false);
                }

                if (bossBullets[i].GetPictureBox().Right < 0 || !bossBullets[i].IsAlive)
                {
                    gameForm.Controls.Remove(bossBullets[i].GetPictureBox());
                    bossBullets.RemoveAt(i);
                }
            }
        }

        public void TakeDamage()
        {
            health--;

            // Show visual feedback when taking damage
            GetPictureBox().BackColor = Color.Red;

            if (health <= 0)
            {
                IsAlive = false;
                shootTimer.Stop();
                GetPictureBox().Visible = false;
            }
        }
    }
}