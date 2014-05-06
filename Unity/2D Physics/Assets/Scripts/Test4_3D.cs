using UnityEngine;
using System.Collections;
using ThisProject;

public class Test4_3D : MonoBehaviour
{
	bool loaded = false;
	Texture2D atlas;

	void Start()
	{
		GameObject obj;

		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				obj = ItemFactory.CreateItem((ItemShape)j, (ItemMaterial)i);
				obj.transform.position = new Vector3(i * 1.5f, j * 1.5f);
			}
		}


		//ItemFactory.CreateItem(ItemShape.Triangle, ItemMaterial.Rubber);


		//for (int i = 0; i < 50; i++)
		//{
		//  obj = ItemFactory.CreateTriangleMesh();
		//  obj.renderer.sortingOrder = i;
		//}

		//for (int i = 0; i < 50; i++)
		//{
		//  obj = ItemFactory.CreateTriangleMesh2();
		//  obj.renderer.sortingOrder = i + 100;
		//}

		
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
