using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looser : MonoBehaviour
{
	public static Looser instance;

	[SerializeField] CanvasGroup gr;
	[SerializeField] GameObject loosePanel;

	private void Awake() {
		instance = this;
	}
	public void Loose() {
		loosePanel.SetActive(true);
		StartCoroutine(FadeIn(1f));
	}

	IEnumerator FadeIn(float duration) {
		int n = (int)(duration / 0.01f);
		for (int i = 0; i <= n; i++ ) {
			gr.alpha = (float)i / n;
			yield return new WaitForSeconds(duration / (n + 1));
		}
	}
}
