using Assets.src.body;
using Assets.src.constraint;
using System;
using UnityEngine;

namespace Assets.src.braid
{
    /**
     * 用六棱柱来生成头发，生成过程比较复杂，单独封装一个生成类
     */
    public class BraidGenerator
    {
        private int[][] verticesPosIndex;
        private Vector3[] vertices;
        private int[] triangles;
        private Vector2[] uv;
        public BraidBody body;
        public Mesh mesh;
        /**
         * @param bornPos 开始生成的起始点
         * @param length 长度
         * @param radius 半径
         * @param mass 单个质点的质量
         * @param stretchCoefficient 拉伸系数
         * @param thickModulus 保持厚度的弹性系数
         * @param bendModulus 弯曲的弹性系数
         */
        public BraidGenerator(Vector3 bornPos, int length, float radius, float mass, float stretchCoefficient, float thickModulus, float bendModulus)
        {
            initMesh(bornPos, length, radius);
            initBody(mass, stretchCoefficient, thickModulus, bendModulus);
        }

        private void initMesh(Vector3 bornPos, int length, float radius)
        {
            int vertLineNum = length + 1;//逐行生成，顶点的行数是长度+1
            vertices = new Vector3[vertLineNum * 6];
            uv = new Vector2[vertLineNum * 6];
            verticesPosIndex = new int[vertLineNum][];
            triangles = new int[length * 6 * 2 * 3];//六棱柱有6个面，每个面包含length*2个三角形，每个三角形包含3个顶点
            int triangleIterator = 0;
            for (int i = 0; i < vertLineNum; i++)
            {
                verticesPosIndex[i] = new int[6];
                Vector3 circlePos = new Vector3(bornPos.x, bornPos.y, bornPos.z + i);//每行6个顶点的圆心坐标
                for (int j = 0; j < 6; j++)
                {
                    int vertIndex = i * 6 + j;
                    verticesPosIndex[i][j] = vertIndex;
                    Vector3 vertex = new Vector3();
                    double angle = Math.PI / 3 * j;
                    vertex.x = (float)(circlePos.x + radius * Math.Cos(angle));
                    vertex.y = (float)(circlePos.y + radius * Math.Sin(angle));
                    vertex.z = circlePos.z;
                    vertices[vertIndex] = vertex;
                    uv[vertIndex].x = 1.0f / 6 * j;
                    uv[vertIndex].y = 1.0f / length * i;
                    if (i < vertLineNum - 1)
                    {
                        triangles[triangleIterator++] = vertIndex;
                        triangles[triangleIterator++] = j == 5 ? vertIndex + 1 : vertIndex + 7;
                        triangles[triangleIterator++] = j == 5 ? vertIndex - 5 : vertIndex + 1;
                        triangles[triangleIterator++] = vertIndex;
                        triangles[triangleIterator++] = vertIndex + 6;
                        triangles[triangleIterator++] = j == 5 ? vertIndex + 1 : vertIndex + 7;
                    }
                }
            }
            mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
        }

        private void initBody(float mass, float stretchCoefficient, float thickModulus, float bendModulus)
        {
            body = new BraidBody(mesh, mass, stretchCoefficient);
            initBendingConstraints(thickModulus, bendModulus);
            //TODO 使用应力约束来代替距离和弯曲约束，会弯曲得更自然
            //initShearStressConstraint(thickModulus);
        }

        private void initBendingConstraints(float thickModulus, float bendModulus)
        {
            for (int i = 0; i < verticesPosIndex.Length; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    int vertIndex1 = verticesPosIndex[i][j];
                    int vertIndex2 = j < 5 ? verticesPosIndex[i][j + 1] : j - 5;
                    int vertIndex3 = j < 4 ? verticesPosIndex[i][j + 2] : j - 4;
                    BendingConstraint b1 = new BendingConstraint(body, vertIndex1, vertIndex2, vertIndex3, thickModulus);
                    body.constraints.Add(b1);
                    if (i < verticesPosIndex.Length - 2)
                    {
                        int vertIndex21 = verticesPosIndex[i + 1][j];
                        int vertIndex31 = verticesPosIndex[i + 2][j];
                        BendingConstraint b2 = new BendingConstraint(body, vertIndex1, vertIndex21, vertIndex31, bendModulus);
                        body.constraints.Add(b2);
                    }
                }
            }
        }
        //TODO 使用应力约束来代替距离和弯曲约束，会弯曲得更自然
        private void initShearStressConstraint(float thickModulus)
        {
            for (int i = 0; i < verticesPosIndex.Length; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (i < verticesPosIndex.Length - 1)
                    {
                        int vertIndex1 = verticesPosIndex[i][j];
                        int vertIndex2 = j < 5 ? verticesPosIndex[i][j + 1] : j - 5;
                        int vertIndex3 = j < 4 ? verticesPosIndex[i][j + 2] : j - 4;
                        int vertIndex4 = j < 5 ? verticesPosIndex[i + 1][j + 1] : j - 5;
                        ShearStressConstraint c = new ShearStressConstraint(body, vertIndex1, vertIndex2, vertIndex3, vertIndex4, thickModulus);
                        body.constraints.Add(c);
                    }
                    if (i > 0)
                    {
                        int vertIndex1 = verticesPosIndex[i][j];
                        int vertIndex2 = j < 5 ? verticesPosIndex[i][j + 1] : j - 5;
                        int vertIndex3 = j < 4 ? verticesPosIndex[i][j + 2] : j - 4;
                        int vertIndex4 = j < 5 ? verticesPosIndex[i - 1][j + 1] : j - 5;
                        ShearStressConstraint c = new ShearStressConstraint(body, vertIndex1, vertIndex2, vertIndex3, vertIndex4, thickModulus);
                        body.constraints.Add(c);
                    }
                }
            }
        }
    }
}
