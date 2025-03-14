using UnityEngine;

public class Draggable2D : MonoBehaviour
{
	private bool isDragging;

	private Vector2 dragOffset;


	private void OnMouseDown()
	{
		isDragging = true;
		var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		dragOffset = transform.position - mousePosition;
	}

	private void OnMouseDrag()
	{
		if (isDragging)
		{
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			transform.position = mousePosition + dragOffset;
		}
	}

	private void OnMouseUp()
	{
		isDragging = false;
	}
}
