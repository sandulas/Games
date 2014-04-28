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

		obj = ItemFactory.CreateItem(ItemShape.Circle, ItemMaterial.FixedMetal);
		obj.transform.position = new Vector3(-3, 0, 0);

		obj = ItemFactory.CreateItem(ItemShape.Circle, ItemMaterial.Ice);
		obj.transform.position = new Vector3(-1.5f, 0, 0);

		obj = ItemFactory.CreateItem(ItemShape.Circle, ItemMaterial.Metal);
		obj.transform.position = new Vector3(0, 0, 0);

		obj = ItemFactory.CreateItem(ItemShape.Circle, ItemMaterial.Rubber);
		obj.transform.position = new Vector3(1.5f, 0, 0);

		obj = ItemFactory.CreateItem(ItemShape.Circle, ItemMaterial.Wood);
		obj.transform.position = new Vector3(3f, 0, 0);

		obj = ItemFactory.CreateItem(ItemShape.Rectangle, ItemMaterial.Rubber);
		obj.transform.position = new Vector3(4f, 0, 0);



		return;
	
		for (int i = 0; i < 50; i++)
		{
			ItemFactory.CreateItem(ItemShape.Circle, ItemMaterial.Ice);
		}

		for (int i = 0; i < 1; i++)
		{
			obj = ItemFactory.CreateItem(ItemShape.Circle, ItemMaterial.Wood);
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
