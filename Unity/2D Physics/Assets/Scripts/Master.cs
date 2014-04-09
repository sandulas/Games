using UnityEngine;
using System.Collections;

public enum ObjectMaterial { FixedMetal, Metal, Wood, Rubber, Ice }

public class Master
{
	private static GameObject InnerCorner1, InnerEdge1, OuterCorner1, OuterEdge1;

	private static Material spriteMaterial;

	private static Texture2D atlas1;
	private const float pixelsToUnits = 200f;
	
	private static Rect InnerCorner1Rect = new Rect(1, 2, 9, 9);
	private static Rect InnerEdge1Rect = new Rect(13, 2, 1, 9);
	private static Rect OuterCorner1Rect = new Rect(1, 13, 19, 19);
	private static Rect OuterEdge1Rect = new Rect(23, 13, 1, 19);
	private const int Outer1Offset = 10;

	private enum EffectType { Inner, Outer }	

	static Master()
	{
		atlas1 = (Texture2D)Resources.Load("Atlas4");
		spriteMaterial = Resources.Load<Material>("SpriteMaterial");

		InnerCorner1 = CreateObject(InnerCorner1Rect, new Vector2(0f, 1f)); InnerCorner1.SetActive(false);
		InnerEdge1 = CreateObject(InnerEdge1Rect, new Vector2(0.5f, 1f)); InnerEdge1.SetActive(false);
		OuterCorner1 = CreateObject(OuterCorner1Rect, new Vector2(1f, 0f)); OuterCorner1.SetActive(false);
		OuterEdge1 = CreateObject(OuterEdge1Rect, new Vector2(0.5f, 0f)); OuterEdge1.SetActive(false);
	}

	private static GameObject CreateObject(Rect rect, Vector2 pivot)
	{

		SpriteRenderer renderer;
		GameObject gameObject;

		gameObject = new GameObject("Object");
		renderer = gameObject.AddComponent<SpriteRenderer>();
		renderer.sprite = Sprite.Create(atlas1, rect, pivot, pixelsToUnits);
		renderer.material = spriteMaterial;

		return gameObject;
	}

	private static void createEffect(GameObject parentObject, GameObject effectSource, EffectType effectType, string name, Vector3 position, Vector3 rotation, Vector3 scale)
	{
		GameObject effect;

		effect = (GameObject)GameObject.Instantiate(effectSource, position, Quaternion.Euler(rotation));
		effect.transform.localScale = scale;

		effect.name = name;
		effect.transform.parent = parentObject.transform;
		effect.renderer.sortingLayerID = parentObject.renderer.sortingLayerID;
		if (effectType == EffectType.Inner)
			effect.renderer.sortingOrder = parentObject.renderer.sortingOrder + 1;
		else
			effect.renderer.sortingOrder = parentObject.renderer.sortingOrder - 1;

		effect.SetActive(true);

	}

	private static void createBoxEffect(GameObject parentObject, GameObject effectCorner, GameObject effectEdge, EffectType effectType, int pixelOffset)
	{
		float parentX, parentY, parentWidth, parentHeight;

		Rect parentRect = ((SpriteRenderer)parentObject.renderer).sprite.rect;

		parentX = parentObject.transform.localPosition.x;
		parentY = parentObject.transform.localPosition.y;
		parentWidth = parentRect.width / pixelsToUnits;
		parentHeight = parentRect.height / pixelsToUnits;

		Vector3 position, rotation, scale;
		position = Vector3.zero; rotation = Vector3.zero; scale = Vector3.one;

		float offset = pixelOffset / pixelsToUnits;
		string effectName = "";
				
		//Corners
		position.Set(parentX - parentWidth / 2 + offset, parentY + parentHeight / 2 - offset, 1);
		effectName = effectType.ToString() + " Top Left";
		createEffect(parentObject, effectCorner, effectType, effectName, position, rotation, scale);

		position.Set(parentX + parentWidth / 2 - offset, parentY + parentHeight / 2 - offset, 1);
		rotation.Set(0, 0, -90);
		effectName = effectType.ToString() + " Top Right";
		createEffect(parentObject, effectCorner, effectType, effectName, position, rotation, scale);

		position.Set(parentX + parentWidth / 2 - offset, parentY - parentHeight / 2 + offset, 1);
		rotation.Set(0, 0, 180);
		effectName = effectType.ToString() + " Bottom Right";
		createEffect(parentObject, effectCorner, effectType, effectName, position, rotation, scale);

		position.Set(parentX - parentWidth / 2 + offset, parentY - parentHeight / 2 + offset, 1);
		rotation.Set(0, 0, 90);
		effectName = effectType.ToString() + " Bottom Left";
		createEffect(parentObject, effectCorner, effectType, effectName, position, rotation, scale);

		//Edges		
		int cornerPixelWidth = (int)((SpriteRenderer)effectCorner.renderer).sprite.rect.width;
		int edgePixelWidth =  (int)((SpriteRenderer)effectEdge.renderer).sprite.rect.width;

		int edgePixelPadding;
		if (effectType == EffectType.Outer) edgePixelPadding = pixelOffset;
		else edgePixelPadding = cornerPixelWidth;

		position.Set(parentX, parentY + parentHeight / 2 - offset, 1);
		scale.Set((parentRect.width - edgePixelPadding * 2) / edgePixelWidth, 1, 1);
		rotation.Set(0, 0, 0);
		effectName = effectType.ToString() + " Top";
		createEffect(parentObject, effectEdge, effectType, effectName, position, rotation, scale);

		position.Set(parentX + parentWidth / 2 - offset, parentY, 1);
		scale.Set((parentRect.height - edgePixelPadding * 2) / edgePixelWidth, 1, 1);
		rotation.Set(0, 0, -90);
		effectName = effectType.ToString() + " Right";
		createEffect(parentObject, effectEdge, effectType, effectName, position, rotation, scale);

		position.Set(parentX, parentY - parentHeight / 2 + offset, 1);
		scale.Set((parentRect.width - edgePixelPadding * 2) / edgePixelWidth, 1, 1);
		rotation.Set(0, 0, 180);
		effectName = effectType.ToString() + " Bottom";
		createEffect(parentObject, effectEdge, effectType, effectName, position, rotation, scale);

		position.Set(parentX - parentWidth / 2 + offset, parentY, 1);
		scale.Set((parentRect.height - edgePixelPadding * 2) / edgePixelWidth, 1, 1);
		rotation.Set(0, 0, 90);
		effectName = effectType.ToString() + " Left";
		createEffect(parentObject, effectEdge, effectType, effectName, position, rotation, scale);
	}

	private static void CreateBoxEffect1(GameObject parentObject)
	{
		createBoxEffect(parentObject, OuterCorner1, OuterEdge1, EffectType.Outer, Outer1Offset);
		createBoxEffect(parentObject, InnerCorner1, InnerEdge1, EffectType.Inner, 0);
	}

	public static GameObject CreateBox(ObjectMaterial material, Vector2 size)
	{
		GameObject gameObject;
		Rect rect = new Rect(0, 2048, 100, 100);

		switch (material)
		{
			case ObjectMaterial.FixedMetal:
				rect.Set(1, 540 - size.y + 1, size.x, size.y);
				break;
			case ObjectMaterial.Metal:
				rect.Set(1, 2046 - size.y + 1, size.x, size.y);
				break;
			case ObjectMaterial.Wood:
				rect.Set(1, 1544 - size.y + 1, size.x, size.y);
				break;
			case ObjectMaterial.Rubber:
				rect.Set(1, 1042 - size.y + 1, size.x, size.y);
				break;
			case ObjectMaterial.Ice:
				rect.Set(1, 1042 - size.y + 1, size.x, size.y);
				break;
		}

		gameObject = CreateObject(rect, new Vector2(0.5f, 0.5f));
		CreateBoxEffect1(gameObject);

		return gameObject;
	}
}
