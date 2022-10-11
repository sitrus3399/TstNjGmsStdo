using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Customer", menuName ="ScriptableObjects/Customer")]
public class Customer : ScriptableObject
{
	public string customerName;
	public ScriptableObject customerMenu; //Menu yang akan dipesan customer
	//public Menu menuScriptableObject = (Menu)customerMenu;
	public int customerMaxTime;
	public int customerMinTime;
	public Sprite customerImage;
}
