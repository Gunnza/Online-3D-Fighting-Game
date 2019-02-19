using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blocking : MonoBehaviour {
	
	public bool block = false;
	
	public float startBlockStamina = 100;
	public float currentBlockStamina;
	
	public Slider staminaSlider;
	GameObject guiStaminaBar; 
	GameObject staminaFill;
	Image image;
	
	//Temp Booleans
	bool recharge = false;
	
	//Color
	public Color maxStaminaColor;
	public Color minStaminaColor;
	//public Image fill;
	
	void Awake()
	{
		guiStaminaBar = this.gameObject.transform.GetChild(3).GetChild(2).GetChild(0).gameObject;
		staminaSlider = guiStaminaBar.GetComponent<Slider>();
		
	    currentBlockStamina = startBlockStamina;
		
		staminaFill = this.gameObject.transform.GetChild(3).GetChild(2).GetChild(0).GetChild(1).GetChild(0).gameObject;
		image = staminaFill.GetComponent<Image>();
		//staminaFill.color = Color.yellow; 
	}
	
	void Update()
	{
		staminaSlider.value = currentBlockStamina;
		
		if(currentBlockStamina<= 10)
		{
			image.color = Color.red;
		}
		else
		{
			image.color = Color.yellow;
		}
		
		if(currentBlockStamina < startBlockStamina)
		{
			guiStaminaBar.SetActive(true);
		}
		else
		{
			guiStaminaBar.SetActive(false);
		}
		
		//fill.color = Color.Lerp(minStaminaColor, maxStaminaColor, (float)currentBlockStamina / startBlockStamina);
	}
}
		
		


