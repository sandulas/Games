using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Debug.Log(Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
		}


		//RaycastHit2D hitInfo = new RaycastHit2D();
		//Ray2D ray = Camera.main.ScreenPointToRay2D(Input.mousePosition);

		//if (Physics2D.Raycast(ray, out hitInfo))
		//{
		//  if (hover_state == HoverState.NONE)
		//  {
		//    hitInfo.collider.SendMessage("OnMouseEnter", SendMessageOptions.DontRequireReceiver);
		//    hoveredGO = hitInfo.collider.gameObject;
		//  }
		//  hover_state = HoverState.HOVER;
		//}
		//else
		//{
		//  if (hover_state == HoverState.HOVER)
		//  {
		//    hoveredGO.SendMessage("OnMouseExit", SendMessageOptions.DontRequireReceiver);
		//  }
		//  hover_state = HoverState.NONE;
		//}




		//if (Input.GetMouseButtonDown(0))
		//{
		//  Debug.Log("button down");

		//  if (gameObject.collider2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
		//    Debug.Log(gameObject.name);
		//}
	}
}
