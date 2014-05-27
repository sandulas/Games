﻿using UnityEngine;
using System.Collections;
using ThisProject;

public class Test4_3D : MonoBehaviour
{
	static bool loaded = false;
	GameObject obj;

	void Start()
	{
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
		if (Time.realtimeSinceStartup > 2 && !loaded)
		{
			Debug.Log(Time.realtimeSinceStartup);

			GameObject.Find("Quad").renderer.sortingLayerName = "Background";

			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					obj = Item.Create((ItemShape)j, (ItemMaterial)i, 1, 1);
					obj.transform.position = new Vector3(i * 1.5f, j * 1.5f);

					Item.Resize(obj, 1.5f, 0.5f);
				}
			}

			Item.ChangeMaterial(obj, ItemMaterial.Rubber);

			loaded = true;
			Debug.Log(Time.realtimeSinceStartup);
		}

		//if (Time.realtimeSinceStartup > 3)
		//{
		//  Item.Resize(obj, obj.GetComponent<ItemProperties>().Width + 0.01f, 2);
		//}
	}
}
