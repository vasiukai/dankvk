﻿using System;
using UnityEngine;

namespace Bolt.DanKVK
{
	public class PlayerNamePlate : Bolt.EntityBehaviour<IPlayerState>
	{
		[SerializeField]
		Vector3 offset;

		[SerializeField]
		TextMesh text;

		void NameChanged ()
		{
			text.text = state.name;
		}

		void TeamChanged ()
		{
			switch (state.team) {
			case Player.TEAM_RED:
				text.GetComponent<Renderer> ().material.color = Color.red;
				break;
			case Player.TEAM_BLUE:
				text.GetComponent<Renderer> ().material.color = Color.blue;
				break;
			}
		}

		void Update ()
		{
			text.GetComponent<Renderer> ().enabled = !entity.hasControl;

			if (!entity.hasControl) {
				try {
					transform.LookAt (PlayerCamera.instance.transform);
					transform.rotation = Quaternion.LookRotation (-transform.forward);
				} catch (Exception exn) {
					BoltLog.Exception (exn);
				}
			}
		}

		public override void Attached ()
		{
			state.AddCallback ("name", NameChanged);
			state.AddCallback ("team", TeamChanged);
		}
	}
}
