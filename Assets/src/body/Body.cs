using Assets.src.constraint;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.src.body
{
    public class Body
    {
        public Vector3[] positions { get; private set; }
        public Vector3[] newPositions { get; private set; }
        public Vector3[] velocities { get; private set; }
        public List<AConstraint> constraints { get; private set; } = new List<AConstraint>();
        public int particlesNum { get; private set; }
        public float damping = 1.0f;
        private float _radius;
        public float particleRadius
        {
            get { return _radius; }
            protected set { _radius = value; particleDiameter = 2.0f * value; }
        }
        public float particleDiameter { get; private set; }
        public float particleMass { get; protected set; }
        public Body(int particlesNum, float radius, float mass)
        {
            this.particlesNum = particlesNum;
            positions = new Vector3[particlesNum];
            newPositions = new Vector3[particlesNum];
            velocities = new Vector3[particlesNum];
            _radius = radius;
            particleMass = mass;
        }
        public void projectConstraints(double dt)
        {
            for (int i = 0; i < constraints.Count; i++)
            {
                constraints[i].doConstraint(dt);
            }
        }
    }
}
