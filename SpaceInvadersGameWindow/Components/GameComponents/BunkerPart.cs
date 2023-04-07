using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using System.Numerics;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GameWindow.Components.GameComponents
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
            Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform));

            NextClip();

            // Suppressing the "Null When Leaving a Constructor" warning
            sprite!.ToString();
        }

        private int timesHit = 1;
        public void Hit()
        {
            if (timesHit == 4)
                Dispose();
            timesHit++;
            NextClip();
        }
        public void NextClip()
        {
            string bunkerImagePath;
            switch (part)
            {
                case BunkerParts.TopLeft:
                    bunkerImagePath = @$"Resources\Images\Bunker\TopLeft{timesHit}.png";
                    break;
                case BunkerParts.BottomLeft:
                    bunkerImagePath = @$"Resources\Images\Bunker\BottomLeft{timesHit}.png";
                    break;
                case BunkerParts.MiddleTopLeft:
                    bunkerImagePath = $@"Resources\Images\Bunker\MiddleTopLeft{timesHit}.png";
                    break;
                case BunkerParts.MiddleBottomLeft:
                    bunkerImagePath = $@"Resources\Images\Bunker\MiddleBottomLeft{timesHit}.png";
                    break;
                case BunkerParts.MiddleTopRight:
                    bunkerImagePath = $@"Resources\Images\Bunker\MiddleTopRight{timesHit}.png";
                    break;
                case BunkerParts.MiddleBottomRight:
                    bunkerImagePath = $@"Resources\Images\Bunker\MiddleBottomRight{timesHit}.png";
                    break;
                case BunkerParts.TopRight:
                    bunkerImagePath = $@"Resources\Images\Bunker\TopRight{timesHit}.png";
                    break;
                case BunkerParts.BottomRight:
                    bunkerImagePath = $@"Resources\Images\Bunker\BottomRight{timesHit}.png";
                    break;
                default:
                    bunkerImagePath = $@"Resources\Images\Bunker\TopLeft{timesHit}.png";
                    break;
            }

            sprite.ChangeImage(bunkerImagePath);
        }
        public void Dispose()
        {
            sprite.Dispose();
            col.Dispose();
            transform.Dispose();
        }
    }
}
