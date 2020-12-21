using Assets.src.body;
using Assets.src.collision;
using Assets.src.force;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.src.solver
{
    /**
     * Please use utf-8 encoding to read the comment
     * PBD流程的核心控制类
     */
    public class Solver
    {
        public int solverIteration;
        public int collisionIteration;
        public float stopThreshold = 0.1f;
        private List<Body> bodies = new List<Body>();
        private List<AEnvironmentForce> forces = new List<AEnvironmentForce>();
        private List<ACollision> collisions = new List<ACollision>();
        public Solver(int solverIteration = 3, int collisionIteration = 2)
        {
            this.solverIteration = solverIteration;
            this.collisionIteration = collisionIteration;
        }
        public void addForce(AEnvironmentForce force)
        {
            forces.Add(force);
        }
        public void removeForce(AEnvironmentForce force)
        {
            forces.Remove(force);
        }
        public void addCollision(ACollision collision)
        {
            collisions.Add(collision);
        }
        public void removeCollision(ACollision collision)
        {
            collisions.Remove(collision);
        }
        public void addBody(Body body)
        {
            bodies.Add(body);
        }
        public void removeBody(Body body)
        {
            bodies.Remove(body);
        }
        public void solve(float dt)
        {
            if (dt == 0.0)
            {
                return;
            }
            applyForces(dt);
            caculateNewPositions(dt);
            solveCollisions();
            projectConstraints();
            updateVelocities(dt);
            updatePositions();
        }
        private void applyForces(float dt)
        {
            for (int j = 0; j < bodies.Count; j++)
            {
                Body body = bodies[j];
                for (int i = 0; i < body.particlesNum; i++)
                {
                    body.velocities[i] -= body.velocities[i] * body.damping * dt;
                }
                for (int i = 0; i < forces.Count; i++)
                {
                    forces[i].applyForce(dt, body);
                }
            }
        }
        private void caculateNewPositions(float dt)
        {
            for (int j = 0; j < bodies.Count; j++)
            {
                Body body = bodies[j];

                for (int i = 0; i < body.particlesNum; i++)
                {
                    body.newPositions[i] = body.positions[i] + dt * body.velocities[i];
                }
            }
        }
        private void solveCollisions()
        {
            List<ACollisionContact> contacts = new List<ACollisionContact>();
            for (int i = 0; i < collisions.Count; i++)
            {
                collisions[i].findContacts(bodies, contacts);
            }
            float dt = 1.0f / collisionIteration;
            for (int i = 0; i < collisionIteration; i++)
            {
                for (int j = 0; j < contacts.Count; j++)
                {
                    contacts[j].solveContact(dt);
                }
            }
        }
        private void projectConstraints()
        {
            float dt = 1.0f / solverIteration;

            for (int i = 0; i < solverIteration; i++)
            {
                for (int j = 0; j < bodies.Count; j++)
                {
                    bodies[j].projectConstraints(dt);
                }
            }
        }
        private void updateVelocities(float dt)
        {
            float threshold2 = stopThreshold * dt;
            threshold2 *= threshold2;
            for (int j = 0; j < bodies.Count; j++)
            {
                Body body = bodies[j];
                for (int i = 0; i < body.particlesNum; i++)
                {
                    Vector3 d = body.newPositions[i] - body.positions[i];
                    body.velocities[i] = d / dt;
                    float m = body.velocities[i].sqrMagnitude;
                    if (m < threshold2)
                        body.velocities[i].x = body.velocities[i].y = body.velocities[i].z = 0;
                }
            }
        }
        private void updatePositions()
        {
            for (int j = 0; j < bodies.Count; j++)
            {
                Body body = bodies[j];
                for (int i = 0; i < body.particlesNum; i++)
                {
                    body.positions[i] = body.newPositions[i];
                }
            }
        }
    }
}
