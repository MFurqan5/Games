using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AirStrike1.BL;
using Helicopter;

namespace AirStrike1
{
    public partial class Form1 : Form
    {
        // Player and movement related
        private PlayerBL player;
        private HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private List<BulletBL> activeBullets = new List<BulletBL>();
        private int bulletCooldown = 0;
        private const int FIRE_RATE = 5;

        // Enemy related
        private List<Enemy> enemies = new List<Enemy>();
        private Timer spawnTimer = new Timer();
        private Random random = new Random();
        private int spawnCounter = 0;

        // Game assets paths
        private string enemyImagePath = @"C:\Users\User\Desktop\REPOS\Games\AirStrike1\AirStrike1\Resources\enemyPlane.gif";
        private string helicopterImagePath = @"C:\Users\User\Desktop\REPOS\Games\AirStrike1\AirStrike1\Resources\flying_helicopter.gif";
        private string bulletImagePath = @"C:\Users\User\Desktop\REPOS\Games\AirStrike1\AirStrike1\Resources\bullet.png";

        // Game timer
        private Timer gameTimer;

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            try
            {
                // Setup form
                this.DoubleBuffered = true;
                this.ClientSize = new Size(1000, 600);
                this.KeyPreview = true;

                // Create player
                if (!System.IO.File.Exists(helicopterImagePath))
                    throw new Exception("Helicopter image not found");

                player = new PlayerBL(
                    
                    height: 200,
                    width: 200,
                    x: 100,
                    y: this.ClientSize.Height / 2 - 30
                );

                this.Controls.Add(player.GetPictureBox());
                player.GetPictureBox().BringToFront();

                // Setup timers
                gameTimer = new Timer();
                gameTimer.Interval = 16; // ~60 FPS
                gameTimer.Tick += GameUpdate;
                gameTimer.Start();

                spawnTimer.Interval = 1000; // Spawn enemies every second
                spawnTimer.Tick += SpawnTimer_Tick;
                spawnTimer.Start();

                // Event handlers
                this.KeyDown += OnKeyDown;
                this.KeyUp += OnKeyUp;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Game initialization failed: {ex.Message}");
                this.Close();
            }
        }

        private void GameUpdate(object sender, EventArgs e)
        {
            try
            {
                // Player movement
                foreach (Keys key in pressedKeys)
                {
                    player.Move(key);
                }

                // Bullet cooldown
                if (bulletCooldown > 0) bulletCooldown--;

                // Update bullets
                for (int i = activeBullets.Count - 1; i >= 0; i--)
                {
                    activeBullets[i].Move();

                    // Check for bullet-enemy collisions
                    foreach (var enemy in enemies)
                    {
                        if (activeBullets[i].GetPictureBox().Bounds.IntersectsWith(enemy.GetPictureBox().Bounds))
                        {
                            enemy.setIsAlive(false);
                            activeBullets[i].setIsAlive((false));
                            break;
                        }
                    }

                    // Remove off-screen bullets or bullets that hit enemies
                    if (activeBullets[i].GetPictureBox().Left > this.ClientSize.Width ||
                        !activeBullets[i].IsAlive)
                    {
                        this.Controls.Remove(activeBullets[i].GetPictureBox());
                        activeBullets.RemoveAt(i);
                    }
                }

                // Move all enemies and check for collisions with player
                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    enemies[i].Move(Keys.None);

                    // Check for player-enemy collision
                    if (player.GetPictureBox().Bounds.IntersectsWith(enemies[i].GetPictureBox().Bounds))
                    {
                        // Handle player hit (you might want to add health or game over logic here)
                        enemies[i].setIsAlive(false);
                    }

                    if (!enemies[i].IsAlive)
                    {
                        this.Controls.Remove(enemies[i].GetPictureBox());
                        enemies.RemoveAt(i);
                    }
                }
            }
            catch (Exception ex)
            {
                gameTimer.Stop();
                MessageBox.Show($"Game error: {ex.Message}");
            }
        }

        private void SpawnTimer_Tick(object sender, EventArgs e)
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
                    height: 54,
                    width: 159,
                    x: this.ClientSize.Width,
                    y: randomY,
                    form: this
                );

                this.Controls.Add(newEnemy.GetPictureBox());
                enemies.Add(newEnemy);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Enemy spawn failed: {ex.Message}");
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            pressedKeys.Add(e.KeyCode);

            // Firing with spacebar
            if (e.KeyCode == Keys.Space && bulletCooldown <= 0)
            {
                BulletBL newBullet = player.Fire();
                activeBullets.Add(newBullet);
                this.Controls.Add(newBullet.GetPictureBox());
                newBullet.GetPictureBox().BringToFront();
                bulletCooldown = FIRE_RATE;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            pressedKeys.Remove(e.KeyCode);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            // Ensure arrow keys are processed
            return keyData == Keys.Left ||
                   keyData == Keys.Right ||
                   keyData == Keys.Up ||
                   keyData == Keys.Down ||
                   base.IsInputKey(keyData);
        }
    }
}