using Assets.src.body;

namespace Assets.src.force
{
    public abstract class AEnvironmentForce
    {
        public abstract void applyForce(double dt, Body body);
    }
}
