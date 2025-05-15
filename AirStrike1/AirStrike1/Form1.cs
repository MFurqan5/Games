using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AirStrike1.BL;

namespace AirStrike1
{
    public partial class Form1 : Form
    {
        private PlayerBL player;
        private HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private List<BulletBL> activeBullets = new List<BulletBL>();
        private Timer gameTimer;
        private int bulletCooldown = 0;
        private const int FIRE_RATE = 5;
        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Setup form

            this.DoubleBuffered = true;
            this.ClientSize = new Size(1000, 600);
            this.KeyPreview = true;
            //var enemies = new[] { EnemyBL.EnemyType.Type1, EnemyBL.EnemyType.Type2, EnemyBL.EnemyType.Type3 };
            //var randomType = enemies[new Random().Next(3)];

            //var newEnemy = new EnemyBL(
            //    type: randomType,
            //    startX: 300,
            //    startY: 425,
            //    player: player
            //);

            //this.Controls.Add(newEnemy.GetPictureBox());

            // Create player
            Image helicopterImage = Image.FromFile("C:\\Users\\User\\source\\repos\\Airstrike\\Airstrike\\Assets\\flying_helicopter.gif");

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

            // Event handlers
            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;
        }

        private void GameUpdate(object sender, EventArgs e)
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

                // Remove off-screen bullets
                if (activeBullets[i].GetPictureBox().Left > this.ClientSize.Width ||
                    !activeBullets[i].IsAlive)
                {
                    this.Controls.Remove(activeBullets[i].GetPictureBox());
                    activeBullets.RemoveAt(i);
                }
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
