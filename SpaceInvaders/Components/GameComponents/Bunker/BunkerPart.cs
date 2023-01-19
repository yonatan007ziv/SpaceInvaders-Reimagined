using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Components.Renderer;
using SpaceInvaders.DataStructures;

namespace SpaceInvaders.Components.GameComponents.Bunker
{
    internal class BunkerPart
    {
        Bitmap[] stages;
        int stage;

        public SpriteRenderer sR;
        //Collider col;


        public BunkerPart(Bitmap[] stages, Vector2 pos)
        {
            sR = new SpriteRenderer(stages[0], pos);
            this.stages = stages;
            stage = 0;
            sR.Image = this.stages[stage];
        }
        public void GetHit()
        {
            stage++;
            if (stage > 3)
                Dispose();
            else
                sR.Image = stages[stage];
        }
        private void Dispose()
        {
            sR.Dispose();
        }
    }
}
