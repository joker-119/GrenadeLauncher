using System.Linq;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using ItemManager;
using UnityEngine;
using MEC;
using ServerMod2.API;
using Smod2;


namespace GrenadeLauncher
{
	public class EventHandler : IEventHandlerTeamRespawn, IEventHandlerRoundStart, IEventHandlerShoot,
		IEventHandlerWaitingForPlayers, IEventHandlerRoundEnd
	{
		private readonly GrenadeLauncherPlugin plugin;
		public EventHandler(GrenadeLauncherPlugin plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			if (!plugin.Enabled)
				PluginManager.Manager.DisablePlugin(plugin);
			plugin.Started = true;
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			plugin.Started = true;
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
							choices = ris.posIds.Where(x => x.posID == "049_Medkit")
								.Select(x => new SpawnLocation(x.position.position, x.position.rotation)).ToList();
							break;
						case "173":
							choices = ris.posIds
								.Where(x => x.posID == "RandomPistol" &&
								            x.position.parent.gameObject.name == "Root_173").Select(x =>
									new SpawnLocation(x.position.position, x.position.rotation)).ToList();
							break;
						case "096":
							choices = ris.posIds.Where(x => x.posID == "Fireman")
								.Select(x => new SpawnLocation(x.position.position, x.position.rotation)).ToList();
							break;
						case "nuke":
							choices = ris.posIds.Where(x => x.posID == "Nuke").Select(x =>
								new SpawnLocation(x.position.position, x.position.rotation)).ToList();
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
			if (item == null || item.Handler.PsuedoType != plugin.LauncherId) return;

			GameObject player = (GameObject) ev.Player.GetGameObject();
			Vector3 forward = player.GetComponent<Scp049PlayerScript>().plyCam.transform.forward;

			Vector3 position = player.transform.position;
			Timing.RunCoroutine(Physics.Raycast(position, forward, out RaycastHit raycast, plugin.MaxRange, kWallMask)
				? plugin.Functions.Explode(ev.Player, position + forward * raycast.distance, plugin.Delay * raycast.distance)
				: plugin.Functions.Explode(ev.Player, position + forward * plugin.MaxRange, plugin.Delay * plugin.MaxRange));

			WeaponManager wep = player.GetComponent<WeaponManager>();

			int shots = plugin.Krakatoa;

			for (int i = 0; i < shots; i++)
				wep.CallRpcConfirmShot(true, wep.curWeapon);
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			plugin.Started = false;
		}

		private const int kWallMask = 1 << 30 | // Lockers
		                              1 << 27 | // Door
		                              1 << 14 | // Glass
		                              1 << 9 | // Pickups
		                              1 << 0; // Default

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