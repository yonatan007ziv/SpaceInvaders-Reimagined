using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using GameWindow.Factories;
using System.Numerics;

namespace GameWindow.Components.GameComponents
{
    /// <summary>
    /// Class describing a wall
    /// </summary>
    internal class Wall
    {
        public static Wall? Ceiling, Floor, RightWall, LeftWall;

        private Transform transform;
        private Sprite? sprite;
        private Collider col;

        /// <summary>
        /// Make walls for the local game
        /// </summary>
        public static void MakeLocalGameWalls()
        {
            Ceiling = new Wall(new Vector2(MainWindow.referenceSize.X, 5), new Vector2(MainWindow.referenceSize.X / 2, 5), Image.Red);
            Floor = new Wall(new Vector2(MainWindow.referenceSize.X, 5), new Vector2(MainWindow.referenceSize.X / 2, MainWindow.referenceSize.Y / 1.08f), Image.Green);
            LeftWall = new Wall(new Vector2(5, MainWindow.referenceSize.Y), new Vector2(25, MainWindow.referenceSize.Y / 2));
            RightWall = new Wall(new Vector2(5, MainWindow.referenceSize.Y), new Vector2(MainWindow.referenceSize.X - 25, MainWindow.referenceSize.Y / 2));
        }

        /// <summary>
        /// Make walls for the online game
        /// </summary>
        public static void MakeOnlineGameWalls()
        {
            Ceiling = new Wall(new Vector2(MainWindow.referenceSize.X, 5), new Vector2(MainWindow.referenceSize.X / 2, 5), Image.Red);
            Floor = new Wall(new Vector2(MainWindow.referenceSize.X, 5), new Vector2(MainWindow.referenceSize.X / 2, MainWindow.referenceSize.Y * 0.735f + 25), Image.Green);
            LeftWall = new Wall(new Vector2(5, MainWindow.referenceSize.Y), new Vector2(25, MainWindow.referenceSize.Y / 2));
            RightWall = new Wall(new Vector2(5, MainWindow.referenceSize.Y), new Vector2(MainWindow.referenceSize.X - 25, MainWindow.referenceSize.Y / 2));
        }

        /// <summary>
        /// Builds a wall object
        /// </summary>
        /// <param name="scale"> The scale of the wall </param>
        /// <param name="pos"> The position of the wall </param>
        public Wall(Vector2 scale, Vector2 pos)
        {
            transform = new Transform(scale, pos);
            col = new Collider(transform, this, CollisionLayer.Wall);
        }

        /// <summary>
        /// Builds a wall object with an image
        /// </summary>
        /// <param name="scale"> The scale of the wall </param>
        /// <param name="pos"> The position of the wall </param>
        /// <param name="image"> The image of the wall </param>
        public Wall(Vector2 scale, Vector2 pos, Image image)
        {
            transform = new Transform(scale, pos);
            col = new Collider(transform, this, CollisionLayer.Wall);
            sprite = UIElementFactory.CreateSprite(transform, image);

            // Suppressing the "Null When Leaving a Constructor" warning
            sprite!.ToString();
        }

        /// <summary>
        /// Disposes the current <see cref="Wall"/> object
        /// </summary>
        public void Dispose()
        {
            transform.Dispose();
            sprite?.Dispose();
            col.Dispose();
        }

        /// <summary>
        /// Disposes all walls
        /// </summary>
        public static void DisposeAll()
        {
            Ceiling?.Dispose();
            Floor?.Dispose();
            RightWall?.Dispose();
            LeftWall?.Dispose();
        }
    }
}