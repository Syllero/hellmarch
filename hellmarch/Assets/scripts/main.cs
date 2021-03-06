﻿using UnityEngine;
using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using AssemblyCSharp;
using UnityEngine.UI;

public class main : MonoBehaviour {

    private static int TEAM_ASSIGNER = 0;
	List<PlayerInstance> players;

    public Dictionary<int, List<GameObject>> units = new Dictionary<int, List<GameObject>>();
    public GameObject nuke;
    public GameObject camera;
    public Vector3 defaultCamPos;
    public Quaternion defaultCamRot;

    public Texture redDude;
    public Texture blueDude;


    public static int width = 50;
    public static int height = 25;

    public static int xOffset = 25;
    public static int zOffset = 125;

    public static int winningTeam = -1;
    bool didReset = false;

    AudioSource ptf;

    private Vector3 victoryPosition = Vector3.zero;

    private Dictionary<int, List<string>> victoryEffects = new Dictionary<int, List<string>>();

	public Text countdowner;
	private float countdown = 30;
    // Use this for initialization
    void Start () {
		
		players = new List<PlayerInstance> (); 

        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += OnConnect;

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("RedTeam"), LayerMask.NameToLayer("Ignore"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("BlueTeam"), LayerMask.NameToLayer("Ignore"));

        victoryEffects.Add(0, new List<string>());
        victoryEffects.Add(1, new List<string>());

        victoryEffects[0].Add("FX_Fireworks_Blue_Large");
        victoryEffects[0].Add("FX_Fireworks_Blue_Small");

        victoryEffects[1].Add("FX_Fireworks_Yellow_Large");
        victoryEffects[1].Add("FX_Fireworks_Yelow_Small");

        redDude = Resources.Load("dude_red") as Texture;
        blueDude = Resources.Load("dude_blue") as Texture;

        units.Add(0, new List<GameObject>());
        units.Add(1, new List<GameObject>());

        nuke = Instantiate(Resources.Load("bomb")) as GameObject;
        nuke.transform.position = new Vector3(-52, 0, 0);

        camera = GameObject.FindGameObjectWithTag("MainCamera");

        ptf = camera.GetComponents<AudioSource>()[1];
        ptf.Play();

        defaultCamPos = new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z);
        defaultCamRot = new Quaternion(camera.transform.rotation.x, camera.transform.position.y, camera.transform.position.z, camera.transform.rotation.w);

        /*
        for (int i = 0; i < 10; i++)
        {
            SpawnUnit("dude", 0, 0, 8);
            SpawnUnit("suicideDude", 0, 1, 8);
            SpawnUnit("pusher", 0, 2, 8);
        }

        for (int i = 0; i < 10; i++)
        {
            SpawnUnit("dude", 1, 0, 0);
            SpawnUnit("suicideDude", 1, 1, 0);
            SpawnUnit("pusher", 1, 2, 0);
        }
        */
    }

    List<GameObject> GetTeam(int team)
    {
        return new List<GameObject>();
    }

	public void SpawnUnit(string type, int team, int row, int column, int fagget = -1)
    {
		if (countdown <= 0) {
			GameObject go = Instantiate (Resources.Load (type)) as GameObject;
			/*
		if (fagget != -1) {
			for (int i = 0; i < players.Count; i++) {
				if (players [i].AirConsoleId == fagget) {
					GameObject avatar = Resources.Load<GameObject> ("Avatar");
					avatar.GetComponents<AvatarLoading> () [0].url = players [i].user_profile_url;
					avatar = Instantiate (avatar);
					avatar.transform.SetParent (go.transform);
				}
			}
		}*/

			//Renderer renderer = go.GetComponent<Renderer>();
			var renderers = go.GetComponentsInChildren<Renderer> ();

			for (int i = 0; i < renderers.Length; i++) {
				if (renderers [i].name == "Cube Man") {
					renderers [i].material.SetTexture ("_MainTex", team == 0 ? blueDude : redDude);
				}
			}
                                                     

			Vector3 direction = team == 0 ? new Vector3 (0, 0, 1) : new Vector3 (0, 0, -1);
			go.GetComponent<unit> ().Initialize (direction, team, this);

			float spawnX = xOffset - row * width;
			float spawnZ = zOffset - column * height;

			if (team == 0) {
				go.transform.position = new Vector3 (Random.Range (spawnX, spawnX - width), 0, Random.Range (-170, -150));
			} else {
				go.transform.position = new Vector3 (Random.Range (spawnX, spawnX - width), 0, Random.Range (150, 170));
			}


			units [team].Add (go);
		}
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (countdown >= 0) {
			countdown -= Time.deltaTime;
			countdowner.text = ""+(int)countdown;
		} else {
			countdowner.text = "";
		}

        if (winningTeam == -1)
        {
            for (int i = 0; i < players.Count; i++)
            {
				if (countdown <= 0) {
					players [i].Update ();
				}
					players [i].SyncToPlayer ();
            }
        }

        LocalControls();

        if (main.winningTeam > -1)
        {
            for (int i = 0; i < 5; i++)
            {
                if (Random.Range(0, 10) == 0)
                {
                    int effect = Random.Range(0, 1);

                    GameObject ps = Instantiate(Resources.Load("SimpleFX/Prefabs/" + victoryEffects[winningTeam][effect])) as GameObject;
                    ps.transform.position = new Vector3(Random.Range(-100, 100), 5, Random.Range(-250, 250));

                    Destroy(ps, 3);

                    GameObject su = Instantiate(Resources.Load("SimpleFX/Prefabs/FX_Explosion_Rubble")) as GameObject;

                    if (winningTeam == 0)
                        su.transform.position = nuke.transform.position + new Vector3(Random.Range(-100, 100), 0, Random.Range(25, 125));
                    else
                        su.transform.position = nuke.transform.position + new Vector3(Random.Range(-100, 100), 0, -Random.Range(25, 125));

                    if (Random.Range(0, 100) < 20)
                    {
                        var explosionSounds = su.GetComponents<AudioSource>();
                        explosionSounds[Random.Range(0, explosionSounds.Length)].Play();
                    }

                    Destroy(su, 3);
                } 
				countdown = 30;
            }

            if (!didReset)
            {
                if (main.winningTeam == 0)
                {
                    GameObject winner = GameObject.Instantiate(Resources.Load("blueTeamWins")) as GameObject;
                    Destroy(winner, 3);
                }
                else
                {
                    GameObject winner = GameObject.Instantiate(Resources.Load("redTeamWins")) as GameObject;
                    Destroy(winner, 3);
                }

                didReset = true; 

                Invoke("ResetGame", 10);
            }
        }

    }

    public void ResetGame()
    { 
        foreach(var kvp in units)
        {
            kvp.Value.ForEach(x =>
            {
                Destroy(x);
            });

            kvp.Value.Clear();
        }

        
        Destroy(nuke);  

        nuke = Instantiate(Resources.Load("bomb")) as GameObject;
        nuke.transform.position = new Vector3(-52, 0, 0);

        winningTeam = -1;
        didReset = false;

        TEAM_ASSIGNER = 0;

        List<int> playerIDs = new List<int>();
        players.ForEach(x =>
        {
            playerIDs.Add(x.AirConsoleId);
        });

        players.Clear();

        for (int i = 0; i < playerIDs.Count; i++)
        {
            int temp = playerIDs[i];
            int randomIndex = Random.Range(i, playerIDs.Count);
            playerIDs[i] = playerIDs[randomIndex];
            playerIDs[randomIndex] = temp;
        }

        playerIDs.ForEach(x =>
        {
            players.Add(new PlayerInstance(x, TEAM_ASSIGNER++ % 2));
        });

        players.ForEach(x =>
        {
            x.SyncToPlayer();
        });

        ptf.Play();
    }

    private void LocalControls()
    {
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            SpawnUnit("dude", 0, 0, 8);
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            SpawnUnit("suicideDude", 0, 0, 8);
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            SpawnUnit("pusher", 0, 0, 8);
        }

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            SpawnUnit("dude", 0, 1, 8);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            SpawnUnit("suicideDude", 0, 1, 8);
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            SpawnUnit("pusher", 0, 1, 8);
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            SpawnUnit("dude", 0, 2, 8);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SpawnUnit("suicideDude", 0, 2, 8);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            SpawnUnit("pusher", 0, 2, 8);
        }




        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpawnUnit("dude", 1, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            SpawnUnit("suicideDude", 1, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnUnit("pusher", 1, 0, 0);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            SpawnUnit("dude", 1, 1, 0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SpawnUnit("suicideDude", 1, 1, 0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            SpawnUnit("pusher", 1, 1, 0);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            SpawnUnit("dude", 1, 2, 0);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            SpawnUnit("suicideDude", 1, 2, 0);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            SpawnUnit("pusher", 1, 2, 0);
        }
    }

    void OnMessage(int from, JToken data) {
		Debug.Log("Got message from " + from + " " + data.ToString ());
		for (int i = 0; i < players.Count; i++) {
			if (players [i].AirConsoleId == from) {
				players [i].ReceieveData (data);
			}
		}
    }

    void OnConnect(int device_id) {
		Debug.Log ("User connected" + device_id);
		players.Add (new PlayerInstance (device_id, TEAM_ASSIGNER++ % 2));
    }

	void OnDisconnect( int device_id ) {
		Debug.Log ("User disconnected" + device_id);

		for (int i = 0; i < players.Count; i++) {
			if (players [i].AirConsoleId == device_id) {
				players [i].Destroy ();
				players.RemoveAt (i);
				break;
			}
		}
	}
}
