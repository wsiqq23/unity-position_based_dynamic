using Assets.src.body;
using Assets.src.solver;
using System;
using UnityEngine;

namespace Assets.src.constraint
{
    /**
     * 剪应力约束
     */
    public class ShearStressConstraint : AConstraint
    {
        private int i0, i1, i2, i3;
        private Matrix3x3 invRestMatrix;
        private float elasticModulus;
        private Vector3[] correction;

        public ShearStressConstraint(Body body, int i0, int i1, int i2, int i3, float elasticModulus) : base(body)
        {

            this.i0 = i0;
            this.i1 = i1;
            this.i2 = i2;
            this.i3 = i3;
            this.elasticModulus = elasticModulus;
            correction = new Vector3[4];
            Vector3 x0 = body.positions[i0];
            Vector3 x1 = body.positions[i1];
            Vector3 x2 = body.positions[i2];
            Vector3 x3 = body.positions[i3];
            Matrix3x3 restMatrix = new Matrix3x3();
            restMatrix.setColumn(0, x1 - x0);
            restMatrix.setColumn(1, x2 - x0);
            restMatrix.setColumn(2, x3 - x0);
            invRestMatrix = restMatrix.getInverse();
        }

        public override void doConstraint(double di)
        {
            Vector3 x0 = body.newPositions[i0];
            Vector3 x1 = body.newPositions[i1];
            Vector3 x2 = body.newPositions[i2];
            Vector3 x3 = body.newPositions[i3];
            bool res = solveConstraint(x0, x1, x2, x3);
            if (res)
            {
                body.newPositions[i0] += correction[0] * (float)di;
                body.newPositions[i1] += correction[1] * (float)di;
                body.newPositions[i2] += correction[2] * (float)di;
                body.newPositions[i3] += correction[3] * (float)di;
            }

        }

        private bool solveConstraint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {

            double eps = 1e-6;
            correction[0] = Vector3.zero;
            correction[1] = Vector3.zero;
            correction[2] = Vector3.zero;
            correction[3] = Vector3.zero;
            Vector3[] c = new Vector3[3];
            c[0] = invRestMatrix.getColumn(0);
            c[1] = invRestMatrix.getColumn(1);
            c[2] = invRestMatrix.getColumn(2);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    Matrix3x3 m = new Matrix3x3();
                    // 高斯赛德尔迭代
                    m.setColumn(0, (p1 + correction[1]) - (p0 + correction[0]));
                    m.setColumn(1, (p2 + correction[2]) - (p0 + correction[0]));
                    m.setColumn(2, (p3 + correction[3]) - (p0 + correction[0]));

                    Vector3 fi = m * c[i];
                    Vector3 fj = m * c[j];

                    float sij = Vector3.Dot(fi, fj);

                    Vector3[] d = new Vector3[4];
                    d[0] = new Vector3(0, 0, 0);

                    for (int k = 0; k < 3; k++)
                    {
                        d[k + 1] = fj * invRestMatrix[k, i] + fi * invRestMatrix[k, j];

                        d[0] -= d[k + 1];
                    }


                    float lambda = (d[0].sqrMagnitude + d[1].sqrMagnitude + d[2].sqrMagnitude + d[3].sqrMagnitude) / body.particleMass;

                    if (Math.Abs(lambda) < eps) continue;

                    if (i == j)
                    {
                        lambda = (sij - 1.0f) / lambda * elasticModulus;
                    }
                    else
                    {
                        lambda = sij / lambda * elasticModulus;
                    }

                    correction[0] -= lambda / body.particleMass * d[0];
                    correction[1] -= lambda / body.particleMass * d[1];
                    correction[2] -= lambda / body.particleMass * d[2];
                    correction[3] -= lambda / body.particleMass * d[3];
                }
            }

            return true;
        }
    }
}
