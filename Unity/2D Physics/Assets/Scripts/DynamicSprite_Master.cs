using UnityEngine;
using System.Collections;

public class DynamicSprite_Master : MonoBehaviour
{
	private Texture2D atlas, atlas2;
	private Sprite[] frames, frames2;

	void Awake()
	{
		atlas = (Texture2D)Resources.Load("Atlas2");
		atlas2 = (Texture2D)Resources.Load("Atlas3");
		frames = Resources.LoadAll<Sprite>("Atlas2");
		frames2 = Resources.LoadAll<Sprite>("Atlas3");

	}

	void Start()
	{

		GameObject testObject;

		testObject = Master.CreateObject(new Rect(1, 511, 150, 30), new Vector2(0.5f, 0.5f));
		testObject.renderer.sortingLayerName = "Elements";
		testObject.renderer.sortingOrder = 1;
		testObject.transform.position = new Vector3(0f, 0f, 0f);

		Master.ApplyBoxEffect1(testObject);

		//SpriteRenderer renderer;
		//GameObject gameObject;
		//Sprite sprite;		
		//GameObject parent = new GameObject("Parent");

		////Texture Test 1
		//gameObject = new GameObject("Object");
		//renderer = gameObject.AddComponent<SpriteRenderer>();
		//sprite = Sprite.Create(atlas, new Rect(1024, 1524, 100, 100), new Vector2(0.5f, 0.5f), 100f);
		//renderer.sprite = sprite;
		//renderer.sortingLayerName = "Elements";
		//renderer.sortingOrder = 1;
		//gameObject.transform.position = new Vector3(0f, 2f, 0);
		//gameObject.transform.parent = parent.transform;

		////Texture Test 2
		//gameObject = new GameObject("Object");
		//renderer = gameObject.AddComponent<SpriteRenderer>();
		//sprite = Sprite.Create(atlas2, new Rect(10, 1800, 200, 200), new Vector2(0.5f, 0.5f), 200f);
		//renderer.sprite = sprite;
		//renderer.sortingLayerName = "Elements";
		//renderer.sortingOrder = 1;
		//gameObject.transform.position = new Vector3(2f, 2f, 0);
		//gameObject.transform.parent = parent.transform;

		////Texture Test 1
		//gameObject = new GameObject("Test Wood");
		//renderer = gameObject.AddComponent<SpriteRenderer>();
		//renderer.sprite = frames2[0];
		//renderer.sortingLayerName = "Elements";
		//renderer.sortingOrder = 2;
		//gameObject.transform.position = new Vector3(-0.95f, 0.1f, 0);
		//gameObject.transform.parent = parent.transform;

		////Texture
		//gameObject = new GameObject("Object");
		//renderer = gameObject.AddComponent<SpriteRenderer>();
		//sprite = Sprite.Create(atlas, new Rect(0, 1024, 200, 30), new Vector2(0.5f, 0.5f), 100);
		//renderer.sprite = sprite;
		//renderer.sortingLayerName = "Elements";
		//renderer.sortingOrder = 1;
		//gameObject.transform.parent = parent.transform;

		////Inner Shadow - Corners
		//gameObject = new GameObject("Inner Corner TL");
		//renderer = gameObject.AddComponent<SpriteRenderer>();
		//renderer.sprite = frames[0];
		//renderer.sortingLayerName = "Elements";
		//renderer.sortingOrder = 2;
		//gameObject.transform.position = new Vector3(-0.95f, 0.1f, 0);
		//gameObject.transform.parent = parent.transform;

		//gameObject = new GameObject("Inner Corner TR");
		//renderer = gameObject.AddComponent<SpriteRenderer>();
		//renderer.sprite = frames[0];
		//renderer.sortingLayerName = "Elements";
		//renderer.sortingOrder = 2;
		//gameObject.transform.position = new Vector3(0.95f, 0.1f, 0);
		//gameObject.transform.localEulerAngles = new Vector3(0, 0, 270);
		//gameObject.transform.parent = parent.transform;

		//gameObject = new GameObject("Inner Corner BR");
		//renderer = gameObject.AddComponent<SpriteRenderer>();
		//renderer.sprite = frames[0];
		//renderer.sortingLayerName = "Elements";
		//renderer.sortingOrder = 2;
		//gameObject.transform.position = new Vector3(0.95f, -0.1f, 0);
		//gameObject.transform.localEulerAngles = new Vector3(0, 0, 180);
		//gameObject.transform.parent = parent.transform;

		//gameObject = new GameObject("Inner Corner BL");
		//renderer = gameObject.AddComponent<SpriteRenderer>();
		//renderer.sprite = frames[0];
		//renderer.sortingLayerName = "Elements";
		//renderer.sortingOrder = 2;
		//gameObject.transform.position = new Vector3(-0.95f, -0.1f, 0);
		//gameObject.transform.localEulerAngles = new Vector3(0, 0, 90);
		//gameObject.transform.parent = parent.transform;

		////Edges
		//gameObject = new GameObject("Inner Edge T");
		//renderer = gameObject.AddComponent<SpriteRenderer>();
		//renderer.sprite = frames[1];
		//renderer.sortingLayerName = "Elements";
		//renderer.sortingOrder = 2;
		//gameObject.transform.localScale = new Vector3(190, 1f, 1f);
		//gameObject.transform.position = new Vector3(-0.95f, 0.1f, 0);
		//gameObject.transform.parent = parent.transform;

		//gameObject = new GameObject("Inner Edge R");
		//renderer = gameObject.AddComponent<SpriteRenderer>();
		//renderer.sprite = frames[1];
		//renderer.sortingLayerName = "Elements";
		//renderer.sortingOrder = 2;
		//gameObject.transform.localScale = new Vector3(20, 1f, 1f);
		//gameObject.transform.position = new Vector3(0.95f, 0.1f, 0);
		//gameObject.transform.localEulerAngles = new Vector3(0, 0, 270);
		//gameObject.transform.parent = parent.transform;

		//gameObject = new GameObject("Inner Edge B");
		//renderer = gameObject.AddComponent<SpriteRenderer>();
		//renderer.sprite = frames[1];
		//renderer.sortingLayerName = "Elements";
		//renderer.sortingOrder = 2;
		//gameObject.transform.localScale = new Vector3(190, 1f, 1f);
		//gameObject.transform.position = new Vector3(0.95f, -0.1f, 0);
		//gameObject.transform.localEulerAngles = new Vector3(0, 0, 180);
		//gameObject.transform.parent = parent.transform;

		//gameObject = new GameObject("Inner Edge L");
		//renderer = gameObject.AddComponent<SpriteRenderer>();
		//renderer.sprite = frames[1];
		//renderer.sortingLayerName = "Elements";
		//renderer.sortingOrder = 2;
		//gameObject.transform.localScale = new Vector3(20, 1f, 1f);
		//gameObject.transform.position = new Vector3(-0.95f, -0.1f, 0);
		//gameObject.transform.localEulerAngles = new Vector3(0, 0, 90);
		//gameObject.transform.parent = parent.transform;

		////Outer Shadow
		//gameObject = new GameObject("Outer Corner TL");
		//renderer = gameObject.AddComponent<SpriteRenderer>();
		//renderer.sprite = frames[2];
		//renderer.sortingLayerName = "Elements";
		//renderer.sortingOrder = 0;
		//gameObject.transform.position = new Vector3(-0.98f, 0.13f, 0);
		//gameObject.transform.parent = parent.transform;
	}

	void Update()
	{

	}
}
