using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RayLight : MonoBehaviour {
	
	public Material mat;
	public float range;
	public LayerMask layerMask;
	
	[HideInInspector]
	public Vector3[] endpoints;
	
	
	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, range);
	}
	
	float GetAngle(Vector3 v)
	{
		Vector2 diff = v - transform.position;
		return Mathf.Atan2(diff.y, diff.x);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		//Get the unquie vertices of all ShadowCasters in range
		List<Vector3> uniqueVertices = new List<Vector3>();
		foreach(RayLightVertex rlv in FindObjectsOfType<RayLightVertex>())
		{
			if(rlv.useChildren)
			{
				foreach(Transform t in rlv.GetComponentInChildren<Transform>())
				{
					if(Vector2.Distance(transform.position, t.position) <= range)
						uniqueVertices.Add(t.position);
				}
			}
			else
			{
				if(Vector2.Distance(transform.position, rlv.transform.position) <= range)
				{
					uniqueVertices.Add(rlv.transform.position);
				}
			}

		}
		
		float[] angles = new float[uniqueVertices.Count * 3];
		
		//Find all unique angles
		int uniqueVertextCount = uniqueVertices.Count * 3;
		for(int i = 3; i <= uniqueVertextCount; i += 3)
		{
			//We create angle offsets here to account of the edges of objects that light should move past.
			//This also creates the correct ordering of vertexts for drawing.
			float a = GetAngle(uniqueVertices[(i / 3) - 1]);
			angles[i - 3] = a - 0.001f;
			angles[i - 2] = a;
			angles[i - 1] = a + 0.001f;
		}
		
		//Sort the array by angle
		Array.Sort<float> (angles);
		
		int angleLength = angles.Length;
		
		
		endpoints = new Vector3[angleLength];
		
		//Find the endpoint of each light shaft
		for(int i = 0; i < angleLength; i++)
		{
			Vector2 d = new Vector2(Mathf.Cos(angles[i]), Mathf.Sin(angles[i]));
			RaycastHit2D rh = Physics2D.Raycast(transform.position, d, range, layerMask);
			if(rh)
				endpoints[i] = rh.point;
			else
				endpoints[i] = (Vector2)transform.position + range * d;
		}
	}
}
