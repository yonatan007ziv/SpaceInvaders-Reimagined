﻿using GameWindow.Components.Miscellaneous;
using GameWindow.Components.Pages;
using GameWindow.Components.UIElements;
using GameWindow.Factories;
using GameWindow.Systems;
using System.Numerics;
using System.Windows.Input;

namespace GameWindow.Components.GameComponents
{
    /// <summary>
    /// A class implementing a local game
    /// </summary>
    internal class LocalGame
    {
        public static LocalGame? instance;

        private Player player;
        private bool gaveExtraLife = false;
        private int score = 0;
        private int livesLeft = 3;

        public int Score
        {
            get { return score; }
            set { score = value; creditsLabel.Text = "CREDIT " + score; if (score >= 1500 && !gaveExtraLife) { LivesLeft++; gaveExtraLife = true; } }
        }
        public int LivesLeft
        {
            get { return livesLeft; }
            set { livesLeft = value; livesLabel.Text = "LIVES: " + livesLeft; }
        }

        private CustomLabel creditsLabel;
        private CustomLabel livesLabel;
        private CustomLabel? lostLabel;
        private CustomButton? playAgainButton;
        private CustomButton? mainMenuButton;

        /// <summary>
        /// Builds a local game
        /// </summary>
        public LocalGame()
        {
            instance = this;

            creditsLabel = UIElementFactory.CreateLabel(new Transform(new Vector2(50, 50), new Vector2(MainWindow.referenceSize.X / 1.25f, MainWindow.referenceSize.Y / 1.15f)), "", System.Windows.Media.Colors.White);
            livesLabel = UIElementFactory.CreateLabel(new Transform(new Vector2(50, 50), new Vector2(25, MainWindow.referenceSize.Y / 1.15f)), "", System.Windows.Media.Colors.White);
            player = new Player(new Vector2(50, MainWindow.referenceSize.Y * 0.735f));

            StartGame();
        }

        #region Game Preparation

        /// <summary>
        /// Starts a new game
        /// </summary>
        public async void StartGame()
        {
            creditsLabel?.Dispose();
            livesLabel?.Dispose();
            player?.Dispose();

            creditsLabel = UIElementFactory.CreateLabel(new Transform(new Vector2(50, 50), new Vector2(MainWindow.referenceSize.X / 1.25f, MainWindow.referenceSize.Y / 1.15f)), "", System.Windows.Media.Colors.White);
            livesLabel = UIElementFactory.CreateLabel(new Transform(new Vector2(50, 50), new Vector2(25, MainWindow.referenceSize.Y / 1.15f)), "", System.Windows.Media.Colors.White);
            player = new Player(new Vector2(50, MainWindow.referenceSize.Y * 0.735f));

            InputHandler.Disable(true);
            Player.Pause();
            Wall.MakeLocalGameWalls();
            MakeBunkers();

            gaveExtraLife = false;
            Score = 0;
            LivesLeft = 3;

            InputHandler.AddInputLoop(InputLoop);

            await Invader.PlotInvaders();
            Invader.UnpauseAll();
            Player.Unpause();
            InputHandler.Disable(false);
        }

        /// <summary>
        /// Makes the bunkers
        /// </summary>
        private static void MakeBunkers()
        {
            Bunker.AllBunkers[0] = new Bunker(new Vector2(0.4f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3));
            Bunker.AllBunkers[1] = new Bunker(new Vector2(0.8f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3));
            Bunker.AllBunkers[2] = new Bunker(new Vector2(1.2f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3));
            Bunker.AllBunkers[3] = new Bunker(new Vector2(1.6f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3));
        }
        #endregion

        #region Pause Menu
        public static bool Paused;
        private static bool HeldRestart;
        private static bool heldEscape;
        private static LocalPauseMenu? pauseMenu;

        /// <summary>
        /// Input Loop for the local game
        /// </summary>
        private void InputLoop()
        {
            if (InputHandler.keysDown.Contains(Key.Escape))
            {
                if (!heldEscape)
                    PauseUnpause(!Paused);
                heldEscape = true;
            }
            else
                heldEscape = false;

            if (InputHandler.keysDown.Contains(Key.R))
            {
                if (!HeldRestart)
                {
                    Invader.PauseAll();
                    Dispose();
                    pauseMenu?.Dispose();
                    StartGame();
                }
                HeldRestart = true;
            }
            else
                HeldRestart = false;
        }

        /// <summary>
        /// Pauses or Unpauses the current <see cref="LocalGame"/>
        /// </summary>
        /// <param name="pause"> Whether to pause or unpause </param>
        public static void PauseUnpause(bool pause)
        {
            Paused = pause;
            if (pause)
            {
                SoundManager.StopAllSounds();
                Bullet.PauseAll();
                Invader.PauseAll();
                Player.Pause();
                pauseMenu = new LocalPauseMenu();
            }
            else
            {
                Bullet.UnpauseAll();
                Invader.UnpauseAll();
                Player.Unpause();
                pauseMenu?.Dispose();
            }
        }

        /// <summary>
        /// Freezes the game or Unfreezes the game
        /// </summary>
        /// <param name="freeze"> Whether to freeze the game or unfreeze it </param>
        public static void FreezeUnfreeze(bool freeze)
        {
            if (freeze)
            {
                InputHandler.RemoveInputLoop(instance!.InputLoop);
                SoundManager.StopAllSounds();
                Bullet.PauseAll();
                Invader.PauseAll();
                Player.Pause();
            }
            else
            {
                InputHandler.AddInputLoop(instance!.InputLoop);
                Bullet.UnpauseAll();
                Invader.UnpauseAll();
                Player.Unpause();
            }
        }
        #endregion

        /// <summary>
        /// Called when the player loses the game <br/>
        /// Opens "LostMenu"
        /// </summary>
        public void Lost()
        {
            Dispose();

            lostLabel = UIElementFactory.CreateLabel(new Transform(new Vector2(MainWindow.referenceSize.X, MainWindow.referenceSize.Y), new Vector2(MainWindow.referenceSize.X / 2, MainWindow.referenceSize.Y / 2)),
                $"You Lost\nCredits: {Score}", System.Windows.Media.Colors.White);

            playAgainButton = UIElementFactory.CreateButton(new Transform(new Vector2(MainWindow.referenceSize.X / 5, MainWindow.referenceSize.Y / 5), new Vector2(MainWindow.referenceSize.X * 3 / 4, MainWindow.referenceSize.Y * 5 / 6)),
                () =>
                {
                    DisposeLostMenu();
                    Dispose();
                    StartGame();
                }, System.Windows.Media.Color.FromRgb(0, 255, 255), "Play Again");

            mainMenuButton = UIElementFactory.CreateButton(
                new Transform(new Vector2(MainWindow.referenceSize.X / 5, MainWindow.referenceSize.Y / 5), new Vector2(MainWindow.referenceSize.X / 4, MainWindow.referenceSize.Y * 5 / 6)),
                () =>
                {
                    DisposeLostMenu();
                    Dispose();
                    new GameMainMenu();
                }, System.Windows.Media.Color.FromRgb(0, 0, 0), "Main Menu");
        }

        /// <summary>
        /// Disposes the "LostMenu"
        /// </summary>
        private void DisposeLostMenu()
        {
            lostLabel?.Dispose();
            playAgainButton?.Dispose();
            mainMenuButton?.Dispose();
        }

        /// <summary>
        /// Disposes the current <see cref="LocalGame"/>
        /// </summary>
        public void Dispose()
        {
            SoundManager.StopAllSounds();
            InputHandler.RemoveInputLoop(InputLoop);
            Wall.DisposeAll();
            Bunker.DisposeAll();
            Bullet.DisposeAll();
            Invader.DisposeAll();
            Invader.Stage = 1;
            player.Dispose();
            livesLabel.Dispose();
            creditsLabel.Dispose();
        }
    }
}