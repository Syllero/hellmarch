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
            { "soldier", new UnitInfo(3.0f, 600, "dude")},
            { "bomber", new UnitInfo(4.5f, 1100, "suicideDude")},
            { "pusher", new UnitInfo(2.0f, 250, "pusher")},
        };

        static readonly Dictionary<int, TeamInfo> TEAM_INFO = new Dictionary<int, TeamInfo> {
            { 0, new TeamInfo(8)},
            { 1, new TeamInfo(0)},
        };

        int air_console_id;
        int team_id;
        float last_sync = 0;
		String nickname;
		public String user_profile_url;
        int money = 1000;
        Dictionary<String, int> build_queue = new Dictionary<String, int> {
            { "soldier", 0 },
            { "bomber", 0 },
            { "pusher", 0 },
        };
        List<KeyValuePair<DateTime, String>> build_list = new List<KeyValuePair<DateTime, String>>();
        List<String> garrison_list = new List<String>();

		int income_per_second = 120;

		public int AirConsoleId
		{
			get { return air_console_id; }
		}

		static int right_count = 0;
		static int left_count = 0;

		static GameObject leftavatar;
		static GameObject rightavatar;

		static List<Vector3> old_left_positions = new List<Vector3> ();
		static List<Vector3> old_right_positions = new List<Vector3> ();

		static Dictionary<int,GameObject> avatars = new Dictionary<int,GameObject>(); 

		public PlayerInstance ( int air_console_id, int team_id )
		{
			this.air_console_id = air_console_id;
            this.team_id = team_id;
			this.nickname = AirConsole.instance.GetNickname (this.air_console_id);
			this.user_profile_url = AirConsole.instance.GetProfilePicture (this.air_console_id);

			if (!leftavatar) {
				leftavatar = GameObject.FindGameObjectWithTag ("AvatarPosLeft");
			}
			if (!rightavatar) {
				rightavatar = GameObject.FindGameObjectWithTag ("AvatarPosRight");
			}

			if (!avatars.ContainsKey (air_console_id)) {
				GameObject avatar = Resources.Load<GameObject> ("Avatar");
				avatar.GetComponents<AvatarLoading> () [0].url = user_profile_url;
				avatar = GameObject.Instantiate (avatar);

				if (team_id == 0) {
					if (old_right_positions.Count > 0) {
						avatar.transform.position = old_right_positions [old_right_positions.Count - 1];
						old_right_positions.RemoveAt (old_right_positions.Count - 1);
					} else {
						avatar.transform.position = rightavatar.transform.position + new Vector3 (-15 * right_count++, 0, 0);
					}
				} else {
					if (old_left_positions.Count > 0) {
						avatar.transform.position = old_left_positions [old_left_positions.Count - 1];
						old_left_positions.RemoveAt (old_left_positions.Count - 1);
					} else {
						avatar.transform.position = leftavatar.transform.position + new Vector3 (-15 * left_count++, 0, 0);
					}
				}
					
				avatars.Add (air_console_id, avatar);
			}
			//avatar.transform.SetParent (go.transform);
		}
	
		public void Destroy()
		{
			if (team_id == 0) {
				old_right_positions.Add (avatars [air_console_id].transform.position);
			} else {
				old_left_positions.Add (avatars [air_console_id].transform.position);
			}
					
			GameObject.Destroy(avatars[air_console_id]);
			avatars.Remove (air_console_id);
		}

		public void Update()
		{
		    float delta = Time.deltaTime;
		    this.money += (int)(income_per_second * delta);

		    this.FinishBuilds();
		    this.ProcessQueue();
		}

        public void SyncToPlayer()
        {
			float currentTime = Time.realtimeSinceStartup;
			if (currentTime - this.last_sync >= SYNC_INTERVAL) {
				this.last_sync = currentTime;
				JObject root = new JObject ();
				JObject queue_info = new JObject ();
				root ["action"] = "update";
				root ["team"] = this.team_id;
				root ["money"] = this.money;
				root ["garrison"] = this.garrison_list.Count;
				foreach (KeyValuePair<String, int> queue in this.build_queue) {
					queue_info [queue.Key] = queue.Value;
				}
				root ["queue"] = queue_info;
				AirConsole.instance.Message (this.air_console_id, root);
			}
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
						bool first = true;
						if (first) {
							main_object.SpawnUnit (UNIT_INFO [type].build_name, this.team_id, row, column, this.air_console_id);
							first = false;
						} else {
							main_object.SpawnUnit (UNIT_INFO [type].build_name, this.team_id, row, column);
						}
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
