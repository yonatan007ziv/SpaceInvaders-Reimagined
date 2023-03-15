using GameWindow.Components.Initializers;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Components.UIElements;
using GameWindow.Systems;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace GameWindow.Components.GameComponents
{
    internal class Invader
    {
        public static List<Invader> invaders = new List<Invader>();
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
            Vector2 scale;
            switch (type)
            {
                default:
                    throw new Exception();
                case EnemyTypes.Octopus:
                    pointsReward = 10;
                    scale = new Vector2(12, 8);
                    break;
                case EnemyTypes.Crab:
                    pointsReward = 20;
                    scale = new Vector2(11, 8);
                    break;
                case EnemyTypes.Squid:
                    pointsReward = 30;
                    scale = new Vector2(8, 8);
                    break;
                case EnemyTypes.UFO:
                    pointsReward = 100;
                    scale = new Vector2(16, 8);
                    break;
            }

            transform = new Transform(scale, pos);
            col = new Collider(transform, this, Collider.Layers.Invader);
            sprite = new Sprite(transform);

            this.type = type;
            switch (type)
            {
                default:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\Images\MissingSprite.png");
                    throw new Exception();
                case EnemyTypes.Octopus:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\Images\Enemies\Octopus1.png");
                    pointsReward = 10;
                    break;
                case EnemyTypes.Crab:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\Images\Enemies\Crab1.png");
                    pointsReward = 20;
                    break;
                case EnemyTypes.Squid:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\Images\Enemies\Squid1.png");
                    pointsReward = 30;
                    break;
                case EnemyTypes.UFO:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\Images\Enemies\UFO.png");
                    pointsReward = 100;
                    break;
            }

            invaders.Add(this);
        }

        private static void MoveInvadersDown()
        {
            for (int i = invaders.Count - 1; i >= 0; i--)
                invaders[i].transform.Position += new Vector2(0, 12);
        }
        public static void MoveInvaders()
        {
            foreach (Invader i in invaders)
                i.DecideDir();

            for (int i = invaders.Count - 1; i >= 0; i--)
                invaders[i].Move();

            SpriteSwitch = !SpriteSwitch;
        }

        public static int dir = 1;
        private static Random random = new Random();
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
        private void Move()
        {
            transform.Position += new Vector2(10 * dir, 0);
            NextClip();

            // 4% (arbitrary) Chance to shoot
            int rand = random.Next(100 / 4) + 1;
            if (rand == 1)
                Shoot();
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
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\Images\MissingSprite.png");
                    throw new Exception();
                case EnemyTypes.Octopus:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\Images\Enemies\Octopus{clipNum}.png");
                    pointsReward = 10;
                    break;
                case EnemyTypes.Crab:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\Images\Enemies\Crab{clipNum}.png");
                    pointsReward = 20;
                    break;
                case EnemyTypes.Squid:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\Images\Enemies\Squid{clipNum}.png");
                    pointsReward = 30;
                    break;
                case EnemyTypes.UFO:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\Images\Enemies\UFO.png");
                    pointsReward = 100;
                    break;
            }
        }
        public async void Death()
        {
            invaders.Remove(this);
            Dispose();

            SoundManager.PlaySound(@"Resources\Sounds\InvaderDeath.wav");

            // Invader Explosion
            Transform ExplosionTransform = new Transform(new Vector2(13, 8), transform.Position);
            Sprite ExplosionSprite = new Sprite(ExplosionTransform, @"Resources\Images\Enemies\InvaderDeath.png");

            LocalGame.instance!.Score += pointsReward;

            await Task.Delay(500);

            ExplosionTransform.Dispose();
            ExplosionSprite.Dispose();
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
        public static void PlotInvaders(int startX, int startY)
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 11; j++)
                {
                    EnemyTypes type;
                    if (i < 1)
                        type = EnemyTypes.Squid;
                    else if (i < 3)
                        type = EnemyTypes.Crab;
                    else
                        type = EnemyTypes.Octopus;
                    Invader invader = new Invader(type, new Vector2(j * 16 + startX, i * 24 + startY));
                    invader.sprite.image.IsEnabled = true;
                }
        }

        public static async void StartInvaders(LocalGame currentGame)
        {
            while (invaders.Count > 0)
            {
                MoveInvaders();
                await Task.Delay(invaders.Count * 25);
            }

            if (invaders.Count == 0)
                currentGame.Won();
            else
                currentGame.Lost();
        }
    }
}