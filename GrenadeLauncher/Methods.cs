using System.Linq;
using System.Collections.Generic;
using Smod2.API;
using ItemManager;
using MEC;
using UnityEngine;
using ServerMod2.API;
using Smod2;

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

			player.PersonalBroadcast(5, "You have spawned with a rocket launcher!", false);
		}

		public IEnumerator<float> Explode(Player player, Vector3 pos, float delay)
		{
			GameObject ply = (GameObject)player.GetGameObject();
			GrenadeManager gm = ply.GetComponent<GrenadeManager>();
			string gid = "SERVER_" + player.PlayerId + ":" + (gm.smThrowInteger + 4096);

			yield return Timing.WaitForSeconds(delay);

			gm.CallRpcThrowGrenade(0, player.PlayerId, gm.smThrowInteger++ + 4096, Vector3.zero, true, pos, false, 0);
			gm.CallRpcUpdate(gid, new Vector3(pos.x, pos.y, pos.z), Quaternion.Euler(Vector3.zero), Vector3.zero, Vector3.zero);
			gm.CallRpcExplode(gid, player.PlayerId);

			foreach (Player plyr in plugin.Server.GetPlayers())
			{
				float range = Vector3.Distance(plyr.GetPosition().ToVector3(), pos);
				if (range >= plugin.ExplosionRange + 1f) continue;
				if (CheckFriendly(player, plyr) && plyr.PlayerId != player.PlayerId && plugin.Started) continue;

				float falloff = plugin.ExplosionFalloff * range;
				float damage = plugin.ExplosionDamage - falloff;
				plugin.Info($"Max Damage: {plugin.ExplosionDamage}.  Falloff: {plugin.ExplosionFalloff}.   Range: {range}.   Calculation: {plugin.ExplosionDamage} - {plugin.ExplosionFalloff} * {range} = {damage}");
				if (damage < 0)
					damage = 0;
				if (range < 1)
					damage = plugin.ExplosionDamage;
				plyr.Damage((int)damage);
				plugin.Info($"Dealing {(int)damage} damage.");
			}
		}

		private bool CheckFriendly(Player shooter, Player target)
		{
			if (plugin.Started)
				return false;
			if (shooter.TeamRole.Team == target.TeamRole.Team) return true;

			switch (shooter.TeamRole.Team)
			{
				case Smod2.API.Team.CLASSD:
				case Smod2.API.Team.CHAOS_INSURGENCY:
					{
						return target.TeamRole.Team == Smod2.API.Team.CLASSD || target.TeamRole.Team == Smod2.API.Team.CHAOS_INSURGENCY;
					}
				case Smod2.API.Team.SCIENTIST:
				case Smod2.API.Team.NINETAILFOX:
					{
						return target.TeamRole.Team == Smod2.API.Team.SCIENTIST ||
							   target.TeamRole.Team == Smod2.API.Team.NINETAILFOX;
					}
				case Smod2.API.Team.TUTORIAL when plugin.SerpentsHand:
					{
						return target.TeamRole.Team == Smod2.API.Team.TUTORIAL ||
							   target.TeamRole.Team == Smod2.API.Team.SCP;
					}
			}
			return false;
		}
	}
}