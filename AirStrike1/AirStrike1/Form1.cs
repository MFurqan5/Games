using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AirStrike1.BL;
//using Helicopter;

namespace AirStrike1
{
    public partial class Form1 : Form
    {
        IScoreManager scoreManager = new ScoreManager();
        Label scoreLabel = new Label();
        Label healthLabel = new Label();
        // Add boss health label
        Label bossHealthLabel = new Label();

        private PlayerBL player;
        private HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private List<BulletBL> activeBullets = new List<BulletBL>();
        private int bulletCooldown = 0;
        private const int FIRE_RATE = 5;

        private List<GameObjectBL> enemies = new List<GameObjectBL>();
        private Timer spawnTimer = new Timer();
        private Random random = new Random();

        private BossEnemy bossEnemy;
        private bool bossSpawned = false;

        private string enemyImagePath = @"D:\visual studio\gameimage\enemyPlane.gif";
        private string bossImagePath = @"D:\visual studio\gameimage\red_flipped_helicopter.gif";
        private string helicopterImagePath = @"D:\visual studio\gameimage\flying_helicopter.gif";
        private string bulletImagePath = @"D:\visual studio\gameimage\bullet.png";

        private Timer gameTimer;

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
            InitializeLabels();
        }

        private void InitializeLabels()
        {
            scoreLabel.Text = "Score: 0";
            scoreLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            scoreLabel.ForeColor = Color.White;
            scoreLabel.BackColor = Color.Transparent;
            scoreLabel.Location = new Point(10, 10);
            scoreLabel.AutoSize = true;
            this.Controls.Add(scoreLabel);

            healthLabel.Text = "Health: 5";
            healthLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            healthLabel.ForeColor = Color.White;
            healthLabel.BackColor = Color.Transparent;
            healthLabel.Location = new Point(10, 40);
            healthLabel.AutoSize = true;
            this.Controls.Add(healthLabel);

            // Add boss health label (initially invisible)
            bossHealthLabel.Text = "Boss Health: 5";
            bossHealthLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            bossHealthLabel.ForeColor = Color.Red;
            bossHealthLabel.BackColor = Color.Transparent;
            bossHealthLabel.Location = new Point(10, 70);
            bossHealthLabel.AutoSize = true;
            bossHealthLabel.Visible = false; // Initially invisible
            this.Controls.Add(bossHealthLabel);
        }

        private void InitializeGame()
        {
            try
            {
                this.DoubleBuffered = true;
                this.ClientSize = new Size(1000, 600);
                this.KeyPreview = true;

                if (!System.IO.File.Exists(helicopterImagePath))
                    throw new Exception("Helicopter image not found");

                player = new PlayerBL(200, 200, 100, this.ClientSize.Height / 2 - 30);
                this.Controls.Add(player.GetPictureBox());
                player.GetPictureBox().BringToFront();

                gameTimer = new Timer();
                gameTimer.Interval = 16;
                gameTimer.Tick += GameUpdate;
                gameTimer.Start();

                spawnTimer.Interval = 1000;
                spawnTimer.Tick += SpawnTimer_Tick;
                spawnTimer.Start();

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
                foreach (Keys key in pressedKeys)
                {
                    player.Move(key);
                }

                if (bulletCooldown > 0) bulletCooldown--;

                UpdateRegularEnemies();
                UpdatePlayerBullets();

                if (scoreManager.GetScore() >= 20 && !bossSpawned)
                {
                    SpawnBoss();
                }

                if (bossSpawned)
                {
                    UpdateBoss();
                }
            }
            catch (Exception ex)
            {
                gameTimer.Stop();
                MessageBox.Show($"Game error: {ex.Message}");
            }
        }

        private void UpdateRegularEnemies()
        {
            // If player is already dead, don't process enemies
            if (player.Health <= 0) return;

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (enemies[i] is Enemy enemy)
                {
                    enemy.Move(Keys.None);

                    if (player.GetPictureBox().Bounds.IntersectsWith(enemy.GetPictureBox().Bounds))
                    {
                        // Stop all timers immediately
                        gameTimer.Stop();
                        spawnTimer.Stop();

                        // Clear all event handlers to prevent multiple calls
                        gameTimer.Tick -= GameUpdate;
                        spawnTimer.Tick -= SpawnTimer_Tick;

                        // Set player health to 0
                        player.TakeDamage(player.Health);
                        healthLabel.Text = "Health: 0";

                        // Show only one message box and exit
                        MessageBox.Show("Game Over! Your helicopter was destroyed.");
                        Application.Exit();
                        return;
                    }

                    if (!enemy.IsAlive)
                    {
                        this.Controls.Remove(enemy.GetPictureBox());
                        enemies.RemoveAt(i);
                    }
                }
            }
        }

        private void UpdatePlayerBullets()
        {
            for (int i = activeBullets.Count - 1; i >= 0; i--)
            {
                activeBullets[i].Move();

                foreach (var enemyObj in enemies)
                {
                    if (enemyObj is Enemy enemy &&
                        activeBullets[i].GetPictureBox().Bounds.IntersectsWith(enemy.GetPictureBox().Bounds))
                    {
                        enemy.setIsAlive(false);
                        activeBullets[i].setIsAlive(false);
                        scoreManager.IncreaseScore(1);
                        scoreLabel.Text = "Score: " + scoreManager.GetScore();
                        break;
                    }
                }

                if (bossSpawned && activeBullets[i].GetPictureBox().Bounds.IntersectsWith(bossEnemy.GetPictureBox().Bounds))
                {
                    // Check if bullet hits the left side of the boss
                    if (activeBullets[i].GetPictureBox().Right >= bossEnemy.GetPictureBox().Left &&
                        activeBullets[i].GetPictureBox().Left <= bossEnemy.GetPictureBox().Left + 20) // 20 pixels buffer for "left side"
                    {
                        // Visual feedback: Flash the boss red when hit
                        bossEnemy.GetPictureBox().BackColor = Color.Red;

                        // Reset the color after a short delay
                        Timer resetColorTimer = new Timer();
                        resetColorTimer.Interval = 100;
                        resetColorTimer.Tick += (sender, e) => {
                            bossEnemy.GetPictureBox().BackColor = Color.Transparent;
                            resetColorTimer.Stop();
                            resetColorTimer.Dispose();
                        };
                        resetColorTimer.Start();

                        bossEnemy.TakeDamage();
                        activeBullets[i].setIsAlive(false);

                        // Update boss health label
                        bossHealthLabel.Text = "Boss Health: " + (bossEnemy.IsAlive ?
                            Math.Max(1, 6 - (6 - bossEnemy.Health)) : 0);  // Convert remaining health to display
                    }

                    if (!bossEnemy.IsAlive)
                    {
                        gameTimer.Stop();
                        MessageBox.Show("Congratulations! You defeated the boss and won the game!");
                        Application.Exit();
                    }
                }

                if (activeBullets.Count > i)
                {
                    if (activeBullets[i].GetPictureBox().Left > this.ClientSize.Width ||
                        !activeBullets[i].IsAlive)
                    {
                        this.Controls.Remove(activeBullets[i].GetPictureBox());
                        activeBullets.RemoveAt(i);
                    }
                }
            }
        }

        private void SpawnBoss()
        {
            foreach (var enemy in enemies)
            {
                this.Controls.Remove(enemy.GetPictureBox());
            }
            enemies.Clear();
            spawnTimer.Stop();

            Image bossImage = Image.FromFile(bossImagePath);
            bossEnemy = new BossEnemy(
                bossImage,
                height: 150,
                width: 200,
                x: this.ClientSize.Width - 250,
                y: this.ClientSize.Height / 2 - 75,
                form: this,
                player: player
            );
            this.Controls.Add(bossEnemy.GetPictureBox());
            bossEnemy.GetPictureBox().BringToFront();
            bossSpawned = true;

            // Show boss health label when boss is spawned
            bossHealthLabel.Visible = true;
            bossHealthLabel.Text = "Boss Health: 5";
        }

        private void UpdateBoss()
        {
            bossEnemy.Move();
            bossEnemy.UpdateBullets();
            healthLabel.Text = "Health: " + player.Health;

            if (player.Health <= 0)
            {
                gameTimer.Stop();
                MessageBox.Show("Game Over! Your helicopter was destroyed.");
                Application.Exit();
            }
        }

        private void SpawnTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!System.IO.File.Exists(enemyImagePath))
                    throw new Exception("Enemy image not found");

                Image enemyImage = Image.FromFile(enemyImagePath);
                int minY = 50;
                int maxY = Math.Max(minY + 1, this.ClientSize.Height - 100);
                int randomY = random.Next(minY, maxY);

                GameObjectBL newEnemy = new Enemy(
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
            return keyData == Keys.Left ||
                   keyData == Keys.Right ||
                   keyData == Keys.Up ||
                   keyData == Keys.Down ||
                   base.IsInputKey(keyData);
        }
    }
}