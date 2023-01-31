using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Components.Renderer;
using SpaceInvadersGameWindow.Components.UIElements;
using System.Numerics;

namespace SpaceInvadersGameWindow.Components.GameComponents
{
    class Bullet
    {
        public Transform transform;
        protected Sprite sprite;
        protected Collider col;
        protected static float bulletSpeed = 5;

        public Bullet(Vector2 pos)
        {
            transform = new Transform(new Vector2(1, 7), pos);
            col = new Collider(transform, this);
            sprite = new Sprite(transform, @"Resources\RawFiles\Images\Bullet.png");
        }
    }
}