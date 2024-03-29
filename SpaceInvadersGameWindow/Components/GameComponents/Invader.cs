﻿using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using GameWindow.Factories;
using GameWindow.Systems;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace GameWindow.Components.GameComponents
{
    /// <summary>
    /// Implements an enemy from the classic arcade game Space Invaders (1978)
    /// </summary>
    internal class Invader
    {
        /// <summary>
        /// Represents the types of available invaders in the game
        /// </summary>
        private enum InvaderType
        {
            Octopus, Crab, Squid, UFO,
        }

        // Constants
        private const int INVADER_SPEED_DOWN = 8;
        private const int INVADER_SPEED_SIDE = 2;
        private const int UFO_SPEED = 2;
        private const int MIN_TIME_BETWEEN_UFOS = 10;

        // Global variables
        private static Invader[] Invaders = new Invader[55];
        private static Invader? CurrentUFO = null;
        private static Random _Random = new Random();
        private static int InvaderCount = 0;
        public static int Stage = 1;

        // Uniform Invader Settings
        private static int InvaderDir = 1;
        private static bool SpriteSwitch = true;

        public static void PauseAll()
        {
            if (!DisposedCts)
                cts.Cancel();
            UFOPaused = true;
        }

        public static async void UnpauseAll()
        {
            cts = new CancellationTokenSource();
            DisposedCts = false;

            UFOPaused = false;
            UFOMovementLoop();

            try { await MovingInvadersLoop(cts.Token); }
            catch (OperationCanceledException) { cts.Dispose(); DisposedCts = true; }
        }

        private static void ResetInfo()
        {
            InvaderDir = 1;
            SpriteSwitch = true;

            InvaderCount = 0;
            CycleCount = 0;

            stopwatchUFO.Reset();
        }

        /// <summary>
        /// Plots the invaders on the screen
        /// </summary>
        /// <returns> A <see cref="Task"/> representing the async state of plotting </returns>
        public static async Task PlotInvaders()
        {
            ResetInfo();
            UFOPaused = true;

            int startX = (int)(MainWindow.referenceSize.X / 6);
            int startY = (int)(MainWindow.referenceSize.Y / 10);
            for (int i = 4; i >= 0; i--)
                for (int j = 10; j >= 0; j--)
                {
                    InvaderType type;
                    if (i < 1)
                        type = InvaderType.Squid;
                    else if (i < 3)
                        type = InvaderType.Crab;
                    else
                        type = InvaderType.Octopus;
                    new Invader(type, new Vector2(j * 16 + startX, i * 24 + startY));
                    await Task.Delay(25);
                }
            await Task.Delay(250);
            UFOPaused = false;
        }

        /// <summary>
        /// Disposes all invaders and resets the current state for the next game
        /// </summary>
        public static void DisposeAll()
        {
            if (!DisposedCts)
            {
                cts.Cancel();
                cts.Dispose();
            }

            for (int i = 0; i < Invaders.Length; i++)
                Invaders[i]?.Dispose();
            CurrentUFO?.Dispose();

            ResetInfo();
        }

        /// <summary>
        /// Builds an <see cref="Invader"/> opponent
        /// </summary>
        /// <param name="type"> The <see cref="InvaderType"/> of the invader </param>
        /// <param name="pos"> A <see cref="Vector2"/> representing the position of the invader on the screen </param>
        /// <exception cref="ArgumentException"> Thrown if <paramref name="type"/> is not valid </exception>
        private Invader(InvaderType type, Vector2 pos)
        {
            this.type = type;

            Image image;
            Vector2 scale;
            switch (type)
            {
                default:
                    throw new ArgumentException($"InvaderType {type} isn't Valid");
                case InvaderType.Octopus:
                    image = Image.Octopus1;
                    pointsReward = 10;
                    scale = new Vector2(12, 8);
                    break;
                case InvaderType.Crab:
                    image = Image.Crab1;
                    pointsReward = 20;
                    scale = new Vector2(11, 8);
                    break;
                case InvaderType.Squid:
                    image = Image.Squid1;
                    pointsReward = 30;
                    scale = new Vector2(8, 8);
                    break;
                case InvaderType.UFO:
                    image = Image.UFO;
                    int rand = _Random.Next(4);
                    pointsReward = rand == 3 ? 300 : (rand == 2 ? 150 : (rand == 1 ? 100 : 50)); // Random 300, 150, 100 or 50
                    scale = new Vector2(16, 8);
                    break;
            }

            transform = new Transform(scale, pos);
            col = new Collider(transform, this, CollisionLayer.Invader);
            sprite = UIElementFactory.CreateSprite(transform, image);

            if (type == InvaderType.UFO)
                CurrentUFO = this;
            else
                Invaders[arrPos = InvaderCount++] = this;

            // Suppressing the "Null When Leaving a Constructor" warning
            sprite!.ToString();
        }

        /// <summary>
        /// Resets the current invaders' info stored in the class' global scope
        /// </summary>

        #region Pause Unpause
        private static bool DisposedCts;
        private static CancellationTokenSource cts = new CancellationTokenSource();

        #endregion

        #region Invaders Cycle
        private static int CycleCount = 0;

        /// <summary>
        /// Executes the game loop for moving the invaders and checking win/lose conditions
        /// </summary>
        /// <param name="token"> The cancellation token used to stop the game loop </param>
        /// <returns> A <see cref="Task"/> representing the async state of the loop </returns>
        /// <exception cref="OperationCanceledException"> Thrown when the game loop is canceled via the cancellation token </exception>
        private static async Task MovingInvadersLoop(CancellationToken token)
        {
            while (true)
            {
                int toWait = (int)CycleTimeCurveMilliseconds();
                try { await Task.Delay(toWait, token); }
                catch { token.ThrowIfCancellationRequested(); }

                CycleInvaders();

                if (InvaderCount == 0 && LocalGame.instance!.LivesLeft > 0 && !cts.IsCancellationRequested)
                {
                    LocalGame.instance.Score += Stage++ * 55;
                    Bullet.DisposeAll();
                    InputHandler.Disable(true);
                    await PlotInvaders();
                    InputHandler.Disable(false);
                    UnpauseAll();
                    return;
                }
            }
        }

        /// <summary>
        /// Runs through the entire 'Space Invaders' movement cycle
        /// </summary>
        /// <remarks>
        /// The cycle includes the following steps:
        /// <list type="number">
        ///     <item> Decides if one of the invaders should shoot (random) </item>
        ///     <item> Decides the direction of the invaders (left or right) and whether they should go down </item>
        ///     <item> If the invaders need to move down, they move down. Otherwise, they move to the direction determined previously </item>
        ///     <item> Plays the classic 'Space Invaders' "Boop" sound </item>
        ///     <item> Checks if any of the invaders have reached the bottom of the screen </item>
        ///     <item> Generates a UFO (by chance) </item>
        /// </list>
        /// </remarks>
        public static void CycleInvaders()
        {
            if (_Random.Next(100) < Invaders.Length)
                Invaders[_Random.Next(Invaders.Length)]?.Shoot();

            bool shouldGoDown = false;
            for (int i = 0; i < Invaders.Length; i++)
                if (Invaders[i]?.DecideDir() ?? false)
                    shouldGoDown = true;

            if (shouldGoDown)
                MoveInvadersDown();
            else
                MoveInvadersSide();

            CycleBeatSound();

            for (int i = 0; i < Invaders.Length; i++)
                if (Invaders[i]?.CheckLose() ?? false)
                {
                    LocalGame.instance!.Lost();
                    return;
                }

            GenerateUFO();

            SpriteSwitch = !SpriteSwitch;
            CycleCount++;
        }

        /// <summary>
        /// Plays the classic 'Space Invaders' "Boop" sound each new invader movement cycle
        /// </summary>
        private static void CycleBeatSound()
        {
            switch (CycleCount % 2)
            {
                case 0:
                    SoundManager.PlaySound(Sound.CycleBeat1, 1);
                    break;
                case 1:
                    SoundManager.PlaySound(Sound.CycleBeat2, 1);
                    break;
            }
        }

        /// <summary>
        /// Moves all invaders to the side at speed <see cref="INVADER_SPEED_SIDE"/>. Direction determined by <see cref="InvaderDir"/>
        /// </summary>
        private static void MoveInvadersSide()
        {
            for (int i = 0; i < Invaders.Length; i++)
            {
                if (Invaders[i] == null) continue;

                Invaders[i].transform.Position += new Vector2(INVADER_SPEED_SIDE * InvaderDir, 0);
                Invaders[i].NextClip();
            }
        }

        /// <summary>
        /// Moves all invaders down at speed <see cref="INVADER_SPEED_DOWN"/>
        /// </summary>
        private static void MoveInvadersDown()
        {
            for (int i = 0; i < Invaders.Length; i++)
            {
                if (Invaders[i] == null) continue;

                Invaders[i].transform.Position += new Vector2(0, INVADER_SPEED_DOWN);
                Invaders[i].NextClip();
            }
        }

        /// <summary>
        /// Implements y = mx + b<br/>
        /// Where:<br/>
        /// y = Time Between Cycles<br/>
        /// x = Number of Alive Invaders<br/>
        /// m = Current Slope (5 Different Stages)<br/>
        /// b = Current 'y' Intercept (5 Different Stages)
        /// </summary>
        /// <returns> The Time Between Invader Movement Cycles In Milliseconds </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown if the number of invaders is outside the valid range of (0 &lt;= <see cref="InvaderCount"/> &lt;= 55) </exception>
        private static double CycleTimeCurveMilliseconds()
        {
            if (0 <= InvaderCount && InvaderCount <= 11)
                return Am * InvaderCount + Ab;
            else if (11 <= InvaderCount && InvaderCount <= 22)
                return Bm * InvaderCount + Bb;
            else if (22 <= InvaderCount && InvaderCount <= 33)
                return Cm * InvaderCount + Cb;
            else if (33 <= InvaderCount && InvaderCount <= 44)
                return Dm * InvaderCount + Db;
            else if (44 <= InvaderCount && InvaderCount <= 55)
                return Em * InvaderCount + Eb;
            throw new ArgumentOutOfRangeException($"Number of Invaders Outside Bounds: (0 <= {InvaderCount} <= 55)");
        }

        #region curve
        /// <summary>
        /// The Following Points are Implemented:
        /// (1, 3) (11, 25) (22, 75) (33, 250) (44, 500) (55, 1250)
        /// </summary>
        private const double Ax1 = 1, Ay1 = 3, Ax2 = 11, Ay2 = 25;
        private const double Bx1 = 11, By1 = 25, Bx2 = 22, By2 = 75;
        private const double Cx1 = 22, Cy1 = 75, Cx2 = 33, Cy2 = 250;
        private const double Dx1 = 33, Dy1 = 250, Dx2 = 44, Dy2 = 500;
        private const double Ex1 = 44, Ey1 = 500, Ex2 = 55, Ey2 = 1250;

        // Calculating slopes and 'y' intercepts at compile time
        private const double Am = (Ay2 - Ay1) / (Ax2 - Ax1);
        private const double Bm = (By2 - By1) / (Bx2 - Bx1);
        private const double Cm = (Cy2 - Cy1) / (Cx2 - Cx1);
        private const double Dm = (Dy2 - Dy1) / (Dx2 - Dx1);
        private const double Em = (Ey2 - Ey1) / (Ex2 - Ex1);
        private const double Ab = Ay1 - Am * Ax1;
        private const double Bb = By1 - Bm * Bx1;
        private const double Cb = Cy1 - Cm * Cx1;
        private const double Db = Dy1 - Dm * Dx1;
        private const double Eb = Ey1 - Em * Ex1;
        #endregion
        #endregion

        #region UFO
        private static bool UFOPaused = false;
        private static Stopwatch stopwatchUFO = new Stopwatch();

        /// <summary>
        /// Generates a new UFO randomly, if the following conditions are met:
        /// <list type="bullet">
        ///     <item> Elapsed at least <see cref="MIN_TIME_BETWEEN_UFOS"/> seconds from last UFO </item>
        ///     <item> <see cref="InvaderCount"/> larger or equal to 8 </item>
        ///     <item> <see cref="CurrentUFO"/> is null </item>
        /// </list>
        /// </summary>
        private static void GenerateUFO()
        {
            if (_Random.Next(50) == 0 && stopwatchUFO.Elapsed.Seconds >= MIN_TIME_BETWEEN_UFOS && InvaderCount >= 8 && CurrentUFO == null)
            {
                new Invader(InvaderType.UFO, new Vector2(-25, 15));
                UFOMovementLoop();
            }
        }

        /// <summary>
        /// Handles the UFO movement
        /// </summary>
        /// <remarks>
        /// The UFO is moved towards the right side of the screen at a constant speed <see cref="UFO_SPEED"/><br/>
        /// When the UFO leaves the screen, it is disposed
        /// </remarks>
        private static async void UFOMovementLoop()
        {
            bool playingSound = false;
            while (!(CurrentUFO == null || UFOPaused))
            {
                if (!playingSound)
                {
                    SoundManager.PlaySound(Sound.UFO);
                    playingSound = true;
                }

                CurrentUFO.transform.Position += new Vector2(UFO_SPEED, 0);

                if (CurrentUFO.transform.Position.X > MainWindow.referenceSize.X + 16)
                {
                    stopwatchUFO.Restart();
                    SoundManager.StopSound(Sound.UFO);
                    CurrentUFO.Dispose();
                    CurrentUFO = null;
                }

                await Task.Delay(1000 / MainWindow.TARGET_FPS);
            }
        }

        /// <summary>
        /// Handles the death of a UFO, updating the game score accordingly
        /// </summary>
        private void UFODeath()
        {
            stopwatchUFO.Restart();
            CurrentUFO = null;
            col.Dispose();

            SoundManager.StopSound(Sound.UFO);
            SoundManager.PlaySound(Sound.InvaderDeath);

            // Invader Explosion
            transform.Scale = new Vector2(13, 8);
            sprite.ChangeImage(Image.InvaderDeath);

            LocalGame.instance!.Score += pointsReward;

            Task.Delay(250).ContinueWith((p) => { Dispose(); UFOCreditsBlink(); });
        }

        /// <summary>
        /// Blinks the score of the destroyed UFO at the UFO's position
        /// </summary>
        private async void UFOCreditsBlink()
        {
            CustomLabel credits = UIElementFactory.CreateLabel(new Transform(new Vector2(32, 16), transform.Position), pointsReward.ToString(), System.Windows.Media.Colors.White);

            for (int i = 0; i < 5; i++)
            {
                credits.Visible(i % 2 == 0);
                await Task.Delay(250);
            }
            credits.Dispose();
        }
        #endregion

        #region Invader Specific
        private Transform transform;
        private Sprite sprite;
        private Collider col;

        private int arrPos;
        private int pointsReward;
        private InvaderType type;

        /// <summary>
        /// Checks whether the invader have reached the bottom
        /// </summary>
        /// <returns> Whether the invader have reached the bottom </returns>
        private bool CheckLose()
        {
            return transform.Position.Y > 2 * MainWindow.referenceSize.Y / 3.25;
        }

        /// <summary>
        /// Updates <see cref="InvaderDir"/> according to the wall collisions
        /// </summary>
        /// <returns> Whether the invaders should move down </returns>
        private bool DecideDir()
        {
            // Check touching wall
            Collider? col = this.col.TouchingCollider();
            if (col != null)
            {
                int prevDir = InvaderDir;
                if (col.parent == Wall.RightWall)
                    InvaderDir = -1;
                else if (col.parent == Wall.LeftWall)
                    InvaderDir = 1;
                if (InvaderDir != prevDir)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Instantiates a new <see cref="InvaderBullet"/> instance at the invader's position
        /// </summary>
        private void Shoot()
        {
            new InvaderBullet(transform.Position);
        }

        /// <summary>
        /// Updates the sprite of the Invader to the next frame of its animation
        /// </summary>
        private void NextClip()
        {
            Image image;
            switch (type)
            {
                default:
                    throw new InvalidTimeZoneException();
                case InvaderType.Octopus:
                    image = SpriteSwitch ? Image.Octopus2 : Image.Octopus1;
                    pointsReward = 10;
                    break;
                case InvaderType.Crab:
                    image = SpriteSwitch ? Image.Crab2 : Image.Crab1;
                    pointsReward = 20;
                    break;
                case InvaderType.Squid:
                    image = SpriteSwitch ? Image.Squid2 : Image.Squid1;
                    pointsReward = 30;
                    break;
            }

            sprite.ChangeImage(image);
        }

        /// <summary>
        /// Kills the invader, plays death sound, adds points to the score, and disposes the explosion after a delay
        /// </summary>
        public void Kill()
        {
            if (type == InvaderType.UFO)
            {
                UFODeath();
                return;
            }

            InvaderCount--;
            Invaders[arrPos] = null!;
            col.Dispose();

            SoundManager.PlaySound(Sound.InvaderDeath);

            // Invader Explosion
            transform.Scale = new Vector2(13, 8);
            sprite.ChangeImage(Image.InvaderDeath);

            LocalGame.instance!.Score += pointsReward;

            Task.Delay(500).ContinueWith((p) => Dispose());
        }

        /// <summary>
        /// Disposes the current <see cref="Invader"/> object
        /// </summary>
        private void Dispose()
        {
            sprite.Dispose();
            col.Dispose();
            transform.Dispose();
        }
        #endregion
    }
}