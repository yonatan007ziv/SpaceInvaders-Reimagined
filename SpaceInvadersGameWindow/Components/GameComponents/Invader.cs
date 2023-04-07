using GameWindow.Components.Initializers;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using GameWindow.Systems;
using System;
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

        // Plots the invaders
        public static async Task PlotInvaders()
        {
            invaderCount = 0;
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
        }

        // Disposes all invaders
        public static void DisposeAll()
        {
            if (!DisposedCts)
                cts.Cancel();

            invaderCount = 0;
            for (int i = 0; i < invaders.Length; i++)
                invaders[i]?.Dispose();
            invaders = new Invader[55];
            currentUFO?.Dispose();
            currentUFO = null;
            LastInvaderDown = 0;
            LastInvaderSide = 0;
        }

        #region Pause Unpause
        private static bool DisposedCts;
        private static CancellationTokenSource cts = new CancellationTokenSource();
        public static async void PauseUnpauseInvaders(bool pause)
        {
            if (pause)
                cts.Cancel();
            else
            {
                cts = new CancellationTokenSource();
                DisposedCts = false;

                UFOMovementLoop();
                try { await MovingInvadersLoop(cts.Token); }
                catch (OperationCanceledException) { cts.Dispose(); DisposedCts = true; }
            }
        }
        #endregion

        #region Invaders Cycle
        private static bool FinishedCycle = true;
        private static bool GoingDown = false;
        private static int LastInvaderSide = 0;
        private static int LastInvaderDown = 0;
        private static async Task MovingInvadersLoop(CancellationToken token)
        {
            while (true)
            {
                CycleInvaders(/*token*/);

                if (invaderCount == 0 && LocalGame.instance!.LivesLeft > 0)
                {
                    Bullet.DisposeAll();
                    InputHandler.Disabled = true;
                    await PlotInvaders();
                    InputHandler.Disabled = false;
                    PauseUnpauseInvaders(false);
                    return;
                }

                int toWait = (int)CycleTimeMilliseconds(invaderCount);
                try { await Task.Delay(toWait, token); }
                catch { token.ThrowIfCancellationRequested(); }
            }
        }
        private static double CycleTimeMilliseconds(int x) { return -(x * x) * (x - 110) / 150; }
        public static void CycleInvaders(/*CancellationToken token*/)
        {
            if (random.Next(50) < invaders.Length && FinishedCycle)
                invaders[random.Next(invaders.Length)]?.Shoot();

            FinishedCycle = false;

            for (int i = 0; i < invaders.Length; i++)
                if (invaders[i]?.DecideDir() ?? false)
                    GoingDown = true;

            if (GoingDown)
                MoveInvadersDown(/*token*/);
            else
                MoveInvadersSide(/*token*/);

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
            FinishedCycle = true;
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
        private static void GenerateUFO()
        {
            if (random.Next(50) == 0 && invaders.Length >= 8 && currentUFO == null)
            {
                new Invader(EnemyTypes.UFO, new Vector2(-25, 15));
                UFOMovementLoop();
            }
        }
        private static async void UFOMovementLoop()
        {
            bool playingSound = false;
            while (!(currentUFO == null || LocalGame.Paused))
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
                case EnemyTypes.UFO:
                    imagePath = $@"Resources\Images\Enemies\UFO.png";
                    pointsReward = 100;
                    break;
            }

            sprite.ChangeImage(imagePath);
        }

        // Kills the invader
        public void Kill()
        {
            if (type == EnemyTypes.UFO)
            {
                currentUFO = null;
                col.Dispose();

                SoundManager.StopSound(Sounds.UFO);
                SoundManager.PlaySound(Sounds.InvaderDeath);

                // Invader Explosion
                transform.Scale = new Vector2(13, 8);
                sprite.ChangeImage(@"Resources\Images\Enemies\InvaderDeath.png");


                LocalGame.instance!.Score += pointsReward;

                Task.Delay(500).ContinueWith((p) => Dispose());
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