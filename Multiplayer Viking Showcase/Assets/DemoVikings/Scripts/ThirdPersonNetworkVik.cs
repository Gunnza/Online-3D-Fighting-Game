using UnityEngine;
using System.Collections;


public class ThirdPersonNetworkVik : Photon.MonoBehaviour
{
    ThirdPersonCameraNET cameraScript;
    ThirdPersonControllerNET controllerScript;

	
	//My New Scripts
	PlayerHealth playerHealth;
	Blocking blocking;
	
	CharacterController characterController;
	Mecanim_Control_melee mecanim_Control_Melee;
	
    private bool appliedInitialUpdate;
	GameObject guiHealthbar;
	GameObject guiStaminaBar;
	GameObject guiKills;
	GameObject guiDeaths;
	
	
	
	public static int myIndex;
	public static bool gameStarted;
	public static bool gameOver;
	
	
	
	//Booleans Netwrok values of spawns
	public static bool spawn1Crowded = false;
	public static bool spawn2Crowded = false;
	public static bool spawn3Crowded = false;
	public static bool spawn4Crowded = false;
	//Boolean to tell if game has started
	//public static bool gameStarted = false;
	
	public int playerID;
	
	//Camera Orbit
	GameObject mainCamera;
	MouseOrbit mouseOrbit;
	
	Animator anim;

	
	
	
    void Awake()
    {
	
		anim = GetComponent<Animator>();
		guiHealthbar = this.gameObject.transform.GetChild(3).GetChild(1).GetChild(0).gameObject;
		guiKills = this.gameObject.transform.GetChild(3).GetChild(1).GetChild(1).gameObject;
		guiDeaths = this.gameObject.transform.GetChild(3).GetChild(1).GetChild(2).gameObject;
		guiStaminaBar = this.gameObject.transform.GetChild(3).GetChild(2).GetChild(0).gameObject;
		
        cameraScript = GetComponent<ThirdPersonCameraNET>();
        controllerScript = GetComponent<ThirdPersonControllerNET>();
		characterController = GetComponent<CharacterController>();
		mecanim_Control_Melee = GetComponent<Mecanim_Control_melee>();
		
		//New Scripts 
		playerHealth = GetComponent<PlayerHealth>();
		blocking = GetComponent<Blocking>();
		
    }
	
	IEnumerator WaitForCamera()
	{
		yield return new WaitForSecondsRealtime(.5f);
		
		//Wait because index wont be in range at very start
		mainCamera = this.gameObject.transform.GetChild(6).gameObject;
		mouseOrbit = mainCamera.GetComponent<MouseOrbit>();
		mouseOrbit.target = this.gameObject.transform;
		
		
	}

    void Start()
    {
	
        //TODO: Bugfix to allow .isMine and .owner from AWAKE!
        if (photonView.isMine)
        {
            //MINE: local player, simply enable the local scripts
         //   cameraScript.enabled = true;
            controllerScript.enabled = true;
			playerHealth.enabled = true;
			blocking.enabled = true;
			
			
		
			characterController.enabled = true;
			mecanim_Control_Melee.enabled = true;
			
			guiHealthbar.SetActive(true);
			guiStaminaBar.SetActive(true);
			guiKills.SetActive(true);
			guiDeaths.SetActive(true);
			
			playerID = PhotonNetwork.player.ID;
			
			StartCoroutine(WaitForCamera());
			
			//Camera Position
            Camera.main.transform.parent = transform;
            Camera.main.transform.localPosition = new Vector3(0, 2, -10);
            Camera.main.transform.localEulerAngles = new Vector3(10, 0, 0);
        }
        else
        {    
         //   cameraScript.enabled = false;
            controllerScript.enabled = true;
			playerHealth.enabled = false;
			blocking.enabled = false;
			
			characterController.enabled = false;
			mecanim_Control_Melee.enabled = false;
			
			guiHealthbar.SetActive(false);
			guiStaminaBar.SetActive(false);
			guiKills.SetActive(false);
			guiDeaths.SetActive(false);
			

        }
        controllerScript.SetIsRemotePlayer(!photonView.isMine);
        gameObject.name = gameObject.name + photonView.viewID;
    }
	
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            // stream.SendNext((int)controllerScript._characterState);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
			//stream.SendNext (GetComponent<CharacterController>());
            //stream.SendNext(GetComponent<Rigidbody>().velocity); 
			
			//Send Hitboxes 
			stream.SendNext(transform.GetChild(1).gameObject.activeSelf);
			stream.SendNext(transform.GetChild(2).gameObject.activeSelf);
			
			//Send Boolean Values
			stream.SendNext(spawn1Crowded);
			stream.SendNext(spawn2Crowded);
			stream.SendNext(spawn3Crowded);
			stream.SendNext(spawn4Crowded);
			
			//Sending game over boolean
			stream.SendNext(gameOver);
			
			//Send Health
			//stream.SendNext(playerHealth.currentHealth);
			
			//Send Kills
			//stream.SendNext(kills);
			
			//Send Player ID
			stream.SendNext(playerID);
	
			
			//Spawning
			stream.SendNext(myIndex);
			stream.SendNext(gameStarted);
			
			//Sending Animation bools
			stream.SendNext(anim.GetFloat("Axis_Horizontal"));
			stream.SendNext(anim.GetFloat("Axis_Vertical"));
			stream.SendNext(anim.GetFloat("LeftMouseClicks"));
			stream.SendNext(anim.GetBool("LeftMouseClick"));
			stream.SendNext(anim.GetBool("RightMouse"));
			//stream.SendNext(mecanim_Control_Melee.animator.GetFloat("LeftShift_axis"));
			
			
			
			//Send Health int and UI
			//stream.SendNext(playerHealth.currentHealth);

        }
        else
        {
            //Network player, receive data
            //controllerScript._characterState = (CharacterState)(int)stream.ReceiveNext();
            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
			//GetComponent<CharacterController>() = (CharacterController).stream.RecieveNext();
            //GetComponent<Rigidbody>().velocity = (Vector3)stream.ReceiveNext();
			
			//Recieve Hitboxes
			transform.GetChild(1).gameObject.SetActive((bool) stream.ReceiveNext());
			transform.GetChild(2).gameObject.SetActive((bool) stream.ReceiveNext());
			
			//Send Boolean Values
			spawn1Crowded = (bool)stream.ReceiveNext();
			spawn2Crowded = (bool)stream.ReceiveNext();
			spawn3Crowded = (bool)stream.ReceiveNext();
			spawn4Crowded = (bool)stream.ReceiveNext();
			
			//Recieving game over boolean variable 
			gameOver = (bool)stream.ReceiveNext();
			
			//Recieve health
			//playerHealth.currentHealth = (int)stream.ReceiveNext();
			
			//Recieve kills
			//kills = (int)stream.ReceiveNext();
			
			//Recieve Player ID
			playerID = (int)stream.ReceiveNext();
			
			//Spawning
			myIndex = (int)stream.ReceiveNext();
			gameStarted = (bool)stream.ReceiveNext();
			
			//Animations recieving 
			anim.SetFloat("Axis_Horizontal", (float)stream.ReceiveNext());
			anim.SetFloat("Axis_Vertical", (float)stream.ReceiveNext());
			anim.SetFloat("LeftMouseClicks", (float)stream.ReceiveNext());
			anim.SetBool("LeftMouseClick", (bool)stream.ReceiveNext());
			anim.SetBool("RightMouse", (bool)stream.ReceiveNext());
			//mecanim_Control_Melee.animator.SetFloat("LeftShift_axis", (float)stream.ReceiveNext());
			
			
			
			
			//Recieve health int and UI
			//playerHealth.currentHealth = (int)stream.ReceiveNext();
		

            if (!appliedInitialUpdate)
            {
                appliedInitialUpdate = true;
                transform.position = correctPlayerPos;
                transform.rotation = correctPlayerRot;
               // GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
    }

    private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

    void Update()
    {
        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
        }
		
		if(gameOver == true)
		{
			playerHealth.gameOver = true;
		}
		
		//Turning on and off the stamina bar
		if(blocking.currentBlockStamina < blocking.startBlockStamina)
		{
			guiStaminaBar.SetActive(true);
		}
		else
		{
			guiStaminaBar.SetActive(false);
		}
    }

    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        //We know there should be instantiation data..get our bools from this PhotonView!
        // object[] objs = photonView.instantiationData; //The instantiate data..
        //bool[] mybools = (bool[])objs[0];   //Our bools!
        //disable the axe and shield meshrenderers based on the instantiate data
        //MeshRenderer[] rens = GetComponentsInChildren<MeshRenderer>();
        //rens[0].enabled = mybools[0];//Axe
        //rens[1].enabled = mybools[1];//Shield

    }

}