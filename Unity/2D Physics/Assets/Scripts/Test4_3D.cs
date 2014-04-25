using UnityEngine;
using System.Collections;
using ThisProject;

public class Test4_3D : MonoBehaviour
{
	bool loaded = false;
	Texture2D atlas;

	void Start()
	{
		for (int i = 0; i < 1; i++)
			ObjectFactory.CreateCircle(1.5f, PhysicsMaterial.Metal);

		//for (int i = 0; i < 1500; i++)
		//  ObjectFactory.CreateTriangleMesh();

		//for (int i = 0; i < 3000; i++)
		//  ObjectFactory.CreateTriangleMesh2();

		for (int i = 0; i < 1; i++)
			//ObjectFactory.CreateCircleMesh(0.5f, 9, 10);
			ObjectFactory.CreateCircleEffect(1.5f, Effects.Ice);

		ObjectFactory.CreateCircleEffect(1.5f, Effects.Solid);
	}

	void Update()
	{
		//if (Time.realtimeSinceStartup > 3 && !loaded)
		//{
		//  Debug.Log(Time.realtimeSinceStartup);
		//  atlas = (Texture2D)Resources.Load("Atlas1");
		//  loaded = true;
		//  Debug.Log(Time.realtimeSinceStartup);
		//}
	}
}
