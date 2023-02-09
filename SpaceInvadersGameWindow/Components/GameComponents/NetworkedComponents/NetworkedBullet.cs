﻿using SpaceInvaders.Components.GameComponents;
using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Systems;
using SpaceInvadersGameWindow.Components.GameComponents.NetworkedComponents;
using SpaceInvadersGameWindow.Components.UIElements;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SpaceInvadersGameWindow.Components.GameComponents
{
    class NetworkedBullet
    {
        public delegate void hit(string s);
        private hit onBulletHit;

        public Transform transform;
        private Sprite sprite;
        private Collider? col;
        private float bulletSpeed = 3;

        public NetworkedBullet(Vector2 pos, hit onBulletHit)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                transform = new Transform(new Vector2(3, 7), pos);
                col = new Collider(transform, this);
                sprite = new Sprite(transform, @"Resources\RawFiles\Images\Bullet\Bullet.png");
            });

            this.onBulletHit = onBulletHit;
            LocalBulletLoop();
        }
        public NetworkedBullet(Vector2 pos)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                transform = new Transform(new Vector2(3, 7), pos);
                col = new Collider(transform, this);
                sprite = new Sprite(transform, @"Resources\RawFiles\Images\Bullet\Bullet.png");
            });

            OnlineBulletLoop();
        }

        public async void LocalBulletLoop()
        {
            Vector2 SpeedVector = new Vector2(0, -bulletSpeed);
            while (col!.TouchingCollider() == null || col.TouchingCollider()!.parent is NetworkedPlayer || col.TouchingCollider()!.parent is Bullet)
            {
                transform.Position += SpeedVector;
                await Task.Delay(1000 / MainWindow.TargetFPS);
            }

            onBulletHit($"BULLET EXPLOSION POS:({transform.Position.X},{transform.Position.Y})");
            BulletExplosion();
        }
        public async void OnlineBulletLoop()
        {
            Vector2 SpeedVector = new Vector2(0, -bulletSpeed);
            while (col!.TouchingCollider() == null || col.TouchingCollider()!.parent is NetworkedPlayer || col.TouchingCollider()!.parent is Bullet)
            {
                transform.Position += SpeedVector;
                await Task.Delay(1000 / MainWindow.TargetFPS);
            }

            Dispose();
        }
        public async void BulletExplosion()
        {
            SoundManager.PlaySound(@"Resources\RawFiles\Sounds\BulletExplosion.wav");

            // Bullet Explosion
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                Transform ExplosionTransform = new Transform(new Vector2(6, 8), transform.Position);
                Sprite ExplosionSprite = new Sprite(ExplosionTransform, @"Resources\RawFiles\Images\Bullet\BulletExplosion.png");
                
                await Task.Delay(500);

                ExplosionTransform.Dispose();
                ExplosionSprite.Dispose();
            });

            Dispose();
        }

        public void Dispose()
        {
            sprite.Dispose();
            col?.Dispose();
            transform.Dispose();
        }
    }
}