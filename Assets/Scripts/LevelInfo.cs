using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level")]
public class LevelInfo : ScriptableObject
{
	[SerializeField] List<WordInfo> wis;
}
