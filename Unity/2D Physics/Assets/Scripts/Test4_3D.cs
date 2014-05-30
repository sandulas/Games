using UnityEngine;
using System.Collections;
using ThisProject;

public class Test4_3D : MonoBehaviour
{
	static bool loaded = false;
	GameObject obj;

	void Start()
	{
		GameObject.Find("Quad").renderer.sortingLayerName = "Background";
	}

	void Update()
	{		
		if (Time.realtimeSinceStartup > 1 && !loaded)
		{
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					obj = Item.Create((ItemShape)j, (ItemMaterial)i, 6.6666f, 6.6666f);
					obj.transform.position = new Vector3(i * 1.5f, j * 1.5f);

					Item.Resize(obj, 1.5f, 0.5f);
				}
			}

			loaded = true;
		}

		//if (Time.realtimeSinceStartup > 2)
		//{
		//  obj = GameObject.Find("Item: Circle 10");
		//  Item.Resize(obj, obj.GetComponent<ItemProperties>().Width + 0.01f, 2);
		//  Item.ChangeMaterial(obj, ItemMaterial.Ice);
		//}
	}
}
