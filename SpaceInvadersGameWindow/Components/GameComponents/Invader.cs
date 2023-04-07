using GameWindow.Components.Initializers;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using GameWindow.Systems;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GameWindow.Components.GameComponents
{
    internal class Invader
    {
        public enum EnemyTypes
        {
            Octopus, Crab, Squid, UFO,
        }

        // Constants
        private const float InvaderSpeedDown = 8;
        private const float InvaderSpeedSide = 2;
        private const float UFOSpeed = 1;

        // Global Settings
        private static int dir = 1;
        private static bool SpriteSwitch = false;

        // Global variables
        private static Invader?[] invaders = new Invader[55];
        private static Invader? currentUFO = null;
        private static Random random = new Random();
        private static int invaderCount = 0;

        public Invader(EnemyTypes type, Vector2 pos)
        {
            this.type = type;

            string imgPath;
            Vector2 scale;
            switch (type)
            {
                default:
                    imgPath = "";
                    pointsReward = -1;
                    scale = new Vector2(0, 0);
                    throw new Exception();
                case EnemyTypes.Octopus:
                    imgPath = @$"Resources\Images\Enemies\Octopus1.png";
                    pointsReward = 10;
                    scale = new Vector2(12, 8);
                    break;
                case EnemyTypes.Crab:
                    imgPath = @$"Resources\Images\Enemies\Crab1.png";
                    pointsReward = 20;
                    scale = new Vector2(11, 8);
                    break;
                case EnemyTypes.Squid:
                    imgPath = @$"Resources\Images\Enemies\Squid1.png";
                    pointsReward = 30;
                    scale = new Vector2(8, 8);
                    break;
                case EnemyTypes.UFO:
                    imgPath = @$"Resources\Images\Enemies\UFO.png";
                    int rand = random.Next(4);
                    pointsReward = rand == 3 ? 300 : (rand == 2 ? 150 : (rand == 1 ? 100 : 50)); // Random 300, 150, 100 or 50
                    scale = new Vector2(16, 8);
                    break;
            }

            transform = new Transform(scale, pos);
            col = new Collider(transform, this, Collider.Layers.Invader);

            // UI Objects need to be created in an STA thread
            Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform, imgPath));

            if (type == EnemyTypes.UFO)
                currentUFO = this;
            else
                invaders[arrPos = invaderCount++] = this;

            // Suppressing the "Null When Leaving a Constructor" warning
            sprite!.ToString();
        }

        private static void ResetInfo()
        {
            dir = 1;
            SpriteSwitch = false;

            invaderCount = 0;

            CycleCount = 0;
            FinishedCycle = true;
            GoingDown = false;
            LastInvaderSide = 0;
            LastInvaderDown = 0;
        }

        // Plots the invaders
        public static async Task PlotInvaders()
        {
            ResetInfo();
            UFOPaused = true;

            int startX = (int)(MainWindow.referenceSize.X / 6);
            int startY = (int)(MainWindow.referenceSize.Y / 10);
            for (int i = 4; i >= 0; i--)
                for (int j = 10; j >= 0; j--)
                {
                    EnemyTypes type;
                    if (i < 1)
                        type = EnemyTypes.Squid;
                    else if (i < 3)
                        type = EnemyTypes.Crab;
                    else
                        type = EnemyTypes.Octopus;
                    new Invader(type, new Vector2(j * 16 + startX, i * 24 + startY));
                    await Task.Delay(25);
                }
            await Task.Delay(500);
            UFOPaused = false;
        }

        // Disposes all invaders
        public static void DisposeAll()
        {
            if (!DisposedCts)
                cts.Cancel();

            for (int i = 0; i < invaders.Length; i++)
                invaders[i]?.Dispose();

            ResetInfo();
        }

        #region Pause Unpause
        private static bool DisposedCts;
        private static CancellationTokenSource cts = new CancellationTokenSource();
        public static async void PauseUnpauseInvaders(bool pause)
        {
            if (pause)
            {
                if (!DisposedCts)
                    cts.Cancel();
                UFOPaused = true;
            }
            else
            {
                cts = new CancellationTokenSource();
                DisposedCts = false;

                UFOPaused = false;
                UFOMovementLoop();

                try { await MovingInvadersLoop(cts.Token); }
                catch (OperationCanceledException) { cts.Dispose(); DisposedCts = true; }
            }
        }
        #endregion

        #region Invaders Cycle
        private static int CycleCount = 0;
        private static bool FinishedCycle = true;
        private static bool GoingDown = false;
        private static int LastInvaderSide = 0;
        private static int LastInvaderDown = 0;
        private static async Task MovingInvadersLoop(CancellationToken token)
        {
            while (true)
            {
                int toWait = (int)CycleTimeCurveMilliseconds(invaderCount);                
                FinishedCycle = false;

                try { await Task.Delay(toWait, token); }
                catch { token.ThrowIfCancellationRequested(); }

                CycleInvaders(/*token*/);

                FinishedCycle = true;


                if (invaderCount == 0 && LocalGame.instance!.LivesLeft > 0)
                {
                    Bullet.DisposeAll();
                    InputHandler.Disabled = true;
                    await PlotInvaders();
                    InputHandler.Disabled = false;
                    PauseUnpauseInvaders(false);
                    return;
                }
            }
        }
        private static double CycleTimeCurveMilliseconds(int x) { return -(x * x) * (x - 110) / 150 + 10; }
        public static void CycleInvaders(/*CancellationToken token*/)
        {
            if (random.Next(50) < invaders.Length && FinishedCycle)
                invaders[random.Next(invaders.Length)]?.Shoot();


            for (int i = 0; i < invaders.Length; i++)
                if (invaders[i]?.DecideDir() ?? false)
                    GoingDown = true;

            if (GoingDown)
                MoveInvadersDown(/*token*/);
            else
                MoveInvadersSide(/*token*/);

            CycleBeatSound();

            for (int i = 0; i < invaders.Length; i++)
                if (invaders[i]?.CheckLose() ?? false)
                {
                    LocalGame.instance!.Lost();
                    return;
                }

            GenerateUFO();

            LastInvaderDown = 0;
            LastInvaderSide = 0;
            SpriteSwitch = !SpriteSwitch;
            CycleCount++;
        }
        private static void CycleBeatSound()
        {
            switch (CycleCount % 2)
            {
                case 0:
                    SoundManager.PlaySound(Sounds.CycleBeat1, 1);
                    break;
                case 1:
                    SoundManager.PlaySound(Sounds.CycleBeat2, 1);
                    break;
            }
        }
        private static void MoveInvadersSide(/*CancellationToken token*/)
        {
            for (int i = LastInvaderSide; i < invaders.Length; LastInvaderSide = ++i)
            {
                if (invaders[i] == null) continue;

                using (Transform t = new Transform(new Vector2(0, 0), new Vector2(0, 0)))
                    (invaders[i]?.transform ?? t).Position += new Vector2(InvaderSpeedSide * dir, 0);
                invaders[i]?.NextClip();

                /*
                try { await Task.Delay(1, token); }
                catch
                {
                    LastInvaderSide++;
                    token.ThrowIfCancellationRequested();
                }
                */
            }
        }
        private static void MoveInvadersDown(/*CancellationToken token*/)
        {
            GoingDown = true;
            for (int i = LastInvaderDown; i < invaders.Length; LastInvaderDown = ++i)
            {
                if (invaders[i] == null) continue;

                using (Transform t = new Transform(new Vector2(0, 0), new Vector2(0, 0)))
                    (invaders[i]?.transform ?? t).Position += new Vector2(0, InvaderSpeedDown);
                invaders[i]?.NextClip();

                /*
                try { await Task.Delay(1, token); }
                catch
                {
                    LastInvaderDown++;
                    token.ThrowIfCancellationRequested();
                }
                */
            }
            GoingDown = false;
        }
        #endregion

        #region UFO
        private static bool UFOPaused = false;
        private static void GenerateUFO()
        {
            if (random.Next(50) == 0 && invaderCount >= 8 && currentUFO == null)
            {
                new Invader(EnemyTypes.UFO, new Vector2(-25, 15));
                UFOMovementLoop();
            }
        }
        private static async void UFOMovementLoop()
        {
            bool playingSound = false;
            while (!(currentUFO == null || UFOPaused))
            {
                if (!playingSound)
                {
                    SoundManager.PlaySound(Sounds.UFO);
                    playingSound = true;
                }


                currentUFO.transform.Position += new Vector2(UFOSpeed, 0);

                if (currentUFO.transform.Position.X > MainWindow.referenceSize.X)
                {
                    SoundManager.StopSound(Sounds.UFO);
                    currentUFO.Dispose();
                    currentUFO = null;
                }

                await Task.Delay(1000 / (MainWindow.TARGET_FPS * 16));
            }
        }
        private void UFODeath()
        {
            currentUFO = null;
            col.Dispose();

            SoundManager.StopSound(Sounds.UFO);
            SoundManager.PlaySound(Sounds.InvaderDeath);

            // Invader Explosion
            transform.Scale = new Vector2(13, 8);
            sprite.ChangeImage(@"Resources\Images\Enemies\InvaderDeath.png");


            LocalGame.instance!.Score += pointsReward;


            Task.Delay(250).ContinueWith((p) => { Dispose(); UFOCreditsBlink(); });

        }
        private async void UFOCreditsBlink()
        {
            CustomLabel credits = null!;

            // UI Objects need to be created in an STA thread
            Application.Current.Dispatcher.Invoke(() => credits = new CustomLabel(new Transform(new Vector2(32, 16), transform.Position), pointsReward.ToString(), System.Windows.Media.Colors.White));

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
        private EnemyTypes type;

        // Checks wheter the invaders have reached the bottom
        private bool CheckLose()
        {
            return transform.Position.Y > 2 * MainWindow.referenceSize.Y / 3.25;
        }

        /// Updates <see cref="dir"/>
        /// <returns> Whether the invaders should move down </returns>
        private bool DecideDir()
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
                    return true;
            }
            return false;
        }

        // Shoots from the invader
        private void Shoot()
        {
            new InvaderBullet(transform.Position);
        }

        // Updates the invader's sprite
        private void NextClip()
        {
            int clipNum = SpriteSwitch ? 2 : 1;
            string imagePath;
            switch (type)
            {
                default:
                    imagePath = $@"Resources\Images\MissingSprite.png";
                    break;
                case EnemyTypes.Octopus:
                    imagePath = @$"Resources\Images\Enemies\Octopus{clipNum}.png";
                    pointsReward = 10;
                    break;
                case EnemyTypes.Crab:
                    imagePath = $@"Resources\Images\Enemies\Crab{clipNum}.png";
                    pointsReward = 20;
                    break;
                case EnemyTypes.Squid:
                    imagePath = $@"Resources\Images\Enemies\Squid{clipNum}.png";
                    pointsReward = 30;
                    break;
            }

            sprite.ChangeImage(imagePath);
        }

        // Kills the invader
        public void Kill()
        {
            if (type == EnemyTypes.UFO)
            {
                UFODeath();
                return;
            }

            invaderCount--;
            invaders[arrPos] = null;
            col.Dispose();

            SoundManager.PlaySound(Sounds.InvaderDeath);

            // Invader Explosion
            transform.Scale = new Vector2(13, 8);
            sprite.ChangeImage(@"Resources\Images\Enemies\InvaderDeath.png");


            LocalGame.instance!.Score += pointsReward;

            Task.Delay(500).ContinueWith((p) => Dispose());
        }

        // Disposes the invader
        private void Dispose()
        {
            sprite.Dispose();
            col.Dispose();
            transform.Dispose();
        }
        #endregion
    }
}