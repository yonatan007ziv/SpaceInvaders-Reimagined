using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using System.Numerics;
using System.Windows;

namespace GameWindow.Components.GameComponents
{
    internal class Wall
    {
        public static Wall? Ceiling, Floor, RightWall, LeftWall;
        private Transform transform;
        private Sprite sprite;
        private Collider col;
        public Wall(Vector2 scale, Vector2 pos)
        {
            transform = new Transform(scale, pos);
            col = new Collider(transform, this, CollisionLayer.Wall);

            // UI Objects need to be created in an STA thread
            Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform));

            // Suppressing the "Null When Leaving a Constructor" warning
            sprite!.ToString();
        }
        public Wall(Vector2 scale, Vector2 pos, string image)
        {
            transform = new Transform(scale, pos);
            col = new Collider(transform, this, CollisionLayer.Wall);

            // UI Objects need to be created in an STA thread
            Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform, image));

            // Suppressing the "Null When Leaving a Constructor" warning
            sprite!.ToString();
        }
        public void Dispose()
        {
            transform.Dispose();
            sprite.Dispose();
            col.Dispose();
        }
        public static void DisposeAll()
        {
            Ceiling?.Dispose();
            Floor?.Dispose();
            RightWall?.Dispose();
            LeftWall?.Dispose();
        }
    }
}