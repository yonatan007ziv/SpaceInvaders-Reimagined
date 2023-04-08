﻿using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;

namespace GameWindow.Components.GameComponents
{
    public enum BulletType
    {
        Charge,
        Imperfect,
        ZigZag,
        Normal
    }

    class Bullet
    {
        public static List<Bullet> AllBullets = new List<Bullet>();

        public Transform transform;
        private float originalBulletSpeed;
        protected float bulletSpeed;
        protected Sprite sprite;
        protected Collider col;
        protected bool bulletHit;
        protected BulletType bulletType;

        public static void DisposeAll()
        {
            for (int i = 0; i < AllBullets.Count; i++)
            {
                if (AllBullets[i] == null) continue;

                AllBullets[i].bulletHit = true;
                AllBullets[i]?.col.Dispose();
                AllBullets[i]?.sprite.Dispose();
                AllBullets[i]?.transform.Dispose();
            }
            PlayerBullet.instance = null;
            AllBullets.Clear();
        }
        public static void PauseUnpauseBullets(bool pause)
        {
            for (int i = 0; i < AllBullets.Count; i++)
            {
                if (AllBullets[i] == null) continue;

                if (pause)
                    AllBullets[i].bulletSpeed = 0;
                else
                    AllBullets[i].bulletSpeed = AllBullets[i].originalBulletSpeed;
            }
        }
        public Bullet(Vector2 pos,int speed, BulletType bulletType, CollisionLayer colliderLayer)
        {
            AllBullets.Add(this);
            this.bulletType = bulletType;
            originalBulletSpeed = speed;
            bulletSpeed = originalBulletSpeed;
            transform = new Transform(new Vector2(3, 7), pos);
            col = new Collider(transform, this, colliderLayer);

            // UI Objects need to be created in an STA thread
            Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform, @"Resources\Images\Bullet\Bullet.png"));

            // Suppressing the "Null When Leaving a Constructor" warning
            sprite!.ToString();
        }
        public void BulletExplosion()
        {
            bulletHit = true;
            col.Dispose();

            // Bullet Explosion
            transform.Scale = new Vector2(6, 8);
            sprite.ChangeImage(@"Resources\Images\Bullet\BulletExplosion.png");

            Task.Delay(500).ContinueWith((p) => Dispose());
        }
        private void Dispose()
        {
            col.Dispose();
            sprite.Dispose();
            transform.Dispose();
            AllBullets.Remove(this);
        }
    }
}