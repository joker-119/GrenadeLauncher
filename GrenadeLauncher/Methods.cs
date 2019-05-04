using System.Linq;
using System.Collections.Generic;
using Smod2.API;
using ItemManager;
using MEC;
using UnityEngine;

namespace GrenadeLauncher
{
	public class Methods
	{
		private readonly GrenadeLauncherPlugin plugin;
		public Methods(GrenadeLauncherPlugin plugin) => this.plugin = plugin;

		public IEnumerator<float> GiveLauncher(Player player)
		{
			yield return Timing.WaitForSeconds(1f);

			foreach (Smod2.API.Item item in player.GetInventory().Where(x => x.ItemType == plugin.Handler.DefaultType))
				item.Remove();

			Items.Handlers[plugin.LauncherId].Create(((GameObject)player.GetGameObject()).GetComponent<Inventory>());

			player.PersonalBroadcast(5, "You have spawned with a grenade launcher!", false);
		}
	}
}