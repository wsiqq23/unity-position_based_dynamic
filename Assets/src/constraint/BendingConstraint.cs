using Assets.src.body;
using UnityEngine;

namespace Assets.src.constraint
{

    public class BendingConstraint : AConstraint
    {
        /* 弹性模量 */
        private float elasticModulus;
        /* 原始长度 */
        private float originLength;
        private int i1, i2, i3;
        public BendingConstraint(Body body, int i1, int i2, int i3, float elasticModulus) : base(body)
        {
            this.i1 = i1;
            this.i2 = i2;
            this.i3 = i3;
            this.elasticModulus = elasticModulus;
            Vector3 center = (body.positions[i1] + body.positions[i2] + body.positions[i3]) / 3.0f;
            originLength = (body.positions[i3] - center).magnitude;
        }

        public override void doConstraint(double dt)
        {
            Vector3 center = (body.newPositions[i1] + body.newPositions[i2] + body.newPositions[i3]) / 3.0f;
            Vector3 dirCenter = body.newPositions[i3] - center;
            float distCenter = dirCenter.magnitude;
            float diff = 1.0f - (originLength / distCenter);
            float mass = body.particleMass;
            float w = mass + mass * 2.0f + mass;
            Vector3 dirForce = dirCenter * diff;
            Vector3 fa = elasticModulus / 2.0f * dirForce * (float)dt;
            body.newPositions[i1] += fa;
            Vector3 fb = elasticModulus / 2.0f * dirForce * (float)dt;
            body.newPositions[i2] += fb;
            Vector3 fc = -elasticModulus * dirForce * (float)dt;
            body.newPositions[i3] += fc;
        }
    }
}
