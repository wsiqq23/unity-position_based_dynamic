using Assets.src.body;
using Assets.src.braid;
using Assets.src.constraint;
using Assets.src.force;
using Assets.src.solver;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BraidDemo : MonoBehaviour
{
    // Start is called before the first frame update
    private Mesh mesh;
    private Material material;
    private Matrix4x4[] drawPlanePosMatrix;
    private DateTime startTime;
    private Solver solver;
    private BraidBody braidBody;
    BraidGenerator meshGenerator;
    List<AbsolutelyPosConstraint> jointConstraints = new List<AbsolutelyPosConstraint>();
    void Start()
    {
        Application.targetFrameRate = 60;
        startTime = DateTime.Now;
        initBraid();
    }

    private void initBraid()
    {
        meshGenerator = new BraidGenerator(new Vector3(0, 20, 0), 20, 1, 1, 0.3f, 0.5f, 0.5f);
        mesh = meshGenerator.mesh;
        braidBody = meshGenerator.body;
        braidBody.damping = 1;
        material = Resources.Load<Material>("DoubleEdgeMaterial");
        drawPlanePosMatrix = new Matrix4x4[] { Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(Vector3.zero), Vector3.one) };
        solver = new Solver();
        solver.addBody(braidBody);
        solver.addForce(new DirectedForce(0, -9.8f, 0));
        Bounds jointBounds = new Bounds(new Vector3(0, 20, 0), new Vector3(2, 2, 1));
        for (int i = 0; i < braidBody.particlesNum; i++)
        {
            Vector3 pos = braidBody.positions[i];
            if (jointBounds.Contains(pos))
            {
                AbsolutelyPosConstraint posConstraint = new AbsolutelyPosConstraint(braidBody, i);
                posConstraint.setPosition(pos);
                jointConstraints.Add(posConstraint);
                braidBody.constraints.Add(posConstraint);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        //加一个位移观察效果
        TimeSpan time = DateTime.Now - startTime;
        if (time.TotalSeconds > 3)
        {
            foreach (AbsolutelyPosConstraint c in jointConstraints)
            {
                c.y += (float)(Math.Sin(time.TotalMilliseconds / 500) / 8);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            solver.solve((float)(1.0 / 60.0 / 3.0));
        }
        mesh.vertices = braidBody.positions;
        Graphics.DrawMeshInstanced(mesh, 0, material, drawPlanePosMatrix, 1);
    }
}
