using GameWindow.Components.GameComponents;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using System.Numerics;
using System.Windows;

namespace GameWindow.Components.NetworkedComponents
{
    /// <summary>
    /// A class implementing a networked bunker part
    /// </summary>
    internal class NetworkedBunkerPart
    {
        private Transform transform;
        private Collider col;
        private Sprite sprite;

        public int bunkerID;
        public int imagePathIndex = 1;
        public BunkerPartType part;
        public bool flipped;

        /// <summary>
        /// Builds a bunker part
        /// </summary>
        /// <param name="part"> Part's type </param>
        /// <param name="pos"> A <see cref="Vector2"/> representing the part's position </param>
        /// <param name="bunkerID"> Bunker's ID </param>
        /// <param name="flipped"> Whether the bunker part sprite is flipped or not </param>
        public NetworkedBunkerPart(BunkerPartType part, Vector2 pos, int bunkerID, bool flipped)
        {
            this.part = part;
            this.flipped = flipped;
            this.bunkerID = bunkerID;

            transform = new Transform(new Vector2(6, 8), pos);

            CollisionLayer layer = (0 <= bunkerID && bunkerID <= 3) ? CollisionLayer.BunkerA : CollisionLayer.BunkerB;
            col = new Collider(transform, this, layer);

            // UI Objects need to be created in an STA thread
            Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform, ""));

            UpdateClip();

            // Suppressing the "Null When Leaving a Constructor" warning
            sprite!.ToString();
        }

        /// <summary>
        /// When the bunker parts get hit
        /// </summary>
        public void Hit()
        {
            imagePathIndex++;
            if (imagePathIndex >= 4)
            {
                Dispose();
                return;
            }
            UpdateClip();
        }

        /// <summary>
        /// Next bunker part's clip
        /// </summary>
        private void UpdateClip()
        {
            string bunkerImagePath;
            switch (part)
            {
                default:
                    bunkerImagePath = "";
                    break;
                case BunkerPartType.TopLeft:
                    bunkerImagePath = @$"Resources\Images\Bunker\TopLeft{imagePathIndex}{(flipped ? "Opponent" : "")}.png";
                    break;
                case BunkerPartType.BottomLeft:
                    bunkerImagePath = @$"Resources\Images\Bunker\BottomLeft{imagePathIndex}{(flipped ? "Opponent" : "")}.png";
                    break;
                case BunkerPartType.MiddleTopLeft:
                    bunkerImagePath = $@"Resources\Images\Bunker\MiddleTopLeft{imagePathIndex}{(flipped ? "Opponent" : "")}.png";
                    break;
                case BunkerPartType.MiddleBottomLeft:
                    bunkerImagePath = $@"Resources\Images\Bunker\MiddleBottomLeft{imagePathIndex}{(flipped ? "Opponent" : "")}.png";
                    break;
                case BunkerPartType.MiddleTopRight:
                    bunkerImagePath = $@"Resources\Images\Bunker\MiddleTopRight{imagePathIndex}{(flipped ? "Opponent" : "")}.png";
                    break;
                case BunkerPartType.MiddleBottomRight:
                    bunkerImagePath = $@"Resources\Images\Bunker\MiddleBottomRight{imagePathIndex}{(flipped ? "Opponent" : "")}.png";
                    break;
                case BunkerPartType.TopRight:
                    bunkerImagePath = $@"Resources\Images\Bunker\TopRight{imagePathIndex}{(flipped ? "Opponent" : "")}.png";
                    break;
                case BunkerPartType.BottomRight:
                    bunkerImagePath = $@"Resources\Images\Bunker\BottomRight{imagePathIndex}{(flipped ? "Opponent" : "")}.png";
                    break;
            }
            sprite.ChangeImage(bunkerImagePath);
        }

        /// <summary>
        /// Disposes the curret <see cref="BunkerPart"/>
        /// </summary>
        public void Dispose()
        {
            sprite.Dispose();
            col.Dispose();
            transform.Dispose();
        }
    }
}