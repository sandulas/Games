using UnityEngine;
using System.Collections;

public class DynamicSprite_Master : MonoBehaviour
{
	public Texture2D atlas;


	void Start()
	{
		GameObject gameObject = new GameObject("test");
		gameObject.AddComponent<SpriteRenderer>();

		Sprite sprite = Sprite.Create(atlas, new Rect(1024, 1024, 200, 200), new Vector2(0.5f, 0.5f), 100);
		gameObject.GetComponent<SpriteRenderer>().sprite = sprite;

		//Debug.Log(gameObject.transform.position);
	}

	void Update()
	{

	}
}
