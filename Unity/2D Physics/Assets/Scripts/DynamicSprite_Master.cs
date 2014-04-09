﻿using UnityEngine;
using System.Collections;

public class DynamicSprite_Master : MonoBehaviour
{
	void Awake()
	{

	}

	void Start()
	{
		GameObject testObject;

		for (int i = 0; i < 100; i++)
		{
			testObject = Master.CreateBox(ObjectMaterial.FixedMetal, new Vector2(800, 200));
			testObject.transform.position = new Vector3(0, 3, 0);
		}

		testObject = Master.CreateBox(ObjectMaterial.Metal, new Vector2(800, 200));
		testObject.transform.position = new Vector3(0, 1.5f, 0);

		testObject = Master.CreateBox(ObjectMaterial.Wood, new Vector2(800, 200));
		testObject.transform.position = new Vector3(0, 0, 0);

		testObject = Master.CreateBox(ObjectMaterial.Rubber, new Vector2(800, 200));
		testObject.transform.position = new Vector3(0, -1.5f, 0);

		testObject = Master.CreateBox(ObjectMaterial.Ice, new Vector2(800, 200));
		testObject.transform.position = new Vector3(0, -3, 0);
	}

	void Update()
	{

	}
}
