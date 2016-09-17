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
		int air_console_id;
		String nickname;
		String user_profile_url;

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
			SyncToPlayer();
		}

		public void SyncToPlayer()
		{
			
			JObject data = new JObject();
			data["money"] = 1000;

			AirConsole.instance.Message (this.air_console_id, data);
		}

		public void ReceieveData(JToken data)
		{

		}
	}
}