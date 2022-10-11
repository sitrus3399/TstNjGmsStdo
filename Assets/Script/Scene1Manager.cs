using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene1Manager : MonoBehaviour
{
	public GameManager gameManager;
	[SerializeField] private ScriptableObject[] stageSerializableObjects;
	public GameObject ButtonPanel;
    GameObject button11, button12, button13, button21, button22, button23;
	GameObject lock11, lock12, lock13, lock21, lock22, lock23;
	public Animator transition;
	int stageIndex;
	
	void Awake()
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
    void Start()
    {
		#region Input Game Object
        button11 = ButtonPanel.transform.Find("Button11").gameObject;
		button12 = ButtonPanel.transform.Find("Button12").gameObject;
		button13 = ButtonPanel.transform.Find("Button13").gameObject;
		button21 = ButtonPanel.transform.Find("Button21").gameObject;
		button22 = ButtonPanel.transform.Find("Button22").gameObject;
		button23 = ButtonPanel.transform.Find("Button23").gameObject;
		
		lock11 = ButtonPanel.transform.Find("Button11/Lock").gameObject;
		lock12 = ButtonPanel.transform.Find("Button12/Lock").gameObject;
		lock13 = ButtonPanel.transform.Find("Button13/Lock").gameObject;
		lock21 = ButtonPanel.transform.Find("Button21/Lock").gameObject;
		lock22 = ButtonPanel.transform.Find("Button22/Lock").gameObject;
		lock23 = ButtonPanel.transform.Find("Button23/Lock").gameObject;
		#endregion
		
		#region Cek Game Manager		
		if(gameManager.clear11){ //Cek awal apa saja yang telah di unlock, jika ada fitur save dan load dan perubahan dari game manager
			lock12.SetActive(false); //"level 2-1 unlock jika user MENANG stage 1-1" - Jadi unlock 2-1 dan 1-2 mempunyai syarat unlock yang sama
			lock21.SetActive(false);
			if(gameManager.clear12){ 
				lock13.SetActive(false);
			}
			else{
				gameManager.clear13 = false;
				lock13.SetActive(true);
			}
		}
		else
		{
			lock12.SetActive(true);
			lock13.SetActive(true);
			gameManager.clear12 = false;
			gameManager.clear13 = false;
			lock21.SetActive(true);
			gameManager.clear21 = false;
		}
		
		if(gameManager.clear21){
			lock22.SetActive(false);
			if(gameManager.clear22){
				lock23.SetActive(false);
			}
			else{
				lock23.SetActive(true);
				gameManager.clear23 = false;
			}
		}
		else{
			lock22.SetActive(true);
			lock23.SetActive(true);
			gameManager.clear22 = false;
			gameManager.clear23 = false;
		}
		#endregion
    }
	
    void Update()
    {
        
    }
	
	public void Button11Clicked()
	{
		AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/click");
		gameManager.audioSFX.PlayOneShot(clip);
		stageIndex = 0;
		StartCoroutine(LoadStage());
	}
	
	
	public void Button12Clicked()
	{
		if (gameManager.clear11 && !gameManager.clear12)
		{
			lock13.SetActive(false);
			gameManager.clear12 = true;
			AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/finish");
			gameManager.audioSFX.PlayOneShot(clip);
		}
		else if (gameManager.clear12)
		{
			AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/click");
			gameManager.audioSFX.PlayOneShot(clip);
		}
		else
		{
			AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/nope");
			gameManager.audioSFX.PlayOneShot(clip);
		}
	}
	
	public void Button13Clicked()
	{
		if (gameManager.clear12 && !gameManager.clear13)
		{
			gameManager.clear13 = true;
			AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/finish");
			gameManager.audioSFX.PlayOneShot(clip);
		}
		else if (gameManager.clear13)
		{
			AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/click");
			gameManager.audioSFX.PlayOneShot(clip);
		}
		else
		{
			AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/nope");
			gameManager.audioSFX.PlayOneShot(clip);
		}
	}
	
	public void Button21Clicked()
	{
		if (gameManager.clear11)
		{
			stageIndex = 1; // Karena playable stage cuma 2 jadi stage index hanya ada 0 dan 1;
			AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/click");
			gameManager.audioSFX.PlayOneShot(clip);
			StartCoroutine(LoadStage());
		}
		else if (!gameManager.clear11)
		{
			lock21.SetActive(true);
			AudioClip clip = Resources.Load<AudioClip>("nope");
			gameManager.audioSFX.PlayOneShot(clip);
		}
	}
	
	public void Button22Clicked()
	{
		if (gameManager.clear21 && !gameManager.clear22)
		{
			lock23.SetActive(false);
			gameManager.clear22 = true;
			
			AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/finish");
			gameManager.audioSFX.PlayOneShot(clip);
		}
		else if (gameManager.clear22)
		{
			AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/click");
			gameManager.audioSFX.PlayOneShot(clip);
		}
		else
		{
			AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/nope");
			gameManager.audioSFX.PlayOneShot(clip);
		}
	}
	
	public void Button23Clicked()
	{
		if (gameManager.clear22 && !gameManager.clear23)
		{
			gameManager.clear23 = true;
			AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/finish");
			gameManager.audioSFX.PlayOneShot(clip);
		}
		else if (gameManager.clear23)
		{
			AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/click");
			gameManager.audioSFX.PlayOneShot(clip);
		}
		else
		{
			AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/nope");
			gameManager.audioSFX.PlayOneShot(clip);
		}
	}
	
	IEnumerator LoadStage()
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(2f);
		gameManager.StageInfo((Stage)stageSerializableObjects[stageIndex]);
        SceneManager.LoadScene(1);
    }
}