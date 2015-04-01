using UnityEngine;
using System.Collections;

namespace Sandulas
{
	public class IntVector2
	{
		public int x, y;

		public IntVector2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	public static class MyVector3
	{
		public static Vector3 Get(Vector2 vector2)
		{
			return new Vector3(vector2.x, vector2.y);
		}
	}

	public static class MyTransform
	{
		public static void SetPositionXY(Transform transform, float x, float y)
		{
			transform.position = new Vector3(x, y, transform.position.z);
		}
		public static void SetPositionXY(Transform transform, Vector2 xy)
		{
			SetPositionXY(transform, xy.x, xy.y);
		}
		public static void SetPositionX(Transform transform, float x)
		{
			transform.position = new Vector3(x, transform.position.y, transform.position.z);
		}
		public static void SetPositionY(Transform transform, float y)
		{
			transform.position = new Vector3(transform.position.x, y, transform.position.z);
		}
		public static void MoveY(Transform transform, float yOffset)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
		}

		public static void SetLocalPositionXY(Transform transform, float x, float y)
		{
			transform.localPosition = new Vector3(x, y, transform.localPosition.z);
		}
		public static void SetLocalPositionXY(Transform transform, Vector2 xy)
		{
			SetLocalPositionXY(transform, xy.x, xy.y);
		}
		public static void SetLocalPositionX(Transform transform, float x)
		{
			transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
		}
		public static void SetLocalPositionY(Transform transform, float y)
		{
			transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
		}


		public static void SetScaleXY(Transform transform, float x, float y)
		{
			transform.localScale = new Vector3(x, y, transform.localScale.z);
		}
		public static void SetScaleXY(Transform transform, Vector2 xy)
		{
			SetScaleXY(transform, xy.x, xy.y);
		}
		public static void SetScaleX(Transform transform, float x)
		{
			transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
		}
		public static void SetScaleY(Transform transform, float y)
		{
			transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
		}
	}

	public class MyRect
	{
		public float Top, Left, Bottom, Right;

		public float Height
		{
			get { return Top - Bottom; }
		}
		public float Width
		{
			get { return Right - Left; }
		}
		public Vector2 Center
		{
			get { return new Vector2(Left + Width / 2, Bottom + Height / 2); }
		}

		public MyRect(float top, float left, float bottom, float right)
		{
			this.Top = top;
			this.Left = left;
			this.Bottom = bottom;
			this.Right = right;
		}

		public Vector2 GetInsidePosition(Vector2 position)
		{
			Vector2 insidePosition = position;

			if (insidePosition.x < Left) insidePosition.x = Left + 0.00001f;
			else if (insidePosition.x > Right) insidePosition.x = Right - 0.00001f;

			if (insidePosition.y < Bottom) insidePosition.y = Bottom + 0.00001f;
			else if (insidePosition.y > Top) insidePosition.y = Top - 0.00001f;

			return insidePosition;
		}

		public void Draw()
		{
			Debug.DrawLine(new Vector3(Left, Top, 0), new Vector3(Right, Bottom, 0), Color.red, 10000, false);
			Debug.DrawLine(new Vector3(Left, Bottom, 0), new Vector3(Right, Top, 0), Color.red, 10000, false);
			Debug.DrawLine(new Vector3(Left, Bottom, 0), new Vector3(Right, Bottom, 0), Color.red, 10000, false);
			Debug.DrawLine(new Vector3(Left, Top, 0), new Vector3(Right, Top, 0), Color.red, 10000, false);
			Debug.DrawLine(new Vector3(Left, Bottom, 0), new Vector3(Left, Top, 0), Color.red, 10000, false);
			Debug.DrawLine(new Vector3(Right, Bottom, 0), new Vector3(Right, Top, 0), Color.red, 10000, false);
		}
	}
}