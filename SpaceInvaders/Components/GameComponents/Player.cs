using SpaceInvaders.Components.Controllers;
using SpaceInvaders.Components.Renderer;
using SpaceInvaders.Resources;
using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.DataStructures;
using SpaceInvaders.Components.PhysicsEngine.Collider;

namespace SpaceInvaders.Components.GameComponents
{
    internal class Player
    {
        public Transform transform;
        public Collider col;
        private SpriteRenderer sR;
        public Player(Vector2 pos)
        {
            sR = new SpriteRenderer(Sprites.Player, pos);
            col = new Collider(this, new Vector2(sR.Size), pos);
            transform = new Transform(pos, new Vector2(sR.Size));
            transform.AddPositionDel((vec) => sR.SetPosition(vec));
            transform.AddScaleDel((vec) => sR.SetScale(vec));
            new CharacterController(transform, col);
            GameWindow.Instance!.Controls.Add(sR);
        }
    }
}