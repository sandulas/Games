using UnityEngine;
using System.Collections;

public class Test4_3D : MonoBehaviour
{
	void Start()
	{
		for (int i = 0; i < 50; i++)
			ObjectFactory.CreateDiskMesh(30);

		//for (int i = 0; i < 1500; i++)
		//  ObjectFactory.CreateTriangleMesh();

		for (int i = 0; i < 3000; i++)
			ObjectFactory.CreateTriangleMesh2();

		ObjectFactory.CreateCircleMesh(30, 0.5f, 9, 10);
	}

	void Update()
	{

	}
}
