using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Components.Renderer;
using SpaceInvaders.Components.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using SpaceInvadersGameWindow;
using SpaceInvaders.Systems;

namespace SpaceInvaders.Components.GameComponents
{
    internal class Invader
    {
        public static List<Invader> invaders = new List<Invader>();
        public enum EnemyTypes
        {
            Octopus, Crab, Squid, UFO,
        }

        public Transform transform;
        private SpriteRenderer sR;
        private Collider col;

        private int pointsReward;
        private EnemyTypes type;
        public Invader(EnemyTypes type, Vector2 pos)
        {
            Vector2 scale;
            switch (type)
            {
                default:
                    scale = new Vector2(0, 0);
                    throw new Exception();
                case EnemyTypes.Octopus:
                    scale = new Vector2(12, 8);
                    break;
                case EnemyTypes.Crab:
                    scale = new Vector2(11, 8);
                    break;
                case EnemyTypes.Squid:
                    scale = new Vector2(8, 8);
                    break;
                case EnemyTypes.UFO:
                    scale = new Vector2(16, 8);
                    break;
            }

            transform = new Transform(scale, pos);
            col = new Collider(transform, this);
            sR = new SpriteRenderer(transform);

            this.type = type;
            int times = 1;
            switch (type)
            {
                default:
                    sR.Source = SpriteRenderer.BitmapImageMaker(@$"Resources\RawFiles\Images\MissingSprite.png");
                    throw new Exception();
                case EnemyTypes.Octopus:
                    sR.Source = SpriteRenderer.BitmapImageMaker(@$"Resources\RawFiles\Images\Enemies\Octopus{times}.png");
                    pointsReward = 10;
                    break;
                case EnemyTypes.Crab:
                    sR.Source = SpriteRenderer.BitmapImageMaker(@$"Resources\RawFiles\Images\Enemies\Crab{times}.png");
                    pointsReward = 20;
                    break;
                case EnemyTypes.Squid:
                    sR.Source = SpriteRenderer.BitmapImageMaker(@$"Resources\RawFiles\Images\Enemies\Squid{times}.png");
                    pointsReward = 30;
                    break;
                case EnemyTypes.UFO:
                    sR.Source = SpriteRenderer.BitmapImageMaker(@$"Resources\RawFiles\Images\Enemies\UFO.png");
                    pointsReward = 100;
                    break;
            }

            invaders.Add(this);
        }

        private static void MoveInvadersDown()
        {
            for (int i = invaders.Count - 1; i >= 0; i--)
                invaders[i].transform.AddPosY(24);
        }
        public static void MoveInvaders()
        {
            for (int i = invaders.Count - 1; i >= 0; i--)
                invaders[i].Move();
            timesMoved++;
        }

        public static int dir = 1;
        private static Random random = new Random();
        private static int timesMoved = 0;

        private void Move()
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

            // 1% (temp, arbitrary) Chance to shoot
            int rand = random.Next(100);
            if (rand == 0)
                Shoot();

            transform.AddPosX(10 * dir);
            NextClip();
        }
        private void Shoot()
        {
            new InvaderBullet(transform.position);
        }

        void NextClip()
        {
            int times = 2;
            if (timesMoved % 2 == 0)
                times = 1;
            switch (type)
            {
                default:
                    sR.Source = SpriteRenderer.BitmapImageMaker(@$"Resources\RawFiles\Images\MissingSprite.png");
                    throw new Exception();
                case EnemyTypes.Octopus:
                    sR.Source = SpriteRenderer.BitmapImageMaker(@$"Resources\RawFiles\Images\Enemies\Octopus{times}.png");
                    pointsReward = 10;
                    break;
                case EnemyTypes.Crab:
                    sR.Source = SpriteRenderer.BitmapImageMaker(@$"Resources\RawFiles\Images\Enemies\Crab{times}.png");
                    pointsReward = 20;
                    break;
                case EnemyTypes.Squid:
                    sR.Source = SpriteRenderer.BitmapImageMaker(@$"Resources\RawFiles\Images\Enemies\Squid{times}.png");
                    pointsReward = 30;
                    break;
                case EnemyTypes.UFO:
                    sR.Source = SpriteRenderer.BitmapImageMaker(@$"Resources\RawFiles\Images\Enemies\UFO.png");
                    pointsReward = 100;
                    break;
            }
        }
        public async void Death()
        {
            Dispose();

            SoundManager.PlaySound(@"Resources\RawFiles\Sounds\InvaderDeath.wav");

            // Invader Explosion
            Transform ExplosionTransform = new Transform(new Vector2(25, 8), transform.position);
            SpriteRenderer ExplosionSR = new SpriteRenderer(transform, @"Resources\RawFiles\Images\Enemies\InvaderDeath.png");

            //GameSettings.score += pointsReward;

            await Task.Delay(500);

            ExplosionTransform.Dispose();
            ExplosionSR.Dispose();
        }
        private void Dispose()
        {
            invaders.Remove(this);
            sR.Dispose();
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
                    Invader invader = new Invader(type, new Vector2(j * 16 * MainWindow.ratio + startX, i * 24 * MainWindow.ratio + startY));
                    invader.sR.IsEnabled = false;
                }

            for (int i = invaders.Count - 1; i >= 0; i--)
                invaders[i].sR.IsEnabled = true;
        }
    }
}