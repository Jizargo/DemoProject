using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPoints : MonoBehaviour {

    public ComputeShader comShader;
    public Shader shader;
    public Transform point;
    ComputeBuffer dataBuffer;
    public Material mat;
    int kernel;
    int init = 0;
    public float radius;

    private int scrollSpeed1;
    private int scrollSpeed2 = 32;
    private float timer = 0;
    // Use this for initialization
    void Start () {
        dataBuffer = new ComputeBuffer(4 * 4 * 32 * 32, 36);//算出buffer数量
        kernel = comShader.FindKernel("CSMain");
        comShader.SetBuffer(kernel, "dataBuffer", dataBuffer);
        comShader.SetFloat("radius", radius);
    }
    
    private void OnRenderObject()
    {

        comShader.Dispatch(kernel, 32, 32, 1);

        mat.SetBuffer("dataBuffer", dataBuffer);

        comShader.SetVector("mousePos", point.position);
        comShader.SetFloat("deltaTime", Time.deltaTime);
        comShader.SetVector("originPos", transform.position);
        comShader.SetInt("init", init);

        mat.SetPass(0);

        Graphics.DrawProcedural(MeshTopology.Points, 4 * 4 * scrollSpeed1 * scrollSpeed2);//scrollSpeed2 = 32?max:false
        init = 1;
    }

    private void OnDestroy()
    {
        dataBuffer.Release();
    }

    // Update is called once per frame
    void Update () {
        timer += Time.deltaTime * 10f;
        if (scrollSpeed1 < 32)
        {
            if (timer >1)
            {
                timer = 0;
                scrollSpeed1 += 1;
            }
            
        }
        else
        {
            scrollSpeed1 = 32;
            if (timer >1)
            {
                timer = 0;
                scrollSpeed2 -= 1;
            }
            if (scrollSpeed2 <= 0)
            {
                scrollSpeed2 = 0;
                int cache = 0;
                cache = scrollSpeed1;
                scrollSpeed1 = scrollSpeed2;
                scrollSpeed2 = cache;
            }
        }
        point.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z - Camera.main.transform.position.z));
	}
}
