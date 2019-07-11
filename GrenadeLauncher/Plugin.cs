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
		[ConfigOption] public int Shell = 24;

		[ConfigOption("ntf_spawn")] public bool NtfSpawn = false;
		[ConfigOption("ci_spawn")] public bool CiSpawn = true;

		[ConfigOption] public string[] SpawnLocations = { "173" };
		public bool Started = false;


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

			Shell = ConfigManager.Manager.Config.GetIntValue("glauncher_shell", 24);
			Handler = new CustomWeaponHandler<GrenadeLauncher>(LauncherId)
			{
				DefaultType = GetItemTypeFromId(Shell),
				AmmoName = "Launchable Grenades",
				DefaultReserveAmmo = ReserveAmmo
			};
			Handler.Register();
			Info("Shell ID: " + Shell + " Type From Shell: " + GetItemTypeFromId(Shell) + " Selected Type: " + Handler.DefaultType);
			
			Items.AddRecipe(new Id914Recipe(KnobSetting.FINE, Shell, LauncherId, 1));
		}
		
		public ItemType GetItemTypeFromId(int id)
		{
			switch (id)
			{
				case 16:
					return ItemType.MICROHID;
				case 13:
					return ItemType.COM15;
				case 20:
					return ItemType.E11_STANDARD_RIFLE;
				case 21:
					return ItemType.P90;
				case 23:
					return ItemType.MP4;
				default:
					return ItemType.LOGICER;
			}
		}
	}
}