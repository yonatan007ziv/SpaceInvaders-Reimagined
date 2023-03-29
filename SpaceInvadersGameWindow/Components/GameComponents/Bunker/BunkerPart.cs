using GameWindow.Components.Miscellaneous;
using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Components.UIElements;
using System.Numerics;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GameWindow.Components.GameComponents.Bunker
{
    internal class BunkerPart
    {
        public enum BunkerParts
        {
            TopLeft = 1,
            BottomLeft = 2,
            MiddleTopLeft = 3,
            MiddleBottomLeft = 4,
            MiddleTopRight = 5,
            MiddleBottomRight = 6,
            TopRight = 7,
            BottomRight = 8,
        }

        BunkerParts part;
        Transform transform;
        Sprite sprite;
        Collider col;

        public BunkerPart(BunkerParts part, Vector2 pos)
        {
            this.part = part;
            transform = new Transform(new Vector2(6, 8), pos);
            col = new Collider(transform, this, Collider.Layers.Bunker);

            // UI Objects need to be created in an STA thread
            Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform, @""));

            NextClip();

            // Suppressing the "Null When Leaving a Constructor" warning
            sprite!.ToString();
        }

        private int timesHit = 1;
        public void Hit()
        {
            if (timesHit == 3)
                Dispose();
            timesHit++;
            NextClip();
        }
        public void NextClip()
        {
            BitmapImage bunkerImage;
            switch (part)
            {
                case BunkerParts.TopLeft:
                    bunkerImage = Sprite.BitmapFromPath(@$"Resources\Images\Bunker\TopLeft{timesHit}.png");
                    break;
                case BunkerParts.BottomLeft:
                    bunkerImage = Sprite.BitmapFromPath(@$"Resources\Images\Bunker\BottomLeft{timesHit}.png");
                    break;
                case BunkerParts.MiddleTopLeft:
                    bunkerImage = Sprite.BitmapFromPath($@"Resources\Images\Bunker\MiddleTopLeft{timesHit}.png");
                    break;
                case BunkerParts.MiddleBottomLeft:
                    bunkerImage = Sprite.BitmapFromPath($@"Resources\Images\Bunker\MiddleBottomLeft{timesHit}.png");
                    break;
                case BunkerParts.MiddleTopRight:
                    bunkerImage = Sprite.BitmapFromPath($@"Resources\Images\Bunker\MiddleTopRight{timesHit}.png");
                    break;
                case BunkerParts.MiddleBottomRight:
                    bunkerImage = Sprite.BitmapFromPath($@"Resources\Images\Bunker\MiddleBottomRight{timesHit}.png");
                    break;
                case BunkerParts.TopRight:
                    bunkerImage = Sprite.BitmapFromPath($@"Resources\Images\Bunker\TopRight{timesHit}.png");
                    break;
                case BunkerParts.BottomRight:
                    bunkerImage = Sprite.BitmapFromPath($@"Resources\Images\Bunker\BottomRight{timesHit}.png");
                    break;
                default:
                    bunkerImage = Sprite.BitmapFromPath($@"Resources\Images\Bunker\TopLeft{timesHit}.png");
                    break;
            }
            sprite.ChangeImage(bunkerImage);
        }
        public void Dispose()
        {
            sprite.Dispose();
            col.Dispose();
            transform.Dispose();
        }
    }
}
