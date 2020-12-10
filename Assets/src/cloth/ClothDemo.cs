using Assets.src.body;
using Assets.src.constraint;
using Assets.src.force;
using Assets.src.solver;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ClothDemo : MonoBehaviour
{
    private Mesh mesh;
    private Material material;
    private Matrix4x4[] drawPlanePosMatrix;
    DateTime startTime;
    private ClothBody clothBody;
    private Solver solver;
    private const float width = 20;
    private const float height = 12;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        startTime = DateTime.Now;
        Vector3 pos = new Vector3(0, 10, 0);
        initClothMesh(width, height, pos);
        initClothBody();
    }
    List<int> jointIndexes = new List<int>();
    private void initClothMesh(float width, float height, Vector3 position)
    {
        int columns = (int)(width + 1);
        int rows = (int)(height + 1);
        int verticlesNum = columns * rows;
        mesh = new Mesh();
        List<Vector3> verticles = new List<Vector3>(verticlesNum);
        List<Vector3> normals = new List<Vector3>(verticlesNum);
        List<Vector4> tangents = new List<Vector4>(verticlesNum);
        List<Vector2> uvs = new List<Vector2>(verticlesNum);
        List<int> triangleVerts = new List<int>();
        drawPlanePosMatrix = new Matrix4x4[] { Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(Vector3.zero), Vector3.one) };
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 localPos = new Vector3(j, i, 0) + position;
                int index = i * columns + j;
                verticles.Add(localPos);
                normals.Add(Vector3.back);
                tangents.Add(new Vector4(-1, 0, 0, -1));
                uvs.Add(new Vector2(1.0f / width * j, 1.0f / height * i));
                if (i < height && j < width)
                {
                    triangleVerts.Add(index);
                    triangleVerts.Add(index + 1);
                    triangleVerts.Add(index + columns + 1);
                    triangleVerts.Add(index);
                    triangleVerts.Add(index + columns + 1);
                    triangleVerts.Add(index + columns);
                }
                if (j == 0)
                {
                    jointIndexes.Add(index);
                }
            }
        }
        mesh.vertices = verticles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.tangents = tangents.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangleVerts.ToArray();
        mesh.bounds = new Bounds(new Vector3(0, 0, 0), new Vector3(width, height, 0));
        material = Resources.Load<Material>("Cloth/ClothMaterial");
    }
    DirectedForce windX, windY, windZ;
    List<AbsolutelyPosConstraint> jointConstraints = new List<AbsolutelyPosConstraint>();
    private void initClothBody()
    {
        solver = new Solver();
        clothBody = new ClothBody(mesh, 0.5f, 1, 0.25f, 0.25f);
        clothBody.damping = 1;
        solver.addBody(clothBody);
        windX = new DirectedForce(10, 0, 0);
        windY = new DirectedForce(0, 0, 0);
        windZ = new DirectedForce(0, 0, 0);
        solver.addForce(windX);
        solver.addForce(windY);
        solver.addForce(windZ);
        solver.addForce(new DirectedForce(0, -9.8f, 0));
        for (int i = 0; i < jointIndexes.Count; i++)
        {
            int index = jointIndexes[i];
            AbsolutelyPosConstraint posConstraint = new AbsolutelyPosConstraint(clothBody, index);
            posConstraint.setPosition(mesh.vertices[index]);
            jointConstraints.Add(posConstraint);
            clothBody.constraints.Add(posConstraint);
        }
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            solver.solve((float)(1.0 / 60.0 / 3.0));
        }
        mesh.vertices = clothBody.positions;
        Graphics.DrawMeshInstanced(mesh, 0, material, drawPlanePosMatrix, 1);
        TimeSpan timeSpan = DateTime.Now - startTime;
        System.Random r = new System.Random();
        windX.direction.x += (float)Math.Sin(timeSpan.TotalMilliseconds);
        windY.direction.y = (float)Math.Sin(timeSpan.TotalMilliseconds + r.Next(10, 100)) * 40.0f;
        windZ.direction.z = (float)Math.Sin(timeSpan.TotalMilliseconds + r.Next(10, 100)) * 20.0f;
    }

    void OnDestroy()
    {
    }
}
