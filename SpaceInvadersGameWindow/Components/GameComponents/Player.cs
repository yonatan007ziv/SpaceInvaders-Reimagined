﻿using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using GameWindow.Factories;
using GameWindow.Systems;
using System.Numerics;
using System.Threading.Tasks;

namespace GameWindow.Components.GameComponents
{
    /// <summary>
    /// A class describing a local player
    /// </summary>
    internal class Player
    {
        public const float INVINCIBILITY_PERIOD = 1.5f;

        public Transform transform;
        private Sprite sprite;
        public Collider col;

        private PlayerController controller;
        private bool invincible = false;

        /// <summary>
        /// Builds a local player
        /// </summary>
        /// <param name="pos"> position of the player </param>
        public Player(Vector2 pos)
        {
            transform = new Transform(new Vector2(13, 8), pos);
            col = new Collider(transform, this, CollisionLayer.Player);
            sprite = UIElementFactory.CreateSprite(transform, Image.Player);

            controller = new PlayerController(this);

            // Suppressing the "Null When Leaving a Constructor" warning
            sprite!.ToString();
        }

        /// <summary>
        /// <list type="number">
        ///     <item> Checks if no lives left, if so, ends the game and returns</item>
        ///     <item> Disables input </item>
        ///     <item> Plays death sound </item>
        ///     <item> Freezes the <see cref="LocalGame"/> </item>
        ///     <item> Plays the death animation </item>
        ///     <item> Unfreezes the <see cref="LocalGame"/> </item>
        ///     <item> Respawns the current <see cref="Player"/> </item>
        ///     <item> Plays the respawn animation </item>
        /// </list>
        /// </summary>
        public async void Kill()
        {
            if (invincible) return;

            if (--LocalGame.instance!.LivesLeft == 0)
            {
                LocalGame.instance.Lost();
                return;
            }

            invincible = true;
            PlayerController.disabled = true;

            SoundManager.StopAllSounds();
            SoundManager.PlaySound(Sound.PlayerDeath);
            transform.Scale = new Vector2(16, 8);

            LocalGame.FreezeUnfreeze(true);
            await DeathAnimation();
            LocalGame.FreezeUnfreeze(false);

            Respawn();
            await InvincibilityAnimation();
            invincible = false;
        }

        /// <summary>
        /// Plays the death animation
        /// </summary>
        /// <returns> A task representing the asynchronous operation of the death animation </returns>
        private async Task DeathAnimation()
        {
            for (int i = 0; i < 12; i++)
            {
                sprite.ChangeImage(i % 2 == 0 ? Image.PlayerDeath1 : Image.PlayerDeath2);
                await Task.Delay(1000 / (MainWindow.TARGET_FPS / 6));
            }
        }

        /// <summary>
        /// Respawns the <see cref="Player"/>
        /// </summary>
        private void Respawn()
        {
            transform.Scale = new Vector2(13, 8);
            sprite.ChangeImage(Image.Player);
            PlayerController.disabled = false;
        }

        /// <summary>
        /// Run invincibility for <see cref="INVINCIBILITY_PERIOD"/> seconds
        /// </summary>
        /// <returns> A task representing the asynchronous operation of the invincibility animation </returns>
        private async Task InvincibilityAnimation()
        {
            for (int i = 0; i < INVINCIBILITY_PERIOD * 10; i++)
            {
                sprite.Visible(i % 2 == 0);
                await Task.Delay(1000 / 10);
            }
            sprite.Visible(true);
        }

        public static void Pause()
        {
            PlayerController.disabled = true;
        }

        public static void Unpause()
        {
            PlayerController.disabled = false;
        }

        /// <summary>
        /// Disposes the current <see cref="Player"/> object
        /// </summary>
        public void Dispose()
        {
            controller.Dispose();
            col.Dispose();
            sprite.Dispose();
            transform.Dispose();
        }
    }
}