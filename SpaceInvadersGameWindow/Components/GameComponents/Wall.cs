using GameWindow.Components.Miscellaneous;
using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Components.UIElements;
using System.Numerics;

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
            sprite = new Sprite(transform);
            col = new Collider(transform, this, Collider.Layers.Wall);
        }
        public Wall(Vector2 scale, Vector2 pos, string image)
        {
            transform = new Transform(scale, pos);
            sprite = new Sprite(transform, image);
            col = new Collider(transform, this, Collider.Layers.Wall);
        }
        public void Dispose()
        {
            transform.Dispose();
            sprite.Dispose();
            col.Dispose();
        }
    }
}