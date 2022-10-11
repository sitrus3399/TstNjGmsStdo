using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public bool clear11, clear12, clear13, clear21, clear22, clear23;
	public AudioSource audioSFX, audioBGM, audioVoice;
	
	#region Get ScriptableObject
	public string stageNameManager;
	public string stageDescriptionManager;
	public float stageTimeManager;
	public int customerTargetManager;
	public int customerMaxManager;
	public int stoveSlotManager;
	public int plateSlotManager;
	public int customerSlotManager;
	public Sprite stageImageManager;	
	#endregion
	
	void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
	}
	
    public void StageInfo(Stage stage)
	{
		stageNameManager = stage.stageName;
		stageDescriptionManager = stage.stageDescription;
		stageTimeManager = stage.stageTime;
		customerTargetManager = stage.customerTarget;
		customerMaxManager = stage.customerMax;
		stoveSlotManager = stage.stoveSlot;
		plateSlotManager = stage.plateSlot;
		customerSlotManager = stage.customerSlot;
		stageImageManager = stage.stageImage;
	}
}