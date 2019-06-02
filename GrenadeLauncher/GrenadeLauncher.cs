using Smod2.API;
using ServerMod2.API;
using ItemManager;

namespace GrenadeLauncher
{
	public class GrenadeLauncher : CustomWeapon
	{
		public override int MagazineCapacity => GrenadeLauncherPlugin.MagazineSize;
		public override float FireRate => GrenadeLauncherPlugin.FireRate;

		public override bool OnPickup()
		{
			Player ply = new SmodPlayer(PlayerObject);

			ply.PersonalBroadcast(5, "You have picked up a <b>Rocket Launcher</b>!", false);

			return base.OnPickup();
		}

		protected override void OnValidShoot(UnityEngine.GameObject target, ref float damage)
		{
			SmodPlayer ply;

			try { ply = new SmodPlayer(target); }
			catch { return; }

			damage = 0;
		}

	}
}