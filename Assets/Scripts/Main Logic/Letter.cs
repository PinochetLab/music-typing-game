using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
	[SerializeField] Text text;
	[SerializeField] Color color;
	char letter = ' ';
	bool passed = false;
	Vector3 startScale;

	private void Start() {
		startScale = transform.localScale;
	}

	public char GetLetter() {
		return letter;
	}

	public void SetLetter(char _letter) {
		letter = _letter;
		text.text = letter.ToString().ToLower();
	}

	public int SetDisable() {
		if ( !passed ) {
			text.color = Color.grey;
			return 1;
		}
		return 0;
	}

	public void SetComplete() {
		passed = true;
		text.color = color;
		StartCoroutine(SomeScale());
	}

	IEnumerator SomeScale() {
		float time = 0.2f;
		float magnitude = 0.1f;
		int n = 20;
		for (int i = 0; i <= n; i++ ) {
			transform.localScale = startScale * (1 + magnitude * Mathf.Sin(Mathf.PI * (float)i / n));
			yield return new WaitForSeconds(time / n);
		}
	}
}
