using Smod2;
using Smod2.API;
using Smod2.Config;
using Smod2.Attributes;
using ItemManager;
using ItemManager.Recipes;
using ItemManager.Utilities;
using System;

namespace GrenadeLauncher
{
	[PluginDetails(
		author = "Joker119",
		name = "Grenade Launcher",
		description = "A gun that launches grenades.",
		id = "joker.customitems.GrenadeLauncher",
		version = "1.0.5",
		configPrefix = "glauncher",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
	)]

	public class GrenadeLauncherPlugin : Plugin
	{
		public CustomWeaponHandler<GrenadeLauncher> Handler { get; private set; }

		public Methods Functions { get; private set; }

		[ConfigOption] public readonly int LauncherId = 107;

		[ConfigOption] public bool Enabled = true;
		[ConfigOption] public bool SerpentsHand = false;

		[ConfigOption] public float MaxRange = 35f;
		[ConfigOption] public float Delay = 0.025f;
		[ConfigOption] public float ExplosionRange = 10f;
		[ConfigOption] public float ExplosionFalloff = 25f;
		[ConfigOption] public float ExplosionDamage = 300f;

		[ConfigOption] public static float FireRate = 0.25f;
		[ConfigOption] public static int MagazineSize = 6;
		[ConfigOption] public static int ReserveAmmo = 0;

		[ConfigOption] public int Krakatoa = 1;
		[ConfigOption] public int WorldSpawnCount = 1;

		[ConfigOption("ntf_spawn")] public bool NtfSpawn = false;
		[ConfigOption("ci_spawn")] public bool CiSpawn = true;

		[ConfigOption] public string[] SpawnLocations = { "173" };
		public bool started = false;


		public Random Gen = new Random();

		public override void OnDisable()
		{
			Info("Grenade Launcher disabled.");
		}

		public override void OnEnable()
		{
			Info("Grenade Launcher enabled. hehe xd");
		}

		public override void Register()
		{
			AddEventHandlers(new EventHandler(this));

			Functions = new Methods(this);

			Handler = new CustomWeaponHandler<GrenadeLauncher>(LauncherId)
			{
				DefaultType = ItemType.LOGICER,
				AmmoName = "Launchable Grenades",
				DefaultReserveAmmo = ReserveAmmo
			};
			Handler.Register();
			Items.AddRecipe(new Id914Recipe(KnobSetting.FINE, (int)ItemType.LOGICER, LauncherId, 1));
		}
	}
}