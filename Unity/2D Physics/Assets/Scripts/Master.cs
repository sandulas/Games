using UnityEngine;
using System.Collections;

public class Master
{
	public static GameObject InnerCorner1, InnerEdge1, OuterCorner1, OuterEdge1;

	private static Material spriteMaterial;
	public static Texture2D atlas1;

	static Master()
	{
		atlas1 = (Texture2D)Resources.Load("Atlas4");
		spriteMaterial = Resources.Load<Material>("SpriteMaterial");
		InnerCorner1 = CreateObject(atlas1, 1, 2, 9, 9, 0f, 1f);
		InnerEdge1 = CreateObject(atlas1, 13, 2, 1, 9, 0f, 1f);
		OuterCorner1 = CreateObject(atlas1, 1, 13, 19, 19, 1f, 0f);
		OuterEdge1 = CreateObject(atlas1, 23, 13, 1, 19, 0f, 0f);
	}

	public static GameObject CreateObject(Texture2D atlas, float x1, float y1, float x2, float y2, float pivot_x, float pivot_y)
	{

		SpriteRenderer renderer;
		GameObject gameObject;

		gameObject = new GameObject("Object");
		renderer = gameObject.AddComponent<SpriteRenderer>();
		renderer.sprite = Sprite.Create(atlas1, new Rect(x1, y1, x2, y2), new Vector2(pivot_x, pivot_y), 200f);
		renderer.material = spriteMaterial;

		return gameObject;
	}

	public static void ApplyBoxEffect1(GameObject gameObject)
	{
		Rect spriteRect = ((SpriteRenderer)gameObject.renderer).sprite.rect;
	}
}
