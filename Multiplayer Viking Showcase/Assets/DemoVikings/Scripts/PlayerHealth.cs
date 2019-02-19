using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth1 : Photon.MonoBehaviour {
	
	public int startingHealth = 100;
	public int currentHealth;
	public Image damageImage;
	public AudioClip deathClip;
	public float flashSpeed = 5f;
	public Color flashColour = new Color(1f, 0f, 0f, 0.1f);
	
	//Slider Bar Variables 
	public Slider healthSlider;
	GameObject guiHealthbar; 
	
	//Spawn Array and index
	public int spawnIndex;
	public Transform[] spawnPoints;
	
	//UI Stats
	public int startingDeaths;
	public int currentDeaths;
	public Text deathsText;
	
	//GameOver UI
	GameObject gameOverUI;
	GameObject youWinText;
	GameObject youLoseText;
	
	//Kills
	public int totalKills;
	public Text killsText;
	int scoreLimit = 5;
	
	Camera camera;
	GameObject camera2;
	
	Animator anim;

	public AudioSource playerAudio;
    // public AudioClip 
    public AudioClip strikingClip;
    public AudioClip swingingClip;
    public AudioClip defendingClip;
	public AudioClip walkClip;

    bool isDead;
	bool damaged;
	
	//Components 
	ThirdPersonControllerNET thirdPersonController;
	CapsuleCollider capsuleCollider;
	
	//Game Over boolean
	public bool gameOver = false;
	bool stopUpdate = false;
	bool invincableTimer = false;
	
	
	void Awake ()
	{
		//Setting Components
		thirdPersonController = GetComponent<ThirdPersonControllerNET>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		
		//Getting the exact healthbar
		guiHealthbar = this.gameObject.transform.GetChild(3).GetChild(1).GetChild(0).gameObject;
		healthSlider = guiHealthbar.GetComponent<Slider>();
		anim = GetComponent <Animator>();
		playerAudio = GetComponent<AudioSource>();
		currentHealth = startingHealth;
	}

	void Update () {
		
		//healthSlider.value = currentHealth;
		//currentDeaths =(int)PhotonNetwork.player.customProperties["Deaths"];
		
		//Update UI stats
		deathsText.text = "Deaths: " + currentDeaths;
		killsText.text = "Kills: " + totalKills;
		
		if(damaged)
		{
			damageImage.color = flashColour;
		}
		else
		{
			damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
		}
		damaged = false;
		
		//If the gameover bool is true
		if(gameOver == true )
		{
			gameOverUI = this.gameObject.transform.GetChild(3).GetChild(3).gameObject;
			gameOverUI.SetActive(true);

            if (totalKills < scoreLimit)
            {
                youLoseText = this.gameObject.transform.GetChild(3).GetChild(3).GetChild(2).gameObject;
                youLoseText.SetActive(true);
                thirdPersonController.enabled = false;
            }
            else if (totalKills >= scoreLimit)
			{
				youWinText = this.gameObject.transform.GetChild(3).GetChild(3).GetChild(1).gameObject;
				youWinText.SetActive(true);
				thirdPersonController.enabled = false;
			}
		
		 }
	}
	
	
	public void TakeDamage ( int amount) {
		
		damaged = true; 
		//Debug.Log ("Taken Damage");
	
		if(invincableTimer == false)
		currentHealth -= amount;
		
		healthSlider.value = currentHealth;
		Debug.Log (thirdPersonController.strikeObject.GetComponent<PlayerHealth>().totalKills);
		
		//thirdPersonController.strikeObject.GetComponent<PlayerHealth>().HitEnemy();
		
		//Damager Object
		//ThirdPersonControllerNET thirdPersonControllerNET = GetComponent<ThirdPersonControllerNET>();
		//int strikeObjectID = thirdPersonNetworkVik.playerID;;
		//Debug.Log ("DamagerID " + strikeObjectID);
		
		if(currentHealth <= 0 && !isDead)
		{
			//totalKills++;
			thirdPersonController.strikeObject.GetComponent<PlayerHealth>().UpdateKills();
			
			//Turning off art and control 
			//.transform.GetChild(0).gameObject.SetActive(false);
			//.transform.GetChild(5).gameObject.SetActive(false);
			
			currentDeaths++;
			
			//Chnage Health
			currentHealth = startingHealth;
			healthSlider.value = currentHealth;
			
			//Respawn
			//StartCoroutine(Respawn());
			transform.position = spawnPoints[spawnIndex].transform.position;
			transform.rotation = spawnPoints[spawnIndex].transform.rotation;
			
			//Start Invincable timer
			//StartCoroutine(InvincableTimer());
			
			//Change the spawn
			UpdateSpawn();
			
		}
	}
	IEnumerator Respawn()
	{
		yield return new WaitForSecondsRealtime(.5f);
		//Respawn art and control 
		gameObject.transform.GetChild(0).gameObject.SetActive(true);
		gameObject.transform.GetChild(5).gameObject.SetActive(true);
	}
	
	IEnumerator InvincableTimer()
	{ 
		invincableTimer = true;
		yield return new WaitForSecondsRealtime(2);
		invincableTimer = false;
	}
	
	
	//photonView.RPC ("Respawn", PhotonTargets.All);
			//Death();
	public void UpdateKills()
	{
		totalKills++;
		Debug.Log ("Update Kills");
		if( totalKills >= scoreLimit)
		{
			ThirdPersonNetworkVik.gameOver = true;
		}
	}
	

	/*void OnGUI() //Test Button
	{
		if(GetComponent<PhotonView>().isMine && gameObject.tag == "Player")
		{
			if(GUI.Button (new Rect ( Screen.width-100, 0, 100, 40), "Kills++")) 
			{
				totalKills++;
				
				if( totalKills >= scoreLimit)
				{
					ThirdPersonNetworkVik.gameOver = true;
				}
				//Death ();
			}
		}
	}
	*/
	
	IEnumerator RespawnTimer()
	{
		//Camera Change
		gameObject.transform.GetChild(4).gameObject.SetActive(true);
		gameObject.transform.GetChild(5).gameObject.SetActive(false);
		
		//Disable object on network 
		photonView.RPC ("DisableObjects", PhotonTargets.All);
		
		//Wait 3 seconds
		yield return new WaitForSecondsRealtime(3);
		
		//Camera Change
		gameObject.transform.GetChild(5).gameObject.SetActive(true);
		gameObject.transform.GetChild(4).gameObject.SetActive(false);
		
		//Reset the health 
		currentHealth = startingHealth;
		
		//Activate objects on network
		photonView.RPC ("Respawn", PhotonTargets.All);
		
		isDead = false;
		
	}
		
		//Animate death
		//anim.SetTrigger("Die");
		//playerAudio.clip = deathClip;
		//playerAudio.Play(
	void UpdateSpawn()
	{
		spawnIndex++;
		
		for (int i = 0; i <= spawnIndex ; i++)
        {
            if (spawnIndex >= 3)
            {
                spawnIndex  = 0;
            }
		}
	}
	
	public void LeaveButton()
        {
        //Enabling everything again 
        youWinText = this.gameObject.transform.GetChild(3).GetChild(3).GetChild(1).gameObject;
        youLoseText = this.gameObject.transform.GetChild(3).GetChild(3).GetChild(2).gameObject;
        youLoseText.SetActive(true);
        youWinText.SetActive(true);

        thirdPersonController.enabled = true;

        currentHealth = startingHealth;
        currentDeaths = startingDeaths;
        totalKills = 0;

        //Photon leaves room
        PhotonNetwork.LeaveRoom();  
        }

   
}

