using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Components.Renderer;
using SpaceInvaders.Resources;
using SpaceInvaders.Components.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using SpaceInvadersGameWindow;
using System.Diagnostics;

namespace SpaceInvaders.Components.GameComponents
{
    internal class Invader : ICollidable
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

            transform = new Transform(pos, scale);
            col = new Collider(this, transform.position, transform.scale);
            sR = new SpriteRenderer(transform.position, transform.scale);

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

            transform.AddScaleDel((vec) => col.SetScale(vec));
            transform.AddScaleDel((vec) => sR.SetScale(vec));

            transform.AddPositionDel((vec) => col.SetPosition(vec));
            transform.AddPositionDel((vec) => sR.SetPosition(vec));

            invaders.Add(this);
        }

        public static void MoveInvadersDown()
        {
            for (int i = invaders.Count - 1; i >= 0; i--)
                invaders[i].transform.AddPosY(50);
        }
        public static void MoveInvaders()
        {
            for (int i = invaders.Count - 1; i >= 0; i--)
                invaders[i].Move();
            timesMoved++;
        }
        public static int dir = 1;
        private static int timesMoved = 0;
        public void Move()
        {
            transform.AddPosX(10 * dir);

            NextClip();
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
        private void Dispose()
        {
            sR.Dispose();
            col.Dispose();
        }
        public static void PlotInvaders(int startX, int startY)
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 11; j++)
                {
                    Vector2 size;
                    EnemyTypes type;
                    if (i < 1)
                        type = EnemyTypes.Squid;
                    else if (i < 3)
                        type = EnemyTypes.Crab;
                    else
                        type = EnemyTypes.Octopus;
                    Invader invader = new Invader(type, new Vector2(j * 75 + startX, i * 75 + startY));
                    invader.sR.IsEnabled = false;
                }

            for (int i = invaders.Count - 1; i >= 0; i--)
                invaders[i].sR.IsEnabled = true;
        }

        async void Kill()
        {
            col.Dispose();

            invaders.Remove(this);

            //SoundManager.PlaySound(Sounds.InvaderDeath);

            sR.Source = SpriteRenderer.BitmapImageMaker(@"Resources\RawFiles\Images\Enemies\InvaderDeath.png");

            //sR.SetScale(new Vector2(65, 40)); //* GameSettings.ScreenReferenceSize);
            //transform.UpdatePosition();

            //GameSettings.score += pointsReward;

            await Task.Delay(500);
            Dispose();
        }
    }
}