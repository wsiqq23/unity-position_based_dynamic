using Assets.src.body;
using UnityEngine;

namespace Assets.src.force
{
    /**
     * Please use utf-8 encoding to read the comment
     * 
     */
    class DirectedForce : AEnvironmentForce
    {
        public Vector3 direction;
        private double v1;
        private int v2;
        private int v3;

        public DirectedForce(Vector3 direction)
        {
            this.direction = direction;
        }

        public DirectedForce(float x, float y, float z)
        {
            direction = new Vector3(x, y, z);
        }

        public override void applyForce(double dt, Body body)
        {
            for (int i = 0; i < body.particlesNum; i++)
            {
                body.velocities[i] += direction * (float)dt;
            }
        }
    }
}
