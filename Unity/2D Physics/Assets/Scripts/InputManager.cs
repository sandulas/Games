using UnityEngine;
using System.Collections;
using ThisProject;

public class InputManager : MonoBehaviour
{
	GameObject draggedObject = null;
	Vector3 draggedObjectOffset;

	void Start()
	{
	}

	void Update()
	{
		Collider2D collider;
		Vector3 touchPosition;

		if (Input.GetMouseButton(0))
		{
			touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			touchPosition.z = 0;
		}
		else touchPosition = Vector3.zero;
		

		if (Input.GetMouseButtonDown(0))
		{
			collider = Physics2D.OverlapPoint(touchPosition);

			if (collider != null)
			{
				draggedObject = collider.gameObject;
				draggedObjectOffset = touchPosition - draggedObject.transform.position;

				if (draggedObject.name == "Pause")
				{
					if (Time.timeScale == 0) Time.timeScale = 1;
					else Time.timeScale = 0;
				}

				if (draggedObject.name.StartsWith("Item"))	Item.BringToFront(draggedObject);
			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			collider = Physics2D.OverlapPoint(touchPosition);

			draggedObject = null;
		}

		if (Input.GetMouseButton(0))
		{
			//don't alter the z coordinate
			//if (draggedObject != null) draggedObject.transform.position = touchPosition - draggedObjectOffset;
		}

	}
}
