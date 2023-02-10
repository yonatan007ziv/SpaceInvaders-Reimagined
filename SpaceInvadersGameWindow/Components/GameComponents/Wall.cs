using GameWindow.Components.Miscellaneous;
using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Components.UIElements;
using System.Numerics;

namespace GameWindow.Components.GameComponents
{
    internal class Wall
    {
        public static Wall? Ceiling, Floor, RightWall, LeftWall;
        public Transform transform;
        public Wall(Vector2 scale, Vector2 pos)
        {
            transform = new Transform(scale, pos);
            new Sprite(transform);
            new Collider(transform, this);
        }
        public Wall(Vector2 scale, Vector2 pos, string image)
        {
            transform = new Transform(scale, pos);
            new Sprite(transform, image);
            new Collider(transform, this);
        }
    }
}