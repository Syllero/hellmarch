﻿using UnityEngine;
using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using AssemblyCSharp;

public class main : MonoBehaviour {

	List<PlayerInstance> players;

    public Dictionary<int, List<GameObject>> units = new Dictionary<int, List<GameObject>>();

	// Use this for initialization
	void Start () {
		players = new List<PlayerInstance> ();
        Debug.Log("asofpasfpoasfusa");
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += OnConnect;

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("RedTeam"), LayerMask.NameToLayer("Ignore"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("BlueTeam"), LayerMask.NameToLayer("Ignore"));

        units.Add(0, new List<GameObject>());
        units.Add(1, new List<GameObject>());

        for (int i = 0; i < 10; i++)
        {
            GameObject su = Instantiate(Resources.Load("dude")) as GameObject;
            su.transform.Translate(-50 + i * 10, 0, -100);
            su.GetComponent<shooterUnit>().Initialize(new Vector3(0,0,1), 0, this);
            units[0].Add(su);
        }

        for (int i = 0; i < 10; i++)
        {
            GameObject su = Instantiate(Resources.Load("dude")) as GameObject;
            su.transform.Translate(-50 + i * 12, 0, 100);
            su.GetComponent<shooterUnit>().Initialize(new Vector3(0, 0, -1), 1, this);
            units[1].Add(su);
        }
    }

    List<GameObject> GetTeam(int team)
    {
        return new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < players.Count; i++) {
			players [i].Update ();
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
		players.Add (new PlayerInstance (device_id));
    }

	void OnDisconnect( int device_id ) {
		Debug.Log ("User disconnected" + device_id);

		for (int i = 0; i < players.Count; i++) {
			if (players [i].AirConsoleId == device_id) {
				players.RemoveAt (i);
				break;
			}
		}
	}
}
