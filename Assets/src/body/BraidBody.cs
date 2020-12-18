using Assets.src.constraint;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.src.body
{
    public class BraidBody : Body
    {
        private Mesh mesh;
        private float stretchCoefficient;
        public BraidBody(Mesh mesh, float mass, float stretchCoefficient) : base(mesh.vertexCount, mass)
        {
            this.mesh = mesh;
            this.stretchCoefficient = stretchCoefficient;
            initPositions();
            initDistanceConstraints();
        }
        private void initPositions()
        {
            for (int i = 0; i < particlesNum; i++)
            {
                newPositions[i].x = positions[i].x = mesh.vertices[i].x;
                newPositions[i].y = positions[i].y = mesh.vertices[i].y;
                newPositions[i].z = positions[i].z = mesh.vertices[i].z;
            }
        }
        private void initDistanceConstraints()
        {
            HashSet<int> set = new HashSet<int>();
            for (int i = 0; i < mesh.triangles.Length - 2; i += 3)
            {
                int i1 = mesh.triangles[i];
                int i2 = mesh.triangles[i + 1];
                int i3 = mesh.triangles[i + 2];
                set.Add(i1 > i2 ? i2 * 10000 + i1 : i1 * 10000 + i2);
                set.Add(i2 > i3 ? i3 * 10000 + i2 : i2 * 10000 + i3);
                set.Add(i1 > i3 ? i3 * 10000 + i1 : i1 * 10000 + i3);
            }
            foreach (int indexHash in set)
            {
                int i1 = indexHash / 10000;
                int i2 = indexHash % 10000;
                DistanceConstraint c = new DistanceConstraint(this, i1, i2, stretchCoefficient);
                constraints.Add(c);
            }
        }

    }
}
