using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvadersGameWindow.Components.UIElements;
using System.Numerics;

namespace SpaceInvaders.Components.GameComponents
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
    }
}