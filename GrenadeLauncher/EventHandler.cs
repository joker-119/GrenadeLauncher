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
			if (GrenadeLauncherPlugin.grenaders.Contains(ev.Player.PlayerId) && ev.Player.GetCurrentItem().ItemType == GrenadeLauncherPlugin.Handler.DefaultType)
				ev.Player.ThrowGrenade(ItemType.FRAG_GRENADE, false, Vector.Zero, false, ev.Player.GetPosition(), true, 0.2f, false);
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