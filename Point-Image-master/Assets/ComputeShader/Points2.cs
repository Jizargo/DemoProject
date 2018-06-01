using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Points2 : MonoBehaviour {

    public struct Particle
    {
        public Vector2 position;
        public Vector2 velocity;
    }

    public ComputeShader computeShader;
    public Material material;

    ComputeBuffer particles;

    const int WARP_SIZE = 1024;

    //粒子数量
    int size = 1024000;
    int stride;

    int warpCount;

    int kernelIndex;

    Particle[] initBuffer;

    /*private void OnDrawGizmos()
    {
        var p = new Particle[size];
        particles.GetData(p);
        for (int i = 0; i < size; i++)
        {
            Gizmos.DrawSphere(p.position, 0.1f);
        }
    }*/

    // Use this for initialization
    void Start()
    {
        //运算次数
        warpCount = Mathf.CeilToInt((float)size / WARP_SIZE);

        //计算数据所占内存大小
        stride = Marshal.SizeOf(typeof(Particle));
        //ComputeBuffer 用于向ComputeShader 传递自定义结构
        particles = new ComputeBuffer(size, stride);

        initBuffer = new Particle[size];

        for (int i = 0; i < size; i++)
        {
            initBuffer[i] = new Particle();
            initBuffer[i].position = Random.insideUnitCircle * 10f;
            initBuffer[i].velocity = Vector2.zero;
        }
        //写入 数据到 computeBuffer
        //之后会用于在 cpu 到 compute shader 到 shader 直接传递参数
        particles.SetData(initBuffer);

        //Compute Shader 里可执行函数的 id, 后面update里调用
        //Update 为Compute Shader 里函数的 函数名
        kernelIndex = computeShader.FindKernel("Update");

        //为compute shader 的 函数 写入数据buffer
        computeShader.SetBuffer(kernelIndex, "Particles", particles);

        //为shader 写入数据buffer
        material.SetBuffer("Particles", particles);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            //重置buffer 往compute shader 里更新自定义数据也是这样写
            particles.SetData(initBuffer);
        }

        //传递 int float Vector3 等数据 Vector2 和 Vector3 要转成 float[2]
        //float[3] 才可以用
        computeShader.SetInt("shouldMove", Input.GetMouseButton(0) ? 1 : 0);
        var mousePosition = GetMousePosition();
        computeShader.SetFloats("mousePosition", mousePosition);
        computeShader.SetFloat("dt", Time.deltaTime);

        //执行一次compute shader 里的函数
        computeShader.Dispatch(kernelIndex, warpCount, 1, 1);
    }

    float[] GetMousePosition()
    {
        var mp = Input.mousePosition;
        var v = Camera.main.ScreenToWorldPoint(mp);
        return new float[] { v.x, v.y };
    }

    //渲染效果
    void OnRenderObject()
    {
        //写入材质球 pass
        material.SetPass(0);
        //渲染粒子
        Graphics.DrawProcedural(MeshTopology.Points, 1, size);
    }

    void OnDestroy()
    {
        if (particles != null)
            particles.Release();
    }
}
