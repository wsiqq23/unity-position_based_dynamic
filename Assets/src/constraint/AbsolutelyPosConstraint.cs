using Assets.src.body;
using UnityEngine;

namespace Assets.src.constraint
{
    /**
     * Please use utf-8 encoding to read the comment
     * 绝对位移约束
     */
    public class AbsolutelyPosConstraint : AConstraint
    {
        private int index;
        public float x, y, z;
        public AbsolutelyPosConstraint(Body body, int index) : base(body)
        {
            this.index = index;
        }
        public void setPosition(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public void setPosition(Vector3 pos)
        {
            setPosition(pos.x, pos.y, pos.z);
        }
        public override void doConstraint(double dt)
        {
            body.positions[index].x = body.newPositions[index].x = x;
            body.positions[index].y = body.newPositions[index].y = y;
            body.positions[index].z = body.newPositions[index].z = z;
        }
    }
}
