using UnityEngine;
using System.Collections;

public class Master
{
	public static GameObject InnerCorner1, InnerEdge1, OuterCorner1, OuterEdge1;

	private static Material spriteMaterial;

	private static Texture2D atlas1;
	private const float pixelsToUnits = 200f;
	
	private static Rect InnerCorner1Rect = new Rect(1, 2, 9, 9);
	private static Rect InnerEdge1Rect = new Rect(13, 2, 1, 9);
	private static Rect OuterCorner1Rect = new Rect(1, 13, 19, 19);
	private static Rect OuterEdge1Rect = new Rect(23, 13, 1, 19);
	private const float Outer1Offset = 10 / pixelsToUnits;

	private enum EffectType { Inner, Outer }
	private enum EffectPosition { TopLeft, Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left }
	

	static Master()
	{
		atlas1 = (Texture2D)Resources.Load("Atlas4");
		spriteMaterial = Resources.Load<Material>("SpriteMaterial");

		InnerCorner1 = CreateObject(InnerCorner1Rect, new Vector2(0f, 1f)); InnerCorner1.SetActive(false);
		InnerEdge1 = CreateObject(InnerEdge1Rect, new Vector2(0.5f, 1f)); InnerEdge1.SetActive(false);
		OuterCorner1 = CreateObject(OuterCorner1Rect, new Vector2(1f, 0f)); OuterCorner1.SetActive(false);
		OuterEdge1 = CreateObject(OuterEdge1Rect, new Vector2(0.5f, 0f)); OuterEdge1.SetActive(false);
	}

	public static GameObject CreateObject(Rect rect, Vector2 pivot)
	{

		SpriteRenderer renderer;
		GameObject gameObject;

		gameObject = new GameObject("Object");
		renderer = gameObject.AddComponent<SpriteRenderer>();
		renderer.sprite = Sprite.Create(atlas1, rect, pivot, pixelsToUnits);
		renderer.material = spriteMaterial;

		return gameObject;
	}

	private static void ApplyEffect(GameObject parentObject, GameObject effectSource, EffectType effectType, EffectPosition effectPosition, Vector2 effectOffset, float edgePadding)
	{
		float parentX, parentY, parentWidth, parentHeight;
		GameObject effect;

		Rect parentRect = ((SpriteRenderer)parentObject.renderer).sprite.rect;

		parentX = parentObject.transform.localPosition.x;
		parentY = parentObject.transform.localPosition.y;
		parentWidth = parentRect.width / pixelsToUnits;
		parentHeight = parentRect.height / pixelsToUnits;

		Vector3 position, rotation, scale;
		position = Vector3.zero; rotation = Vector3.zero; scale = Vector3.one;

		switch (effectPosition)
		{
			case EffectPosition.TopLeft:
				position.Set(parentX - parentWidth / 2 + effectOffset.x, parentY + parentHeight / 2 - effectOffset.y, 1);
				break;
			case EffectPosition.TopRight:
				position.Set(parentX + parentWidth / 2 - effectOffset.x, parentY + parentHeight / 2 - effectOffset.y, 1);
				rotation.Set(0, 0, -90);
				break;
			case EffectPosition.BottomRight:
				position.Set(parentX + parentWidth / 2 - effectOffset.x, parentY - parentHeight / 2 + effectOffset.y, 1);
				rotation.Set(0, 0, 180);
				break;
			case EffectPosition.BottomLeft:
				position.Set(parentX - parentWidth / 2 + effectOffset.x, parentY - parentHeight / 2 + effectOffset.y, 1);
				rotation.Set(0, 0, 90);
				break;

			case EffectPosition.Top:
				break;
			case EffectPosition.Right:
				break;
			case EffectPosition.Bottom:
				break;
			case EffectPosition.Left:
				break;
		}

		effect = (GameObject)GameObject.Instantiate(
			effectSource, position, Quaternion.Euler(rotation));
		
		effect.name = effectType.ToString() + " " + effectPosition.ToString();
		effect.transform.parent = parentObject.transform;
		effect.renderer.sortingLayerID = parentObject.renderer.sortingLayerID;
		if (effectType == EffectType.Inner)
			effect.renderer.sortingOrder = parentObject.renderer.sortingOrder + 1;
		else
			effect.renderer.sortingOrder = parentObject.renderer.sortingOrder - 1;

		effect.SetActive(true);
	}

	public static void ApplyBoxEffect1(GameObject parentObject)
	{
		ApplyEffect(parentObject, OuterCorner1, EffectType.Outer, EffectPosition.TopLeft, new Vector2(Outer1Offset, Outer1Offset), 0f);
		ApplyEffect(parentObject, OuterCorner1, EffectType.Outer, EffectPosition.TopRight, new Vector2(Outer1Offset, Outer1Offset), 0f);
		ApplyEffect(parentObject, OuterCorner1, EffectType.Outer, EffectPosition.BottomRight, new Vector2(Outer1Offset, Outer1Offset), 0f);
		ApplyEffect(parentObject, OuterCorner1, EffectType.Outer, EffectPosition.BottomLeft, new Vector2(Outer1Offset, Outer1Offset), 0f);
		
		ApplyEffect(parentObject, InnerCorner1, EffectType.Inner, EffectPosition.TopLeft, Vector2.zero, 0f);
		ApplyEffect(parentObject, InnerCorner1, EffectType.Inner, EffectPosition.TopRight, Vector2.zero, 0f);
		ApplyEffect(parentObject, InnerCorner1, EffectType.Inner, EffectPosition.BottomRight, Vector2.zero, 0f);
		ApplyEffect(parentObject, InnerCorner1, EffectType.Inner, EffectPosition.BottomLeft, Vector2.zero, 0f);

		//float parentX, parentY, parentWidth, parentHeight;
		//GameObject gameObject;

		//Rect parentRect = ((SpriteRenderer)parentObject.renderer).sprite.rect;

		//parentX = parentObject.transform.localPosition.x;
		//parentY = parentObject.transform.localPosition.y;
		//parentWidth = parentRect.width / pixelsToUnits;
		//parentHeight = parentRect.height / pixelsToUnits;

		#region Inner Effect

		////Inner Top Left Corner
		//gameObject = (GameObject)GameObject.Instantiate(
		//  InnerCorner1,	new Vector3(parentX - parentWidth / 2, parentY + parentHeight / 2, 1), Quaternion.Euler(0, 0, 0));
		//gameObject.name = "Inner Top Left Corner"; gameObject.SetActive(true); gameObject.transform.parent = parentObject.transform;

		////Inner Top Right Corner
		//gameObject = (GameObject)GameObject.Instantiate(
		//  InnerCorner1, new Vector3(parentX + parentWidth / 2, parentY + parentHeight / 2, 1), Quaternion.Euler(0, 0, -90));
		//gameObject.name = "Inner Top Right Corner"; gameObject.SetActive(true); gameObject.transform.parent = parentObject.transform;

		////Inner Bottom Right Corner
		//gameObject = (GameObject)GameObject.Instantiate(
		//  InnerCorner1, new Vector3(parentX + parentWidth / 2, parentY - parentHeight / 2, 1), Quaternion.Euler(0, 0, 180));
		//gameObject.name = "Inner Bottom Right Corner"; gameObject.SetActive(true); gameObject.transform.parent = parentObject.transform;

		////Inner Bottom Left Corner
		//gameObject = (GameObject)GameObject.Instantiate(
		//  InnerCorner1, new Vector3(parentX - parentWidth / 2, parentY - parentHeight / 2, 1), Quaternion.Euler(0, 0, 90));
		//gameObject.name = "Inner Bottom Left Corner"; gameObject.SetActive(true); gameObject.transform.parent = parentObject.transform;


		////Inner Top Edge
		//gameObject = (GameObject)GameObject.Instantiate(
		//  InnerEdge1, new Vector3(parentX, parentY + parentHeight / 2, 1), Quaternion.Euler(0, 0, 0));
		//gameObject.transform.localScale = new Vector3((parentRect.width - InnerCorner1Rect.width * 2) / InnerEdge1Rect.width, 1, 1);
		//gameObject.name = "Inner Top Edge"; gameObject.SetActive(true); gameObject.transform.parent = parentObject.transform;

		////Inner Right Edge
		//gameObject = (GameObject)GameObject.Instantiate(
		//  InnerEdge1, new Vector3(parentX + parentWidth / 2, parentY, 1), Quaternion.Euler(0, 0, -90));
		//gameObject.transform.localScale = new Vector3((parentRect.height - InnerCorner1Rect.height * 2) / InnerEdge1Rect.width, 1, 1);
		//gameObject.name = "Inner Right Edge"; gameObject.SetActive(true); gameObject.transform.parent = parentObject.transform;

		////Inner Bottom Edge
		//gameObject = (GameObject)GameObject.Instantiate(
		//  InnerEdge1, new Vector3(parentX, parentY - parentHeight / 2, 1), Quaternion.Euler(0, 0, 180));
		//gameObject.transform.localScale = new Vector3((parentRect.width - InnerCorner1Rect.width * 2) / InnerEdge1Rect.width, 1, 1);
		//gameObject.name = "Inner Bottom Edge"; gameObject.SetActive(true); gameObject.transform.parent = parentObject.transform;

		////Inner Left Edge
		//gameObject = (GameObject)GameObject.Instantiate(
		//  InnerEdge1, new Vector3(parentX - parentWidth / 2, parentY, 1), Quaternion.Euler(0, 0, 90));
		//gameObject.transform.localScale = new Vector3((parentRect.height - InnerCorner1Rect.height * 2) / InnerEdge1Rect.width, 1, 1);
		//gameObject.name = "Inner Left Edge"; gameObject.SetActive(true); gameObject.transform.parent = parentObject.transform;

		#endregion

		#region Outer Effect

		////Outer Top Left Corner
		//gameObject = (GameObject)GameObject.Instantiate(
		//  OuterCorner1, new Vector3(parentX - parentWidth / 2 + Outer1Offset, parentY + parentHeight / 2 - Outer1Offset, 1), Quaternion.Euler(0, 0, 0));
		//gameObject.name = "Outer Top Left Corner"; gameObject.SetActive(true); gameObject.transform.parent = parentObject.transform;
		//gameObject.renderer.sortingLayerID = parentObject.renderer.sortingLayerID;
		//gameObject.renderer.sortingOrder = parentObject.renderer.sortingOrder - 1;

		////Outer Top Right Corner
		//gameObject = (GameObject)GameObject.Instantiate(
		//  OuterCorner1, new Vector3(parentX + parentWidth / 2 - Outer1Offset, parentY + parentHeight / 2 - Outer1Offset, 1), Quaternion.Euler(0, 0, -90));
		//gameObject.name = "Outer Top Right Corner"; gameObject.SetActive(true); gameObject.transform.parent = parentObject.transform;
		//gameObject.renderer.sortingLayerID = parentObject.renderer.sortingLayerID;
		//gameObject.renderer.sortingOrder = parentObject.renderer.sortingOrder - 1;

		////Outer Bottom Right Corner
		//gameObject = (GameObject)GameObject.Instantiate(
		//  OuterCorner1, new Vector3(parentX + parentWidth / 2 - Outer1Offset, parentY - parentHeight / 2 + Outer1Offset, 1), Quaternion.Euler(0, 0, 180));
		//gameObject.name = "Outer Bottom Right Corner"; gameObject.SetActive(true); gameObject.transform.parent = parentObject.transform;
		//gameObject.renderer.sortingLayerID = parentObject.renderer.sortingLayerID;
		//gameObject.renderer.sortingOrder = parentObject.renderer.sortingOrder - 1;

		////Outer Bottom Left Corner
		//gameObject = (GameObject)GameObject.Instantiate(
		//  OuterCorner1, new Vector3(parentX - parentWidth / 2 + Outer1Offset, parentY - parentHeight / 2 + Outer1Offset, 1), Quaternion.Euler(0, 0, 90));
		//gameObject.name = "Outer Bottom Left Corner"; gameObject.SetActive(true); gameObject.transform.parent = parentObject.transform;
		//gameObject.renderer.sortingLayerID = parentObject.renderer.sortingLayerID;
		//gameObject.renderer.sortingOrder = parentObject.renderer.sortingOrder - 1;


		////Inner Top Edge
		//gameObject = (GameObject)GameObject.Instantiate(
		//  InnerEdge1, new Vector3(parentX, parentY + parentHeight / 2, 1), Quaternion.Euler(0, 0, 0));
		//gameObject.transform.localScale = new Vector3((parentRect.width - InnerCorner1Rect.width * 2) / InnerEdge1Rect.width, 1, 1);
		//gameObject.name = "Inner Top Edge"; gameObject.SetActive(true); gameObject.transform.parent = parentObject.transform;

		////Inner Right Edge
		//gameObject = (GameObject)GameObject.Instantiate(
		//  InnerEdge1, new Vector3(parentX + parentWidth / 2, parentY, 1), Quaternion.Euler(0, 0, -90));
		//gameObject.transform.localScale = new Vector3((parentRect.height - InnerCorner1Rect.height * 2) / InnerEdge1Rect.width, 1, 1);
		//gameObject.name = "Inner Right Edge"; gameObject.SetActive(true); gameObject.transform.parent = parentObject.transform;

		////Inner Bottom Edge
		//gameObject = (GameObject)GameObject.Instantiate(
		//  InnerEdge1, new Vector3(parentX, parentY - parentHeight / 2, 1), Quaternion.Euler(0, 0, 180));
		//gameObject.transform.localScale = new Vector3((parentRect.width - InnerCorner1Rect.width * 2) / InnerEdge1Rect.width, 1, 1);
		//gameObject.name = "Inner Bottom Edge"; gameObject.SetActive(true); gameObject.transform.parent = parentObject.transform;

		////Inner Left Edge
		//gameObject = (GameObject)GameObject.Instantiate(
		//  InnerEdge1, new Vector3(parentX - parentWidth / 2, parentY, 1), Quaternion.Euler(0, 0, 90));
		//gameObject.transform.localScale = new Vector3((parentRect.height - InnerCorner1Rect.height * 2) / InnerEdge1Rect.width, 1, 1);
		//gameObject.name = "Inner Left Edge"; gameObject.SetActive(true); gameObject.transform.parent = parentObject.transform;

		#endregion
	}
}
