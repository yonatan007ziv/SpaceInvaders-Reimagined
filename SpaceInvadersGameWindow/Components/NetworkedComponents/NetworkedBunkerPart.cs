using GameWindow.Components.GameComponents;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using System.Numerics;
using System.Windows;

namespace GameWindow.Components.NetworkedComponents
{
    internal class NetworkedBunkerPart
    {
        public int BunkerID;
        public int imagePathIndex = 0;
        public PieceTypes part;
        public bool flipped;
        private Transform transform;
        private Collider col;
        private Sprite sprite;

        public NetworkedBunkerPart(PieceTypes part, Vector2 pos, int bunkerId, bool flipped)
        {
            this.part = part;
            this.flipped = flipped;
            BunkerID = bunkerId;

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
        private void NextClip()
        {
            string bunkerImagePath;
            switch (part)
            {
                default:
                    bunkerImagePath = "";
                    break;
                case PieceTypes.TopLeft:
                    bunkerImagePath = @$"Resources\Images\Bunker\TopLeft{imagePathIndex}{(flipped ? "Opponent" : "")}.png";
                    break;
                case PieceTypes.BottomLeft:
                    bunkerImagePath = @$"Resources\Images\Bunker\BottomLeft{imagePathIndex}{(flipped ? "Opponent" : "")}.png";
                    break;
                case PieceTypes.MiddleTopLeft:
                    bunkerImagePath = $@"Resources\Images\Bunker\MiddleTopLeft{imagePathIndex}{(flipped ? "Opponent" : "")}.png";
                    break;
                case PieceTypes.MiddleBottomLeft:
                    bunkerImagePath = $@"Resources\Images\Bunker\MiddleBottomLeft{imagePathIndex}{(flipped ? "Opponent" : "")}.png";
                    break;
                case PieceTypes.MiddleTopRight:
                    bunkerImagePath = $@"Resources\Images\Bunker\MiddleTopRight{imagePathIndex}{(flipped ? "Opponent" : "")}.png";
                    break;
                case PieceTypes.MiddleBottomRight:
                    bunkerImagePath = $@"Resources\Images\Bunker\MiddleBottomRight{imagePathIndex}{(flipped ? "Opponent" : "")}.png";
                    break;
                case PieceTypes.TopRight:
                    bunkerImagePath = $@"Resources\Images\Bunker\TopRight{imagePathIndex}{(flipped ? "Opponent" : "")}.png";
                    break;
                case PieceTypes.BottomRight:
                    bunkerImagePath = $@"Resources\Images\Bunker\BottomRight{imagePathIndex}{(flipped ? "Opponent" : "")}.png";
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