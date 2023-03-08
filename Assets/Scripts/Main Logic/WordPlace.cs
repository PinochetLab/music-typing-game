using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordPlace : MonoBehaviour
{
	List<Letter> letters = new List<Letter>();
	[SerializeField] GameObject letterPrefab;
	[SerializeField] WordsTarget target;
	[SerializeField] RectTransform rect;
	[SerializeField] Font font;
	int currentLetter = 0;
	int lettersCount;
	float max = 0;
	WordInfo wi;

	static char[] alphabet = new char[26] {
		'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h',
		'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
		'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
		'y', 'z',
	};

	void Clear() {
		for (int i = 0; i < transform.childCount; i++ ) {
			Destroy(transform.GetChild(i).gameObject);
		}
	}

	public void SetTarget(WordsTarget tg) {
		target = tg;
	}

	public bool TryType(char c) {
		if ( c.ToString().ToUpper() == letters[currentLetter].GetLetter().ToString().ToUpper() ) {
			if (currentLetter == lettersCount - 1 ) {
				letters[currentLetter].SetComplete();
				target.OnCompleteWord(wi);
				return true;
			}
			letters[currentLetter].SetComplete();
			currentLetter++;
			return true;
		}
		return false;
	}

	public void SetUp() {
		max = 0;
		foreach (char b in alphabet ) {
			font.RequestCharactersInTexture((b).ToString());
			font.GetCharacterInfo(b, out CharacterInfo info2);
			float m = info2.advance;
			if ( m > max ) max = m;
		}
	}


	public void CreateWord(WordInfo word) {
		Clear();
		wi = word;
		letters = new List<Letter>();
		currentLetter = 0;
		lettersCount = word.Word.Length;
		float s = 0;
		char[] cs = word.Word.ToCharArray();
		for (int i = 0; i < cs.Length; i++ ) {
			char c = cs[i];
			Letter letter = Instantiate(letterPrefab, transform).GetComponent<Letter>();

			var r = letter.GetComponent<RectTransform>();

			font.RequestCharactersInTexture(c.ToString());
			font.GetCharacterInfo(c, out CharacterInfo info);
			float size = info.advance;

			r.sizeDelta = new Vector2(r.sizeDelta.x * (size + 1f) / max, r.sizeDelta.y);

			s += r.sizeDelta.x;

			letter.SetLetter(c);
			letters.Add(letter);
		}

		rect.sizeDelta = new Vector2(s + 40f, rect.sizeDelta.y);
	}

	public int SetDisable() {
		int res = 0;
		foreach (Letter letter in letters ) {
			res += letter.SetDisable();
		}
		return res;
	}
}
