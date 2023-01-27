using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Components.Renderer;
using System.Numerics;
using System.Threading.Tasks;

namespace SpaceInvaders.Components.GameComponents
{
    internal class Wall
    {
        public static Wall? Ceiling, RightWall, LeftWall;
        public Transform transform;
        public Wall(Vector2 scale, Vector2 pos)
        {
            transform = new Transform(scale, pos);
            new SpriteRenderer(transform, @"");
            new Collider(transform, this);
        }
    }
}