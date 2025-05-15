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
        IScoreManager scoreManager = new ScoreManager();
        Label scoreLabel = new Label();

        private PlayerBL player;
        private HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private List<BulletBL> activeBullets = new List<BulletBL>();
        private int bulletCooldown = 0;
        private const int FIRE_RATE = 5;

        private List<GameObjectBL> enemies = new List<GameObjectBL>();
        private Timer spawnTimer = new Timer();
        private Random random = new Random();

        private string enemyImagePath = @"D:\\visual studio\\gameimage\\enemyPlane.gif";
        private string helicopterImagePath = @"D:\\visual studio\\gameimage\\flying_helicopter.gif";
        private string bulletImagePath = @"D:\\visual studio\\gameimage\\bullet.png";

        private Timer gameTimer;

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
            scoreLabel.Text = "Score: 0";
            scoreLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            scoreLabel.ForeColor = Color.White;
            scoreLabel.BackColor = Color.Transparent;
            scoreLabel.Location = new Point(10, 10);
            scoreLabel.AutoSize = true;
            this.Controls.Add(scoreLabel);
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

                            if (scoreManager.GetScore() >= 20)
                            {
                                gameTimer.Stop();
                                spawnTimer.Stop();
                                MessageBox.Show("Congratulations! You won the game!");
                                Application.Exit();
                            }
                            break;
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

                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    if (enemies[i] is Enemy enemy)
                    {
                        enemy.Move(Keys.None);

                        if (player.GetPictureBox().Bounds.IntersectsWith(enemy.GetPictureBox().Bounds))
                        {
                            this.Controls.Remove(enemy.GetPictureBox());
                            enemy.setIsAlive(false);
                            this.Controls.Remove(player.GetPictureBox());

                            gameTimer.Stop();
                            spawnTimer.Stop();
                            MessageBox.Show("Game Over! You collided with the enemy.");
                            Application.Exit();
                        }

                        if (!enemy.IsAlive)
                        {
                            this.Controls.Remove(enemy.GetPictureBox());
                            enemies.RemoveAt(i);
                        }
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