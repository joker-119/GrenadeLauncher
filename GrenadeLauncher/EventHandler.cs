using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventSystem;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using ItemManager;
using ItemManager.Utilities;
using UnityEngine;
using MEC;
using RemoteAdmin;

namespace GrenadeLauncher
{
	public class EventHandler : IEventHandlerTeamRespawn, IEventHandlerRoundStart, IEventHandlerShoot, IEventHandlerWaitingForPlayers, IEventHandlerPlayerDie
	{
		private readonly GrenadeLauncherPlugin plugin;
		public EventHandler(GrenadeLauncherPlugin plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{

		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			RandomItemSpawner ris = UnityEngine.Object.FindObjectOfType<RandomItemSpawner>();

			List<SpawnLocation> spawns = new List<SpawnLocation>();

			for (int i = 0; i < plugin.WorldSpawnCount; i++)
			{
				foreach (string raw in plugin.SpawnLocations)
				{
					string sl = raw.ToLower();
					plugin.Info("Spawning Grenade Launcher at " + sl);
					List<SpawnLocation> choices = null;

					switch (sl)
					{
						case "049":
							choices = ris.posIds.Where(x => x.posID == "049_Medkit").Select(x => new SpawnLocation(x.position.position, x.position.rotation)).ToList();
							break;
						case "173":
							choices = ris.posIds.Where(x => x.posID == "RandomPistol" && x.position.parent.gameObject.name == "Root_173").Select(x => new SpawnLocation(x.position.position, x.position.rotation)).ToList();
							break;
						case "096":
							choices = ris.posIds.Where(x => x.posID == "Fireman").Select(x => new SpawnLocation(x.position.position, x.position.rotation)).ToList();
							break;
						case "nuke":
							choices = ris.posIds.Where(x => x.posID == "Nuke").Select(x => new SpawnLocation(x.position.position, x.position.rotation)).ToList();
							break;
						default:
							plugin.Info("Invalid spawn location.");
							break;
					}
					spawns.Add(choices[plugin.Gen.Next(0, choices.Count)]);
				}
			}

			foreach (SpawnLocation sl in spawns)
				GrenadeLauncherPlugin.Handler.CreateOfType(sl.Position, sl.Rotation);
		}

		public void OnShoot(PlayerShootEvent ev)
		{
			var player = (GameObject) ev.Player.GetGameObject();
			if (GrenadeLauncherPlugin.grenaders.Contains(ev.Player.PlayerId) && ev.Player.GetCurrentItem().ItemType == GrenadeLauncherPlugin.Handler.DefaultType)
				ThrowGrenade(ItemType.FRAG_GRENADE, false, Vector.Up, false, player.transform.position, false, 0.0f, player, false);
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (GrenadeLauncherPlugin.grenaders.Contains(ev.Player.PlayerId))
				GrenadeLauncherPlugin.grenaders.Remove(ev.Player.PlayerId);
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (ev.SpawnChaos && plugin.CISpawn)
			{
				int r = plugin.Gen.Next(ev.PlayerList.Count);

				plugin.Functions.GiveLauncher(ev.PlayerList[r]);
			}
			else if (plugin.NTFSpawn)
			{
				int r = plugin.Gen.Next(ev.PlayerList.Count);

				plugin.Functions.GiveLauncher(ev.PlayerList[r]);
			}
		}
			public void ThrowGrenade(ItemType grenadeType, bool isCustomDirection, Vector direction, bool isEnvironmentallyTriggered, Vector3 position, bool isCustomForce, float throwForce, GameObject player, bool slowThrow = false)
		{
			int num = 0;
			GameObject truePly = player;
			GrenadeManager component = player.GetComponent<GrenadeManager>();
			Vector3 forward = truePly.GetComponent<Scp049PlayerScript>().plyCam.transform.forward;
			Grenade component2 = UnityEngine.Object.Instantiate<GameObject>(component.availableGrenades[num].grenadeInstance).GetComponent<Grenade>();
			component2.gameObject.AddComponent<GrenadeScript>();
			GrenadeScript script = component2.GetComponent<GrenadeScript>();
			script.thrower = player;
			component2.id = truePly.GetComponent<QueryProcessor>().PlayerId + ":" + (component.smThrowInteger + 4096);
			GrenadeManager.grenadesOnScene.Add(component2);
			component2.SyncMovement(component.availableGrenades[num].GetStartPos(truePly), (truePly.GetComponent<Scp049PlayerScript>().plyCam.transform.forward + Vector3.up / 4f).normalized * throwForce, Quaternion.Euler(component.availableGrenades[num].startRotation), component.availableGrenades[num].angularVelocity);
			GrenadeManager grenadeManager = component;
			int id = num;
			int playerId = truePly.GetComponent<QueryProcessor>().PlayerId;
			GrenadeManager grenadeManager2 = component;
			int smThrowInteger = grenadeManager2.smThrowInteger;
			grenadeManager2.smThrowInteger = smThrowInteger + 1;
			grenadeManager.CallRpcThrowGrenade(id, playerId, smThrowInteger + 4096, forward, false, position, slowThrow, 0);
			component2.SyncMovement(component.availableGrenades[num].GetStartPos(truePly), (truePly.GetComponent<Scp049PlayerScript>().plyCam.transform.forward + Vector3.up / 4f).normalized * throwForce, Quaternion.Euler(component.availableGrenades[num].startRotation), component.availableGrenades[num].angularVelocity);
			component.CallRpcUpdate(component2.id, component.availableGrenades[num].GetStartPos(truePly) + Vector3.up * 2.8380203f, Quaternion.Euler(component.availableGrenades[num].startRotation), (truePly.GetComponent<Scp049PlayerScript>().plyCam.transform.forward + Vector3.up / 4f).normalized * throwForce * forward.magnitude, component.availableGrenades[num].angularVelocity);
		}
	}

	public struct SpawnLocation
	{
		public Vector3 Position { get; private set; }
		public Quaternion Rotation { get; private set; }
		public SpawnLocation(Vector3 p, Quaternion r)
		{
			this.Position = p;
			this.Rotation = r;
		}
	}
}