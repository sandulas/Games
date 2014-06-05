using UnityEngine;
using System.Collections;
using ThisProject;

public class SceneManager : MonoBehaviour
{
	public Vector2 SceneSize;
	public GameObject Background;

	static bool loaded = false;
	GameObject obj;

	void Start()
	{
		Time.timeScale = 1;

		Background.transform.localScale = new Vector3(SceneSize.x, SceneSize.y, 1);
		Background.renderer.material.mainTextureScale = new Vector2(SceneSize.x / 10, SceneSize.y / 10);
	}

	void Update()
	{		
		if (Time.realtimeSinceStartup > 1 && !loaded)
		{
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					obj = Item.Create((ItemShape)j, (ItemMaterial)i, 1.5f, 1.5f);
					obj.transform.position = new Vector3(i * 1.5f - 3, j * 1.5f, obj.transform.position.z);

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
