using UnityEngine;
using System.Collections;

public class DynamicSprite_Master : MonoBehaviour
{
	private Texture2D atlas;
	private Sprite[] frames;

	void Awake()
	{
		atlas = (Texture2D)Resources.Load("Atlas2");
		frames = Resources.LoadAll<Sprite>("Atlas2");
	}

	void Start()
	{
		SpriteRenderer renderer;
		GameObject gameObject;
		
		GameObject parent = new GameObject("Parent");

		gameObject = new GameObject("Object");
		renderer = gameObject.AddComponent<SpriteRenderer>();
		Sprite sprite = Sprite.Create(atlas, new Rect(0, 1024, 200, 30), new Vector2(0.5f, 0.5f), 100);
		renderer.sprite = sprite;
		renderer.sortingLayerName = "Elements";
		renderer.sortingOrder = 1;
		gameObject.transform.parent = parent.transform;

		//Inner Shadow - Corners
		gameObject = new GameObject("Inner Corner TL");
		renderer = gameObject.AddComponent<SpriteRenderer>();
		renderer.sprite = frames[0];
		renderer.sortingLayerName = "Elements";
		renderer.sortingOrder = 2;
		gameObject.transform.position = new Vector3(-0.95f, 0.1f, 0);
		gameObject.transform.parent = parent.transform;

		gameObject = new GameObject("Inner Corner TR");
		renderer = gameObject.AddComponent<SpriteRenderer>();
		renderer.sprite = frames[0];
		renderer.sortingLayerName = "Elements";
		renderer.sortingOrder = 2;
		gameObject.transform.position = new Vector3(0.95f, 0.1f, 0);
		gameObject.transform.localEulerAngles = new Vector3(0, 0, 270);
		gameObject.transform.parent = parent.transform;

		gameObject = new GameObject("Inner Corner BR");
		renderer = gameObject.AddComponent<SpriteRenderer>();
		renderer.sprite = frames[0];
		renderer.sortingLayerName = "Elements";
		renderer.sortingOrder = 2;
		gameObject.transform.position = new Vector3(0.95f, -0.1f, 0);
		gameObject.transform.localEulerAngles = new Vector3(0, 0, 180);
		gameObject.transform.parent = parent.transform;

		gameObject = new GameObject("Inner Corner BL");
		renderer = gameObject.AddComponent<SpriteRenderer>();
		renderer.sprite = frames[0];
		renderer.sortingLayerName = "Elements";
		renderer.sortingOrder = 2;
		gameObject.transform.position = new Vector3(-0.95f, -0.1f, 0);
		gameObject.transform.localEulerAngles = new Vector3(0, 0, 90);
		gameObject.transform.parent = parent.transform;

		//Edges
		gameObject = new GameObject("Inner Edge T");
		renderer = gameObject.AddComponent<SpriteRenderer>();
		renderer.sprite = frames[1];
		renderer.sortingLayerName = "Elements";
		renderer.sortingOrder = 2;
		gameObject.transform.localScale = new Vector3(190, 1f, 1f);
		gameObject.transform.position = new Vector3(-0.95f, 0.1f, 0);
		gameObject.transform.parent = parent.transform;

		gameObject = new GameObject("Inner Edge R");
		renderer = gameObject.AddComponent<SpriteRenderer>();
		renderer.sprite = frames[1];
		renderer.sortingLayerName = "Elements";
		renderer.sortingOrder = 2;
		gameObject.transform.localScale = new Vector3(20, 1f, 1f);
		gameObject.transform.position = new Vector3(0.95f, 0.1f, 0);
		gameObject.transform.localEulerAngles = new Vector3(0, 0, 270);
		gameObject.transform.parent = parent.transform;

		gameObject = new GameObject("Inner Edge B");
		renderer = gameObject.AddComponent<SpriteRenderer>();
		renderer.sprite = frames[1];
		renderer.sortingLayerName = "Elements";
		renderer.sortingOrder = 2;
		gameObject.transform.localScale = new Vector3(190, 1f, 1f);
		gameObject.transform.position = new Vector3(0.95f, -0.1f, 0);
		gameObject.transform.localEulerAngles = new Vector3(0, 0, 180);
		gameObject.transform.parent = parent.transform;

		gameObject = new GameObject("Inner Edge L");
		renderer = gameObject.AddComponent<SpriteRenderer>();
		renderer.sprite = frames[1];
		renderer.sortingLayerName = "Elements";
		renderer.sortingOrder = 2;
		gameObject.transform.localScale = new Vector3(20, 1f, 1f);
		gameObject.transform.position = new Vector3(-0.95f, -0.1f, 0);
		gameObject.transform.localEulerAngles = new Vector3(0, 0, 90);
		gameObject.transform.parent = parent.transform;

		//Outer Shadow
		gameObject = new GameObject("Outer Corner TL");
		renderer = gameObject.AddComponent<SpriteRenderer>();
		renderer.sprite = frames[2];
		renderer.sortingLayerName = "Elements";
		renderer.sortingOrder = 0;
		gameObject.transform.position = new Vector3(-0.98f, 0.13f, 0);
		gameObject.transform.parent = parent.transform;
	}

	void Update()
	{

	}
}
