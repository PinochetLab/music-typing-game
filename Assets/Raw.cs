using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raw : MonoBehaviour
{
	[SerializeField] RectTransform rect;

	public void Add(float dx) {
		rect.sizeDelta = new Vector2(rect.sizeDelta.x + dx, rect.sizeDelta.y);
	}
}
