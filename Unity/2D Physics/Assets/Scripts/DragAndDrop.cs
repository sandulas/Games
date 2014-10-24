using UnityEngine;
using System.Collections;

public class DragAndDrop : MonoBehaviour
{
	public delegate void MoveToPosition(GameObject gameObject, Vector3 position);
	public MoveToPosition MoveToPositionMethod;

	Vector3 offset;

	void Awake()
	{
		MyInputEvents inputEvents = gameObject.GetComponent<MyInputEvents>();
		if (inputEvents == null) inputEvents = gameObject.AddComponent<MyInputEvents>();

		inputEvents.OnTouch += OnTouch;
		inputEvents.OnDrag += OnDrag;
	}

	void OnTouch(GameObject sender, Camera camera)
	{
		offset = Input.mousePosition - camera.WorldToScreenPoint(sender.transform.position);
	}

	void OnDrag(GameObject sender, Camera camera)
	{
		if (MoveToPositionMethod != null)
			MoveToPositionMethod(sender, camera.ScreenToWorldPoint(Input.mousePosition - offset));
		else
			sender.transform.position = camera.ScreenToWorldPoint(Input.mousePosition - offset);
	}

	public void Drag(Camera camera)
	{
		offset = Input.mousePosition - camera.WorldToScreenPoint(gameObject.transform.position);
		MyInput.Drag(gameObject, camera);
	}
}
