using System;
using UnityEngine;

namespace Assets.src.solver
{
    /**
     * unity中没有3x3矩阵，自己定义一个
     */
    public class Matrix3x3
    {
        public float m00, m01, m02;
        public float m10, m11, m12;
        public float m20, m21, m22;
        public Matrix3x3()
        {
            m00 = m01 = m02 = m10 = m11 = m12 = m20 = m21 = m22 = 0;
        }
        public Matrix3x3(float m00, float m01, float m02,
                          float m10, float m11, float m12,
                          float m20, float m21, float m22)
        {
            this.m00 = m00; this.m01 = m01; this.m02 = m02;
            this.m10 = m10; this.m11 = m11; this.m12 = m12;
            this.m20 = m20; this.m21 = m21; this.m22 = m22;
        }
        public Matrix3x3(float v)
        {
            m00 = m01 = m02 = v;
            m10 = m11 = m12 = v;
            m20 = m21 = m22 = v;
        }
        public float this[int i, int j]
        {
            get
            {
                int k = i + j * 3;
                switch (k)
                {
                    case 0: return m00;
                    case 1: return m10;
                    case 2: return m20;
                    case 3: return m01;
                    case 4: return m11;
                    case 5: return m21;
                    case 6: return m02;
                    case 7: return m12;
                    case 8: return m22;
                    default: throw new IndexOutOfRangeException("Matrix3x3 index out of range: " + i + "," + j);
                }
            }
            set
            {
                int k = i + j * 3;
                switch (k)
                {
                    case 0: m00 = value; break;
                    case 1: m10 = value; break;
                    case 2: m20 = value; break;
                    case 3: m01 = value; break;
                    case 4: m11 = value; break;
                    case 5: m21 = value; break;
                    case 6: m02 = value; break;
                    case 7: m12 = value; break;
                    case 8: m22 = value; break;
                    default: throw new IndexOutOfRangeException("Matrix3x3 index out of range: " + i + "," + j);
                }
            }
        }
        public Matrix3x3 getInverse()
        {
            //辅因子法求逆矩阵
            Matrix3x3 invMatrix = new Matrix3x3(m11 * m22 - m12 * m21, m02 * m21 - m01 * m22, m01 * m12 - m02 * m11,
                m00 * m22 - m02 * m20, m00 * m22 - m02 * m20, m02 * m10 - m00 * m12,
                m10 * m21 - m11 * m20, m01 * m20 - m00 * m21, m00 * m11 - m01 * m10);
            float iterator = m00 * invMatrix.m00 + m01 * invMatrix.m10 + m02 * invMatrix.m20;
            //迭代系数太小，没有逆矩阵
            if (Math.Abs(iterator) <= 1e-06)
            {
                return new Matrix3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);
            }
            float fInvDet = (float)(1.0f / iterator);
            invMatrix.m00 *= fInvDet; invMatrix.m01 *= fInvDet; invMatrix.m02 *= fInvDet;
            invMatrix.m10 *= fInvDet; invMatrix.m11 *= fInvDet; invMatrix.m12 *= fInvDet;
            invMatrix.m20 *= fInvDet; invMatrix.m21 *= fInvDet; invMatrix.m22 *= fInvDet;
            return invMatrix;
        }

        public Vector3 getColumn(int col)
        {
            switch (col)
            {
                case 0:
                    return new Vector3(m00, m10, m20);
                case 1:
                    return new Vector3(m01, m11, m21);
                case 2:
                    return new Vector3(m02, m12, m22);
                default:
                    throw new IndexOutOfRangeException("Matrix3x3 dont have column " + col);
            }
        }

        public void setColumn(int col, Vector3 v)
        {
            switch (col)
            {
                case 0:
                    m00 = v.x; m10 = v.y; m20 = v.z;
                    return;
                case 1:
                    m01 = v.x; m11 = v.y; m21 = v.z;
                    return;
                case 2:
                    m02 = v.x; m12 = v.y; m22 = v.z;
                    return;
                default:
                    throw new IndexOutOfRangeException("Matrix3x3 dont have column " + col);
            }
        }
        public static Matrix3x3 operator +(Matrix3x3 m1, Matrix3x3 m2)
        {
            return new Matrix3x3(m1.m00 + m2.m00, m1.m01 + m2.m01, m1.m02 + m2.m02,
                m1.m10 + m2.m10, m1.m11 + m2.m11, m1.m12 + m2.m12,
                m1.m20 + m2.m20, m1.m21 + m2.m21, m1.m22 + m2.m22);
        }
        public static Matrix3x3 operator -(Matrix3x3 m1, Matrix3x3 m2)
        {
            return new Matrix3x3(m1.m00 - m2.m00, m1.m01 - m2.m01, m1.m02 - m2.m02,
                 m1.m10 - m2.m10, m1.m11 - m2.m11, m1.m12 - m2.m12,
                 m1.m20 - m2.m20, m1.m21 - m2.m21, m1.m22 - m2.m22);
        }
        public static Matrix3x3 operator *(Matrix3x3 m1, Matrix3x3 m2)
        {
            float m00 = m1.m00 * m2.m00 + m1.m01 * m2.m10 + m1.m02 * m2.m20;
            float m01 = m1.m00 * m2.m01 + m1.m01 * m2.m11 + m1.m02 * m2.m21;
            float m02 = m1.m00 * m2.m02 + m1.m01 * m2.m12 + m1.m02 * m2.m22;

            float m10 = m1.m10 * m2.m00 + m1.m11 * m2.m10 + m1.m12 * m2.m20;
            float m11 = m1.m10 * m2.m01 + m1.m11 * m2.m11 + m1.m12 * m2.m21;
            float m12 = m1.m10 * m2.m02 + m1.m11 * m2.m12 + m1.m12 * m2.m22;

            float m20 = m1.m20 * m2.m00 + m1.m21 * m2.m10 + m1.m22 * m2.m20;
            float m21 = m1.m20 * m2.m01 + m1.m21 * m2.m11 + m1.m22 * m2.m21;
            float m22 = m1.m20 * m2.m02 + m1.m21 * m2.m12 + m1.m22 * m2.m22;

            return new Matrix3x3(m00, m01, m02, m10, m11, m12, m20, m21, m22);
        }
        public static Vector3 operator *(Matrix3x3 m, Vector3 v)
        {
            return new Vector3(m.m00 * v.x + m.m01 * v.y + m.m02 * v.z, m.m10 * v.x + m.m11 * v.y + m.m12 * v.z, m.m20 * v.x + m.m21 * v.y + m.m22 * v.z);
        }
    }
}
