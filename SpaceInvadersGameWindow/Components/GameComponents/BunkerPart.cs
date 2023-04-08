using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using System.Numerics;
using System.Windows;

namespace GameWindow.Components.GameComponents
{
    internal class BunkerPart
    {
        private PieceTypes part;
        private Transform transform;
        private Sprite sprite;
        private Collider col;
        private int imagePathIndex = 1;

        public BunkerPart(PieceTypes part, Vector2 pos)
        {
            this.part = part;
            transform = new Transform(new Vector2(6, 8), pos);
            col = new Collider(transform, this, CollisionLayer.Bunker);

            // UI Objects need to be created in an STA thread
            Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform));

            NextClip();

            // Suppressing the "Null When Leaving a Constructor" warning
            sprite!.ToString();
        }

        public void Hit()
        {
            if (imagePathIndex == 4)
                Dispose();
            imagePathIndex++;
            NextClip();
        }

        public void NextClip()
        {
            string bunkerImagePath;
            switch (part)
            {
                default:
                    bunkerImagePath = "";
                    break;
                case PieceTypes.TopLeft:
                    bunkerImagePath = @$"Resources\Images\Bunker\TopLeft{imagePathIndex}.png";
                    break;
                case PieceTypes.BottomLeft:
                    bunkerImagePath = @$"Resources\Images\Bunker\BottomLeft{imagePathIndex}.png";
                    break;
                case PieceTypes.MiddleTopLeft:
                    bunkerImagePath = $@"Resources\Images\Bunker\MiddleTopLeft{imagePathIndex}.png";
                    break;
                case PieceTypes.MiddleBottomLeft:
                    bunkerImagePath = $@"Resources\Images\Bunker\MiddleBottomLeft{imagePathIndex}.png";
                    break;
                case PieceTypes.MiddleTopRight:
                    bunkerImagePath = $@"Resources\Images\Bunker\MiddleTopRight{imagePathIndex}.png";
                    break;
                case PieceTypes.MiddleBottomRight:
                    bunkerImagePath = $@"Resources\Images\Bunker\MiddleBottomRight{imagePathIndex}.png";
                    break;
                case PieceTypes.TopRight:
                    bunkerImagePath = $@"Resources\Images\Bunker\TopRight{imagePathIndex}.png";
                    break;
                case PieceTypes.BottomRight:
                    bunkerImagePath = $@"Resources\Images\Bunker\BottomRight{imagePathIndex}.png";
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