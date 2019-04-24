using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventSystem;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using ItemManager;
using ItemManager.Utilities;
using UnityEngine;
using MEC;

namespace GrenadeLauncher
{
	public class EventHandler : IEventHandlerTeamRespawn, IEventHandlerWaitingForPlayers, IEventHandlerShoot
	{
		private readonly GrenadeLauncherPlugin plugin;
		public EventHandler(GrenadeLauncherPlugin plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			if (!plugin.GetConfigBool("glauncher_enabled"))
				PluginManager.Manager.DisablePlugin(plugin);
		}

		public void OnShoot(PlayerShootEvent ev)
		{
			CustomItem item = Items.FindCustomItem(ev.Player, ev.Player.GetCurrentItemIndex());

			if (item == null) return;

			if (item.Handler.PsuedoType == GrenadeLauncherPlugin.LauncherID)
				ev.Player.ThrowGrenade(ItemType.FRAG_GRENADE, false, Vector.Zero, false, ev.Player.GetPosition(), true, 0.2f, false);
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
}