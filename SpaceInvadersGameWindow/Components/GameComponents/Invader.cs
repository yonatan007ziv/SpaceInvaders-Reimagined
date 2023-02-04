using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Components.Renderer;
using SpaceInvaders.Components.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using SpaceInvaders.Systems;
using System.Runtime.InteropServices;
using SpaceInvadersGameWindow.Components.UIElements;

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
            sprite = new Sprite(transform);

            this.type = type;
            switch (type)
            {
                default:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\RawFiles\Images\MissingSprite.png");
                    throw new Exception();
                case EnemyTypes.Octopus:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\RawFiles\Images\Enemies\Octopus1.png");
                    pointsReward = 10;
                    break;
                case EnemyTypes.Crab:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\RawFiles\Images\Enemies\Crab1.png");
                    pointsReward = 20;
                    break;
                case EnemyTypes.Squid:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\RawFiles\Images\Enemies\Squid1.png");
                    pointsReward = 30;
                    break;
                case EnemyTypes.UFO:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\RawFiles\Images\Enemies\UFO.png");
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
            int rand = random.Next(25);
            if (rand == 0)
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
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\RawFiles\Images\MissingSprite.png");
                    throw new Exception();
                case EnemyTypes.Octopus:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\RawFiles\Images\Enemies\Octopus{clipNum}.png");
                    pointsReward = 10;
                    break;
                case EnemyTypes.Crab:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\RawFiles\Images\Enemies\Crab{clipNum}.png");
                    pointsReward = 20;
                    break;
                case EnemyTypes.Squid:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\RawFiles\Images\Enemies\Squid{clipNum}.png");
                    pointsReward = 30;
                    break;
                case EnemyTypes.UFO:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\RawFiles\Images\Enemies\UFO.png");
                    pointsReward = 100;
                    break;
            }
        }
        public async void Death()
        {
            Dispose();

            SoundManager.PlaySound(@"Resources\RawFiles\Sounds\InvaderDeath.wav");

            // Invader Explosion
            Transform ExplosionTransform = new Transform(new Vector2(13, 8), transform.Position);
            Sprite ExplosionSprite = new Sprite(ExplosionTransform, @"Resources\RawFiles\Images\Enemies\InvaderDeath.png");

            //GameSettings.score += pointsReward;

            await Task.Delay(500);

            ExplosionTransform.Dispose();
            ExplosionSprite.Dispose();
        }
        private void Dispose()
        {
            invaders.Remove(this);
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
                    invader.sprite.image.IsEnabled = false;
                }

            for (int i = invaders.Count - 1; i >= 0; i--)
                invaders[i].sprite.image.IsEnabled = true;
        }

    }
}