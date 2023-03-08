using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordsTarget : MonoBehaviour {

	[SerializeField] List<WordInfo> wis;
	[SerializeField] Transform canvas;
	[SerializeField] GameObject columnPrefab;
	[SerializeField] GameObject rawPrefab;
	[SerializeField] GameObject wordPrefab;
	[SerializeField] AudioSource audioSource;
	[SerializeField] AudioSource wrongSound;
	[SerializeField] AudioSource looseMusic;
	[SerializeField] AudioSource note;
	[SerializeField] Text percentsText;
	[SerializeField] Text successText;

	Queue<WordPlace> places = new Queue<WordPlace>();

	[SerializeField] float startT;
	[SerializeField] int maxMistakes = 1;
	[SerializeField] float speed = 1f;

	Transform column;
	Transform raw;
	int lettersCount = 0;
	int rawCount = 0;
	int disableCount = 0;

	bool loose = false;

	int lettersCountNow = 0;


	List<bool> added = new List<bool>();

	int currentCount = 0;

	Dictionary<string, string> keyNames = new Dictionary<string, string>() {
		{"Minus", "-" },
	};

	float time = 0;
	float startTime = 0;

	private void Start() {
		StartActions();
	}
	public void StartActions() {

		looseMusic.Stop();
		audioSource.pitch = 1f;
		var pitchBendGroup = Resources.Load<UnityEngine.Audio.AudioMixerGroup>("Mixer");
		print(pitchBendGroup);
		audioSource.outputAudioMixerGroup = pitchBendGroup;
		Time.timeScale = speed;
		audioSource.pitch = speed;
		pitchBendGroup.audioMixer.SetFloat("Pitch", 1f / speed);
		StartCoroutine(FadeIn(audioSource, 1f));

		foreach (var wi in wis ) {
			if (wi.Word == "{start}" ) {
				startT = wi.StartTime;
				break;
			}
		}


		loose = false;
		places.Clear();
		currentCount = 0;
		disableCount = 0;
		lettersCountNow = 0;
		added = new List<bool>();
		audioSource.Play();
		audioSource.time = startT;
		startTime = startT;
		time = startTime;

		if ( column ) Destroy(column.gameObject);

		foreach ( var wi in wis ) {
			if (wi.Word != "{string}" && wi.Word != "{section}" && wi.Word != "{start}" && wi.Word != "{end}") {
				lettersCount += wi.Word.Length;
			}
			added.Add(false);
		}

		for ( int i = 0; i < added.Count; i++ ) {
			if ( wis[i].StartTime < startT ) added[i] = true;
		}

		column = Instantiate(columnPrefab, canvas).transform;
		raw = Instantiate(rawPrefab, column).transform;
	}
	public void OnCompleteWord(WordInfo word)  {
		places.Dequeue();
	}


	void OnGUI() {
		if ( places.Count == 0 ) return;
		if ( Event.current.isKey && Event.current.type == EventType.KeyDown ) {
			string key = Event.current.keyCode.ToString();
			if ( keyNames.ContainsKey(key) ) key = keyNames[key];
			char[] cs = key.ToCharArray();
			if ( cs.Length > 1 ) return;
			char c = cs[0];
			if (places.Count > 0 ) {
				if ( places.Peek().TryType(c) ) {
					currentCount++;
					note.Play();
				}
				else wrongSound.Play();
			}
		}
	}

	void LooseLogic() {
		successText.text = disableCount.ToString() + "/" + maxMistakes.ToString();
		if (disableCount > maxMistakes && !loose) {
			loose = true;
			Loose();
		}
	}

	IEnumerator FadeOutWithSpeed(AudioSource au, float duration) {
		int n = (int)(duration / 0.05f);
		for (int i = n; i >= 0; i-- ) {
			au.volume = (float)i / n;
			au.pitch = (float)i / n;
			yield return new WaitForSeconds(duration / (n + 1));
		}
		au.Stop();
	}

	IEnumerator FadeIn(AudioSource au, float duration) {
		au.Play();
		int n = (int)(duration / 0.05f);
		for ( int i = 0; i <= n; i++ ) {
			au.volume = (float)i / n;
			print((float)i / n);
			yield return new WaitForSeconds(duration / (n + 1));
		}
	}

	void Loose() {
		Looser.instance.Loose();
		StartCoroutine(LooseCor());
	}

	IEnumerator LooseCor() {
		StartCoroutine(FadeOutWithSpeed(audioSource, 2f));
		print("lala");
		yield return FadeIn(looseMusic, 2f);
	}

	private void Update() {
		successText.text = disableCount.ToString() + "/" + "35";
		if ( loose ) return;
		LooseLogic();
		if ( wrongSound.isPlaying ) {
			audioSource.volume = 0.2f;
		}
		else {
			audioSource.volume = 1f;
		}
		float ratio = (float)currentCount / lettersCount;
		percentsText.text = ((float)((int)(ratio * 10000)) / 100).ToString() + "%";
		time += Time.deltaTime;
		
		for (int i = 0; i < added.Count; i++ ) {
			if ( added[i] ) continue;
			if (time > wis[i].StartTime ) {
				added[i] = true;

				if (wis[i].Word == "{start}" ) {
					continue;
				}

				if ( wis[i].Word == "{end}" ) {
					continue;
				}

				if (wis[i].Word == "{section}" ) {
					print("section");
					Destroy(column.gameObject);

					places.Clear();

					column = Instantiate(columnPrefab, canvas).transform;
					raw = Instantiate(rawPrefab, column).transform;
					rawCount = 1;
					continue;
				}

				if (wis[i].Word == "{string}" ) {
					
					if ( rawCount == 0 ) continue;

					int n = places.Count;
					for (int j = 0; j < n; j++ ) {
						disableCount += places.Dequeue().SetDisable();
					}
					raw = Instantiate(rawPrefab, column).transform;
					rawCount++;
					continue;
				}
				//if ( wordPlace ) Destroy(wordPlace.gameObject);
				var wp = Instantiate(wordPrefab, raw).GetComponent<WordPlace>();
				wp.SetTarget(this);
				wp.SetUp();
				wp.CreateWord(wis[i]);
				lettersCountNow += wis[i].Word.Length;
				raw.GetComponent<Raw>().Add(wp.GetComponent<RectTransform>().sizeDelta.x);
				if ( places.Count == 0 ) rawCount++;
				places.Enqueue(wp);
			}
		}

		if ( Input.GetKeyDown(KeyCode.Space) ) {
			print(time);
		}
	}

}
