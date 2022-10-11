using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Stage", menuName ="ScriptableObjects/Stage")]
public class Stage : ScriptableObject
{
	public string stageName;
	public string stageDescription;
	public float stageTime;
	public int customerTarget;
	public int customerMax;
	public int stoveSlot;
	public int plateSlot;
	public int customerSlot;
	public Sprite stageImage;
}