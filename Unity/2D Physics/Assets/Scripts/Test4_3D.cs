using UnityEngine;
using System.Collections;

public class Test4_3D : MonoBehaviour
{
	public Material material;

	void Start()
	{
		for (int i = 0; i < 50; i++)
			ObjectFactory.CreateDiskMesh(10, material);
	}

	void Update()
	{

	}
}
