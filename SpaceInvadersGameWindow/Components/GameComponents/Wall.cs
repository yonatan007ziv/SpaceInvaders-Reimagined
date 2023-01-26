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
        private SpriteRenderer sR;
        private Collider col;
        public Wall(Vector2 scale, Vector2 pos)
        {
            transform = new Transform(scale, pos);
            sR = new SpriteRenderer(@"", transform.scale, transform.position);
            col = new Collider(this, transform.scale, transform.position);

            transform.AddScaleDel((vec) => col.SetScale(vec));
            transform.AddScaleDel((vec) => sR.SetScale(vec));

            transform.AddPositionDel((vec) => col.SetPosition(vec));
            transform.AddPositionDel((vec) => sR.SetPosition(vec));
        }
    }
}