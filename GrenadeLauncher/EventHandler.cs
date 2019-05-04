using System.Linq;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using ItemManager;
using UnityEngine;
using MEC;
using RemoteAdmin;

namespace GrenadeLauncher
{
	public class EventHandler : IEventHandlerTeamRespawn, IEventHandlerRoundStart, IEventHandlerShoot, IEventHandlerWaitingForPlayers
	{
		private readonly GrenadeLauncherPlugin plugin;
		public EventHandler(GrenadeLauncherPlugin plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{

		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			RandomItemSpawner ris = Object.FindObjectOfType<RandomItemSpawner>();

			List<SpawnLocation> spawns = new List<SpawnLocation>();

			for (int i = 0; i < plugin.WorldSpawnCount; i++)
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

					if (choices == null || choices.Count == 0)
					{
						plugin.Info("Invalid spawn location: " + sl);
						return;
					}
					
					spawns.Add(choices[plugin.Gen.Next(0, choices.Count)]);
				}

			foreach (SpawnLocation sl in spawns)
				plugin.Handler.CreateOfType(sl.Position, sl.Rotation);
		}

		public void OnShoot(PlayerShootEvent ev)
		{
			CustomItem item = ev.Player?.HeldCustomItem();
			if (item != null && item.Handler.PsuedoType == plugin.LauncherId)
			{
				GameObject player = (GameObject) ev.Player.GetGameObject();
				ThrowGrenade(ItemType.FRAG_GRENADE, false, Vector.Zero, false, ev.Player.GetPosition(), true, 0.2f, false, player);
			}
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (ev.SpawnChaos && plugin.CiSpawn)
			{
				int r = plugin.Gen.Next(ev.PlayerList.Count);

				Timing.RunCoroutine(plugin.Functions.GiveLauncher(ev.PlayerList[r]));
			}
			else if (plugin.NtfSpawn)
			{
				int r = plugin.Gen.Next(ev.PlayerList.Count);

				Timing.RunCoroutine(plugin.Functions.GiveLauncher(ev.PlayerList[r]));
			}
		}

		private static void ThrowGrenade(ItemType grenadeType, bool isCustomDirection, Vector direction, bool isEnvironmentallyTriggered, Vector position, bool isCustomForce, float throwForce, bool slowThrow, GameObject player)
		{

			if (player == null) return;
			if (player.GetComponent<GrenadeManager>() == null) return;
			
			int num = 0;
			if (grenadeType != ItemType.FRAG_GRENADE)
			{
				if (grenadeType == ItemType.FLASHBANG) num = 1;
			}
			else
				num = 0;

			GrenadeManager component = player.GetComponent<GrenadeManager>();
			Vector3 forward = player.GetComponent<Scp049PlayerScript>().plyCam.transform.forward;
			
			if (isCustomDirection) 
				forward = new Vector3(direction.x, direction.y, direction.z);
			if (!isCustomForce) 
				throwForce = (!slowThrow ? 1f : 0.5f) * component.availableGrenades[num].throwForce;
			
			Grenade component2 = UnityEngine.Object.Instantiate<GameObject>(component.availableGrenades[num].grenadeInstance).GetComponent<Grenade>();
			component2.gameObject.AddComponent<GrenadeScript>();
			GrenadeScript script = component2.GetComponent<GrenadeScript>();
			script.thrower = player;
			component2.id = player.GetComponent<QueryProcessor>().PlayerId + ":" + (component.smThrowInteger + 4096);
			GrenadeManager.grenadesOnScene.Add(component2);
			component2.SyncMovement(component.availableGrenades[num].GetStartPos(player), (player.GetComponent<Scp049PlayerScript>().plyCam.transform.forward + Vector3.up / 4f).normalized * throwForce, Quaternion.Euler(component.availableGrenades[num].startRotation), component.availableGrenades[num].angularVelocity);
			GrenadeManager grenadeManager = component;
			int id = num;
			int playerId = player.GetComponent<QueryProcessor>().PlayerId;
			GrenadeManager grenadeManager2 = component;
			int smThrowInteger = grenadeManager2.smThrowInteger;
			grenadeManager2.smThrowInteger = smThrowInteger + 1;
			grenadeManager.CallRpcThrowGrenade(id, playerId, smThrowInteger + 4096, forward, isEnvironmentallyTriggered, new Vector3(position.x, position.y, position.z), slowThrow, 0);
		}
	}

	public struct SpawnLocation
	{
		public Vector3 Position { get; private set; }
		public Quaternion Rotation { get; private set; }
		public SpawnLocation(Vector3 p, Quaternion r)
		{
			Position = p;
			Rotation = r;
		}
	}
}