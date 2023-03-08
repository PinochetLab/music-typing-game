using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WordInfo
{
	[SerializeField] string word;
	[SerializeField] float startTime;

	public string Word
	{
		get => word;
		set => word = value;
	}
	public float StartTime { get => startTime; set => startTime = value; }
}
