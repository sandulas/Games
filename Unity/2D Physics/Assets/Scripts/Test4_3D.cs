using UnityEngine;
using System.Collections;
using ThisProject;

public class Test4_3D : MonoBehaviour
{
	bool loaded = false;
	Texture2D atlas;

	void Start()
	{
		for (int i = 0; i < 50; i++)
		{
			ItemFactory.CreateItem(ItemShape.Circle, ItemMaterial.Ice);
		}

		//for (int i = 0; i < 1500; i++)
		//  ObjectFactory.CreateTriangleMesh();

		//for (int i = 0; i < 3000; i++)
		//  ObjectFactory.CreateTriangleMesh2();

		for (int i = 0; i < 1; i++)
		{
			GameObject obj = ItemFactory.CreateItem(ItemShape.Circle, ItemMaterial.Wood);
			obj.transform.position = new Vector3(3, 0, 0);
		}

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
