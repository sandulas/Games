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

		gameObject = new GameObject("test");
		renderer = gameObject.AddComponent<SpriteRenderer>();
		Sprite sprite = Sprite.Create(atlas, new Rect(0, 1024, 200, 30), new Vector2(0.5f, 0.5f), 100);
		renderer.sprite = sprite;
		renderer.sortingLayerName = "Elements";
		renderer.sortingOrder = 1;

		gameObject = new GameObject("test2");
		renderer = gameObject.AddComponent<SpriteRenderer>();
		renderer.sprite = frames[0];
		renderer.sortingLayerName = "Elements";
		renderer.sortingOrder = 2;
		gameObject.transform.position = new Vector3(-0.95f, 0.1f, 0);


		gameObject = new GameObject("test3");
		renderer = gameObject.AddComponent<SpriteRenderer>();
		renderer.sprite = frames[2];
		renderer.sortingLayerName = "Elements";
		renderer.sortingOrder = 0;
		gameObject.transform.position = new Vector3(-0.98f, 0.13f, 0);
	}

	void Update()
	{

	}
}
