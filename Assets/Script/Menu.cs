using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Menu", menuName ="ScriptableObjects/Menu")]
public class Menu : ScriptableObject
{
    public string menuName;
	public int menuPrice;
	public float menuTimecooking;
	public Sprite menuImage;
}
