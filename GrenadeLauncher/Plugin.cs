using Smod2;
using Smod2.API;
using Smod2.Config;
using Smod2.Events;
using Smod2.Commands;
using Smod2.Attributes;
using ItemManager;
using ItemManager.Recipes;
using ItemManager.Utilities;
using System;
using System.Collections.Generic;
using MEC;

namespace GrenadeLauncher
{
	[PluginDetails(
		author = "Joker119",
		name = "Grenade Launcher",
		description = "A gun that launches grenades.",
		id = "joker.customitems.GrenadeLauncher",
		version = "1.0.0",
		configPrefix = "glauncher",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
	)]

	public class GrenadeLauncherPlugin : Plugin
	{
		public static CustomWeaponHandler<GrenadeLauncher> Handler { get; private set; }

		public Methods Functions { get; private set; }

		[ConfigOption]
		public const int LauncherID = 107;

		[ConfigOption]
		public bool Enabled = true;

		[ConfigOption]
		public static float FireRate = 0.25f;

		[ConfigOption]
		public static int MagazineSize = 6;

		[ConfigOption]
		public static int ReserveAmmo = 0;

		[ConfigOption]
		public int WorldSpawnCount = 1;

		[ConfigOption]
		public bool NTFSpawn = false;

		[ConfigOption]
		public bool CISpawn = true;

		[ConfigOption]
		public static List<string> SpawnLocations = new List<string>() { "173" };

		public Random Gen = new System.Random();

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
			AddEventHandlers(new EventHandler(this), Priority.Normal);

			Functions = new Methods(this);

			Handler = new CustomWeaponHandler<GrenadeLauncher>(GrenadeLauncherPlugin.LauncherID)
			{
				DefaultType = ItemType.LOGICER,
				AmmoName = "Launchable Grenades"
			};
			Handler.Register();
			Items.AddRecipe(new Id914Recipe(KnobSetting.FINE, (int)ItemType.LOGICER, GrenadeLauncherPlugin.LauncherID, 1));
		}
	}
}