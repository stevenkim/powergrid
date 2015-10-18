﻿using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {

	public enum State {
		ComputeTurn,
		BuyPlants,
		BuyMaterials,
		BuildCities,
		Bureaucracy,
	};

	public GameObject powerPlantObject;
	
	private State currentState = State.ComputeTurn;
	private ArrayList players = null;
	private ArrayList playerOrderPieces = null;
//	private ArrayList drawDeckPowerPlants = null;
//	private ArrayList inMarketPowerPlants = null;
//		
//	private int marketCount = 8;
//	private int biddableCount = 4;
	private int gameRound = 1;

	private CityGraph graph;

	public TextMesh stateText;

	private int gameEndCityCount = 17;
	private int step2CityCount = 7;
	private int numberOfRegions = 3;
	private int[] cityCountPayoutTable;

	[HideInInspector]
	public int gameStep = 1;

	private int playerTurn = 0;
	
	private PowerPlantMaterialStore materialStore;
	private PowerPlantShop powerplantShop;

	public static GameState instance = null;
	private GameObject cityPopupText;

	public GameObject materialShopPopup;
//	public GameObject powerplantShopPopup;

	public TextMesh coalShopText;
	public TextMesh oilShopText;
	public TextMesh garbageShopText;
	public TextMesh uraniumShopText;
	
	public Player CurrentPlayer() {
		return (Player)players [playerTurn];
	}

	public ArrayList Players {
		get {
			return players;
		}
	}
	
	// Use this for initialization
	void Start () {

		instance = this;
		print ("Initialize game");

		graph = GetComponent<CityGraph> ();

		powerplantShop = GameObject.FindObjectOfType<PowerPlantShop> ();

		//setup players
		players = new ArrayList();

		playerOrderPieces = new ArrayList ();

		playerOrderPieces.Add (GameObject.Find("PlayerOrderPiece1"));
		playerOrderPieces.Add (GameObject.Find("PlayerOrderPiece2"));
		playerOrderPieces.Add (GameObject.Find("PlayerOrderPiece3"));
		playerOrderPieces.Add (GameObject.Find("PlayerOrderPiece4"));

		//create objects from GUI setup
		Player[] p = FindObjectsOfType(typeof(Player)) as Player[];
		for (int i = 0; i < p.Length; i++) {
			players.Add (p [i]);
			((GameObject)playerOrderPieces[i]).GetComponent<Renderer>().material.color = p[i].color;
		}

		players.Sort ();
		players.Reverse ();

		//create material store
		materialStore = new PowerPlantMaterialStore ();

		InitializePayoutTable ();
//		InitializePowerPlants ();
//		DealCards ();
//
//		ShufflePowerPlantCards ();
	}

	public PowerPlantShop PowerplantShop {
		get {
			return powerplantShop;
		}
	}

	void Reset() {
		foreach (Player p in players) {
			p.Reset ();
		}

		powerplantShop.Reset ();

//		inMarketPowerPlants.Clear ();
//		drawDeckPowerPlants.Clear ();
//		InitializePowerPlants ();
//		DealCards ();
//
//		ShufflePowerPlantCards ();
	}

	void InitializePayoutTable() {
		cityCountPayoutTable = new int[21];
		cityCountPayoutTable [0] = 10;
		cityCountPayoutTable [1] = 22;
		cityCountPayoutTable [2] = 33;
		cityCountPayoutTable [3] = 44;
		cityCountPayoutTable [4] = 54;
		cityCountPayoutTable [5] = 64;
		cityCountPayoutTable [6] = 73;
		cityCountPayoutTable [7] = 82;
		cityCountPayoutTable [8] = 90;
		cityCountPayoutTable [9] = 98;
		cityCountPayoutTable [10] = 105;
		cityCountPayoutTable [11] = 112;
		cityCountPayoutTable [12] = 118;
		cityCountPayoutTable [13] = 124;
		cityCountPayoutTable [14] = 129;
		cityCountPayoutTable [15] = 134;
		cityCountPayoutTable [16] = 138;
		cityCountPayoutTable [17] = 142;
		cityCountPayoutTable [18] = 145;
		cityCountPayoutTable [19] = 148;
		cityCountPayoutTable [20] = 150;
	}

	public State CurrentState {
		get {
			return currentState;
		}
	}

	void DoBureaucracy() {
		foreach (Player p in players) {
			int powerCount = 0;

			foreach(PowerPlant pp in p.powerPlants)
				powerCount += pp.RunPlant();
			if(powerCount > 20)
				powerCount = 20;
			p.cash += cityCountPayoutTable[powerCount];
			p.cash += Random.Range(0,20);
		}
	}

	// Update is called once per frame
	void Update () {	
		bool advanceState = false;
		bool advanceTurn = false;

//		graph.DebugDraw ();

		if (Input.GetKeyDown (KeyCode.Space)) {
			advanceTurn = true;
		}

		if (Input.GetKeyDown (KeyCode.R)) {
			Reset();
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			gameStep++;
			if(gameStep == 4)
				gameStep = 1;
		}


		if (cityPopupText == null)
			cityPopupText = GameObject.Find ("CityPopupText");

		if(currentState != State.BuildCities)
			cityPopupText.transform.position = new Vector3(100,100,100);//offscreen
		if (currentState != State.BuyMaterials)
			materialShopPopup.transform.localPosition = new Vector3 (-2.0f,0.3f, -0.05f);

//			cityPopupText.transform.position = new Vector3(100,100,100);//offscreen

		switch (currentState) {
		case  State.ComputeTurn:
//			print (currentState);
			players.Sort ();
			players.Reverse ();

			//set piece colors
			for (int i = 0; i < players.Count; i++) {
				((GameObject)playerOrderPieces [i]).GetComponent<Renderer> ().material.color = ((Player)players[i]).color;
			}

			currentState = State.BuyPlants;
			powerplantShop.Show(true);

			playerTurn = 0;
			break;
		case  State.BuyPlants:
			//show plants
			//click on plant or pass
			//adjust price
			//click on player that buys
			//redraw
			//move plant shop into place


			if (advanceTurn) {
				playerTurn++;
				if(playerTurn == players.Count) {
					advanceState = true;
					playerTurn = players.Count-1;
					powerplantShop.Show(false);
				}
			}

			if(advanceState) {
				powerplantShop.DealCards();
				currentState = State.BuyMaterials;
//				print (currentState);
			}
		break;
		case  State.BuyMaterials:
			//show each player's power plants
			//show market
			//allow buying into each plant by clicking

			materialShopPopup.transform.localPosition = new Vector3 (-0.4f,0.3f, -0.05f);

			if (advanceTurn) {
				playerTurn--;
				if(playerTurn == -1) {
					advanceState = true;
					playerTurn = players.Count-1;
				}
			}

			if(advanceState) {
				currentState = State.BuildCities;
				RecomputeTravelCosts();

//				print (currentState);
			}
		break;
		case  State.BuildCities:
			//hovering over city to shows cost (compute cost by searching over graph)

			if (advanceTurn) {
				CommitCityPurchases();
				playerTurn--;

				if(playerTurn == -1) {

					advanceState = true;
					playerTurn = 0;
				} else {
					RecomputeTravelCosts();
				}

			}
			if(advanceState) {
				currentState = State.Bureaucracy;
				CommitCityPurchases();
			}
		break;
		case  State.Bureaucracy:
//			print (currentState);
			DoBureaucracy();
			materialStore.Restock(players.Count, gameStep);
			currentState = State.ComputeTurn;
			gameRound++;
			break;
		}

		int index = 0;

		for (int i = 0; i < players.Count; i++) {
			GameObject playerOrderPiece = ((GameObject)playerOrderPieces [i]);
			Vector3 pos = playerOrderPiece.transform.position;
			Quaternion rot = Quaternion.identity;
			if(i == playerTurn) {
				pos.y = 0.0f + 0.1f*(1.0f + Mathf.Sin (5*UnityEngine.Time.realtimeSinceStartup));
				rot = Quaternion.Euler (0, 120 * UnityEngine.Time.realtimeSinceStartup, 0);
			} else {
				pos.y = 0.0f;
			}
			playerOrderPiece.transform.position = pos;
			playerOrderPiece.transform.rotation = rot;
		}


	}
	public void RecomputeTravelCosts() {
		graph.RecomputeCityTravelCosts(gameStep, CurrentPlayer());
	}

	public void CommitCityPurchases() {
		foreach (Player p in players)
			p.CommitPurchases ();
	}

	void OnGUI() {

		stateText.text = "Game Step: " + gameStep + 
			"\nActivity: " + currentState;

		int yPos = 10;
//		GUI.Label (new Rect (10, yPos, 500, 20), "Round: " + gameRound + 
//            " Step: " + gameStep + 
//			" State: " + currentState + 
//			" PlayerTurn: " + ((Player)players [playerTurn]).gameObject.name);
//		yPos += 20;
//		GUI.Label(new Rect(10,yPos,500,20),players[0].ToString());
//		yPos += 20;
//		GUI.Label(new Rect(10,yPos,500,20),players[1].ToString());
//		yPos += 20;
//		GUI.Label(new Rect(10,yPos,500,20),players[2].ToString());
//		yPos += 20;
//		GUI.Label(new Rect(10,yPos,500,20),players[3].ToString());
//		yPos += 20;

//		GUI.Label(new Rect(10,yPos,500,20),"Material Store: " +
//		          " Coal: [" + materialStore.QueryInventory(PowerPlantMaterialStore.Type.Coal) + "," + materialStore.QueryCost(PowerPlantMaterialStore.Type.Coal)+"]" +
//		          " Oil: [" + materialStore.QueryInventory(PowerPlantMaterialStore.Type.Oil) + "," + materialStore.QueryCost(PowerPlantMaterialStore.Type.Oil)+"]" +
//		          " Garbage: [" + materialStore.QueryInventory(PowerPlantMaterialStore.Type.Garbage) + "," + materialStore.QueryCost(PowerPlantMaterialStore.Type.Garbage)+"]" +
//		          " Uranium: [" + materialStore.QueryInventory(PowerPlantMaterialStore.Type.Uranium) + "," + materialStore.QueryCost(PowerPlantMaterialStore.Type.Uranium)+"]");
//
//		coalShopText.text = "Coal:\n" + materialStore.QueryInventory(PowerPlantMaterialStore.Type.Coal)+ " @ $" + materialStore.QueryCost(PowerPlantMaterialStore.Type.Coal); 
//		oilShopText.text = "Oil:\n" + materialStore.QueryInventory(PowerPlantMaterialStore.Type.Oil)+ " @ $" + materialStore.QueryCost(PowerPlantMaterialStore.Type.Oil); 
//		garbageShopText.text = "Garbage:\n" + materialStore.QueryInventory(PowerPlantMaterialStore.Type.Garbage)+ " @ $"+ materialStore.QueryCost(PowerPlantMaterialStore.Type.Garbage); 
//		uraniumShopText.text = "Uranium:\n" + materialStore.QueryInventory(PowerPlantMaterialStore.Type.Uranium)+ " @ $"+ materialStore.QueryCost(PowerPlantMaterialStore.Type.Uranium); 
//		yPos += 20;

//		int count = 0;
//		foreach (PowerPlant pp in powerplantShop.inMarketPowerPlants) {
//			if(count < powerplantShop.biddableCount)
//				GUI.Label (new Rect (10, yPos, 500, 20), "Biddable: " + pp.ToString ());
//			else
//				GUI.Label (new Rect (10, yPos, 500, 20), "On Deck: " + pp.ToString ());
//			count++;
//			yPos += 20;
//		}

	}
}
