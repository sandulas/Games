﻿using UnityEngine;
using System.Collections;
using ThisProject;

public class Test4_3D : MonoBehaviour
{
	bool loaded = false;
	Texture2D atlas;

	void Start()
	{
		GameObject obj;

		//for (int i = -5; i < 5; i++)
		//{
		//  obj = ItemFactory.CreateItem(ItemShape.Circle, ItemMaterial.FixedMetal);
		//  obj.transform.position = new Vector3(-3, i * 0.5f, 0);

		//  obj = ItemFactory.CreateItem(ItemShape.Circle, ItemMaterial.Ice);
		//  obj.transform.position = new Vector3(-1.5f, i * 0.5f, 0);

		//  obj = ItemFactory.CreateItem(ItemShape.Circle, ItemMaterial.Metal);
		//  obj.transform.position = new Vector3(0, i * 0.5f, 0);

		//  obj = ItemFactory.CreateItem(ItemShape.Circle, ItemMaterial.Rubber);
		//  obj.transform.position = new Vector3(1.5f, i * 0.5f, 0);

		//  obj = ItemFactory.CreateItem(ItemShape.Circle, ItemMaterial.Wood);
		//  obj.transform.position = new Vector3(3f, i * 0.5f, 0);



		//  obj = ItemFactory.CreateItem(ItemShape.Rectangle, ItemMaterial.FixedMetal);
		//  obj.transform.position = new Vector3(-3, i * 0.5f + 1.5f, 0);

		//  obj = ItemFactory.CreateItem(ItemShape.Rectangle, ItemMaterial.Ice);
		//  obj.transform.position = new Vector3(-1.5f, i * 0.5f + 1.5f, 0);

		//  obj = ItemFactory.CreateItem(ItemShape.Rectangle, ItemMaterial.Metal);
		//  obj.transform.position = new Vector3(0f, i * 0.5f + 1.5f, 0);

		//  obj = ItemFactory.CreateItem(ItemShape.Rectangle, ItemMaterial.Rubber);
		//  obj.transform.position = new Vector3(1.5f, i * 0.5f + 1.5f, 0);

		//  obj = ItemFactory.CreateItem(ItemShape.Rectangle, ItemMaterial.Wood);
		//  obj.transform.position = new Vector3(3, i * 0.5f + 1.5f, 0);
		//}


		ItemFactory.CreateItem(ItemShape.Triangle, ItemMaterial.Rubber);


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
