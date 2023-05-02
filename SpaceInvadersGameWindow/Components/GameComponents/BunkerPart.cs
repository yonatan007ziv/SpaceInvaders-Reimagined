using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using System.Numerics;
using System.Windows;

namespace GameWindow.Components.GameComponents
{
    /// <summary>
    /// Class for representing a <see cref="BunkerPart"/>
    /// </summary>
    internal class BunkerPart
    {
        private Transform transform;
        private Sprite sprite;
        private Collider col;

        private BunkerPartType partType;
        private int imagePathIndex = 1;

        /// <summary>
        /// Initializes a new BunkerPart
        /// </summary>
        /// <param name="partType"> The type of the bunker part </param>
        /// <param name="pos"> A <see cref="Vector2"/> representing the position of the bunker part </param>
        public BunkerPart(BunkerPartType partType, Vector2 pos)
        {
            this.partType = partType;
            transform = new Transform(new Vector2(6, 8), pos);
            col = new Collider(transform, this, CollisionLayer.Bunker);

            // UI Objects need to be created in an STA thread
            Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform, Image.Empty));

            NextClip();

            // Suppressing the "Null When Leaving a Constructor" warning
            sprite!.ToString();
        }

        /// <summary>
        /// On bullet-hit 
        /// </summary>
        public void Hit()
        {
            if (imagePathIndex == 4)
                Dispose();
            imagePathIndex++;
            NextClip();
        }

        /// <summary>
        /// Next clip in the 4-stage animation
        /// </summary>
        /// <remarks>
        /// Stage 1 - Completely Intact <br/>
        /// Stage 2 - Slightly Damaged <br/>
        /// Stage 3 - Moderately Damaged <br/>
        /// Stage 4 - Severely Damaged <br/>
        /// Stage 5 - Completely Broken <br/>
        /// </remarks>
        public void NextClip()
        {
            Image image;
            switch (partType)
            {
                default:
                    throw new System.Exception();
                case BunkerPartType.TopLeft:
                    image = Image.TopLeft1 + imagePathIndex - 1;
                    break;
                case BunkerPartType.BottomLeft:
                    image = Image.BottomLeft1 + imagePathIndex - 1;
                    break;
                case BunkerPartType.MiddleTopLeft:
                    image = Image.MiddleTopLeft1 + imagePathIndex - 1;
                    break;
                case BunkerPartType.MiddleBottomLeft:
                    image = Image.MiddleBottomLeft1 + imagePathIndex - 1;
                    break;
                case BunkerPartType.MiddleTopRight:
                    image = Image.MiddleTopRight1 + imagePathIndex - 1;
                    break;
                case BunkerPartType.MiddleBottomRight:
                    image = Image.MiddleBottomRight1 + imagePathIndex - 1;
                    break;
                case BunkerPartType.TopRight:
                    image = Image.TopRight1 + imagePathIndex - 1;
                    break;
                case BunkerPartType.BottomRight:
                    image = Image.BottomRight1 + imagePathIndex - 1;
                    break;
            }
            sprite.ChangeImage(image);
        }

        /// <summary>
        /// Disposes the current <see cref="BunkerPart"/> object
        /// </summary>
        public void Dispose()
        {
            sprite.Dispose();
            col.Dispose();
            transform.Dispose();
        }
    }
}