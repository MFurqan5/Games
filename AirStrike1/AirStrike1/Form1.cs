using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Helicopter;

namespace AirStrike1
{
    public partial class Form1 : Form
    {
        List<Enemy> enemies = new List<Enemy>();
        Timer gameTimer = new Timer();
        Random random = new Random();
        int spawnCounter = 0;
        PictureBox helicopterPictureBox;
        PictureBox bulletPictureBox;

        // Image paths - set these to your actual paths
        private string enemyImagePath = @"C:\enemyPlane.gif";
        private string helicopterImagePath = @"C:\flying_helicopter.gif";
        private string bulletImagePath = @"C:\bullet.png";

        public Form1()
        {
            InitializeComponent();
            try
            {
                InitializeGameObjects();
                gameTimer.Interval = 30;
                gameTimer.Tick += GameTimer_Tick;
                gameTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Game initialization failed: {ex.Message}");
                this.Close();
            }
        }

        private void InitializeGameObjects()
        {
            // Create helicopter with safe loading
            if (!System.IO.File.Exists(helicopterImagePath))
                throw new Exception("Helicopter image not found");

            helicopterPictureBox = new PictureBox()
            {
                Image = Image.FromFile(helicopterImagePath),
                Size = new Size(80, 50),
                Location = new Point(100, 200),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent
            };
            this.Controls.Add(helicopterPictureBox);

            // Create bullet with safe loading
            if (!System.IO.File.Exists(bulletImagePath))
                throw new Exception("Bullet image not found");

            bulletPictureBox = new PictureBox()
            {
                Image = Image.FromFile(bulletImagePath),
                Size = new Size(20, 10),
                Location = new Point(150, 220),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false,
                BackColor = Color.Transparent
            };
            this.Controls.Add(bulletPictureBox);
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Move all enemies
                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    enemies[i].Move(Keys.None);

                    if (!enemies[i].IsAlive)
                    {
                        this.Controls.Remove(enemies[i].GetPictureBox());
                        enemies.RemoveAt(i);
                    }
                }

                // Spawn new enemies
                spawnCounter++;
                if (spawnCounter >= 20)
                {
                    SpawnEnemy();
                    spawnCounter = 0;
                }

                // Handle bullet movement
                if (bulletPictureBox.Visible)
                {
                    bulletPictureBox.Left += 10;
                    if (bulletPictureBox.Left > this.ClientSize.Width)
                    {
                        bulletPictureBox.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                gameTimer.Stop();
                MessageBox.Show($"Game error: {ex.Message}");
            }
        }

        private void SpawnEnemy()
        {
            try
            {
                if (!System.IO.File.Exists(enemyImagePath))
                    throw new Exception("Enemy image not found");

                Image enemyImage = Image.FromFile(enemyImagePath);

                // Safe random Y position calculation
                int minY = 50;
                int maxY = Math.Max(minY + 1, this.ClientSize.Height - 100);
                int randomY = random.Next(minY, maxY);

                Enemy newEnemy = new Enemy(
                    enemyImage,
                    height: 50,
                    width: 50,
                    x: this.ClientSize.Width,
                    y: randomY,
                    form: this,
                    helicopter: helicopterPictureBox,
                    bullet: bulletPictureBox
                );

                this.Controls.Add(newEnemy.GetPictureBox());
                enemies.Add(newEnemy);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Enemy spawn failed: {ex.Message}");
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                // Fire bullet when space is pressed
                if (e.KeyCode == Keys.Space && !bulletPictureBox.Visible)
                {
                    bulletPictureBox.Left = helicopterPictureBox.Right;
                    bulletPictureBox.Top = helicopterPictureBox.Top + 20;
                    bulletPictureBox.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Input error: {ex.Message}");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}