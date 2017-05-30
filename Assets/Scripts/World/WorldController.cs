using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class WorldController : MonoBehaviour {

	private GameObject globalObject;
	private GameState gs;
	private Graph graph;
	private Rect logoutConfirmRect;
	private int confirmWidth = 300;
	private int confirmHeight = 200;
	private Dictionary<int, int> results = new Dictionary<int, int>();
	public static bool speciesLocCurrent = false;
	private Texture2D bgTexture;
	private bool confirmPopUp = false;
	private Dictionary<int, int> speciesList = new Dictionary<int, int> ();
	private List<int> keyList;




  void Awake() {
    try {
      Game.networkManager.Send(WorldProtocol.Prepare(), ProcessWorld);
    } catch (Exception) {
    }

    globalObject = GameObject.Find ("Global Object");
	gs = globalObject.GetComponent<GameState> ();
	logoutConfirmRect = new Rect ((Screen.width - confirmWidth) / 2, 
			(Screen.height - confirmHeight) / 2, confirmWidth, confirmHeight);
	bgTexture = Resources.Load<Texture2D> (Constants.THEME_PATH + Constants.ACTIVE_THEME + "/gui_bg");
	confirmPopUp = false;
  }
  
  // Use this for initialization
  void Start() {
    Game.StartEnterTransition ();
    if (GameState.world != null) {
      LoadComponents();
	  GameState.UpdateSpDisplay ();
    }
  }

  // Update is called once per frame
  void Update() {
  }

  void OnGUI() {
	if (!speciesLocCurrent) {
	  UpdateSpeciesLoc ();
	}
	
	/*
	if (GUI.Button (new Rect (200, Screen.height - 145f, 80, 30), "FoodWeb")) {
		short action = 2;
		Game.networkManager.Send (SpeciesActionProtocol.Prepare (action), ProcessSpeciesAction2);		
	}
	*/
			
	if (GUI.Button (new Rect (200, Screen.height - 45f, 80, 30), "Logout")) {
	  confirmPopUp = true;
	}

	if (confirmPopUp) {
	  GUI.Window (Constants.CONFIRM_LOGOUT, logoutConfirmRect, MakeConfirmDeleteWindow, "confirm logout", GUIStyle.none);
	}
  }
  

	void MakeConfirmDeleteWindow(int id) {
		Functions.DrawBackground(new Rect(0, 0, confirmWidth, confirmHeight), bgTexture);
		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.alignment = TextAnchor.UpperCenter;
		style.fontSize = 16;

		GUI.Label(new Rect((confirmWidth - 200)/2, 50, 200, 30), "Confirm Logout", style);

		if (GUI.Button (new Rect (40, confirmHeight - 70, 60, 30), "YES")) {
			Game.networkManager.Send (LogoutProtocol.Prepare ((short) 1), ProcessLogout);
			Debug.Log ("sent logout message");
		}

		if (GUI.Button (new Rect (confirmWidth - 100, confirmHeight - 70, 60, 30), "NO")) {
			confirmPopUp = false;
		}			
	}


	public void ProcessLogout (NetworkResponse response)
	{
		ResponseLogout args = response as ResponseLogout;
		Debug.Log ("inside process logout");
		Debug.Log ("Logout details: type, status, playerId: " + args.type + " " + args.status + " " + args.playerId);
		if (args.status == 0) {
			Debug.Log ("logout successful");
		} else {
			Debug.Log ("login failed, server message = " + args.status);
		}

		Game.SwitchScene("Login");
	}


	public void ProcessSpeciesAction2 (NetworkResponse response)
	{
		ResponseSpeciesAction args = response as ResponseSpeciesAction;
		short action = args.action;
		short status = args.status;
		if ((action != 2) || (status != 0)) {
			Debug.Log ("ResponseSpeciesAction2 unexpected result");
			Debug.Log ("action, status = " + action + " " + status);
		}
		speciesList = args.speciesList;
		Debug.Log ("WorldController, ProcessSpeciesAction2, size = " + speciesList.Count);
		if (speciesList.Count == 0) {
			return;
		}

		keyList = new List<int>(speciesList.Keys);
		string spStr = "" + keyList [0];
		keyList.Sort();
		for (int i = 1; i < keyList.Count; i++) {
			spStr += " " + keyList [i];
		}
		action = 8;
		Debug.Log ("WorldController, spStr = :" + spStr + ":");
		Game.networkManager.Send (SpeciesActionProtocol.Prepare (action, spStr));
	}
		


  public void ProcessWorld(NetworkResponse response) {
    ResponseWorld args = response as ResponseWorld;

    if (args.status == 0) {
      GameState.world = args.world;

      SwitchToTileSelect(1);

      LoadComponents();

	  // This is to run simulation of the user's ecosystem upon login
	  // Debug.Log("WorldController: Send PredictionProtocol");
	  // Game.networkManager.Send (PredictionProtocol.Prepare (), ProcessPrediction);
    }
  }

  void LoadComponents() {
    GameObject gObject = GameObject.Find("Global Object");
    
    if (gObject != null) {
      if (gObject.GetComponent<EcosystemScore>() == null) {
        gObject.AddComponent<EcosystemScore>();
      }
      
      if (gObject.GetComponent<GameResources>() == null) {
        gObject.AddComponent<GameResources>();
      }
      
      if (gObject.GetComponent<Clock>() == null) {
        gObject.AddComponent<Clock>();
      }
      
      if (gObject.GetComponent<Chat>() == null) {
        gObject.AddComponent<Chat>();
      }
      
      if (GameObject.Find("Cube").GetComponent<Shop>() == null) {
        GameObject.Find("Cube").AddComponent<Shop>();
      }
	
	  if (GameObject.Find("Cube").GetComponent<Graph>() == null) {
		GameObject.Find("Cube").AddComponent<Graph>();
	  }
	  graph = GameObject.Find("Cube").GetComponent<Graph>();
    }
  }
  
  public void SwitchToTileSelect(int numTilesOwned) {
    //If player owns no tiles, they will need to pick a new home tile
    if (numTilesOwned == 0) {
      GameObject.Find("MapCamera").GetComponent<MapCamera>().FirstTileProcess(true);
      gameObject.AddComponent<TileSelectGUI>();
    }
  }


	public void ProcessPrediction(NetworkResponse response) {
		ResponsePrediction args = response as ResponsePrediction;
		// Debug.Log("WorldController, ProcessPrediction: status = " + args.status);
		if (args.status == 0) {
			Dictionary<int, Species> speciesList = gs.speciesListSave;
			results = args.results;
			foreach (KeyValuePair<int, int> entry in results) {
				// Debug.Log("WorldController, ProcessPrediction: k/v:" + entry.Key + " " + entry.Value);
				if (speciesList.ContainsKey (entry.Key)) {
					speciesList [entry.Key].biomass += entry.Value;
					// Debug.Log("WorldController, ProcessPrediction: new value:" + speciesList [entry.Key].biomass);
				} else {
					// Debug.Log("WorldController, ProcessPrediction: Could not find key:" + entry.Key);
				}
			}
		}
	}
		
	void UpdateSpeciesLoc() {
		Species.xIdx = 0;
		Species.zIdx = 0;
		int player_id = GameState.player.GetID ();
		GameObject zoneObject = GameObject.Find ("Map").GetComponent<Map> ().FindPlayerOwnedTile (player_id);
		if (zoneObject == null) {
			speciesLocCurrent = true;
			return;
		}
		Zone zone = zoneObject.GetComponent<Zone> ();
		float baseX = (zone.column - 20) * 13.85f + (zone.row % 2 == 0 ? 7 : 0) - 1;
		float baseZ = (zone.row - 19) * -11.95f + 3.5f;
		// Debug.Log ("WorldController: zone.row, zone.column: " + zone.row + " " + zone.column);
		GameObject gO = GameObject.Find ("Global Object");
		// if (gO == null) {
		//	Debug.Log ("WorldController: gO is null");
		// }
		Dictionary<int, Species> sL = gO.GetComponent<GameState> ().speciesListSave;
		// if (sL == null) {
		//	Debug.Log ("WorldController: sL is null");
		// }
		// Debug.Log ("WorldController: sL count = " + sL.Count);
		foreach (KeyValuePair<int, Species> entry in sL) {
			Species species = entry.Value;
			// Debug.Log ("WorldController: species assignment");
			// if (species == null) {
			// 	Debug.Log ("WorldController: species == null");
			// }

			List<GameObject> organisms = new List<GameObject> ();
			bool organismCreated = false;
			foreach (GameObject organism in species.speciesList) {
				// Debug.Log ("WorldController: organism assignment");
				// if (organism == null) {
				//	Debug.Log ("WorldController: organism == null");
				// }
				organisms.Add (organism);
				/*
				organism.transform.position = 
						new Vector3 (baseX + Species.xIdx * Species.step, 0, baseZ + Species.zIdx * Species.step);
				Species.UpdateIdx ();
				*/
			}
			for (int i = 0; i < organisms.Count; i++) {
				if (!organismCreated) {
					if (organisms[i] == null) {
						// Debug.Log ("WorldController: organisms, organism == null");
						int tX = Species.xIdx;
						int tZ = Species.zIdx;
						Destroy (organisms [i]);
						organisms[i] = species.CreateAnimal ();
						organismCreated = true;
						Species.xIdx = tX;
						Species.zIdx = tZ;
					}
					organisms[i].transform.position = 
						new Vector3 (baseX + Species.xIdx * Species.step, 0, baseZ + Species.zIdx * Species.step);
					Species.UpdateIdx ();
				}
			}
		}
		speciesLocCurrent = true;
	}
}
