using Assets.src.body;
using UnityEngine;
namespace Assets.src.constraint
{
    /**
     * Please use utf-8 encoding to read the comment
     * 距离约束
     */
    public class DistanceConstraint : AConstraint
    {
        /* 弹性模量 */
        private float elasticModulus;
        /* 原始长度 */
        private float originLength;
        public int i1, i2;
        public DistanceConstraint(Body body, int i1, int i2, float elasticModulus) : base(body)
        {
            this.elasticModulus = elasticModulus;
            this.i1 = i1;
            this.i2 = i2;
            originLength = (body.positions[i2] - body.positions[i1]).magnitude;
        }

        public override void doConstraint(double dt)
        {
            float mass = body.particleMass;
            float sum = mass * 2.0f;
            Vector3 n = body.newPositions[i2] - body.newPositions[i1];
            float d = n.magnitude;
            n.Normalize();
            Vector3 corr = elasticModulus * n * (d - originLength) * sum;
            body.newPositions[i1] += corr * (float)dt / mass;
            body.newPositions[i2] -= corr * (float)dt / mass;
        }
    }
}
