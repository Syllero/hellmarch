using System;
using UnityEngine;
using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using AssemblyCSharp;

namespace AssemblyCSharp
{
	public class PlayerInstance
	{
        static readonly float SYNC_INTERVAL = 0.5f;

        static readonly Dictionary<String, UnitInfo> UNIT_INFO = new Dictionary<string, UnitInfo> {
            { "soldier", new UnitInfo(3.0f, 700, "dude")},
            { "bomber", new UnitInfo(5.0f, 1200, "suicideDude")},
            { "pusher", new UnitInfo(1.5f, 250, "pusher")},
        };

        static readonly Dictionary<int, TeamInfo> TEAM_INFO = new Dictionary<int, TeamInfo> {
            { 0, new TeamInfo(8)},
            { 1, new TeamInfo(0)},
        };

        int air_console_id;
        int team_id;
        float last_sync = 0;
		String nickname;
		String user_profile_url;
        int money = 0;
        Dictionary<String, int> build_queue = new Dictionary<String, int>();
        List<KeyValuePair<DateTime, String>> build_list = new List<KeyValuePair<DateTime, String>>();
        List<String> garrison_list = new List<String>();

		int income_per_second = 100;

		public int AirConsoleId
		{
			get { return air_console_id; }
		}

		public PlayerInstance ( int air_console_id, int team_id )
		{
			this.air_console_id = air_console_id;
            this.team_id = team_id;
			this.nickname = AirConsole.instance.GetNickname (this.air_console_id);
			this.user_profile_url = AirConsole.instance.GetProfilePicture (this.air_console_id);
		}

		public void Update()
		{
		    float delta = Time.deltaTime;
		    this.money += (int)(income_per_second * delta);

		    float currentTime = Time.realtimeSinceStartup;
		    if(currentTime - this.last_sync >= SYNC_INTERVAL)
		    {
			this.SyncToPlayer();
			this.last_sync = currentTime;
		    }
		    this.FinishBuilds();
		    this.ProcessQueue();
		}

        public void SyncToPlayer()
        {
            JObject root = new JObject();
            JObject queue_info = new JObject();
            root["action"] = "update";
            root["money"] = this.money;
            root["garrison"] = this.garrison_list.Count;
            foreach (KeyValuePair<String, int> queue in this.build_queue)
            {
                if(queue.Value > 0)
                {
                    queue_info[queue.Key] = queue.Value;
                }
            }
            root["queue"] = queue_info;
            AirConsole.instance.Message (this.air_console_id, root);
		}

        public void FinishBuilds()
        {
            for (int i = this.build_list.Count - 1; i >= 0; i--)
            {
                if (this.build_list[i].Key < DateTime.Now)
                {
                    this.garrison_list.Add(this.build_list[i].Value);
                    this.build_list.RemoveAt(i);
                }
            }
        }

        public void ProcessQueue()
        {
            Dictionary<String, int> new_build_queue = new Dictionary<String, int>(this.build_queue);
            foreach(KeyValuePair<String, int> queue in this.build_queue)
            {
                if(queue.Value < 1)
                {
                    continue;
                }
                Boolean found = false;
                foreach(KeyValuePair<DateTime, String> item in this.build_list)
                {
                    if(item.Value == queue.Key)
                    {
                        found = true;
                        break;
                    }
                }
                if(!found)
                {
                    DateTime completionDateTime = DateTime.Now.AddSeconds(UNIT_INFO[queue.Key].build_time);
                    this.build_list.Add(new KeyValuePair<DateTime, String>(completionDateTime, queue.Key));
                    new_build_queue[queue.Key] = queue.Value - 1;
                    JObject data = new JObject();
                    data["action"] = "build";
                    data["type"] = queue.Key;
                    data["time"] = UNIT_INFO[queue.Key].build_time * 1000;
                    AirConsole.instance.Message(this.air_console_id, data);
                }
            }
            this.build_queue = new_build_queue;
        }

        public void ReceieveData(JToken data)
		{
            String action = (String)data["action"];
            if ("build" == action)
            {
                String type = (String)data["type"];
                
                if(this.money < UNIT_INFO[type].cost)
                {
                    this.SendError("Insufficient funds!");
                }
                else
                {
                    this.money -= UNIT_INFO[type].cost;
                    if (this.build_queue.ContainsKey(type))
                    {
                        this.build_queue[type] += 1;
                    }
                    else
                    {
                        this.build_queue.Add(type, 1);
                    }
                }
            }
            else if("deploy" == action)
            {
                if(this.garrison_list.Count != 0)
                {
                    main main_object = GameObject.FindGameObjectWithTag("main").GetComponent<main>();
                    int column = TEAM_INFO[this.team_id].start_column;
                    int row = (int)data["row"];
                    foreach (String type in this.garrison_list)
                    {
                        Debug.Log("Building: " + type);
                        main_object.SpawnUnit(UNIT_INFO[type].build_name, this.team_id, row, column);
                    }
                    this.garrison_list.Clear();
                }
            }
		}

        private class UnitInfo
        {
            public float build_time;
            public int cost;
            public String build_name;

            public UnitInfo(float build_time, int cost, String build_name)
            {
                this.build_time = build_time;
                this.cost = cost;
                this.build_name = build_name;
            }
        }

        private class TeamInfo
        {
            public int start_column;

            public TeamInfo(int start_column)
            {
                this.start_column = start_column;
            }
        }

        private void SendError(String errorMessage)
        {
            JObject data = new JObject();
            data["action"] = "error";
            data["message"] = errorMessage;
            AirConsole.instance.Message(this.air_console_id, data);
        }
    }
}
