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
            { "soldier", new UnitInfo(5.0f, 1000)},
            { "bomber", new UnitInfo(7.5f, 1500)},
        };
       
        int air_console_id;
        float last_sync = 0;
		String nickname;
		String user_profile_url;
        int money = 0;
        Dictionary<String, DateTime> cooldowns = new Dictionary<string, DateTime>();
        List<KeyValuePair<DateTime, String>> build_list = new List<KeyValuePair<DateTime, String>>();
        List<String> garrison_list = new List<String>();

		int income_per_second = 100;

		public int AirConsoleId
		{
			get { return air_console_id; }
		}

		public PlayerInstance ( int air_console_id )
		{
			this.air_console_id = air_console_id;
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
		    this.ClearCooldowns();
		}

        public void SyncToPlayer()
        {
            JObject data = new JObject();
		    data["action"] = "update";
		    data["money"] = this.money;
		    data["garrison"] = this.garrison_list.Count;
		    AirConsole.instance.Message (this.air_console_id, data);
		}

        public void FinishBuilds()
        {
            for(int i=this.build_list.Count-1; i>=0; i--)
            {
                if(this.build_list[i].Key < DateTime.Now)
                {
                    this.garrison_list.Add(this.build_list[i].Value);
                    this.build_list.RemoveAt(i);
                }
            }
        }

        public void ClearCooldowns()
        {
            foreach(KeyValuePair<String, DateTime> cooldown in this.cooldowns)
            {
                if(cooldown.Value < DateTime.Now)
                {
                    cooldowns.Remove(cooldown.Key);
                    this.ClearCooldowns();
                    break;
                }
            }
        }

        public void ReceieveData(JToken data)
		{
            String action = (String)data["action"];
            if ("build" == action)
            {
                String type = (String)data["type"];
                if (this.cooldowns.ContainsKey(type))
                {
                    this.SendError("Already in construction!");
                }
                else if(this.money < UNIT_INFO[type].cost)
                {
                    this.SendError("Insufficient funds!");
                } else { 
                    DateTime completionDateTime = DateTime.Now.AddSeconds(UNIT_INFO[type].build_time);
                    this.build_list.Add(new KeyValuePair<DateTime, String>(completionDateTime, type));
                    this.cooldowns.Add(type, completionDateTime);
                    this.money -= UNIT_INFO[type].cost;
                }
            }
            else if("deploy" == action)
            {
                if(this.garrison_list.Count != 0)
                {
                    foreach(String type in this.garrison_list)
                    {
                        Debug.Log("Building: " + type);
                        //Call constructors
                    }
                    this.garrison_list.Clear();
                }
            }
		}

        private class UnitInfo
        {
            public float build_time;
            public int cost;

            public UnitInfo(float build_time, int cost)
            {
                this.build_time = build_time;
                this.cost = cost;
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
