using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random=UnityEngine.Random;

public class Scene2Manager : MonoBehaviour
{
	public GameManager gameManager;
	public TMP_Text stageNameText;
	public TMP_Text goldText;
	int gold;
	public Animator stageDesctiptionAnimator;
	public Animator transition;
	AudioClip clip;
	public TMP_Text gameOverText;
	public GameObject gameOver;
	bool unlimitedTime;
	
	[Header("Stage Time")]
	public TMP_Text stageDesctiptionText;
	public float stageTime; 
	public TMP_Text stageTimeText;
	public Slider stageTimeSlider;
	float currentTime;
	
	[Header("Customer")]
	public GameObject[] customerPlaceholder = new GameObject[4];
	public ScriptableObject[] customerSlot;
	public ScriptableObject[] customerSerializableObjects;
	public GameObject[] customerMenu;
	int customerRandomObject;
	int customerRandomTime;
	int customerRandomSlot;
	int customerRandomMenu;
	int customerTarget;
	int customerSuccess;
	int customerFail;
	int customerCurrent;
	int customerMax;
	int customerNumberSelected;
	bool customerReadySpawn = true;
	
	[Header("Stove and Cooking")]
	public Sprite stoveEmpty;
	public Sprite stoveInCook;
	public GameObject[] stovePlaceholder = new GameObject[4];
	public Slider[] stoveSlider = new Slider[4];
	bool[] cookingFinish = new bool[4];
	bool[] stoveSliderActive = new bool[4];
	ScriptableObject[] stoveSlot;
	public ScriptableObject[] menuSerializableObjects;
	
	[Header("Plate")]
	public Sprite plateEmpty;
	public GameObject[] platePlaceholder = new GameObject[4];
	ScriptableObject[] plateSlot;
	int plateNumberSelected;
	
	public GameObject Background;
	
	
	void Awake()
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
    void Start()
    {
		stoveSlot = new ScriptableObject[gameManager.stoveSlotManager]; //Memberikan gambar kompor
		for (int i = 0; i <= gameManager.stoveSlotManager - 1; i++)
		{
			stovePlaceholder[i].GetComponent<Image>().sprite = stoveEmpty;
		}
	
		plateSlot = new ScriptableObject[gameManager.plateSlotManager]; //Mmemberikan gambar piring
		for (int i = 0; i <= gameManager.plateSlotManager - 1; i++)
		{
			platePlaceholder[i].GetComponent<Image>().sprite = plateEmpty;
		}
		
		customerSlot = new ScriptableObject[gameManager.customerSlotManager];
		for (int i = 0; i <= gameManager.customerSlotManager - 1; i++)
		{
			customerPlaceholder[i].GetComponent<Image>().sprite = null;
		}
		
        stageNameText.text = gameManager.stageNameManager; //Stage Name
		stageDesctiptionText.text = gameManager.stageDescriptionManager; //Stage Description, atau stage objective
		stageTime = gameManager.stageTimeManager; //Time
		currentTime = stageTime;
		stageTimeSlider.maxValue = stageTime; //Time versi slider
		customerTarget = gameManager.customerTargetManager;
		customerMax = gameManager.customerMaxManager;
		Background.GetComponent<Image>().sprite = gameManager.stageImageManager;
    }

    void Update()
    {
		if (currentTime > 0)
		{
			currentTime -= 1 * Time.deltaTime;
			stageTimeSlider.value = currentTime;
			stageTimeText.text = Mathf.Round(currentTime).ToString();
		}
		else if (currentTime <= 0)
		{
			currentTime = 0;
			stageTimeSlider.value = currentTime;
			stageTimeText.text = Mathf.Round(currentTime).ToString();
			if (customerSuccess >= customerTarget) //Jika mencapai target menang
			{
				Victory();
			}
			else if (customerSuccess < customerTarget) //Jika tidak mencapai target kalah
			{
				GameOver();
			}
		}
		
		//Stove Slider Timer
		if (stoveSliderActive[0])
		{
			stoveSlider[0].value -= 1 * Time.deltaTime;
		}
		if (stoveSliderActive[1])
		{
			stoveSlider[1].value -= 1 * Time.deltaTime;
		}
		if (stoveSliderActive[2])
		{
			stoveSlider[2].value -= 1 * Time.deltaTime;
		}
		if (stoveSliderActive[3])
		{
			stoveSlider[3].value -= 1 * Time.deltaTime;
		}
		
		if (customerReadySpawn && customerCurrent <= customerMax) ///Spawn Customer //Dan cek apakah sudah mencapai batas maksimal summon customer. Sebenarnya ini double cek di dalam coroutine juga ada cek lagi
		{
			StartCoroutine(CustomerSpawn());
		}
    }
	
	public void DescriptionOnEnter()
	{
		stageDesctiptionAnimator.SetTrigger("Enter");
	}
	
	public void DescriptionOnExit()
	{
		Debug.Log("Exit");
		stageDesctiptionAnimator.SetTrigger("Exit");
	}
	
	#region Adonan Martabak to Stove
	public void MartabakButton()
	{
		for (int i = 0; i <= gameManager.stoveSlotManager - 1; i++) //Cek slot kompor dan input jika ada slot kosong
		{
			if (stoveSlot[i] == null)
			{
				stoveSlot[i] = Instantiate((Menu)menuSerializableObjects[0]);
				
				Menu menu = (Menu)stoveSlot[i]; //Mengambil ScriptableObject yang akan dieksekusi
				stoveSlider[i].maxValue = menu.menuTimecooking;
				stoveSlider[i].value = menu.menuTimecooking;
				stoveSliderActive[i] = true;
				stoveSlider[i].gameObject.SetActive(true);
				stovePlaceholder[i].GetComponent<Image>().sprite = stoveInCook;
				StartCoroutine(timeCooking(menu, i));
				break;
			}
		}
		clip = Resources.Load<AudioClip>("Audio/SFX/click");
		gameManager.audioSFX.PlayOneShot(clip);
	}
	
	IEnumerator timeCooking(Menu menu, int i)
	{
		yield return new WaitForSeconds(menu.menuTimecooking);
		stovePlaceholder[i].GetComponent<Image>().sprite = menu.menuImage;
		cookingFinish[i] = true;
		stoveSliderActive[i] = false;
		stoveSlider[i].gameObject.SetActive(false);
		clip = Resources.Load<AudioClip>("Audio/SFX/finish");
		gameManager.audioSFX.PlayOneShot(clip);
	}
	#endregion
	
	#region Stove to Plate
	public void stove1()
	{
		if (cookingFinish[0])
		{
			for (int i = 0; i <= gameManager.plateSlotManager - 1; i++) //Cek slot piring dan input jika ada slot kosong
			{
				if (plateSlot[i] == null)
				{
					plateSlot[i] = Instantiate(stoveSlot[0]);
					
					Menu menu = (Menu)plateSlot[i]; //Mengambil ScriptableObject yang akan dieksekusi
					platePlaceholder[i].GetComponent<Image>().sprite = menu.menuImage;
					
					stovePlaceholder[0].GetComponent<Image>().sprite = stoveEmpty;
					stoveSlot[0] = null;
					cookingFinish[0] = false;
					break;
				}
			}
			
		}
		clip = Resources.Load<AudioClip>("Audio/SFX/click");
		gameManager.audioSFX.PlayOneShot(clip);
	}
	
	public void stove2()
	{
		if (cookingFinish[1])
		{
			for (int i = 0; i <= gameManager.plateSlotManager - 1; i++) //Cek slot piring dan input jika ada slot kosong
			{
				if (plateSlot[i] == null)
				{
					plateSlot[i] = Instantiate(stoveSlot[1]);
					
					Menu menu = (Menu)plateSlot[i]; //Mengambil ScriptableObject yang akan dieksekusi
					platePlaceholder[i].GetComponent<Image>().sprite = menu.menuImage;
					
					stovePlaceholder[1].GetComponent<Image>().sprite = stoveEmpty;
					stoveSlot[1] = null;
					cookingFinish[1] = false;
					break;
				}
			}
			
		}
		clip = Resources.Load<AudioClip>("Audio/SFX/click");
		gameManager.audioSFX.PlayOneShot(clip);
	}
	
	public void stove3()
	{
		if (cookingFinish[2])
		{
			for (int i = 0; i <= gameManager.plateSlotManager - 1; i++) //Cek slot piring dan input jika ada slot kosong
			{
				if (plateSlot[i] == null)
				{
					plateSlot[i] = Instantiate(stoveSlot[2]);
					
					Menu menu = (Menu)plateSlot[i]; //Mengambil ScriptableObject yang akan dieksekusi
					platePlaceholder[i].GetComponent<Image>().sprite = menu.menuImage;
					
					stovePlaceholder[2].GetComponent<Image>().sprite = stoveEmpty;
					stoveSlot[2] = null;
					cookingFinish[2] = false;
					break;
				}
			}
			
		}
		clip = Resources.Load<AudioClip>("Audio/SFX/click");
		gameManager.audioSFX.PlayOneShot(clip);
	}
	
	public void stove4()
	{
		if (cookingFinish[3])
		{
			for (int i = 0; i <= gameManager.plateSlotManager - 1; i++) //Cek slot piring dan input jika ada slot kosong
			{
				if (plateSlot[i] == null)
				{
					plateSlot[i] = Instantiate(stoveSlot[3]);
					
					Menu menu = (Menu)plateSlot[i]; //Mengambil ScriptableObject yang akan dieksekusi
					platePlaceholder[i].GetComponent<Image>().sprite = menu.menuImage;
					
					stovePlaceholder[3].GetComponent<Image>().sprite = stoveEmpty;
					stoveSlot[3] = null;
					cookingFinish[3] = false;
					break;
				}
			}
			
		}
		clip = Resources.Load<AudioClip>("Audio/SFX/click");
		gameManager.audioSFX.PlayOneShot(clip);
	}
	#endregion
	
	#region Plate and Topping
	public void plate1()
	{
		plateNumberSelected = 0;
	}
	
	public void plate2()
	{
		plateNumberSelected = 1;
	}
	
	public void plate3()
	{
		plateNumberSelected = 2;
	}
	
	public void plate4()
	{
		plateNumberSelected = 3;
	}
	
	public void ChocolateTopping()
	{
		if(plateSlot[plateNumberSelected] != null)
		{
			Menu menu = (Menu)plateSlot[plateNumberSelected];
			if(menu.menuName == "Martabak")
			{
				Debug.Log("Ketahuan Martabak");
				plateSlot[plateNumberSelected] = null; //Membersihkan ScriptableObject Menu yang lama
				plateSlot[plateNumberSelected] = (Menu)menuSerializableObjects[1]; //Memberikan ScriptableObject Menu yang baru
				menu = (Menu)plateSlot[plateNumberSelected]; //Memanggil menu yang baru
				platePlaceholder[plateNumberSelected].GetComponent<Image>().sprite = menu.menuImage; //Mengganti gambar dengan ScriptableObject Menu yang baru
				
				clip = Resources.Load<AudioClip>("Audio/SFX/finish");
				gameManager.audioSFX.PlayOneShot(clip);
			}
			else if(menu.menuName == "Martabak Keju")
			{
				Debug.Log("Ketahuan Martabak Keju");
				plateSlot[plateNumberSelected] = null;
				plateSlot[plateNumberSelected] = (Menu)menuSerializableObjects[3];
				menu = (Menu)plateSlot[plateNumberSelected];
				platePlaceholder[plateNumberSelected].GetComponent<Image>().sprite = menu.menuImage;
				
				clip = Resources.Load<AudioClip>("Audio/SFX/finish");
				gameManager.audioSFX.PlayOneShot(clip);
			}
			else
			{
				clip = Resources.Load<AudioClip>("Audio/SFX/click");
				gameManager.audioSFX.PlayOneShot(clip);
			}
		}
	}
	
	public void CheeseTopping()
	{
		if(plateSlot[plateNumberSelected] != null)
		{
			Menu menu = (Menu)plateSlot[plateNumberSelected];
			if(menu.menuName == "Martabak")
			{
				Debug.Log("Ketahuan Martabak");
				plateSlot[plateNumberSelected] = null;
				plateSlot[plateNumberSelected] = (Menu)menuSerializableObjects[2];
				menu = (Menu)plateSlot[plateNumberSelected];
				platePlaceholder[plateNumberSelected].GetComponent<Image>().sprite = menu.menuImage;
				
				clip = Resources.Load<AudioClip>("Audio/SFX/finish");
				gameManager.audioSFX.PlayOneShot(clip);
			}
			else if(menu.menuName == "Martabak Coklat")
			{
				Debug.Log("Ketahuan Martabak Coklat");
				plateSlot[plateNumberSelected] = null;
				plateSlot[plateNumberSelected] = (Menu)menuSerializableObjects[3];
				menu = (Menu)plateSlot[plateNumberSelected];
				platePlaceholder[plateNumberSelected].GetComponent<Image>().sprite = menu.menuImage;
				
				clip = Resources.Load<AudioClip>("Audio/SFX/finish");
				gameManager.audioSFX.PlayOneShot(clip);
			}
			else
			{
				clip = Resources.Load<AudioClip>("Audio/SFX/click");
				gameManager.audioSFX.PlayOneShot(clip);
			}
		}
	}
	
	public void Trash()
	{
		if(plateSlot[plateNumberSelected] != null)
		{
			plateSlot[plateNumberSelected] = null;
			platePlaceholder[plateNumberSelected].GetComponent<Image>().sprite = plateEmpty;
		}
	}
	#endregion
	
	#region Customer
	IEnumerator CustomerSpawn()
	{
		customerReadySpawn = false;
		customerRandomObject = Random.Range(0, customerSerializableObjects.Length-1);
		customerRandomSlot = Random.Range(0, gameManager.customerSlotManager);
		customerRandomMenu = Random.Range(0, menuSerializableObjects.Length-1);
		if (customerSlot[customerRandomSlot] != null) // Di cek apakah slot yang dirandom kosong, jika tidak kosong di urut dari awal untuk deteksi yang kosong.
		{
			for (int i = 0; i <= customerSerializableObjects.Length - 1; i++)
			{
				if (customerSlot[i] == null)
				{
					customerRandomSlot = i;
				}
				else if (i == customerSerializableObjects.Length - 1)//Jika memang tidak ada slot yang kosong tunda sementara dan hentikan skrip.
				{
					yield return new WaitForSeconds(5f); //Biar ga spam IEnumerator CustomerSpawn() per frame jadi di jeda 5 detik
					customerReadySpawn = true;
					yield break;
				}
			}
		}
		Customer customer = (Customer)customerSerializableObjects[customerRandomObject]; //Mengambil ScriptableObject yang akan dieksekusi
		customer.customerMenu = Instantiate((Menu)menuSerializableObjects[customerRandomMenu]); //Mengisi slot ScriptableObject pada ScriptableObject customer dengan ScriptableObject Menu
		Menu menu = (Menu)customer.customerMenu;//Mengambil ScriptableObject yang akan dieksekusi
		customerRandomTime = Random.Range(customer.customerMinTime, customer.customerMaxTime);
		Debug.Log("customerRandomSlot" + customerRandomSlot);
		Debug.Log("customerRandomObject" + customerRandomObject);
		Debug.Log("customerRandomTime" + customerRandomTime);
		yield return new WaitForSeconds(customerRandomTime);
		customerSlot[customerRandomSlot] = Instantiate((Customer)customerSerializableObjects[customerRandomObject]);
		customerPlaceholder[customerRandomSlot].GetComponent<Image>().sprite = customer.customerImage;
		customerMenu[customerRandomSlot].GetComponent<Image>().sprite = menu.menuImage;
		customerCurrent += 1;
		if (customerCurrent == customerMax) //Cek apakah sudah mencapai batas maksimal spawn customer
		{
			customerReadySpawn = false;
			yield break;
		}
		else
		{
			customerReadySpawn = true;
		}
	}
	
	public void Customer1()
	{
		customerNumberSelected = 0;
		Menu menu = (Menu)plateSlot[plateNumberSelected]; //Panggil martabak yang dipilih di piring
		Customer customer = (Customer)customerSlot[customerNumberSelected]; //Panggil customernya berdasarkan nomernya
		Menu menuCustomer = (Menu)customer.customerMenu; //Panggil martabak yang dipilih di customer
		if (menu.menuName == menuCustomer.menuName) //Cek apakah nama martabak dipiring sesuai dengan nama martabak permintaan customer
		{
			customerPlaceholder[customerNumberSelected].GetComponent<Image>().sprite = null; //Hapus gambar customer
			customerMenu[customerNumberSelected].GetComponent<Image>().sprite = null; //Hapus gambar menu customer
			customerSlot[customerNumberSelected] = null; //Hapus ScriptableObject customer dan sekarang slot customer kosong lagi
			customerSuccess += 1; //Hitung customer sukses
			gold += menu.menuPrice;
			goldText.text = "Gold : " + gold.ToString();
			platePlaceholder[plateNumberSelected].GetComponent<Image>().sprite = plateEmpty; //Ganti gambar piring kosong
			plateSlot[plateNumberSelected] = null; //Hapus ScriptableObject menu yang ada di piring dan sekarang slot piring kosong lagi
			if (customerSuccess >= customerMax) //Cek kondisi menang (Jika target memenuhi jumlah maksimal customer). Untuk tugas nomer 2 yang "untuk stage 2-1 bikinkan stage dengan type"
			{
				Victory();
			}
		}
	}
	
	public void Customer2()
	{
		customerNumberSelected = 1;
		Menu menu = (Menu)plateSlot[plateNumberSelected]; //Panggil martabak yang dipilih di piring
		Customer customer = (Customer)customerSlot[customerNumberSelected]; //Panggil customernya berdasarkan nomernya
		Menu menuCustomer = (Menu)customer.customerMenu; //Panggil martabak yang dipilih di customer
		if (menu.menuName == menuCustomer.menuName) //Cek apakah nama martabak dipiring sesuai dengan nama martabak permintaan customer
		{
			customerPlaceholder[customerNumberSelected].GetComponent<Image>().sprite = null; //Hapus gambar customer
			customerMenu[customerNumberSelected].GetComponent<Image>().sprite = null; //Hapus gambar menu customer
			customerSlot[customerNumberSelected] = null; //Hapus ScriptableObject customer dan sekarang slot customer kosong lagi
			customerSuccess += 1; //Hitung customer sukses
			gold += menu.menuPrice;
			goldText.text = "Gold : " + gold.ToString();
			platePlaceholder[plateNumberSelected].GetComponent<Image>().sprite = plateEmpty; //Ganti gambar piring kosong
			plateSlot[plateNumberSelected] = null; //Hapus ScriptableObject menu yang ada di piring dan sekarang slot piring kosong lagi
			if (customerSuccess >= customerMax) //Cek kondisi menang (Jika target memenuhi jumlah maksimal customer). Untuk tugas nomer 2 yang "untuk stage 2-1 bikinkan stage dengan type"
			{
				Victory();
			}
		}
	}
	
	public void Customer3()
	{
		customerNumberSelected = 2;
		Menu menu = (Menu)plateSlot[plateNumberSelected]; //Panggil martabak yang dipilih di piring
		Customer customer = (Customer)customerSlot[customerNumberSelected]; //Panggil customernya berdasarkan nomernya
		Menu menuCustomer = (Menu)customer.customerMenu; //Panggil martabak yang dipilih di customer
		if (menu.menuName == menuCustomer.menuName) //Cek apakah nama martabak dipiring sesuai dengan nama martabak permintaan customer
		{
			customerPlaceholder[customerNumberSelected].GetComponent<Image>().sprite = null; //Hapus gambar customer
			customerMenu[customerNumberSelected].GetComponent<Image>().sprite = null; //Hapus gambar menu customer
			customerSlot[customerNumberSelected] = null; //Hapus ScriptableObject customer dan sekarang slot customer kosong lagi
			customerSuccess += 1; //Hitung customer sukses
			gold += menu.menuPrice;
			goldText.text = "Gold : " + gold.ToString();
			platePlaceholder[plateNumberSelected].GetComponent<Image>().sprite = plateEmpty; //Ganti gambar piring kosong
			plateSlot[plateNumberSelected] = null; //Hapus ScriptableObject menu yang ada di piring dan sekarang slot piring kosong lagi
			if (customerSuccess >= customerMax) //Cek kondisi menang (Jika target memenuhi jumlah maksimal customer). Untuk tugas nomer 2 yang "untuk stage 2-1 bikinkan stage dengan type"
			{
				Victory();
			}
		}
	}
	
	public void Customer4()
	{
		customerNumberSelected = 3;
		Menu menu = (Menu)plateSlot[plateNumberSelected]; //Panggil martabak yang dipilih di piring
		Customer customer = (Customer)customerSlot[customerNumberSelected]; //Panggil customernya berdasarkan nomernya
		Menu menuCustomer = (Menu)customer.customerMenu; //Panggil martabak yang dipilih di customer
		if (menu.menuName == menuCustomer.menuName) //Cek apakah nama martabak dipiring sesuai dengan nama martabak permintaan customer
		{
			customerPlaceholder[customerNumberSelected].GetComponent<Image>().sprite = null; //Hapus gambar customer
			customerMenu[customerNumberSelected].GetComponent<Image>().sprite = null; //Hapus gambar menu customer
			customerSlot[customerNumberSelected] = null; //Hapus ScriptableObject customer dan sekarang slot customer kosong lagi
			customerSuccess += 1; //Hitung customer sukses
			gold += menu.menuPrice;
			goldText.text = "Gold : " + gold.ToString();
			platePlaceholder[plateNumberSelected].GetComponent<Image>().sprite = plateEmpty; //Ganti gambar piring kosong
			plateSlot[plateNumberSelected] = null; //Hapus ScriptableObject menu yang ada di piring dan sekarang slot piring kosong lagi
			if (customerSuccess >= customerMax) //Cek kondisi menang (Jika target memenuhi jumlah maksimal customer). Untuk tugas nomer 2 yang "untuk stage 2-1 bikinkan stage dengan type"
			{
				Victory();
			}
		}
	}
	#endregion
	
	public void Victory()
	{
		gameOver.SetActive(true);
		gameOverText.text = "Victory";
		if (gameManager.stageNameManager == "Stage 2.1") // Karena hanya ada 2 kondisi menang maka sekarang skrip hanya skrip sederhana mendeteksi 2 itu saja
		{
			gameManager.clear21 = true;
		}
		if (gameManager.stageNameManager == "Stage 1.1")
		{
			gameManager.clear11 = true;
		}
	}
	
	public void GameOver()
	{
		gameOver.SetActive(true);
		gameOverText.text = "Game Over";
	}
	
	public void BackToMainMenu()
	{
		gameOver.SetActive(false);
		StartCoroutine(LoadStage());
	}
	
	IEnumerator LoadStage()
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(0);
    }
	
	public void RestartStage()
	{
		gameOver.SetActive(false);
		StartCoroutine(Restart());
	}
	
	IEnumerator Restart()
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(1);
    }
}