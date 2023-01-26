namespace SpaceInvaders.Components.PhysicsEngine.Collider
{
    internal interface ICollidable
    {
        public abstract void BulletHit();
        public abstract void InvaderHit();
    }
}