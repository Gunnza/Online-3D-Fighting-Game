using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManagerVik : Photon.MonoBehaviour {

    // this is a object name (must be in any Resources folder) of the prefab to spawn as player avatar.
    // read the documentation for info how to spawn dynamically loaded game objects at runtime (not using Resources folders)
    public string playerPrefabName = "OnlinePlayer";
	
	//Other Script
	
	//public Transform spawnPoint;
	//Array Variables
	public int[] spawnIndex;
	public Transform[] spawnPoints;
    Transform mySpawnPoint;
	public static bool spawn1Crowded;
	public static bool spawn2Crowded;
	public static bool spawn3Crowded;
	public static bool spawn4Crowded;
	
	//UI
	public GameObject roomUI;

	//Cameras
	public GameObject spectatorCamera;
	public GameObject playerCamera;
	
	//Variables
	
	int numberPlayers = 0;
	int playerCount;
	int countDown = 3;
	
	int killCount;
	public Text score;
	
	public Text countText;
	public Text countDownText;
	
	//List of unique player
	List<int> playerIDList; 
	int playerID;

    public AudioSource myAudio;
    public AudioClip mainMenuMusic;
    public AudioClip gameplayMusic;

    //void OnPhotonPlayerConnected(PhotonPlayer otherPlayer)
    //{	
    //	UpdatePlayersCountText();
    //}

    private void Awake()
    {
        myAudio = GetComponent<AudioSource>();
    }

 
	void OnJoinedRoom()
    {
		//Audio Controls
		//myAudio.Stop();
       	myAudio.clip = gameplayMusic;
        myAudio.Play();

		PlayerPrefs.SetString("playerName", PhotonNetwork.playerName); //Players Name
	
		//UI Stuff 
		roomUI.SetActive(true);
		roomUI.transform.GetChild(3).gameObject.SetActive(false);
		
		ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
		props.Add ("Kills", 0);
		props.Add ("Deaths", 0);
		PhotonNetwork.player.SetCustomProperties(props);
		
		//SetCustomProperties(PhotonNetwork.player, 0, PhotonNetwork.playerList.Length -1);
		UpdatePlayersCountText(); 	
	}
	
	
	void UpdatePlayersCountText()
	{
	    playerCount = PhotonNetwork.playerList.Length;
	   	countText.text = playerCount.ToString() + "/4";
			
		if(playerCount >= 1)
		{
			//StartGame();
			StartCoroutine(CountDown());
			//StartCoroutine(HostSpawnFirst());	
		}	
	}

	IEnumerator HostSpawnFirst()
	{
		yield return new WaitForSecondsRealtime(Random.Range(0,3));
		StartCoroutine(CountDown());
	}
		
	IEnumerator CountDown()
		
	{
		roomUI.transform.GetChild(3).gameObject.SetActive(true);
		countDownText.text = "Game Starts in: " + countDown.ToString();
		
		
		countDown--;
		countDownText.text = "Game Starts in: " + countDown.ToString();
		
		yield return new WaitForSecondsRealtime(1);
		countDown--;
		countDownText.text = "Game Starts in: " + countDown.ToString();
		
		yield return new WaitForSecondsRealtime(1);
		countDown--;
		countDownText.text = "Game Starts in: " + countDown.ToString();
		
		StartGame();
	}
	
	
    
	
	/*[PunRPC]
	void UpdatePlayerCount(bool AddToCount){
		
		if (AddToCount)
		{
			playerCount += 1;
			countText.text = playerCount.ToString() + "/4";
		}
		else
		{
			playerCount -= 1;
			countText.text = playerCount.ToString() + "/4";
		}
	}
	*/
	//photonView.RPC("UpdatePlayerCount", PhotonTargets.All, false);
	
	void StartGame() //On joined a room 
    {
        Camera.main.farClipPlane = 1000; //Main menu set this to 0.4 for a nicer BG    
        //prepare instantiation data for the viking: Randomly diable the axe and/or shield
        //bool[] enabledRenderers = new bool[2];
       // enabledRenderers[0] = Random.Range(0,2)==0;//Axe
        //enabledRenderers[1] = Random.Range(0, 2) == 0; ;//Shield
       // object[] objs = new object[1]; // Put our bool data in an object array, to send
       // objs[0] = enabledRenderers;
		
		countDown = 3; //Count Down on Game room UI
		roomUI.SetActive(false);//Game Room UI turned off 
		if(spawnPoints == null) 
		{
			Debug.LogError ("No Spawn Points?");
			return;
		}
		
		//Random Spawn point index from the list in array 
		//myIndex = spawnIndex[Random.Range(0,spawnPoints.Length - 1)];
		//myIndex = 0;
				
		//spectatorCamera.SetActive(false);//Respawning camera set to off 
		
		if(ThirdPersonNetworkVik.gameStarted == false) //If statment so that it doesnt use from thirdperson script 
		{
			mySpawnPoint = spawnPoints[0];
			ThirdPersonNetworkVik.gameStarted = true;
		}
		else
		{
			mySpawnPoint = spawnPoints[ThirdPersonNetworkVik.myIndex];
		}
		
	        PhotonNetwork.Instantiate(this.playerPrefabName 
									 ,mySpawnPoint.transform.position,mySpawnPoint.transform.rotation, 0); 
		
			UpdateSpawn(ThirdPersonNetworkVik.myIndex);
		
		
		
			//PhotonView photonView = this.photonView;
			//photonView.RPC ("UpdateSpawn", PhotonTargets.All, myIndex);	
	}
	
	public void GameOver()
	{
		
	}
	
	
	void UpdateSpawn(int newIndex)
	{
		newIndex++;
		ThirdPersonNetworkVik.myIndex = newIndex;
		for (int i = 0; i <= ThirdPersonNetworkVik.myIndex ; i++)
        {
            if (ThirdPersonNetworkVik.myIndex >= 4)
            {
                ThirdPersonNetworkVik.myIndex  = 0;
            }
		}
	}
		
	
	/*
			//Random Spawn point index from the list in array 
			myIndex = spawnIndex[Random.Range(0,spawnPoints.Length - 1)];
			
			switch (myIndex)
			{
			case 1:
				
				if(ThirdPersonNetworkVik.spawn1Crowded) //Is a player on it
				{
					Debug.Log ("Spawn 1 Crowded Relocating..");
					StartGame();
				}
				else //if not spawn away 
				{
					spectatorCamera.SetActive(false);//Respawning camera set to off 
					mySpawnPoint = spawnPoints[myIndex];	
	        		PhotonNetwork.Instantiate(this.playerPrefabName, mySpawnPoint.transform.position,mySpawnPoint.transform.rotation, 0); //, objs);
				}
					break;
			case 2:
				
				if(ThirdPersonNetworkVik.spawn2Crowded) //Is a player on it
				{
					Debug.Log ("Spawn 2 Crowded Relocating..");
					StartGame();
				}
				else //if not spawn away 
				{
					spectatorCamera.SetActive(false);//Respawning camera set to off 
					mySpawnPoint = spawnPoints[myIndex];	
	        		PhotonNetwork.Instantiate(this.playerPrefabName, mySpawnPoint.transform.position,mySpawnPoint.transform.rotation, 0); //, objs);
				}
				break;
				
			case 3: 
				
				if(ThirdPersonNetworkVik.spawn3Crowded) //Is a player on it
				{
					Debug.Log ("Spawn 3 Crowded Relocating..");
					StartGame();
				}
				else //if not spawn away 
				{
					spectatorCamera.SetActive(false);//Respawning camera set to off 
					mySpawnPoint = spawnPoints[myIndex];	
	        		PhotonNetwork.Instantiate(this.playerPrefabName, mySpawnPoint.transform.position,mySpawnPoint.transform.rotation, 0); //, objs);
				}
				break;
				
			case 4:
				
				if(ThirdPersonNetworkVik.spawn4Crowded) //Is a player on it
				{
					Debug.Log ("Spawn 4 Crowded Relocating..");
					StartGame();
				}
				else //if not spawn away 
				{
					spectatorCamera.SetActive(false);//Respawning camera set to off 
					mySpawnPoint = spawnPoints[myIndex];	
	        		PhotonNetwork.Instantiate(this.playerPrefabName, mySpawnPoint.transform.position,mySpawnPoint.transform.rotation, 0); //, objs);
				}
				break;
			}	
		*/	
    
   
	
	IEnumerator OnLeftRoom()
    {
		roomUI.SetActive(false);
	
		
		playerCount = PhotonNetwork.playerList.Length;
		countText.text = playerCount.ToString() + "/4";
		
		myAudio.Play();
		
        //Easy way to reset the level: Otherwise we'd manually reset the camera
        //Wait untill Photon is properly disconnected (empty room, and connected back to main server)
        while(PhotonNetwork.room!=null || PhotonNetwork.connected==false)
            yield return 0;
        	Application.LoadLevel(Application.loadedLevel);
    }

    void OnGUI()
    {
        if (PhotonNetwork.room == null) return; //Only display this GUI when inside a room

        if (GUILayout.Button("Leave Room"))
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    void OnDisconnectedFromPhoton()
    {
        Debug.LogWarning("OnDisconnectedFromPhoton");
    }

   public void AudioButton()
    {
        if (myAudio.isPlaying)
        {
            myAudio.Stop();
        }
        else
        {
            myAudio.Play();
        }
    }
}
