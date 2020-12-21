using Assets.src.body;

namespace Assets.src.constraint
{

    /**
     * Please use utf-8 encoding to read the comment
     * 所有约束的基类
     */
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
