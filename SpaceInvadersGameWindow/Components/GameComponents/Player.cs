using SpaceInvaders.Components.Controllers;
using SpaceInvaders.Components.Renderer;
using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine.Collider;
using System.Numerics;

namespace SpaceInvaders.Components.GameComponents
{
    internal class Player
    {
        public Transform transform;
        private Collider col;
        private SpriteRenderer sR;
        public Player(Vector2 pos)
        {
            transform = new Transform(pos, new Vector2(13, 8) * 5);
            //col = new Collider(this, transform.position, transform.scale);
            sR = new SpriteRenderer(@"Resources\RawFiles\Images\Player.png", transform.position, transform.scale);
            transform.AddPositionDel((vec) => sR.SetPosition(vec));
            transform.AddScaleDel((vec) => sR.SetScale(vec));
            new CharacterController(transform, col);
        }
    }
}