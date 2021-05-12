using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Media;

namespace Digipel
{

    public partial class Form1 : Form
    {


        Timer timer = new Timer();
        public Form1()
        {
            DoubleBuffered = true;
            Random r = new Random();

            InitializeComponent();
            timer.Enabled = true;
            timer.Interval = 10;
            timer.Tick += new EventHandler(TimerCallback);
            GameManager.init();
            //GameManager.PlayerShip = new ship(0.1f, new Vector2(30f, 30f), new Vector2(30f, 30f), 100, 7, 400, 0.3, -1);
            GameManager.PlayerShip = new ship(0.1f, new Vector2(30f, 30f), new Vector2(30f, 30f), 1000, 700, 10000, 300, -1);

            string appPath = Path.GetDirectoryName(Application.ExecutablePath);

            Explosion.URL = appPath + "/explosion.wav";
            Fire.URL = appPath + "/fire.wav";
            Song.URL = appPath + "/music.wav";
            Crash.URL = appPath + "/impact.wav";

            GameManager.Explosion = Explosion;
            GameManager.Fire = Fire;
            WMPLib.IWMPMedia test = GameManager.Fire.currentMedia;
            GameManager.Song = Song;
            GameManager.Crash = Crash;
            Explosion.Ctlcontrols.stop();
            Fire.Ctlcontrols.stop();
            Song.Ctlcontrols.stop();
            Crash.Ctlcontrols.stop();
            Song.Ctlcontrols.play();
            Song.settings.autoStart = true;
            Song.settings.setMode("loop", true);
        }
        private void TimerCallback(object sender, EventArgs e)
        {
            GameManager.WindowSize = new Vector2(this.DisplayRectangle.Width, this.DisplayRectangle.Height);
            GameManager.Update(10);
            this.Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            SolidBrush Newbrush = new SolidBrush(Color.Black);
            //  Debug.WriteLine(CameraPosition.X);

            BackColor = Color.Black;




            foreach (CollisionModel currentShip in GameManager.CollisionModelList)
            {

                if (currentShip.GetType() == typeof(ship))
                {
                    ship teasdst = (ship)currentShip;

                    Vector2 Waypoint = teasdst.decideDestination();

                    Waypoint = Camera.GetLocalPosition(Waypoint);

                    Waypoint = new Vector2(Waypoint.X * Camera.zoomScalar, Waypoint.Y * Camera.zoomScalar);

                    Vector2 Origin = Camera.GetLocalPosition(currentShip.Position);

                    Origin = new Vector2(Origin.X * Camera.zoomScalar, Origin.Y * Camera.zoomScalar);


                    g.DrawLine(Pens.Red, new Point((int)Origin.X + (int)GameManager.Cameraposition.X, (int)Origin.Y + (int)GameManager.Cameraposition.Y), new Point((int)Waypoint.X + (int)GameManager.Cameraposition.X, (int)Waypoint.Y + (int)GameManager.Cameraposition.Y));
                }
                //foreach (GraphicsObject renderObject in currentShip.returnRenderableMeshes(Vector2.ZeroVector(), 1))
                foreach (GraphicsObject renderObject in currentShip.returnRenderableMeshes(GameManager.Cameraposition, Camera.zoomScalar))
                {
                    e.Graphics.FillPath(renderObject.color, renderObject.renderObject);
                }
            }
            foreach (Asteroid currentAsteroid in GameManager.AsteroidList)
            {
                GraphicsObject renderObject = currentAsteroid.returnRenderableMeshes(GameManager.Cameraposition, Camera.zoomScalar);

                e.Graphics.FillPath(renderObject.color, renderObject.renderObject);
            }



            Vector2 VelocityWaypoint = GameManager.PlayerShip.Velocity; //.returnNormalizedVector();
            VelocityWaypoint = new Vector2(VelocityWaypoint.X * 10, VelocityWaypoint.Y * 10);
            VelocityWaypoint = new Vector2(VelocityWaypoint.X + GameManager.PlayerShip.Position.X, VelocityWaypoint.Y + GameManager.PlayerShip.Position.Y);

            VelocityWaypoint = Camera.GetLocalPosition(VelocityWaypoint);

            VelocityWaypoint = new Vector2(VelocityWaypoint.X * Camera.zoomScalar, VelocityWaypoint.Y * Camera.zoomScalar);

            Vector2 VelocityOrigin = Camera.GetLocalPosition(GameManager.PlayerShip.Position);

            VelocityOrigin = new Vector2(VelocityOrigin.X * Camera.zoomScalar, VelocityOrigin.Y * Camera.zoomScalar);

            g.DrawLine(Pens.Blue, new Point((int)VelocityOrigin.X + (int)GameManager.Cameraposition.X, (int)VelocityOrigin.Y + (int)GameManager.Cameraposition.Y), new Point((int)VelocityWaypoint.X + (int)GameManager.Cameraposition.X, (int)VelocityWaypoint.Y + (int)GameManager.Cameraposition.Y));




            foreach (ExplosionEffect currentexplod in GameManager.ExplosionEffectList)
            {
                GraphicsObject renderObject = currentexplod.Nextframe(GameManager.Cameraposition, Camera.zoomScalar, 10);
                e.Graphics.FillPath(renderObject.color, renderObject.renderObject);

            }
            foreach (AmmoPickup cuasda in GameManager.ammopickuplist)
            {
                GraphicsObject renderObject = cuasda.returnRenderableMeshes(GameManager.Cameraposition, Camera.zoomScalar);
                e.Graphics.FillPath(renderObject.color, renderObject.renderObject);
            }
            int DistanceBetweenLines = 100;

            double AXOver = GameManager.Cameraposition.X % DistanceBetweenLines;
            double AYOver = GameManager.Cameraposition.Y % DistanceBetweenLines;

            for (int Height = -this.DisplayRectangle.Height * 2; Height < this.DisplayRectangle.Height * 2; Height = Height + DistanceBetweenLines)
            {
                for (int Width = -this.DisplayRectangle.Width * 2; Width < this.DisplayRectangle.Width * 2; Width = Width + DistanceBetweenLines)
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    //  Debug.WriteLine(CameraPosition.X);


                    Vector2 newpos = Camera.GetLocalPosition(new Vector2(Camera.Position.X + Width - AXOver, Camera.Position.Y + Height - AYOver));
                    newpos = new Vector2(newpos.X * Camera.zoomScalar, newpos.Y * Camera.zoomScalar);
                    //Vector2 newpos = new Vector2(Width - AXOver, Height - AYOver);

                    //g.FillRectangle(Brushes.Black, (float)Width - (float)AXOver, Height - (float)AYOver, 2, 2);
                    // g.FillRectangle(Brushes.White, (int)newpos.X + (int)GameManager.Cameraposition.X, (int)newpos.Y + (int)GameManager.Cameraposition.Y, (float)2, (float)2);
                    // base.OnPaint(e);
                }
            }


            GraphicsPath test = new GraphicsPath();
            string append = "";
            if (GameManager.PlayerShip.bulletFireRate < GameManager.PlayerShip.timesincelastbulletshot)
            {
                append = "(X) kan skjuta";
            }
            else
            {
                append = "kan skjuta om " + (GameManager.PlayerShip.bulletFireRate - GameManager.PlayerShip.timesincelastbulletshot) * 2;
            }
            append = append + "\n";
            if (GameManager.timesincelastsmartbomb > 2.5)
            {
                append = append + "(F) kan aktivera sköld";
            }
            else
            {
                append = append + "Sköld laddad om " + (2.5 - GameManager.timesincelastsmartbomb) * 2;
            }
            test.AddString("Sköldstyrka " + GameManager.PlayerShip.HP + "%\n" + append, FontFamily.GenericSansSerif, 1, 40, new Point((int)DisplayRectangle.Width / 10, (int)(DisplayRectangle.Height / 1.4)), StringFormat.GenericDefault);
            if (GameManager.PlayerShip.disabled != true)
                e.Graphics.FillPath(Brushes.DarkGreen, test);

            GraphicsPath wtest = new GraphicsPath();
            GraphicsPath gast = new GraphicsPath();

            if (GameManager.PlayerShip.disabled)
            {
                gast.AddRectangle(new Rectangle(new Point(0, DisplayRectangle.Height / 2), new Size(3000, 140)));
                wtest.AddString("DU DOG", FontFamily.GenericSansSerif, 1, 140, new Point((int)DisplayRectangle.Width / 20, (int)(DisplayRectangle.Height / 2)), StringFormat.GenericDefault);
                wtest.AddString("tryck C för att pröva igen", FontFamily.GenericSansSerif, 1, 80, new Point((int)DisplayRectangle.Width / 20, (int)(DisplayRectangle.Height / 1.3)), StringFormat.GenericDefault);
            }

            e.Graphics.FillPath(Brushes.DarkGoldenrod, gast);
            e.Graphics.FillPath(Brushes.DarkRed, wtest);


            if (GameManager.onFirstPage)
            {
                GraphicsPath gasdast = new GraphicsPath();
                GraphicsPath asdasa = new GraphicsPath();
                asdasa.AddRectangle(new Rectangle(new Point(0, 0), new Size(3000, 3140)));

                gasdast.AddString("Isboi har anfallit \nDu måste stoppa han \nFörstör isbois rymdstation\ntryck C för att fortsätta", FontFamily.GenericSansSerif, 1, 40, new Point((int)DisplayRectangle.Width / 20, (int)(DisplayRectangle.Height / 2)), StringFormat.GenericDefault);
                e.Graphics.FillPath(Brushes.Black, asdasa);
                e.Graphics.FillPath(Brushes.DarkGreen, gasdast);

            }
            if (GameManager.onSecondPage)
            {
                GraphicsPath gasdast = new GraphicsPath();
                GraphicsPath asdasa = new GraphicsPath();
                asdasa.AddRectangle(new Rectangle(new Point(0, 0), new Size(3000, 3140)));

                gasdast.AddString("Du lyckades slå isbois första anfall\nMen det kommer mer\n(C)", FontFamily.GenericSansSerif, 1, 40, new Point((int)DisplayRectangle.Width / 20, (int)(DisplayRectangle.Height / 2)), StringFormat.GenericDefault);
                e.Graphics.FillPath(Brushes.Black, asdasa);
                e.Graphics.FillPath(Brushes.DarkGreen, gasdast);

            }
            if (GameManager.onThirdPage)
            {
                GraphicsPath gasdast = new GraphicsPath();
                GraphicsPath asdasa = new GraphicsPath();
                asdasa.AddRectangle(new Rectangle(new Point(0, 0), new Size(3000, 3140)));

                gasdast.AddString("Nu är det bara rymdstationen kvar\n(C)", FontFamily.GenericSansSerif, 1, 40, new Point((int)DisplayRectangle.Width / 20, (int)(DisplayRectangle.Height / 2)), StringFormat.GenericDefault);
                e.Graphics.FillPath(Brushes.Black, asdasa);
                e.Graphics.FillPath(Brushes.DarkGreen, gasdast);

            }
            if (GameManager.onForthPage)
            {
                GraphicsPath gasdast = new GraphicsPath();
                GraphicsPath asdasa = new GraphicsPath();
                asdasa.AddRectangle(new Rectangle(new Point(0, 0), new Size(3000, 3140)));

                gasdast.AddString("ISBOI ÄR besegrad\n(C)", FontFamily.GenericSansSerif, 1, 40, new Point((int)DisplayRectangle.Width / 20, (int)(DisplayRectangle.Height / 2)), StringFormat.GenericDefault);
                e.Graphics.FillPath(Brushes.Black, asdasa);
                e.Graphics.FillPath(Brushes.DarkGreen, gasdast);

            }
            base.OnPaint(e);
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Shift)
            {

            }
            if (e.KeyCode == Keys.C)
            {
                if (GameManager.onFirstPage)
                {
                    GameManager.resetgame();
                    GameManager.Phase1();
                    GameManager.onFirstPage = false;
                    GameManager.currentLevel = 1;
                }
                if (GameManager.onSecondPage)
                {
                    GameManager.resetgame();
                    GameManager.Phase2();
                    GameManager.onSecondPage = false;
                    GameManager.currentLevel++;

                }
                if (GameManager.onThirdPage)
                {
                    GameManager.resetgame();
                    GameManager.Phase3();
                    GameManager.onThirdPage = false;
                    GameManager.currentLevel++;

                }
                if (GameManager.onForthPage)
                {
                    GameManager.resetgame();
                    GameManager.Phase1();
                    GameManager.onFirstPage = false;
                    GameManager.currentLevel = 1;
                }
                if (GameManager.PlayerShip.disabled)
                {
                    GameManager.onFirstPage = true;
                }
            }
            if (e.KeyCode == Keys.X)
            {
                GameManager.PlayerShip.fireGun();
            }
            if (e.KeyCode == Keys.W)
            {
                GameManager.PlayerShip.PosControlVectorThisTick.Y = 1;
            }
            if (e.KeyCode == Keys.A)
            {
                GameManager.PlayerShip.PosControlVectorThisTick.X = -1;
            }
            if (e.KeyCode == Keys.S)
            {
                GameManager.PlayerShip.PosControlVectorThisTick.Y = -1;
            }
            if (e.KeyCode == Keys.D)
            {
                GameManager.PlayerShip.PosControlVectorThisTick.X = 1;
            }
            if (e.KeyCode == Keys.Q)
            {
                GameManager.PlayerShip.RotControlVectorAccThisTick = 1;
            }
            if (e.KeyCode == Keys.Down)
            {
                Camera.zoomScalar = Camera.zoomScalar * 0.9f;
            }
            if (e.KeyCode == Keys.Up)
            {
                Camera.zoomScalar = Camera.zoomScalar * 1.1f;
            }
            if (e.KeyCode == Keys.E)
            {
                GameManager.PlayerShip.RotControlVectorAccThisTick = -1;
            }
            if (e.KeyCode == Keys.Left)
            {
                GameManager.PlayerShip.RotControlVectorAccThisTick = 1;
            }
            if (e.KeyCode == Keys.Right)
            {
                GameManager.PlayerShip.RotControlVectorAccThisTick = -1;
            }
            if (e.KeyCode == Keys.F)
            {
                GameManager.smartbomb();
            }

        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                GameManager.PlayerShip.PosControlVectorThisTick.Y = 0;
            }
            if (e.KeyCode == Keys.A)
            {
                GameManager.PlayerShip.PosControlVectorThisTick.X = 0;
            }
            if (e.KeyCode == Keys.S)
            {
                GameManager.PlayerShip.PosControlVectorThisTick.Y = 0;
            }
            if (e.KeyCode == Keys.D)
            {
                GameManager.PlayerShip.PosControlVectorThisTick.X = 0;
            }
            if (e.KeyCode == Keys.Q)
            {
                GameManager.PlayerShip.RotControlVectorAccThisTick = 0;
            }
            if (e.KeyCode == Keys.E)
            {
                GameManager.PlayerShip.RotControlVectorAccThisTick = 0;
            }
            if (e.KeyCode == Keys.Left)
            {
                GameManager.PlayerShip.RotControlVectorAccThisTick = 0;
            }
            if (e.KeyCode == Keys.Right)
            {
                GameManager.PlayerShip.RotControlVectorAccThisTick = 0;
            }
        }

        private void Form1_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                GameManager.PlayerShip.PosControlVectorThisTick.Y = 0;
            }
            if (e.KeyCode == Keys.A)
            {
                GameManager.PlayerShip.PosControlVectorThisTick.X = 0;
            }
            if (e.KeyCode == Keys.S)
            {
                GameManager.PlayerShip.PosControlVectorThisTick.Y = 0;
            }
            if (e.KeyCode == Keys.D)
            {
                GameManager.PlayerShip.PosControlVectorThisTick.X = 0;
            }
            if (e.KeyCode == Keys.Q)
            {
                GameManager.PlayerShip.RotControlVectorAccThisTick = 0;
            }
            if (e.KeyCode == Keys.E)
            {
                GameManager.PlayerShip.RotControlVectorAccThisTick = 0;
            }
            if (e.KeyCode == Keys.Left)
            {
                GameManager.PlayerShip.RotControlVectorAccThisTick = 0;
            }
            if (e.KeyCode == Keys.Right)
            {
                GameManager.PlayerShip.RotControlVectorAccThisTick = 0;
            }
        }

        private void Form1_Click(object sender, EventArgs e)
        {

        }
    }
    public static class GameManager
    {

        public static AxWMPLib.AxWindowsMediaPlayer Explosion;
        public static AxWMPLib.AxWindowsMediaPlayer Song;
        public static AxWMPLib.AxWindowsMediaPlayer Crash;
        public static AxWMPLib.AxWindowsMediaPlayer Fire;
        

        public static int currentLevel = 1;
        public static List<AmmoPickup> ammopickuplist = new List<AmmoPickup>();

        public static Vector2 WindowSize;

        public static List<CollisionModel> CollisionModelList = new List<CollisionModel>();
        public static List<Asteroid> AsteroidList = new List<Asteroid>();

        public static List<ExplosionEffect> ExplosionEffectList = new List<ExplosionEffect>();

        public static Vector2 Cameraposition = new Vector2(0, 0);

        public static ship PlayerShip;

        public static ship StationShip;

        public static bool onFirstPage = true;
        public static bool onSecondPage = false;
        public static bool onThirdPage = false;
        public static bool onForthPage = false;

        public static double timesincelastsmartbomb = 0;

        public static void smartbomb()
        {
            if (timesincelastsmartbomb > 2.5)
            {
                timesincelastsmartbomb = 0;
                ExplosionEffect asd = new ExplosionEffect(PlayerShip.Position, 2000, PlayerShip.Velocity);
                asd.bluecolorssss = true;
                ExplosionEffectList.Add(asd);
                foreach (Asteroid ass in AsteroidList.ToArray())
                {
                    if (Vector2.returnDifference(ass.Position, PlayerShip.Position).returnLength() < 2000 && ass.Weight < 10000)
                    {
                        double distance = Vector2.returnDifference(ass.Position, PlayerShip.Position).returnLength();
                        //distance = distance - AsteroidList[i].Size / 2;
                        Vector2 Difference = Vector2.returnDifference(ass.Position, PlayerShip.Position);
                        double Angle = Math.Atan2(Difference.Y, Difference.X);
                        double toX = Math.Cos(Angle);
                        double toY = Math.Sin(Angle);

                        ass.Velocity = new Vector2(
                            ass.Velocity.X + toX * 1000
                            ,
                            ass.Velocity.Y + toY * 1000
                        );
                    }
                }
                foreach (CollisionModel ass in CollisionModelList.ToArray())
                {
                    if (Vector2.returnDifference(ass.Position, PlayerShip.Position).returnLength() < 2000)
                    {
                        if (ass.GetType() == typeof(Bullet))
                        {
                            double distance = Vector2.returnDifference(ass.Position, PlayerShip.Position).returnLength();
                            //distance = distance - AsteroidList[i].Size / 2;
                            Vector2 Difference = Vector2.returnDifference(ass.Position, PlayerShip.Position);
                            double Angle = Math.Atan2(Difference.Y, Difference.X);
                            double toX = Math.Cos(Angle);
                            double toY = Math.Sin(Angle);

                            ass.Velocity = new Vector2(
                                ass.Velocity.X + toX * 1000
                                ,
                                ass.Velocity.Y + toY * 1000
                            );
                        }
                    }
                }
            }
        }

        public static void resetgame()
        {
            //PlayerShip = null;
            CollisionModelList = new List<CollisionModel>();
            AsteroidList = new List<Asteroid>();
            ammopickuplist = new List<AmmoPickup>();
            ExplosionEffectList = new List<ExplosionEffect>();

        }
        public static void spawnAsteroids()
        {
            Random r = new Random();
            for (int i = 1; i < 150; i++)
            {
                AsteroidList.Add(new Asteroid(r.Next(50, 1000), new Vector2(r.Next(-10000, 10000), r.Next(-10000, 10000)), new Vector2(r.Next(-200, 200), r.Next(-200, 200))));
            }
            for (int i = 1; i < 5; i++)
            {
                Asteroid toadd = new Asteroid(r.Next(2000, 3000), new Vector2(r.Next(-10000, 10000), r.Next(-10000, 10000)), new Vector2(r.Next(-200, 200), r.Next(-200, 200)));
                //toadd.NoVelocity = true;
                AsteroidList.Add(toadd);
            }
            for (int i = 0; i < 5; i++)
            {
                Asteroid toadd = new Asteroid(r.Next(5000, 9000), new Vector2(r.Next(-10000, 10000), r.Next(-10000, 10000)), new Vector2(r.Next(-100, 100), r.Next(-100, 100)));
                //toadd.NoVelocity = true;
                AsteroidList.Add(toadd);
            }
        }
        public static void spawnRandomShips()
        {
            Random r = new Random();

            for (int i = 0; i < 3; i++)
            {
                ship adder = new ship(0.3f, new Vector2(2f, 2f), new Vector2(2f, 2f), 100, 7, 400, 1, -1, 200, 0);

                adder.addCube(new Vector2(0, 0), new Vector2(0, 75), 3);

                adder.addCube(new Vector2(0, 0), new Vector2(-100, 35), 4);
                adder.addCube(new Vector2(0, 0), new Vector2(100, 35), 4);


                adder.addCube(new Vector2(0, 0), new Vector2(0, -25), 10);


                adder.addCube(new Vector2(0, 0), new Vector2(-5, 25), 2);
                adder.addCube(new Vector2(0, 0), new Vector2(5, 25), 2);


                adder.Position = new Vector2(r.Next(-1000, 1000), r.Next(-1000, 1000));
                CollisionModelList.Add(adder);
            }
        }

        public static void spawnAmmoPickups()
        {
            Random r = new Random();
            for (int i = 0; i < 15; i++)
            {
                ammopickuplist.Add(new AmmoPickup(new Vector2(r.Next(-7000, 7000), r.Next(-7000, 7000))));
            }
        }
        public static void statiospawn()
        {
            ship PLAYER = new ship(0.01f, new Vector2(30f, 30f), new Vector2(30f, 30f), 200, 30, 400, 0.1, -30, 1140, 10);

            PLAYER.MaxVelocity = 1;

            PLAYER.addCube(new Vector2(0, 0), new Vector2(0, 300), 40);
            PLAYER.addCube(new Vector2(0, 0), new Vector2(0, 600), 30);
            PLAYER.addCube(new Vector2(0, 0), new Vector2(0, 900), 20);
            PLAYER.addCube(new Vector2(0, 0), new Vector2(0, 1000), 10);
            PLAYER.addCube(new Vector2(0, 0), new Vector2(0, 1020), 5);

            PLAYER.addCube(new Vector2(0, 0), new Vector2(300, 100), 40);
            PLAYER.addCube(new Vector2(0, 0), new Vector2(-300, 100), 40);

            PLAYER.addCube(new Vector2(0, 0), new Vector2(-300, 100), 40);
            PLAYER.addCube(new Vector2(0, 0), new Vector2(-300, 100), 40);

            PLAYER.addCube(new Vector2(0, 0), new Vector2(-200, -75), 26);
            PLAYER.addCube(new Vector2(0, 0), new Vector2(200, -75), 26);

            PLAYER.addCube(new Vector2(0, 0), new Vector2(-100, -110), 18);
            PLAYER.addCube(new Vector2(0, 0), new Vector2(100, -110), 18);

            PLAYER.addCube(new Vector2(0, 0), new Vector2(-30, -135), 12);
            PLAYER.addCube(new Vector2(0, 0), new Vector2(30, -135), 12);


            PLAYER.MaxVelocity = 1;
            PLAYER.totaltimetopurge = 30;
            PLAYER.transparenttime = 27.5;
            PLAYER.AIEnabled = true;
            PLAYER.novelocity = true;
            PLAYER.explosionsize = 100000;

            PLAYER.Position = new Vector2(0, 0);

            StationShip = PLAYER;
            CollisionModelList.Add(PLAYER);
        }
        public static void dreadspawn()
        {
            ship PLAYER = new ship(0.01f, new Vector2(0.03f, 0.03f), new Vector2(0.03f, 0.03f), 100, 30, 400, 0.1, 30, 740, 10);

            PLAYER.MaxVelocity = 1;

            PLAYER.addCube(new Vector2(0, 0), new Vector2(-100, 0), 40);
            PLAYER.addCube(new Vector2(0, 0), new Vector2(100, 0), 40);


            PLAYER.addCube(new Vector2(0, 0), new Vector2(-100, 500), 20);
            PLAYER.addCube(new Vector2(0, 0), new Vector2(100, 500), 20);

            PLAYER.addCube(new Vector2(0, 0), new Vector2(-50, 300), 10);
            PLAYER.addCube(new Vector2(0, 0), new Vector2(50, 300), 10);

            PLAYER.addCube(new Vector2(0, 0), new Vector2(200, -100), 40);
            PLAYER.addCube(new Vector2(0, 0), new Vector2(-200, -100), 40);


            PLAYER.addCube(new Vector2(0, 0), new Vector2(200, 290), 20);
            PLAYER.addCube(new Vector2(0, 0), new Vector2(-200, 290), 20);


            PLAYER.totaltimetopurge = 30;
            PLAYER.transparenttime = 27.5;
            PLAYER.AIEnabled = true;
            //PLAYER.novelocity = true;
            PLAYER.explosionsize = 100000;

            PLAYER.Position = new Vector2(0, 0);

            StationShip = PLAYER;
            CollisionModelList.Add(PLAYER);

        }

        public static void playerspawn()
        {
            ship PLAYER = new ship(0.14f, new Vector2(65f, 65f), new Vector2(65f, 65f), 100, 7, 600, 0.15, -1);
            PLAYER.MaxVelocity = 120;
            /*
            ship PLAYER = new ship(0.14f, new Vector2(650f, 650f), new Vector2(650f, 650f), 3000, 10, 2000, 0.015, -1, 100, 0);
            PLAYER.AIEnabled = true;

            PLAYER.MaxVelocity = 1200;
            */

            PLAYER.addCube(new Vector2(0, 0), new Vector2(0, 25), 10);

            PLAYER.addCube(new Vector2(0, 0), new Vector2(10, -25), 10);
            PLAYER.addCube(new Vector2(0, 0), new Vector2(-10, -25), 10);

            PLAYER.addCube(new Vector2(0, 0), new Vector2(-100, 35), 10);
            PLAYER.addCube(new Vector2(0, 0), new Vector2(100, 35), 10);

            PLAYER.addCube(new Vector2(0, 0), new Vector2(-50, 17), 10);
            PLAYER.addCube(new Vector2(0, 0), new Vector2(50, 17), 10);


            PLAYER.addCube(new Vector2(0, 0), new Vector2(-75, 24), 10);
            PLAYER.addCube(new Vector2(0, 0), new Vector2(75, 24), 10);

            PLAYER.addCube(new Vector2(0, 0), new Vector2(75, 24), 10);


            PLAYER.AIEnabled = false;

            PLAYER.Position = new Vector2(1350, 1350);
            PlayerShip = PLAYER;

            CollisionModelList.Add(PlayerShip);

        }
        public static void init()
        {
            //    spawnAsteroids();
            //spawnAmmoPickups();
            //statiospawn();
            //  dreadspawn();   
            // spawnFuelPickups();
            string appPath = Path.GetDirectoryName(Application.ExecutablePath);

            // ExplosionSound = new SoundPlayer(appPath + "/explosion.wav");
            // FireSound = new SoundPlayer(appPath + "/fire.wav");
            // MusicSound = new SoundPlayer(appPath + "/music.wav");
            // ImpactSound = new SoundPlayer(appPath + "/impact.wav");


        }

        public static void Phase1()
        {
            playerspawn();
            spawnAsteroids();
            //spawnAmmoPickups();
            spawnRandomShips();
        }
        public static void Phase2()
        {
            playerspawn();
            spawnAsteroids();
            // spawnAmmoPickups();
            spawnRandomShips();
            dreadspawn();

        }

        public static void Phase3()
        {
            playerspawn();
            spawnAsteroids();
            //spawnAmmoPickups();
            spawnRandomShips();
            statiospawn();

        }

        public static void Explode(Bullet bullet, Vector2 Sourcevelocity)
        {
            bullet.disabled = true;
            ExplosionEffect news = new ExplosionEffect(bullet.GetScreenPosition(new Vector2(0, 0)), 100, Sourcevelocity);
            GameManager.ExplosionEffectList.Add(news);
            Random r = new Random();
            for (int i = 0; i < CollisionModelList.Count; i++)
            {
                CollisionModel CMS = CollisionModelList[i];
                if (Vector2.returnDifference(CMS.Position, bullet.Position).returnLength() < 200 && CMS.disabled == false)
                {
                    double distance = Vector2.returnDifference(CMS.Position, bullet.Position).returnLength();
                    //distance = distance - AsteroidList[i].Size / 2;
                    Vector2 Difference = Vector2.returnDifference(CMS.Position, bullet.Position);
                    double Angle = Math.Atan2(Difference.Y, Difference.X);
                    double toX = Math.Cos(Angle);
                    double toY = Math.Sin(Angle);

                    if (CMS.GetType() == typeof(ship))
                    {
                        //Explosion.Ctlcontrols.play();

                        CMS.RotationalVelocity = r.NextDouble() * Math.PI * 0.8 - Math.PI * 0.4;
                        CMS.Velocity = new Vector2(
                            CMS.Velocity.X + toX * -10
                            ,
                            CMS.Velocity.Y + toY * -10
                        );
                        ship test = (ship)CMS;
                        test.takeDamage((int)bullet.returnDamage(test.Velocity));
                    }
                    if (CMS.GetType() == typeof(Bullet))
                    {
                        Explode((Bullet)CMS, CMS.Velocity);
                    }
                }
            }
            /*
            for (int i = 0; i < AsteroidList.Count; i++)
            {
                Asteroid CMS = AsteroidList[i];

                if (Vector2.returnDifference(CMS.Position, bullet.Position).returnLength() < 3000)
                {
                    double distance = Vector2.returnDifference(CMS.Position, bullet.Position).returnLength();
                    //distance = distance - AsteroidList[i].Size / 2;
                    Vector2 Difference = Vector2.returnDifference(CMS.Position, bullet.Position);
                    double Angle = Math.Atan2(Difference.Y, Difference.X);
                    double toX = Math.Cos(Angle);
                    double toY = Math.Sin(Angle);
                    if (CMS.GetType() == typeof(Asteroid))
                    {
                        CMS.Velocity = new Vector2(
                            CMS.Velocity.X + toX * 300
                            ,
                            CMS.Velocity.Y + toY * 300
                        );
                    }
                }
            }
            */
        }
        public static void Update(double ticksPerSecond)
        {

            timesincelastsmartbomb = timesincelastsmartbomb + 1f / 100f;
            if (PlayerShip.HP < 100)
            {
                PlayerShip.HP = PlayerShip.HP + 0.04;
            }
            for (int i = 0; i < AsteroidList.Count; i++)
            {
                for (int j = 0; j < AsteroidList.Count; j++)
                {
                    if (i == j) //IF SAME, skip this
                        continue;
                    double distance = Vector2.returnDifference(AsteroidList[i].Position, AsteroidList[j].Position).returnLength();
                    //distance = distance - (AsteroidList[i].Size / 2 + AsteroidList[j].Size / 2);
                    Vector2 Difference = Vector2.returnDifference(AsteroidList[i].Position, AsteroidList[j].Position);
                    double Angle = Math.Atan2(Difference.Y, Difference.X);
                    double toX = Math.Cos(Angle);
                    double toY = Math.Sin(Angle);

                    if (distance < (AsteroidList[i].Size + AsteroidList[j].Size))
                    {
                        double Over = (AsteroidList[i].Size + AsteroidList[j].Size) - distance;

                        AsteroidList[i].Velocity = new Vector2(
                            AsteroidList[i].Velocity.X + toX * 20
                            ,
                            AsteroidList[i].Velocity.Y + toY * 20
                        );
                        AsteroidList[j].Velocity = new Vector2(
                            AsteroidList[j].Velocity.X + toX * -20
                            ,
                            AsteroidList[j].Velocity.Y + toY * -20
                        );
                        /*
                        if(AsteroidList[i].Weight < AsteroidList[j].Weight)
                        {
                            AsteroidList[i].Position = new Vector2(
                                AsteroidList[i].Position.X + toX * Over
                                ,
                                AsteroidList[i].Position.Y + toY * Over
                                );
                        }
                        else
                        {

                        }
                        */
                        /*
                         double IWeightProp = AsteroidList[i].Size / (AsteroidList[i].Size + AsteroidList[j].Size);
                         double JWeightProp = AsteroidList[j].Size / (AsteroidList[i].Size + AsteroidList[j].Size);
                         double Over = (AsteroidList[i].Size + AsteroidList[j].Size) - distance;
                         AsteroidList[i].Position = new Vector2
                             (
                             AsteroidList[i].Position.X + Over * toX * IWeightProp
                             ,
                             AsteroidList[i].Position.Y + Over * toY * IWeightProp
                             );
                         AsteroidList[j].Position = new Vector2
                             (
                             AsteroidList[j].Position.X + Over * toX * JWeightProp
                             ,
                             AsteroidList[j].Position.Y + Over * toY * JWeightProp
                             );
                         */
                        continue;
                    }

                    AsteroidList[j].Velocity = new Vector2
                    (
                        AsteroidList[j].Velocity.X + AsteroidList[i].Weight / (distance) * toX * 0.5f / ticksPerSecond
                        ,
                        AsteroidList[j].Velocity.Y + AsteroidList[i].Weight / (distance) * toY * 0.5f / ticksPerSecond
                    );
                }
            }

            for (int i = 0; i < AsteroidList.Count; i++)
            {
                if (PlayerShip.AsteroidToShipCollisionCheck(AsteroidList[i].Position, AsteroidList[i].Size) == true)
                {
                    PlayerShip.takeDamage(10);
                }
                for (int j = 0; j < CollisionModelList.Count; j++)
                {
                    double distance = Vector2.returnDifference(AsteroidList[i].Position, CollisionModelList[j].Position).returnLength();
                    //distance = distance - AsteroidList[i].Size / 2;
                    Vector2 Difference = Vector2.returnDifference(AsteroidList[i].Position, CollisionModelList[j].Position);
                    double Angle = Math.Atan2(Difference.Y, Difference.X);
                    double toX = Math.Cos(Angle);
                    double toY = Math.Sin(Angle);

                    if (distance < 5000)
                    {

                        if (CollisionModelList[j].AsteroidToShipCollisionCheck(AsteroidList[i].Position, AsteroidList[i].Size) == true)
                        {
                            // GameManager.Crash.Ctlcontrols.play();
                            if (CollisionModelList[j].GetType() == typeof(Bullet))
                            {
                                Explode((Bullet)CollisionModelList[j], AsteroidList[i].Velocity);
                            }
                            if (CollisionModelList[j].GetType() == typeof(ship))
                            {
                                ship tests = (ship)CollisionModelList[j];
                                // tests.HP = tests.HP - 10;
                                //tests.takeDamage(10);
                            }
                            AsteroidList[i].Velocity = new Vector2(
                                AsteroidList[i].Velocity.X + toX * 10
                                ,
                                AsteroidList[i].Velocity.Y + toY * 10
                            );
                            CollisionModelList[j].Velocity = new Vector2(
                                CollisionModelList[j].Velocity.X + toX * -100
                                ,
                                CollisionModelList[j].Velocity.Y + toY * -100
                            );
                        }
                    }


                    CollisionModelList[j].Velocity = new Vector2
                    (
                        CollisionModelList[j].Velocity.X + AsteroidList[i].Weight / (distance) * toX * 1f / ticksPerSecond
                        ,
                        CollisionModelList[j].Velocity.Y + AsteroidList[i].Weight / (distance) * toY * 1f / ticksPerSecond
                    );
                }
            }

            for (int i = 0; i < CollisionModelList.Count; i++)
            {
                for (int j = 0; j < CollisionModelList.Count; j++)
                {
                    if (i == j || CollisionModelList[j].disabled || CollisionModelList[i].disabled)
                        continue;

                    double distance = Vector2.returnDifference(CollisionModelList[i].Position, CollisionModelList[j].Position).returnLength();
                    //distance = distance - AsteroidList[i].Size / 2;
                    Vector2 Difference = Vector2.returnDifference(CollisionModelList[i].Position, CollisionModelList[j].Position);
                    double Angle = Math.Atan2(Difference.Y, Difference.X);
                    double toX = Math.Cos(Angle);
                    double toY = Math.Sin(Angle);
                    if (distance < 1000)
                    {
                        if (CollisionModelList[i].ShipToShipCollision(CollisionModelList[j]) == true)
                        {
                            //  GameManager.Crash.Ctlcontrols.play();
                            if (CollisionModelList[j].GetType() == typeof(Bullet))
                            {
                                Explode((Bullet)CollisionModelList[j], CollisionModelList[i].Velocity);
                            }
                            if (CollisionModelList[i].GetType() == typeof(Bullet))
                            {
                                Explode((Bullet)CollisionModelList[i], CollisionModelList[j].Velocity);
                            }

                            CollisionModelList[i].Velocity = new Vector2(
                            CollisionModelList[i].Velocity.X + toX * 20
                            ,
                            CollisionModelList[i].Velocity.Y + toY * 20
                        );

                            CollisionModelList[j].Velocity = new Vector2(
                                CollisionModelList[j].Velocity.X + toX * -20
                                ,
                                CollisionModelList[j].Velocity.Y + toY * -20
                            );
                        }
                    }
                }
            }

            foreach (AmmoPickup asd in ammopickuplist.ToArray())
            {
                if (PlayerShip != null)
                {
                    if (PlayerShip.AsteroidToShipCollisionCheck(asd.Position, 80))
                    {
                        PlayerShip.bulletsLeft += 5;
                        ammopickuplist.Remove(asd);
                        Random r = new Random();
                        ammopickuplist.Add(new AmmoPickup(new Vector2(r.Next(-7000, 7000), r.Next(-7000, 7000))));
                    }
                }
            }

            // Cameraposition = new Vector2(PlayerShip.Position.X - Form1.ActiveForm.Width / 2, PlayerShip.Position.Y - Form1.ActiveForm.Height / 2);
            Camera.Position = PlayerShip.Position;
            Camera.Rotation = PlayerShip.rotationHistory[5];
            // Camera.Rotation = PlayerShip.Rotation;
            Cameraposition = new Vector2(WindowSize.X / 2, WindowSize.Y / 2f);
            foreach (CollisionModel shi in CollisionModelList.ToArray())
            {
                shi.Update(ticksPerSecond);
            }
            foreach (Asteroid ast in AsteroidList)
            {
                ast.Update(ticksPerSecond);
            }

            foreach (ExplosionEffect asss in ExplosionEffectList.ToArray())
            {
                if (asss.time > asss.totaltimebeforepurge)
                {
                    ExplosionEffectList.Remove(asss);
                }
            }
            foreach (CollisionModel shi in CollisionModelList.ToArray())
            {
                if (shi.disabled)
                    CollisionModelList.Remove(shi);
            }
            int mounts = 0;
            foreach (CollisionModel asdd in CollisionModelList)
            {
                if (asdd.GetType() == typeof(ship))
                {
                    mounts++;
                }
            }
            if (mounts == 1 && PlayerShip.disabled == false)
            {
                resetgame();
                if (currentLevel == 1)
                {
                    onFirstPage = false;
                    onSecondPage = true;
                    onThirdPage = false;
                    onForthPage = false;
                }
                if (currentLevel == 2)
                {
                    onFirstPage = false;
                    onSecondPage = false;
                    onThirdPage = true;
                    onForthPage = false;
                }
                if (currentLevel == 3)
                {
                    onFirstPage = false;
                    onSecondPage = false;
                    onThirdPage = false;
                    onForthPage = true;
                }
            }
        }
    }
    public class Asteroid
    {
        public bool NoVelocity;
        public double Weight;
        public Vector2 Position;
        public Vector2 Velocity;
        public double Size;
        public void Update(double ticksPerSecond)
        {
            RotationalAndPositionalUpdate(ticksPerSecond);
        }
        public void RotationalAndPositionalUpdate(double ticksPerSecond)
        {

            if (NoVelocity == false)
            {
                Position.X += Velocity.X / ticksPerSecond;
                Position.Y += Velocity.Y / ticksPerSecond;
            }

        }
        public bool IsPositionWithinThisCollider(Vector2 WorldPos)
        {
            Vector2 diff = Vector2.returnDifference(Position, WorldPos);
            if (diff.returnLength() < Size)
            {
                return true;
            }
            return false;
        }
        public bool IsPositionWithinThisColliderCircleCheck(Vector2 WorldPos, double sizeOfObject)
        {
            Vector2 diff = Vector2.returnDifference(Position, WorldPos);
            if (diff.returnLength() < Size + sizeOfObject)
            {
                return true;
            }
            return false;
        }
        public GraphicsObject returnRenderableMeshes(Vector2 CameraPosition, double zoomScalar)
        {
            GraphicsObject renderObjectlist;

            GraphicsPath renderObject = new GraphicsPath();

            Vector2 playerposition = Camera.GetLocalPosition(new Vector2(Position.X, Position.Y));

            playerposition = new Vector2(playerposition.X * zoomScalar, playerposition.Y * zoomScalar);

            double constantincrease = Math.PI / 3;

            for (double i = constantincrease; i < Math.PI * 2; i += constantincrease)
            {
                renderObject.AddLine(
                      (int)(((float)playerposition.X + (float)CameraPosition.X) + Math.Cos(i - constantincrease) * Size * zoomScalar)
                    , (int)(((float)playerposition.Y + (float)CameraPosition.Y) + Math.Sin(i - constantincrease) * Size * zoomScalar)
                    , (int)(((float)playerposition.X + (float)CameraPosition.X) + Math.Cos(i) * Size * zoomScalar)
                    , (int)(((float)playerposition.Y + (float)CameraPosition.Y) + Math.Sin(i) * Size * zoomScalar));
            }
            Matrix testmatrix = new Matrix();
            testmatrix.RotateAt((float)mathExtension.radianToDegree(Camera.Rotation), new PointF((float)playerposition.X + (float)CameraPosition.X, (float)playerposition.Y + (float)CameraPosition.Y));
            renderObject.Transform(testmatrix);
            //renderObject.AddEllipse((float)playerposition.X + (float)CameraPosition.X, (float)playerposition.Y + (float)CameraPosition.Y, (float)Size, (float)Size);
            renderObject.CloseFigure();
            renderObjectlist = new GraphicsObject(renderObject, Brushes.White);

            return renderObjectlist;
        }
        public Asteroid(double Weight, Vector2 Position, Vector2 Velocity)
        {
            //size = Math.Pow(san.Weight,(double)1/3)

            this.Weight = Weight;
            //this.Size = Math.Pow(Weight, (double)1 / 3);
            this.Size = Math.Sqrt(Weight);

            this.Position = Position;
            this.Velocity = Velocity;
            NoVelocity = false;
        }
    }
    public static class Camera
    {
        public static double zoomScalar = 1;

        public static Vector2 Position = Vector2.ZeroVector();
        public static double Rotation = 0;
        public static Vector2 GetLocalPosition(Vector2 ScreenPosition)
        {
            double Rotation = Camera.Rotation + Math.PI / 2;
            Vector2 ParentSpacePosition = ScreenPosition;

            //Ändra position noll axis till den här pivoten
            ParentSpacePosition = new Vector2(ParentSpacePosition.X - Position.X, ParentSpacePosition.Y - Position.Y);
            //ändra position beroende på rotationen av pivoten
            ParentSpacePosition = new Vector2(Math.Cos(Rotation) * ParentSpacePosition.X + Math.Sin(Rotation) * ParentSpacePosition.Y, -Math.Sin(Rotation) * ParentSpacePosition.X + Math.Cos(Rotation * 1) * ParentSpacePosition.Y);
            return ParentSpacePosition;
        }
    }
    public class Thruster
    {
        public CollisionModel parentShip;
        public Vector2 DirectionOfExhaust;
        public Vector2 PositionInLocalSpace;

        public int StartSize;
        public int EndSize;
        public int TimeBetweenStartAndEndSize;

        public Color StartColor;
        public Color EndColor;
        public int TimeBetweenStartAndEndColor;

        public int StartAlpha;
        public int EndAlpha;
        public int TimeBetweenStartAndEndAlpha;
        public ExplosionEffect returnEffectInstance()
        {
            ExplosionEffect news = new ExplosionEffect(parentShip.GetScreenPosition(PositionInLocalSpace), Vector2.combineTwoVectors(parentShip.Velocity, parentShip.GetScreenDirection(new Vector2(DirectionOfExhaust.X * parentShip.Velocity.returnLength(), DirectionOfExhaust.Y * parentShip.Velocity.returnLength()))), this);
            return news;
        }
        public Thruster(ship parentShip, Vector2 DirectionOfExhaust, Vector2 PositionInLocalSpace, Color StartColor, Color EndColor)
        {
            StartSize = 0;
            EndSize = 250;
            TimeBetweenStartAndEndSize = 4;

            this.StartColor = StartColor;
            this.EndColor = EndColor;

            StartAlpha = 255;
            EndAlpha = 0;
            TimeBetweenStartAndEndSize = 3;

        }
    }
    public class AmmoPickup
    {
        public Vector2 Position;

        public bool IsPositionWithinThisCollider(Vector2 WorldPos)
        {
            Vector2 diff = Vector2.returnDifference(Position, WorldPos);
            if (diff.returnLength() < 40)
            {
                return true;
            }
            return false;
        }
        public GraphicsObject returnRenderableMeshes(Vector2 CameraPosition, double zoomScalar)
        {
            GraphicsObject renderObjectlist;

            GraphicsPath renderObject = new GraphicsPath();

            Vector2 playerposition = Camera.GetLocalPosition(new Vector2(Position.X, Position.Y));

            playerposition = new Vector2(playerposition.X * zoomScalar, playerposition.Y * zoomScalar);

            double constantincrease = Math.PI / 3;

            for (double i = constantincrease; i < Math.PI * 2; i += constantincrease)
            {
                renderObject.AddLine(
                      (int)(((float)playerposition.X + (float)CameraPosition.X) + Math.Cos(i - constantincrease) * 15 * zoomScalar)
                    , (int)(((float)playerposition.Y + (float)CameraPosition.Y) + Math.Sin(i - constantincrease) * 15 * zoomScalar)
                    , (int)(((float)playerposition.X + (float)CameraPosition.X) + Math.Cos(i) * 15 * zoomScalar)
                    , (int)(((float)playerposition.Y + (float)CameraPosition.Y) + Math.Sin(i) * 15 * zoomScalar));
            }
            Matrix testmatrix = new Matrix();
            testmatrix.RotateAt((float)mathExtension.radianToDegree(Camera.Rotation), new PointF((float)playerposition.X + (float)CameraPosition.X, (float)playerposition.Y + (float)CameraPosition.Y));
            renderObject.Transform(testmatrix);
            //renderObject.AddEllipse((float)playerposition.X + (float)CameraPosition.X, (float)playerposition.Y + (float)CameraPosition.Y, (float)Size, (float)Size);
            renderObject.CloseFigure();
            renderObjectlist = new GraphicsObject(renderObject, Brushes.Green);

            return renderObjectlist;
        }
        public AmmoPickup(Vector2 Position)
        {
            this.Position = Position;
        }
    }
    public class ship : CollisionModel
    {
        public double reloadtime;
        public double timespentreloading;
        public double transparenttime = 2.5;
        public double totaltimetopurge = 5;
        public double explosionsize = 2000;
        int bulletSpawnDistance = 100;
        int bulletSize;
        int bulletVelocity;
        public double bulletFireRate;
        public int bulletsLeft;
        public double timesincelastbulletshot;

        public double HP;

        public bool AIEnabled = true;

        public double MaxVelocity = 30;

        Vector2 YPositionalAccelerationPerSecond; //X = FORWARD, Y = BACKWARD
        Vector2 XPositionalAccelerationPerSecond; //X = LEFT, Y = RIGHT

        double maxRotationalVelocity = 0.2;
        double RotationalAccelerationPerSecond;

        public Vector2 PosControlVectorThisTick = Vector2.ZeroVector();
        public double RotControlVectorAccThisTick = 0;

        public double[] rotationHistory = new double[100];
        double MsTimeSinceLastRotationUpdate = 0;

        public void takeDamage(int dmg)
        {
            HP = HP - dmg;
            if (HP <= 0)
            {
                this.disabled = true;
                ExplosionEffect news = new ExplosionEffect(GetScreenPosition(new Vector2(0, 0)), explosionsize, Velocity, transparenttime, totaltimetopurge);
                GameManager.ExplosionEffectList.Add(news);
            }
        }
        public void fireGun()
        {
            if (timesincelastbulletshot >= bulletFireRate && bulletsLeft != 0)
            {
                // GameManager.Fire.Ctlcontrols.play();
                bulletsLeft--;
                timesincelastbulletshot = 0;
                Bullet test = new Bullet(GetScreenPosition(new Vector2(bulletSpawnDistance, 0)), GetScreenDirection(new Vector2(bulletVelocity, 0)), bulletSize);
                test.Velocity.X = test.Velocity.X + Velocity.X;
                test.Velocity.Y = test.Velocity.Y + Velocity.Y;
                GameManager.CollisionModelList.Add(test);
            }
        }
        public void ThrustUpdater(double ticksPerSecond)
        {
            if (PosControlVectorThisTick.X > 0)
            {
                ExplosionEffect news = new ExplosionEffect(GetScreenPosition(new Vector2(0, -120)), 30, Vector2.combineTwoVectors(Velocity, GetScreenDirection(new Vector2(0, -0.5 * Velocity.returnLength()))));
                news.time = 0;
                news.bluecolor = true;
                GameManager.ExplosionEffectList.Add(news);

                Velocity.X = Velocity.X + -1 * PosControlVectorThisTick.X * Math.Sin(Rotation) * XPositionalAccelerationPerSecond.X / ticksPerSecond;
                Velocity.Y = Velocity.Y + PosControlVectorThisTick.X * Math.Cos(Rotation) * XPositionalAccelerationPerSecond.X / ticksPerSecond;
            }
            else if (PosControlVectorThisTick.X < 0)
            {
                ExplosionEffect news = new ExplosionEffect(GetScreenPosition(new Vector2(0, 120)), 30, Vector2.combineTwoVectors(Velocity, GetScreenDirection(new Vector2(0, 0.5 * Velocity.returnLength()))));
                news.time = 0;
                news.bluecolor = true;
                GameManager.ExplosionEffectList.Add(news);

                Velocity.X = Velocity.X + -1 * PosControlVectorThisTick.X * Math.Sin(Rotation) * XPositionalAccelerationPerSecond.Y / ticksPerSecond;
                Velocity.Y = Velocity.Y + PosControlVectorThisTick.X * Math.Cos(Rotation) * XPositionalAccelerationPerSecond.Y / ticksPerSecond;
            }
            if (PosControlVectorThisTick.Y > 0)
            {
                ExplosionEffect news = new ExplosionEffect(GetScreenPosition(new Vector2(-30, 0)), 30, Vector2.combineTwoVectors(Velocity, GetScreenDirection(new Vector2(-0.5 * Velocity.returnLength(), 0))));
                news.time = 0;
                news.bluecolor = true;

                GameManager.ExplosionEffectList.Add(news);
                Velocity.X = Velocity.X + PosControlVectorThisTick.Y * Math.Cos(Rotation) * YPositionalAccelerationPerSecond.X / ticksPerSecond;
                Velocity.Y = Velocity.Y + PosControlVectorThisTick.Y * Math.Sin(Rotation) * YPositionalAccelerationPerSecond.X / ticksPerSecond;
            }
            else if (PosControlVectorThisTick.Y < 0)
            {
                ExplosionEffect news = new ExplosionEffect(GetScreenPosition(new Vector2(30, 0)), 30, Vector2.combineTwoVectors(Velocity, GetScreenDirection(new Vector2(0.5 * Velocity.returnLength(), 0))));
                news.time = 0;
                news.bluecolor = true;

                GameManager.ExplosionEffectList.Add(news);
                Velocity.X = Velocity.X + PosControlVectorThisTick.Y * Math.Cos(Rotation) * YPositionalAccelerationPerSecond.Y / ticksPerSecond;
                Velocity.Y = Velocity.Y + PosControlVectorThisTick.Y * Math.Sin(Rotation) * YPositionalAccelerationPerSecond.Y / ticksPerSecond;
            }

            if (RotControlVectorAccThisTick > 0)
            {
                ExplosionEffect news = new ExplosionEffect(GetScreenPosition(new Vector2(0, 100)), 30, Vector2.combineTwoVectors(Velocity, GetScreenDirection(new Vector2(-0.25 * Velocity.returnLength(), 0))));
                news.time = 0;
                news.bluecolor = true;

                GameManager.ExplosionEffectList.Add(news);
            }
            else if (RotControlVectorAccThisTick < 0)
            {
                ExplosionEffect news = new ExplosionEffect(GetScreenPosition(new Vector2(0, -100)), 30, Vector2.combineTwoVectors(Velocity, GetScreenDirection(new Vector2(-0.25 * Velocity.returnLength(), 0))));
                news.time = 0;
                news.bluecolor = true;

                GameManager.ExplosionEffectList.Add(news);
            }


            RotationalVelocity = RotationalVelocity - RotControlVectorAccThisTick * (RotationalAccelerationPerSecond / ticksPerSecond);

            if (mathExtension.returnPositiveNumber(RotationalVelocity) > maxRotationalVelocity)
            {
                RotationalVelocity = RotationalVelocity * 0.99f;
                // RotationalVelocity = RotationalVelocity + RotControlVectorAccThisTick * (RotationalAccelerationPerSecond / ticksPerSecond);
            }

        }

        public override void Update(double ticksPerSecond)
        {
            if (novelocity)
                Velocity = Vector2.ZeroVector();
            timesincelastbulletshot = timesincelastbulletshot + 1f / 100f;
            MsTimeSinceLastRotationUpdate += 1000f / ticksPerSecond;
            if (MsTimeSinceLastRotationUpdate > 10)
            {
                for (int i = rotationHistory.Length - 2; i >= 0; i--)
                {
                    rotationHistory[i + 1] = rotationHistory[i];
                }
                rotationHistory[0] = Rotation;
                MsTimeSinceLastRotationUpdate = 0;
            }

            if (AIEnabled)
                AIUpdate(ticksPerSecond);
            ThrustUpdater(ticksPerSecond);
            RotationalAndPositionalUpdate(ticksPerSecond);
            if (bulletsLeft == 0)
            {
                if (reloadtime > 0)
                {
                    timespentreloading = timespentreloading + 0.01f;
                    if (timespentreloading > reloadtime)
                    {
                        timespentreloading = 0;
                        bulletsLeft = 100;
                    }
                }
            }
            /*
            if (Velocity.returnLength() > MaxVelocity)
            {
                double diff = Velocity.returnLength() - MaxVelocity;
                Vector2 normalized = Velocity.returnNormalizedVector();
                Velocity = new Vector2(Velocity.X - diff * normalized.X, Velocity.Y - diff * normalized.Y);
            }
            */
        }

        public void AIUpdate(double ticksPerSecond)
        {
            fireGun();
            Vector2 Waypoint = decideDestination();

            double RotationWanted = 0;

            Vector2 positionalDelta = getPositionDelta(Waypoint);
            double rotationalDelta = (Rotation - getTriangleRotation(positionalDelta)) - RotationWanted;

            int AmountToRemove = 0;
            // Debug.WriteLine("rotationalDelta " + rotationalDelta);
            // Debug.WriteLine("positionalDeltaa.x " + positionalDelta.X);
            // Debug.WriteLine("positionalDeltaa.y " + positionalDelta.Y);
            if (rotationalDelta > (Math.PI * 1))
            {
                AmountToRemove = (int)(Rotation % (Math.PI * 1));
            }
            if (rotationalDelta < (Math.PI * -1))
            {
                AmountToRemove = (int)(Rotation % (Math.PI * 1));
            }

            rotationalDelta = rotationalDelta - (AmountToRemove * Math.PI * 2);

            PosControlVectorThisTick.Y = Math.Min(1, positionalDelta.returnLength() / 800);
            if (positionalDelta.returnLength() < 400)
            {
                PosControlVectorThisTick.Y = -1 + positionalDelta.returnLength() / 400;
            }

            RotControlVectorAccThisTick = rotationalDelta;
        }
        double getTriangleRotation(Vector2 TriangleSize)
        {
            return Math.Atan2(TriangleSize.Y, TriangleSize.X);
        }
        public Vector2 decideDestination()
        {
            //return new Vector2(3000, 3000);
            return GameManager.PlayerShip.Position;
        }
        Vector2 getPositionDelta(Vector2 destinationPosition)
        {
            return new Vector2(destinationPosition.X - Position.X, destinationPosition.Y - Position.Y);
        }
        public ship(double RotationalAccelerationPerSecond, Vector2 XPositionalAccelerationPerSecond, Vector2 YPositionalAccelerationPerSecond, int HP, int bulletSize, int bulletVelocity, double bulletFireRate, int bulletsLeft)
        {
            this.bulletVelocity = bulletVelocity;
            this.bulletFireRate = bulletFireRate;
            this.bulletsLeft = bulletsLeft;
            this.RotationalAccelerationPerSecond = RotationalAccelerationPerSecond;
            this.XPositionalAccelerationPerSecond = XPositionalAccelerationPerSecond;
            this.YPositionalAccelerationPerSecond = YPositionalAccelerationPerSecond;
            this.HP = HP;
            this.bulletSize = bulletSize;
            brush = Brushes.DarkGray;
            timesincelastbulletshot = 0;
            reloadtime = 0;
            timespentreloading = 0;
        }
        public ship(double RotationalAccelerationPerSecond, Vector2 XPositionalAccelerationPerSecond, Vector2 YPositionalAccelerationPerSecond, int HP, int bulletSize, int bulletVelocity, double bulletFireRate, int bulletsLeft, int bulletspawndistance, double reloadtime)
        {
            this.bulletVelocity = bulletVelocity;
            this.bulletFireRate = bulletFireRate;
            this.bulletsLeft = bulletsLeft;
            this.RotationalAccelerationPerSecond = RotationalAccelerationPerSecond;
            this.XPositionalAccelerationPerSecond = XPositionalAccelerationPerSecond;
            this.YPositionalAccelerationPerSecond = YPositionalAccelerationPerSecond;
            this.HP = HP;
            this.bulletSize = bulletSize;
            brush = Brushes.DarkGray;
            timesincelastbulletshot = 0;
            this.bulletSpawnDistance = bulletspawndistance;
            this.reloadtime = reloadtime;
            timespentreloading = 0;
        }
    }
    public class transform
    {
        public bool novelocity = false;

        public virtual void Update(double ticksPerSecond)
        {

        }
        //wasd movement
        //q and e rotation
        public Vector2 Position = Vector2.ZeroVector();

        public Vector2 Velocity = Vector2.ZeroVector();

        public double Rotation = 0;

        public double RotationalVelocity = 0;

        public Vector2 ForceVectorThisTick = Vector2.ZeroVector();

        public Brush brush = Brushes.White;
        public void RotationalAndPositionalUpdate(double ticksPerSecond)
        {
            Rotation += RotationalVelocity / ticksPerSecond;

            int AmountToRemove = 0;
            if (Rotation > (Math.PI * 1))
            {
                AmountToRemove = (int)(Rotation / (Math.PI * 1));
            }
            if (Rotation < (Math.PI * -1))
            {
                AmountToRemove = (int)(Rotation / (Math.PI * 1));
            }

            this.Rotation = Rotation - (AmountToRemove * Math.PI * 2);




            if (novelocity)
                return;

            Position.X += Velocity.X / ticksPerSecond;
            Position.Y += Velocity.Y / ticksPerSecond;
        }
    }
    public class CollisionModel : transform
    {
        public bool disabled = false;

        public List<square> Squarelist = new List<square>();
        public void addCube(Vector2 Startpos, Vector2 EndPos, int xSize)
        {
            Vector2 LocalPositionRelative = new Vector2(Startpos.X - EndPos.X, Startpos.Y - EndPos.Y);

            double Angle = Math.Atan2(LocalPositionRelative.Y, LocalPositionRelative.X);
            double Length = Math.Sqrt(LocalPositionRelative.X * LocalPositionRelative.X + LocalPositionRelative.Y * LocalPositionRelative.Y);

            square ToAdd = new square(Startpos, new Vector2(xSize, Length), Angle, this);
            Squarelist.Add(ToAdd);
        }
        public Vector2 GetScreenDirection(Vector2 localDirection)
        {
            localDirection = new Vector2(Math.Cos(Rotation) * localDirection.X - Math.Sin(Rotation) * localDirection.Y, Math.Sin(Rotation) * localDirection.X + Math.Cos(Rotation) * localDirection.Y);
            return localDirection;
        }
        public Vector2 GetScreenPosition(Vector2 LocalPosition)
        {
            LocalPosition = new Vector2(Math.Cos(Rotation) * LocalPosition.X - Math.Sin(Rotation) * LocalPosition.Y + Position.X, Math.Sin(Rotation) * LocalPosition.X + Math.Cos(Rotation) * LocalPosition.Y + Position.Y);
            return LocalPosition;
        }
        public List<GraphicsObject> returnRenderableMeshes(Vector2 CameraPosition, double zoomScalar)
        {
            List<GraphicsObject> renderObjectlist = new List<GraphicsObject>();
            foreach (square SBMC in Squarelist)
            {
                GraphicsPath renderObject = new GraphicsPath();
                Vector2 BottomLeft = SBMC.GetScreenPosition(new Vector2(-SBMC.Size.X, 0));
                Vector2 BottomRight = SBMC.GetScreenPosition(new Vector2(SBMC.Size.X, 0));
                Vector2 TopRight = SBMC.GetScreenPosition(new Vector2(SBMC.Size.X, SBMC.Size.Y));
                Vector2 TopLeft = SBMC.GetScreenPosition(new Vector2(-SBMC.Size.X, SBMC.Size.Y));





                BottomLeft = Camera.GetLocalPosition(BottomLeft);
                BottomRight = Camera.GetLocalPosition(BottomRight);
                TopRight = Camera.GetLocalPosition(TopRight);
                TopLeft = Camera.GetLocalPosition(TopLeft);

                BottomLeft = new Vector2(BottomLeft.X * zoomScalar, BottomLeft.Y * zoomScalar);
                BottomRight = new Vector2(BottomRight.X * zoomScalar, BottomRight.Y * zoomScalar);
                TopRight = new Vector2(TopRight.X * zoomScalar, TopRight.Y * zoomScalar);
                TopLeft = new Vector2(TopLeft.X * zoomScalar, TopLeft.Y * zoomScalar);

                /*
                BottomLeft.X = BottomLeft.X + 500;
                BottomRight.X = BottomRight.X + 500;
                TopRight.X = TopRight.X + 500;
                TopLeft.X = TopLeft.X + 500;

                BottomLeft.Y = BottomLeft.Y + 400;
                BottomRight.Y = BottomRight.Y + 400;
                TopRight.Y = TopRight.Y + 400;
                TopLeft.Y = TopLeft.Y + 400;
                */

                renderObject.AddLine((float)BottomLeft.X + (float)CameraPosition.X, (float)BottomLeft.Y + (float)CameraPosition.Y, (float)BottomRight.X + (float)CameraPosition.X, (float)BottomRight.Y + (float)CameraPosition.Y);
                renderObject.AddLine((float)BottomRight.X + (float)CameraPosition.X, (float)BottomRight.Y + (float)CameraPosition.Y, (float)TopRight.X + (float)CameraPosition.X, (float)TopRight.Y + (float)CameraPosition.Y);
                renderObject.AddLine((float)TopRight.X + (float)CameraPosition.X, (float)TopRight.Y + (float)CameraPosition.Y, (float)TopLeft.X + (float)CameraPosition.X, (float)TopLeft.Y + (float)CameraPosition.Y);
                renderObject.AddLine((float)TopLeft.X + (float)CameraPosition.X, (float)TopLeft.Y + (float)CameraPosition.Y, (float)BottomLeft.X + (float)CameraPosition.X, (float)BottomLeft.Y + (float)CameraPosition.Y);



                //renderObject.AddLine((float)BottomLeft.X - (float)CameraPosition.X, (float)BottomLeft.Y - (float)CameraPosition.Y, (float)BottomRight.X - (float)CameraPosition.X, (float)BottomRight.Y - (float)CameraPosition.Y);
                //renderObject.AddLine((float)BottomRight.X - (float)CameraPosition.X, (float)BottomRight.Y - (float)CameraPosition.Y, (float)TopRight.X - (float)CameraPosition.X, (float)TopRight.Y - (float)CameraPosition.Y);
                //renderObject.AddLine((float)TopRight.X - (float)CameraPosition.X, (float)TopRight.Y - (float)CameraPosition.Y, (float)TopLeft.X - (float)CameraPosition.X, (float)TopLeft.Y - (float)CameraPosition.Y);
                //renderObject.AddLine((float)TopLeft.X - (float)CameraPosition.X, (float)TopLeft.Y - (float)CameraPosition.Y, (float)BottomLeft.X - (float)CameraPosition.X, (float)BottomLeft.Y - (float)CameraPosition.Y);

                renderObject.CloseFigure();
                // renderObjectlist.Add(new GraphicsObject(renderObject, Brushes.White));
                renderObjectlist.Add(new GraphicsObject(renderObject, brush));
            }
            GraphicsPath Velocity;
            return renderObjectlist;
        }
        public Vector2 GetLocalPosition(Vector2 ScreenPosition)
        {
            Vector2 ParentSpacePosition = ScreenPosition;

            //Ändra position noll axis till den här pivoten
            ParentSpacePosition = new Vector2(ParentSpacePosition.X - Position.X, ParentSpacePosition.Y - Position.Y);
            //ändra position beroende på rotationen av pivoten
            ParentSpacePosition = new Vector2(Math.Cos(Rotation) * ParentSpacePosition.X + Math.Sin(Rotation) * ParentSpacePosition.Y, -Math.Sin(Rotation) * ParentSpacePosition.X + Math.Cos(Rotation * 1) * ParentSpacePosition.Y);
            return ParentSpacePosition;
        }
        public bool pointToShipCollisionCheck(Vector2 screenPos)
        {
            foreach (square sq in Squarelist)
            {
                if (sq.IsPositionCollidingFromWorld(screenPos))
                    return true;
            }
            return false;
        }
        public bool AsteroidToShipCollisionCheck(Vector2 Position, double Size)
        {
            List<Vector2> BoundingBoxPointCollection = returnBoundingBoxPoints();
            foreach (Vector2 asss in BoundingBoxPointCollection)
            {
                if (Vector2.returnDifference(asss, Position).returnLength() < Size / 2f)
                {
                    return true;
                }
            }
            return false;
        }
        public bool ShipToShipCollision(CollisionModel CollisionModelToChecks)
        {
            List<Vector2> listofboundingboxpoints = new List<Vector2>();
            listofboundingboxpoints = CollisionModelToChecks.returnBoundingBoxPoints();
            foreach (Vector2 point in listofboundingboxpoints)
            {
                foreach (square ass in Squarelist)
                {
                    if (ass.IsPositionCollidingFromWorld(point))
                        return true;
                }
            }
            return false;
        }
        public List<Vector2> returnBoundingBoxPoints()
        {
            List<Vector2> listofboundingboxpoints = new List<Vector2>();
            foreach (square sq in Squarelist)
            {
                foreach (Vector2 ass in sq.returnBoundingBoxPointsWorld())
                {
                    listofboundingboxpoints.Add(ass);
                }
            }
            return listofboundingboxpoints;
        }

        public CollisionModel() { }
    }
    public class Bullet : CollisionModel
    {
        public int weight;
        float timealive;
        public override void Update(double ticksPerSecond)
        {
            timealive = timealive + 1f / 100f;
            if (timealive > 3)
            {
                GameManager.Explode(this, Velocity);
            }
            updaterotThrust(ticksPerSecond);
            RotationalAndPositionalUpdate(ticksPerSecond);
        }
        void updaterotThrust(double ticksPerSecond)
        {
            double VelocityAngle = Math.Atan2(Velocity.Y, Velocity.X);
            double rotationalDelta = (Rotation - VelocityAngle);

            int AmountToRemove = 0;
            // Debug.WriteLine("rotationalDelta " + rotationalDelta);
            // Debug.WriteLine("positionalDeltaa.x " + positionalDelta.X);
            // Debug.WriteLine("positionalDeltaa.y " + positionalDelta.Y);
            if (rotationalDelta > (Math.PI * 1))
            {
                AmountToRemove = (int)(Rotation % (Math.PI * 1));
            }
            if (rotationalDelta < (Math.PI * -1))
            {
                AmountToRemove = (int)(Rotation % (Math.PI * 1));
            }

            rotationalDelta = rotationalDelta - (AmountToRemove * Math.PI * 2);

            RotationalVelocity = RotationalVelocity - rotationalDelta / 100 / ticksPerSecond;
        }
        public double returnDamage(Vector2 velocityOfImpactedObject)
        {
            //Vector2 diff = Vector2.returnDifference(Velocity, velocityOfImpactedObject);
            //return diff.returnLength() * weight;
            return 40;
        }
        public Bullet(Vector2 Position, Vector2 velocity, int weight)
        {
            timealive = 0;
            this.Rotation = Math.Atan2(velocity.Y, velocity.X);
            this.weight = weight;
            this.Velocity = velocity;
            this.Position = Position;
            addCube(new Vector2(0, 0), new Vector2(weight * 1, -6 * weight), (int)(weight * 1));
            addCube(new Vector2(0, 0), new Vector2(-weight * 1, -6 * weight), (int)(weight * 1));
            brush = Brushes.Red;
        }
    }
    public class Missile : CollisionModel
    {

    }
    public class square
    {
        CollisionModel Parent;
        Brush color;
        public Vector2 Position;
        public Vector2 Size;
        public double Rotation;
        public Vector2 GetLocalPositionFromParent(Vector2 ParentPostion)
        {
            Vector2 ParentSpacePosition = ParentPostion;

            //Ändra position noll axis till den här pivoten
            ParentSpacePosition = new Vector2(ParentSpacePosition.X - Position.X, ParentSpacePosition.Y - Position.Y);
            //ändra position beroende på rotationen av pivoten
            ParentSpacePosition = new Vector2(Math.Cos(Rotation) * ParentSpacePosition.X + Math.Sin(Rotation) * ParentSpacePosition.Y, -Math.Sin(Rotation) * ParentSpacePosition.X + Math.Cos(Rotation * 1) * ParentSpacePosition.Y);
            return ParentSpacePosition;
        }
        public Vector2 GetLocalPositionFromWorld(Vector2 ScreenPosition)
        {

            Vector2 ParentSpacePosition = Parent.GetLocalPosition(ScreenPosition);

            //Ändra position noll axis till den här pivoten
            ParentSpacePosition = new Vector2(ParentSpacePosition.X - Position.X, ParentSpacePosition.Y - Position.Y);
            //ändra position beroende på rotationen av pivoten
            ParentSpacePosition = new Vector2(Math.Cos(Rotation) * ParentSpacePosition.X + Math.Sin(Rotation) * ParentSpacePosition.Y, -Math.Sin(Rotation) * ParentSpacePosition.X + Math.Cos(Rotation * 1) * ParentSpacePosition.Y);
            return ParentSpacePosition;
        }
        public bool IsPositionCollidingFromWorld(Vector2 Screenpos)
        {
            Vector2 LocalPosition = GetLocalPositionFromWorld(Screenpos);

            if (LocalPosition.X > -Size.X && LocalPosition.X < Size.X)
            {
                if (LocalPosition.Y > 0 && LocalPosition.Y < Size.Y)
                    return true;
            }
            return false;
        }
        public Vector2[] returnBoundingBoxPointsWorld()
        {
            Vector2[] boundingBoxPointsList = new Vector2[4];
            boundingBoxPointsList[0] = GetScreenPosition(new Vector2(Size.X, 0));
            boundingBoxPointsList[1] = GetScreenPosition(new Vector2(-Size.X, 0));
            boundingBoxPointsList[2] = GetScreenPosition(new Vector2(Size.X, Size.Y));
            boundingBoxPointsList[3] = GetScreenPosition(new Vector2(-Size.X, Size.Y));
            return boundingBoxPointsList;
        }
        public Vector2 GetScreenPosition(Vector2 LocalPosition)
        {
            LocalPosition = new Vector2(Math.Cos(Rotation) * LocalPosition.X - Math.Sin(Rotation) * LocalPosition.Y + Position.X, Math.Sin(Rotation) * LocalPosition.X + Math.Cos(Rotation) * LocalPosition.Y + Position.Y);
            LocalPosition = Parent.GetScreenPosition(LocalPosition);
            return LocalPosition;
        }
        public square(Vector2 Position, Vector2 Size, double Rotation, CollisionModel Parent)
        {
            this.Rotation = Rotation;
            this.Position = Position;
            this.Size = Size;
            this.Parent = Parent;
        }
    }
    public class Vector2
    {
        public Vector2 returnNormalizedVector()
        {
            double x = mathExtension.returnPositiveNumber(X);
            double y = mathExtension.returnPositiveNumber(Y);
            double Xprocentage = X / (x + y);
            double Yprocentage = Y / (x + y);


            if (x == 0 && y == 0)
            {
                return ZeroVector();
            }

            return new Vector2(Xprocentage, Yprocentage);
        }
        public static Vector2 ZeroVector()
        {
            return new Vector2(0, 0);
        }
        public double returnLength()
        {
            return Math.Sqrt(X * X + Y * Y);
        }
        public static Vector2 returnDifference(Vector2 triangle1, Vector2 triangle2)
        {
            return new Vector2(triangle1.X - triangle2.X, triangle1.Y - triangle2.Y);
        }
        public static Vector2 combineTwoVectors(Vector2 triangle1, Vector2 triangle2)
        {
            return new Vector2(triangle1.X + triangle2.X, triangle1.Y + triangle2.Y);
        }
        public double X;
        public double Y;
        public Vector2(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
    public class GraphicsObject
    {
        public GraphicsPath renderObject;
        public Brush color;
        public GraphicsObject(GraphicsPath renderObject, Brush color)
        {
            this.renderObject = renderObject;
            this.color = color;
        }
    }
    public class ExplosionEffect : transform
    {
        public double time;
        Thruster thruster;
        double SizeMultiplier;
        public bool bluecolor = false;
        public bool bluecolorssss = false;

        public double timetotransparent = 2.5;
        public double totaltimebeforepurge = 5;
        public ExplosionEffect(Vector2 Position, double SizeMultiplier, Vector2 Velocity)
        {
            this.Velocity = Velocity;
            time = 0;
            this.Position = Position;
            this.SizeMultiplier = SizeMultiplier;
        }
        public ExplosionEffect(Vector2 Position, double SizeMultiplier, Vector2 Velocity, double timetotransparent, double totaltimebeforepurge)
        {
            this.Velocity = Velocity;
            time = 0;
            this.Position = Position;
            this.SizeMultiplier = SizeMultiplier;
            this.timetotransparent = timetotransparent;
            this.totaltimebeforepurge = totaltimebeforepurge;
        }
        public ExplosionEffect(Vector2 Position, Vector2 Velocity, Thruster thruster)
        {
            this.Velocity = Velocity;
            time = 0;
            this.Position = Position;
        }
        public GraphicsObject Nextframe(Vector2 CameraPosition, double zoomScalar, double ActualTicksPerSecond)
        {
            //Velocity = new Vector2(Velocity.X*0.95,Velocity.Y*0.95);
            RotationalAndPositionalUpdate(10);
            GraphicsObject thisframe = returnRenderableMeshes(CameraPosition, zoomScalar, Math.Sqrt(time / 5f) * SizeMultiplier, null);

            time += 1 / ActualTicksPerSecond;

            if (bluecolorssss)
            {
                if (time < 2.5)
                {
                    thisframe.color = new SolidBrush
                        (
                        Color.FromArgb(
                          0
                        , 100 + (int)Math.Sqrt(time) * 40
                        , 255 - (int)Math.Sqrt(time) * 40));
                }
                else
                {
                    thisframe.color = new SolidBrush(
                        Color.FromArgb(
                            (int)Math.Max((255 * (1 - ((time / 5)))), 0)
                            ,
                            0
                            ,
                            (int)((255 - Math.Sqrt(2.5) * 10))
                            ,
                            (int)Math.Max((255 * (1 - ((time / 5)))), 0)
                            )
                     );

                }
            }
            else
            {
                if (time < 2.5)
                {
                    thisframe.color = new SolidBrush(Color.FromArgb(255 - (int)Math.Sqrt(time) * 40, 100 + (int)Math.Sqrt(time) * 40, 0));
                }
                else
                {
                    thisframe.color = new SolidBrush(
                        Color.FromArgb(
                            (int)Math.Min(Math.Max((255 * (1 - (((time - timetotransparent) / (totaltimebeforepurge - timetotransparent))))), 0), 255)
                            ,
                            (int)((255 - Math.Sqrt(2.5) * 10))
                            ,
                            (int)((100 + (int)Math.Sqrt(2.5) * 10))
                            ,
                            0
                            )
                     );

                }
            }
            return thisframe;
        }
        GraphicsObject returnRenderableMeshes(Vector2 CameraPosition, double zoomScalar, double size, SolidBrush brush)
        {
            GraphicsObject renderObjectlist;

            GraphicsPath renderObject = new GraphicsPath();

            Vector2 playerposition = Camera.GetLocalPosition(new Vector2(Position.X, Position.Y));

            playerposition = new Vector2(playerposition.X * zoomScalar, playerposition.Y * zoomScalar);

            double constantincrease = Math.PI / 24;

            for (double i = constantincrease; i < Math.PI * 2; i += constantincrease)
            {
                renderObject.AddLine(
                      (int)(((float)playerposition.X + (float)CameraPosition.X) + Math.Cos(i - constantincrease) * size * zoomScalar)
                    , (int)(((float)playerposition.Y + (float)CameraPosition.Y) + Math.Sin(i - constantincrease) * size * zoomScalar)
                    , (int)(((float)playerposition.X + (float)CameraPosition.X) + Math.Cos(i) * size * zoomScalar)
                    , (int)(((float)playerposition.Y + (float)CameraPosition.Y) + Math.Sin(i) * size * zoomScalar));
            }
            Matrix testmatrix = new Matrix();
            testmatrix.RotateAt((float)mathExtension.radianToDegree(Camera.Rotation), new PointF((float)playerposition.X + (float)CameraPosition.X, (float)playerposition.Y + (float)CameraPosition.Y));
            renderObject.Transform(testmatrix);
            //renderObject.AddEllipse((float)playerposition.X + (float)CameraPosition.X, (float)playerposition.Y + (float)CameraPosition.Y, (float)Size, (float)Size);
            renderObject.CloseFigure();
            renderObjectlist = new GraphicsObject(renderObject, brush);

            return renderObjectlist;
        }
    }
    public static class mathExtension
    {
        public static double returnPositiveNumber(double number)
        {
            return Math.Sqrt(number * number);
        }
        public static double radianToDegree(double radian)
        {
            return (radian / Math.PI) * 180;
        }
    }
}
