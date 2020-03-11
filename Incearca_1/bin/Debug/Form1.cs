using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WMPLib;

namespace Incearca_1
{
    public partial class Form1 : Form
    {
        WindowsMediaPlayer gameMedia;
        WindowsMediaPlayer shootgMedia;
        WindowsMediaPlayer explosion;
        WindowsMediaPlayer dead;

        PictureBox[] stars;
        PictureBox[] munitions;
        PictureBox[] enemies;
        PictureBox[] enemiesMunition;
        int[] fvAmmo;
        int munitionSpeed;
        int backgroundSpeed;
        int playerSpeed;
        int enemiSpeed;
        int enemiesMunitionSpeed;
        int currentAmmo;
        int limAmmo;
        int score;
        int level;
        int difficulty;
        int points;
        int pct; //scor / tip de inamic (1,2,5)
        bool pause;
        bool gameIsOver;
        bool fire;
        Random rnd;
        int k;

        Image munition = Image.FromFile(@"imagini\munition.png");
        Image e1 = Image.FromFile(@"imagini\e1.png");
        Image e2 = Image.FromFile(@"imagini\e21.png");
        Image e3 = Image.FromFile(@"imagini\e31.png");

        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            pause = false;
            gameIsOver = false;
            score = 0;
            points = 0;
            level = 1;
            difficulty = 8;
            ScoreLbl.Text = score.ToString();
            LevelLbl.Text = level.ToString();

            backgroundSpeed = 4;
            playerSpeed = 4;
            enemiesMunitionSpeed = 4;
            enemiSpeed = 1;
            munitionSpeed = 20;
            currentAmmo = 0;
            limAmmo = 5;
            fvAmmo = new int[limAmmo];
            fire = false;
            stars = new PictureBox[15];
            munitions = new PictureBox[limAmmo];
            rnd = new Random();


            enemies = new PictureBox[9];
            enemiesMunition = new PictureBox[9];


            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i] = new PictureBox();
                enemies[i].Size = new Size(60, 50);
                enemies[i].SizeMode = PictureBoxSizeMode.Zoom;
                enemies[i].BorderStyle = BorderStyle.None;
                enemies[i].Visible = false;
                this.Controls.Add(enemies[i]);
                enemies[i].Location = new Point(10+i*62,-60);
                int x = rnd.Next(1, 4);
                if(x==1)
                    enemies[i].Image = e1;
                if(x==2)
                    enemies[i].Image = e2;
                if(x==3)
                    enemies[i].Image = e3;

            }

            for (int i = 0; i < enemiesMunition.Length; i++)
            {
                enemiesMunition[i] = new PictureBox();
                enemiesMunition[i].Size = new Size(10, 30);
                enemiesMunition[i].Visible = false;
                enemiesMunition[i].Image = Image.FromFile(@"imagini\munition_e.png");
                int x = rnd.Next(0, 9);
                enemiesMunition[i].Location = new Point(enemies[x].Location.X, enemies[x].Location.Y-20);
                this.Controls.Add(enemiesMunition[i]);

            }
                              
            gameMedia = new WindowsMediaPlayer();
            shootgMedia = new WindowsMediaPlayer();
            explosion = new WindowsMediaPlayer();
            dead = new WindowsMediaPlayer();

            gameMedia.URL = "sunete\\background.mp3";
            shootgMedia.URL = "sunete\\shoot.wav";
            explosion.URL = "sunete\\boom.mp3";
            dead.URL = "sunete\\end.mp3";

            gameMedia.settings.setMode("loop", true);
            gameMedia.settings.volume = 20;
            shootgMedia.settings.volume = 30;
            explosion.settings.volume = 60;
            dead.settings.volume = 60;

            explosion.controls.stop();
            dead.controls.stop();


            for (int i = 0; i < munitions.Length; i++)
            {
                munitions[i] = new PictureBox();
                munitions[i].Size = new Size(15,20);
                munitions[i].Image = munition;
                munitions[i].SizeMode = PictureBoxSizeMode.Zoom;
                munitions[i].BorderStyle = BorderStyle.None;
                this.Controls.Add(munitions[i]);
            }

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i] = new PictureBox();
                stars[i].BorderStyle= BorderStyle.None;
                stars[i].Location = new Point(rnd.Next(20, 580), rnd.Next(-10, 400));
                if (i % 2 == i)
                {
                    stars[i].Size = new Size(2, 2);
                    stars[i].BackColor = Color.Wheat;
                }
                else
                {
                    stars[i].Size = new Size(3, 3);
                    stars[i].BackColor = Color.DarkGray;
                }
                this.Controls.Add(stars[i]);
            }

            gameMedia.controls.play();

        }

        private void MoveBgTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < stars.Length / 2; i++)
            {
                stars[i].Top += backgroundSpeed;
                if (stars[i].Top >= this.Height)
                {
                    stars[i].Top = -stars[i].Height;
                }       
            }

            for (int i = stars.Length / 2; i < stars.Length; i++)
            {
                stars[i].Top += backgroundSpeed -2;
                if (stars[i].Top >= this.Height)
                {
                    stars[i].Top = -stars[i].Height;
                }
            }
        }

        private void LeftMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Left > 10)
            {
                Player.Left -= playerSpeed;
            }

        }

        private void RightMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Left < this.Width-Player.Width-25-100)
            {
                Player.Left += playerSpeed;
            }
        }

        private void UpMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Top > 10)
            {
                Player.Top -= playerSpeed;
                if (k < 7)
                    k++;
                Player.Image = Image.FromFile(@"imagini\me" + k.ToString() + ".png");
            }
        }

        private void DownMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Top < this.Height - 2*Player.Height )
            {
                Player.Top += playerSpeed;
            }
        }
       
        private void MoveMunitionTimer_Tick(object sender, EventArgs e)
        {
            int i=0;
            while (i < limAmmo && fvAmmo[i] == 1 && fire == true)
                i++;
            if (i < limAmmo && fire==true)
            {
                shootgMedia.controls.stop();
                shootgMedia.controls.play();
                fvAmmo[i] = 1;
                munitions[i].Location = new Point(Player.Location.X + Player.Width / 2, Player.Location.Y);

            }

            fire = false;
            
                for ( i = 0; i < limAmmo; i++)
                {
                    
                    if (fvAmmo[i] == 1)
                    {
                        munitions[i].Top -= munitionSpeed;
                        munitions[i].Visible = true;
                    }
                    if (munitions[i].Top <= 0)
                    {
                        munitions[i].Visible = false;
                        fvAmmo[i] = 0;
                        currentAmmo--;
                    }                
                }

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!pause)
            {
                if (e.KeyCode == Keys.Right)
                {
                    RightMoveTimer.Start();
                }
                if (e.KeyCode == Keys.Left)
                {
                    LeftMoveTimer.Start();
                }
                if (e.KeyCode == Keys.Up)
                {
                    UpMoveTimer.Start();
                }
                if (e.KeyCode == Keys.Down)
                {
                    DownMoveTimer.Start();
                }
                if (e.KeyCode == Keys.Space)
                {
                    if (currentAmmo < limAmmo)
                    {
                        currentAmmo++;
                        fire = true;
                    }

                }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                RightMoveTimer.Stop();
            }
            if (e.KeyCode == Keys.Left)
            {
                LeftMoveTimer.Stop();
            }
            if (e.KeyCode == Keys.Up)
            {
                k = 1;
                Player.Image = Image.FromFile(@"imagini\me1.png");
                UpMoveTimer.Stop();
            }
            if (e.KeyCode == Keys.Down)
            {
                DownMoveTimer.Stop();
            }


            if (e.KeyCode == Keys.Escape)
            {
                if (!gameIsOver)
                {
                    
                    if (pause)
                    {
                        StartTimers();
                        label1.Visible = false;
                        gameMedia.controls.play();
                        pause = false;
                    }
                    else
                    {
                        label1.Location = new Point(this.Width/2-120,150);
                        label1.Text = "Paused";
                        label1.Visible = true;
                        gameMedia.controls.pause();
                        StopTimers();
                        pause = true;
                    }
                }
            }
            
        }

        private void MoveEnemiesTimer_Tick(object sender, EventArgs e)
        {
            MoveEnemies(enemies, enemiSpeed);
        }

        private void MoveEnemies(PictureBox[] v, int speed)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i].Visible = true;
                v[i].Top += speed;
                Colision();
                if (v[i].Top > this.Height)
                {
                    v[i].Location = new Point(10+i*62,-100);
                }
            }
        }

        private void Colision()
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                for (int j = 0; j < munitions.Length; j++)
                {
                    if (fvAmmo[j] == 1)
                    {
                        if (munitions[j].Bounds.IntersectsWith(enemies[i].Bounds))
                        {
                            explosion.controls.stop();
                            explosion.controls.play();
                            enemies[i].Location = new Point(10 + i * 62, -100);
                            munitions[j].Visible = false;
                            fvAmmo[j] = 0;

                            if (enemies[i].Image == e1)
                                pct = 1*level;
                            else
                                if (enemies[i].Image == e2)
                                    pct = 2*level;
                                else
                                    if (enemies[i].Image == e3)
                                        pct = 5*level;

                            score += pct;
                            points += pct;
                            ScoreLbl.Text = score.ToString();

                            if (points > 100*level)
                            {
                                if(difficulty>0)
                                     difficulty--;
                                enemiSpeed++;
                                enemiesMunitionSpeed++;
                                points = 0;
                                level++;
                                LevelLbl.Text = level.ToString();
                            }
                        }
                    }

                    if (Player.Bounds.IntersectsWith(enemies[i].Bounds))
                    {
                        dead.controls.play();
                        Player.Visible = false;
                        GameOver("Game Over");
                    }
                }

            }
            for (int i = 0; i < munitions.Length; i++)
            {
                for(int j=0; j< enemiesMunition.Length;j++)
                    if (fvAmmo[i] == 1)
                    {
                        if (enemiesMunition[j].Bounds.IntersectsWith(munitions[i].Bounds))
                        {
                            enemiesMunition[j].Visible = false;
                            enemiesMunition[j].Location = new Point(-1,this.Height+100);
                            munitions[i].Visible = false;
                            fvAmmo[i] = 0;
                            

                        }
                    }
            }
        }

        private void GameOver(string str)
        {
            label1.Text = str;
            label1.Location = new Point(120,120);
            label1.Visible = true;
            ReplayBtn.Visible = true;
            ExitBtn.Visible = true;
            gameIsOver = true;

            gameMedia.controls.stop();
            StopTimers();
        }

        private void StopTimers()
        {
            MoveBgTimer.Stop();
            MoveEnemiesTimer.Stop();
            MoveMunitionTimer.Stop();
            EnemiesMunitionTimer.Stop();
        }

        private void StartTimers()
        {
            MoveBgTimer.Start();
            MoveEnemiesTimer.Start();
            MoveMunitionTimer.Start();
            EnemiesMunitionTimer.Start();
        }

        private void EnemiesMunitionTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < enemiesMunition.Length-difficulty; i++)
            {
                if (enemiesMunition[i].Top < this.Height)
                {
                    enemiesMunition[i].Visible = true;
                    enemiesMunition[i].Top += enemiesMunitionSpeed;
                    ColisionEnemiesMunition();
                }
                else
                {
                    enemiesMunition[i].Visible = false;
                    int x = rnd.Next(0, 9);
                    enemiesMunition[i].Location = new Point(enemies[x].Location.X+20,enemies[x].Location.Y+30);

                }
            }
        }

        private void ColisionEnemiesMunition()
        {
            for (int i = 0; i < enemiesMunition.Length; i++)
            {
                if (enemiesMunition[i].Bounds.IntersectsWith(Player.Bounds))
                {
                    enemiesMunition[i].Visible = false;
                    dead.controls.play();
                    Player.Visible = false;
                    GameOver("Game Over");
                }
            }

        }

        private void ReplayBtn_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            InitializeComponent();
            Form1_Load(e, e);
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }
    }
}
