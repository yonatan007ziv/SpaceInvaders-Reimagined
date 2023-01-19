using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Components.Renderer;
using SpaceInvaders.DataStructures;
using SpaceInvaders.Resources;
using SpaceInvaders.Components.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Transactions;
using System.Diagnostics;
using SpaceInvaders.Systems;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Security.Policy;

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
            this.type = type;
            switch (type)
            {
                default:
                    sR = new SpriteRenderer(Sprites.MissingSprite, pos);
                    throw new Exception();
                case EnemyTypes.Octopus:
                    sR = new SpriteRenderer(Sprites.Octopus1, pos);
                    pointsReward = 10;
                    break;
                case EnemyTypes.Crab:
                    sR = new SpriteRenderer(Sprites.Crab1, pos);
                    pointsReward = 20;
                    break;
                case EnemyTypes.Squid:
                    sR = new SpriteRenderer(Sprites.Squid1, pos);
                    pointsReward = 30;
                    break;
                case EnemyTypes.UFO:
                    sR = new SpriteRenderer(Sprites.UFO, pos);
                    pointsReward = 100;
                    break;
            }

            transform = new Transform(pos, new Vector2(sR.Size));
            col = new Collider(this, new Vector2(sR.Size), pos);

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
            Wall.WaitForNext = false;
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
            if (timesMoved % 2 == 0)
            {
                switch (type)
                {
                    default:
                        throw new Exception();
                    case EnemyTypes.Octopus:
                        sR.Image = Sprites.Octopus1;
                        pointsReward = 10;
                        break;
                    case EnemyTypes.Crab:
                        sR.Image = Sprites.Crab1;
                        pointsReward = 20;
                        break;
                    case EnemyTypes.Squid:
                        sR.Image= Sprites.Squid1;
                        pointsReward = 30;
                        break;
                    case EnemyTypes.UFO:
                        sR.Image = Sprites.UFO;
                        pointsReward = 100;
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    default:
                        throw new Exception();
                    case EnemyTypes.Octopus:
                        sR.Image = Sprites.Octopus2;
                        pointsReward = 10;
                        break;
                    case EnemyTypes.Crab:
                        sR.Image = Sprites.Crab2;
                        pointsReward = 20;
                        break;
                    case EnemyTypes.Squid:
                        sR.Image = Sprites.Squid2;
                        pointsReward = 30;
                        break;
                    case EnemyTypes.UFO:
                        sR.Image = Sprites.UFO;
                        pointsReward = 100;
                        break;
                }
            }
            sR.Update();
        }
        private void Dispose()
        {
            sR.Dispose();
            //col.Dispose();
        }
        public static async void PlotInvaders(Form form, int startX, int startY)
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 11; j++)
                {
                    Vector2 size;
                    EnemyTypes type;
                    if (i < 1)
                    {
                        type = EnemyTypes.Squid;
                        size = new Vector2(8, 8);
                    }
                    else if (i < 3)
                    {
                        type = EnemyTypes.Crab;
                        size = new Vector2(11, 8);
                    }
                    else
                    {
                        type = EnemyTypes.Octopus;
                        size = new Vector2(12, 8);
                    }
                    Invader invader = new Invader(type, new Vector2(j * 75 + startX, i * 75 + startY));
                    invader.transform.SetScale(size * 2);
                    invader.transform.SetScale(size * 4);
                }

            for (int i = invaders.Count - 1; i >= 0; i--)
            {
                form.Controls.Add(invaders[i].sR);
                await Task.Delay(10);
            }
        }

        public void ColliderHit(Collider hit)
        {
            if (hit.parent is Bullet)
                Kill();
        }
        async void Kill()
        {
            col.Dispose();

            invaders.Remove(this);

            //SoundManager.PlaySound(Sounds.InvaderDeath);

            sR.Image = Sprites.InvaderDeath;

            sR.SetScale(new Vector2(65, 40)); //* GameSettings.ScreenReferenceSize);

            GameSettings.score += pointsReward;
            await Task.Delay(500);
            Dispose();
        }
    }
}