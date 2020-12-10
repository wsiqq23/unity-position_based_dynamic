using Assets.src.body;

namespace Assets.src.constraint
{
    public abstract class AConstraint
    {
        protected Body body { get; private set; }
        public AConstraint(Body body)
        {
            this.body = body;
        }
        public abstract void doConstraint(double dt);
    }
}
