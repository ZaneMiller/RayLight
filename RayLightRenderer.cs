using UnityEngine;
using System.Collections;

public class RayLightRenderer : MonoBehaviour {

	public 

	void Start()
	{
		Camera mc = Camera.main;
		Camera c = GetComponent<Camera>();
		c.aspect = mc.aspect;
		c.orthographic = mc.orthographic;
		c.orthographicSize = mc.orthographicSize;
	}

	//Draw dem triangles
	void DrawTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Material mat, float depth)
	{
		v1.z = depth;
		v2.z = depth;
		v3.z = depth;
		GL.PushMatrix();
		mat.SetPass(0);
		GL.LoadOrtho();
		GL.Color(mat.color);
		GL.Begin(GL.TRIANGLES);
		GL.Vertex(v1);
		GL.Vertex(v2);
		GL.Vertex(v3);
		GL.End();
		GL.PopMatrix();
	}
	
	//Get dem triangles
	void OnPostRender()
	{
		foreach(RayLight l in FindObjectsOfType<RayLight>())
		{
			Vector3[] endpoints = l.endpoints;
			int len = endpoints.Length;
			for(int i = 0; i < len; i++)
			{
				Vector3 next = i >= len - 1 ? endpoints[0] : endpoints[i + 1];
				DrawTriangle(Camera.main.WorldToViewportPoint(l.transform.position), Camera.main.WorldToViewportPoint(endpoints[i]), Camera.main.WorldToViewportPoint(next), l.mat, l.transform.position.z);
			}
		}
	}
}
