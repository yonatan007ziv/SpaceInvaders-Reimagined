using GameWindow.Components.Initializers;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Components.UIElements;
using GameWindow.Systems;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;

namespace GameWindow.Components.GameComponents
{
    internal class Invader
    {
        public static List<Invader> invaders = new List<Invader>();
        public static Invader? currentUFO = null;
        public static float InvaderSpeed = 5;
        public static float UFOSpeed = 0.8f;
        public static async Task PlotInvaders()
        {
            int startX = (int)(MainWindow.referenceSize.X / 6);
            int startY = (int)(MainWindow.referenceSize.Y / 10);
            for (int i = 4; i >= 0; i--)
                for (int j = 10; j >= 0; j--)
                {
                    EnemyTypes type;
                    if (i < 1)
                        type = EnemyTypes.Squid;
                    else if (i < 3)
                        type = EnemyTypes.Crab;
                    else
                        type = EnemyTypes.Octopus;
                    new Invader(type, new Vector2(j * 16 + startX, i * 24 + startY));
                    await Task.Delay(25);
                }
            await Task.Delay(50);
        }

        public static async void StartInvaders(LocalGame currentGame, Player player)
        {
            while (invaders.Count > 0)
            {
                MoveInvaders();
                await Task.Delay(invaders.Count * 15);
            }

            if (currentGame.LivesLeft > 0)
            {
                Bullet.DisposeAll();
                player.DisableInput();
                await PlotInvaders();
                player.EnableInput();
                StartInvaders(currentGame, player);
            }
        }

        public enum EnemyTypes
        {
            Octopus, Crab, Squid, UFO,
        }

        public Transform transform;
        private Sprite sprite;
        private Collider col;

        private int pointsReward;
        private EnemyTypes type;
        public Invader(EnemyTypes type, Vector2 pos)
        {
            this.type = type;

            string imgPath;
            Vector2 scale;
            switch (type)
            {
                default:
                    imgPath = "";
                    pointsReward = -1;
                    scale = new Vector2(0, 0);
                    throw new Exception();
                case EnemyTypes.Octopus:
                    imgPath = @$"Resources\Images\Enemies\Octopus1.png";
                    pointsReward = 10;
                    scale = new Vector2(12, 8);
                    break;
                case EnemyTypes.Crab:
                    imgPath = @$"Resources\Images\Enemies\Crab1.png";
                    pointsReward = 20;
                    scale = new Vector2(11, 8);
                    break;
                case EnemyTypes.Squid:
                    imgPath = @$"Resources\Images\Enemies\Squid1.png";
                    pointsReward = 30;
                    scale = new Vector2(8, 8);
                    break;
                case EnemyTypes.UFO:
                    imgPath = @$"Resources\Images\Enemies\UFO.png";
                    pointsReward = 100;
                    scale = new Vector2(16, 8);
                    break;
            }

            transform = new Transform(scale, pos);
            col = new Collider(transform, this, Collider.Layers.Invader);

            // UI Objects need to be created in an STA thread
            Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform, imgPath));

            if (type == EnemyTypes.UFO)
                currentUFO = this;
            else
                invaders.Add(this);

            // Suppressing the "Null When Leaving a Constructor" warning
            sprite!.ToString();
        }

        private static Random random = new Random();
        public static void MoveInvaders()
        {
            foreach (Invader i in invaders)
                i.DecideDir();

            for (int i = invaders.Count - 1; i >= 0; i--)
                invaders[i].MoveSide();

            if (random.Next(50) < invaders.Count)
                invaders[random.Next(invaders.Count)].Shoot();

            GenerateUFO();

            SpriteSwitch = !SpriteSwitch;
        }

        #region UFO
        private static void GenerateUFO()
        {
            if (random.Next(50) == 0 && invaders.Count >= 8 && currentUFO == null)
                new Invader(EnemyTypes.UFO, new Vector2(-25, 15)).UFOMovementLoop();
        }
        private async void UFOMovementLoop()
        {
            //SoundManager.PlaySound(SoundManager.Sounds.UFO);
            while (currentUFO != null)
            {
                await Task.Delay(1000 / (MainWindow.TARGET_FPS * 2));
                transform.Position += new Vector2(UFOSpeed, 0);

                if (transform.Position.X > MainWindow.referenceSize.X)
                {
                    currentUFO = null;
                    Dispose();
                }
            }
        }
        #endregion

        private static void MoveInvadersDown()
        {
            foreach (Invader i in invaders)
                i.transform.Position += new Vector2(0, InvaderSpeed * 1.25f);
        }

        public static int dir = 1;
        private static bool SpriteSwitch = false;

        private void DecideDir()
        {
            // Check touching wall
            Collider? col = this.col.TouchingCollider();
            if (col != null)
            {
                int prevDir = dir;
                if (col.parent == Wall.RightWall)
                    dir = -1;
                else if (col.parent == Wall.LeftWall)
                    dir = 1;
                if (dir != prevDir)
                    MoveInvadersDown();
            }
        }
        private void MoveSide()
        {
            transform.Position += new Vector2(InvaderSpeed * dir, 0);
            NextClip();
        }
        private void Shoot()
        {
            new InvaderBullet(transform.Position, 1);
        }

        void NextClip()
        {
            int clipNum = SpriteSwitch ? 2 : 1;
            switch (type)
            {
                default:
                    sprite.ChangeImage(Sprite.BitmapFromPath($@"Resources\Images\MissingSprite.png"));
                    throw new Exception();
                case EnemyTypes.Octopus:
                    sprite.ChangeImage(Sprite.BitmapFromPath(@$"Resources\Images\Enemies\Octopus{clipNum}.png"));
                    pointsReward = 10;
                    break;
                case EnemyTypes.Crab:
                    sprite.ChangeImage(Sprite.BitmapFromPath($@"Resources\Images\Enemies\Crab{clipNum}.png"));
                    pointsReward = 20;
                    break;
                case EnemyTypes.Squid:
                    sprite.ChangeImage(Sprite.BitmapFromPath($@"Resources\Images\Enemies\Squid{clipNum}.png"));
                    pointsReward = 30;
                    break;
                case EnemyTypes.UFO:
                    sprite.ChangeImage(Sprite.BitmapFromPath($@"Resources\Images\Enemies\UFO.png"));
                    pointsReward = 100;
                    break;
            }
        }
        public void Death()
        {
            invaders.Remove(this);

            col.Dispose();

            //SoundManager.PlaySound("InvaderDeath");

            // Invader Explosion
            transform.Scale = new Vector2(13, 8);
            sprite.ChangeImage(Sprite.BitmapFromPath(@"Resources\Images\Enemies\InvaderDeath.png"));

            if (type == EnemyTypes.UFO)
            {
                int rand = random.Next(4);
                switch (rand)
                {
                    case 0:
                        pointsReward = 50;
                        break;
                    case 1:
                        pointsReward = 100;
                        break;
                    case 2:
                        pointsReward = 150;
                        break;
                    case 3:
                        pointsReward = 300;
                        break;
                }
                currentUFO = null;
            }


            LocalGame.instance!.Score += pointsReward;

            Task.Delay(500).ContinueWith((p) => Dispose());
        }
        public static void DisposeAll()
        {
            foreach (Invader i in invaders)
                i.Dispose();
            invaders.Clear();
        }
        private void Dispose()
        {
            sprite.Dispose();
            col.Dispose();
            transform.Dispose();
        }
    }
}