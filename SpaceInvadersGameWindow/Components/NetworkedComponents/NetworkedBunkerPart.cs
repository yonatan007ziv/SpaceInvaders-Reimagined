using GameWindow.Components.GameComponents;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using System.Diagnostics.Eventing.Reader;
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
        private Collider? col;
        private Sprite? sprite;

        public int bunkerID;
        public int imagePathIndex = 1;
        private CollisionLayer layer;
        public BunkerPartType partType;
        public bool flipped;

        /// <summary>
        /// Builds a bunker part
        /// </summary>
        /// <param name="partType"> Part's type </param>
        /// <param name="pos"> A <see cref="Vector2"/> representing the part's position </param>
        /// <param name="bunkerID"> Bunker's ID </param>
        /// <param name="flipped"> Whether the bunker part sprite is flipped or not </param>
        public NetworkedBunkerPart(BunkerPartType partType, Vector2 pos, int bunkerID, bool flipped)
        {
            this.partType = partType;
            this.flipped = flipped;
            this.bunkerID = bunkerID;

            transform = new Transform(new Vector2(6, 8), pos);
            layer = (0 <= bunkerID && bunkerID <= 3) ? CollisionLayer.BunkerA : CollisionLayer.BunkerB;
        }

        /// <summary>
        /// Resets the bunkerpart
        /// </summary>
        public void ResetPart()
        {
            Dispose();

            transform = new Transform(new Vector2(6, 8), transform.Position);
            col = new Collider(transform, this, layer);

            // UI Objects need to be created in an STA thread
            Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform, Image.Empty));
            imagePathIndex = 0;
        }

        /// <summary>
        /// When the bunker parts get hit
        /// </summary>
        public void Hit()
        {
            imagePathIndex++;
            if (imagePathIndex > 4)
            {
                imagePathIndex = 0;
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
            Image image;
            switch (partType)
            {
                default:
                    throw new System.Exception();
                case BunkerPartType.TopLeft:
                    image = (flipped ? Image.TopLeft1Opponent : Image.TopLeft1) + imagePathIndex - 1;
                    break;
                case BunkerPartType.BottomLeft:
                    image = (flipped ? Image.BottomLeft1Opponent : Image.BottomLeft1) + imagePathIndex - 1;
                    break;
                case BunkerPartType.MiddleTopLeft:
                    image = (flipped ? Image.MiddleTopLeft1Opponent : Image.MiddleTopLeft1) + imagePathIndex - 1;
                    break;
                case BunkerPartType.MiddleBottomLeft:
                    image = (flipped ? Image.MiddleBottomLeft1Opponent : Image.MiddleBottomLeft1) + imagePathIndex - 1;
                    break;
                case BunkerPartType.MiddleTopRight:
                    image = (flipped ? Image.MiddleTopRight1Opponent : Image.MiddleTopRight1) + imagePathIndex - 1;
                    break;
                case BunkerPartType.MiddleBottomRight:
                    image = (flipped ? Image.MiddleBottomRight1Opponent : Image.MiddleBottomRight1) + imagePathIndex - 1;
                    break;
                case BunkerPartType.TopRight:
                    image = (flipped ? Image.TopRight1Opponent : Image.TopRight1) + imagePathIndex - 1;
                    break;
                case BunkerPartType.BottomRight:
                    image = (flipped ? Image.BottomRight1Opponent : Image.BottomRight1) + imagePathIndex - 1;
                    break;
            }
            sprite!.ChangeImage(image);
        }

        /// <summary>
        /// Disposes the curret <see cref="BunkerPart"/>
        /// </summary>
        public void Dispose()
        {
            transform.Dispose();
            col?.Dispose();
            sprite?.Dispose();
        }
    }
}