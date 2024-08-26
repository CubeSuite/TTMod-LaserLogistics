using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using EquinoxsModUtils;
using EquinoxsModUtils.Additions;
using System.Collections.Generic;
using System;
using LaserLogistics.Patches;
using EquinoxsDebuggingTools;
using System.Reflection;

namespace LaserLogistics
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class LaserLogisticsPlugin : BaseUnityPlugin
    {
        internal const string MyGUID = "com.equinox.LaserLogistics";
        private const string PluginName = "LaserLogistics";
        private const string VersionString = "1.0.0";

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        // Config Entries

        internal static ConfigEntry<bool> showLasers;
        internal static ConfigEntry<float> laserCooldown;
        internal static ConfigEntry<bool> discoMode;
        internal static ConfigEntry<int> defaultRed;
        internal static ConfigEntry<int> defaultGreen;
        internal static ConfigEntry<int> defaultBlue;

        // Unity Functions

        private void Awake() {
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
            Harmony.PatchAll();

            CreateConfigEntries();
            ApplyPatches();
            LoadPrefabs();

            ModUtils.GameDefinesLoaded += OnGameDefinesLoaded;
            ModUtils.SaveStateLoaded += OnSaveStateLoaded;
            ModUtils.GameSaved += OnGameSaved;

            ContentAdder.AddHeaders();
            ContentAdder.AddUnlocks();
            ContentAdder.AddPMT();

            ContentAdder.AddLaserNode();
            
            ContentAdder.AddPullerModule();
            ContentAdder.AddPusherModule();
            ContentAdder.AddCollectorModule();
            ContentAdder.AddDistributorModule();
            ContentAdder.AddVoidModule();
            ContentAdder.AddCompressorModule();
            ContentAdder.AddExpanderModule();

            ContentAdder.AddRangeUpgrade();
            ContentAdder.AddInfiniteRangeUpgrade();
            ContentAdder.AddSpeedUpgrade();
            ContentAdder.AddStackUpgrade();

            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
            Log = Logger;
        }

        private void Update() {
            LaserNodeGUI.TrackTime(Time.deltaTime);
            PositionMemoryTablet.instance.sSincePositionAdded += Time.deltaTime;
        }

        private void LateUpdate() {
            EDT.SetPacedLogDelay(1f);
        }

        private void OnGUI() {
            if (!Images.initialised) Images.InitialiseStyles();
            if (!ModUtils.hasGameLoaded) return;
            LaserNodeGUI.Draw();
            PositionMemoryTabletGUI.Draw();
        }

        // Events

        private void OnGameDefinesLoaded(object sender, EventArgs e) {
            LaserNodeGUI.LoadResourceInfosToCache();
        }

        private void OnSaveStateLoaded(object sender, EventArgs e) {
            ResIDs.pullerModule = ModUtils.GetResourceIDByName(Names.Items.pullerModule);
            ResIDs.pusherModule = ModUtils.GetResourceIDByName(Names.Items.pusherModule);
            ResIDs.collectorModule = ModUtils.GetResourceIDByName(Names.Items.collectorModule);
            ResIDs.distributorModule = ModUtils.GetResourceIDByName(Names.Items.distributorModule);
            ResIDs.voidModule = ModUtils.GetResourceIDByName(Names.Items.voidModule);
            ResIDs.compressorModule = ModUtils.GetResourceIDByName(Names.Items.compressorModule);
            ResIDs.expanderModule = ModUtils.GetResourceIDByName(Names.Items.expanderModule);

            ResIDs.rangeUpgrade = ModUtils.GetResourceIDByName(Names.Items.rangeUpgrade);
            ResIDs.infiniteRangeUpgrade = ModUtils.GetResourceIDByName(Names.Items.infiniteRangeUpgrade);
            ResIDs.speedUpgrade = ModUtils.GetResourceIDByName(Names.Items.speedUpgrade);
            ResIDs.stackUpgrade = ModUtils.GetResourceIDByName(Names.Items.stackUpgrade);

            QuantumStorageNetwork.Load();
            PositionMemoryTablet.instance.Load();
            LaserNodeManager.Load();
        }

        private void OnGameSaved(object sender, EventArgs e) {
            QuantumStorageNetwork.Save();
            PositionMemoryTablet.instance.Save();
            LaserNodeManager.Save();
        }

        // Private Functions

        private void CreateConfigEntries() {
            showLasers = Config.Bind("General", "Show Lasers", true, new ConfigDescription("When enabled, a laser is fired between a Node and an inventory when items are transferred"));
            laserCooldown = Config.Bind("General", "Laser Cooldown", 0.1f, new ConfigDescription("The amount of seconds that must between lasers firing (only affects visuals / audio)", new AcceptableValueRange<float>(0f, 1f)));
            discoMode = Config.Bind("General", "Disco Mode", false, new ConfigDescription("When enabled, each laser is fired with a random colour"));
            defaultRed = Config.Bind("General", "Default Red", 255, new ConfigDescription("New Nodes will have their Red value set to this", new AcceptableValueRange<int>(0, 255)));
            defaultGreen = Config.Bind("General", "Default Green", 0, new ConfigDescription("New Nodes will have their Green value set to this", new AcceptableValueRange<int>(0, 255)));
            defaultBlue = Config.Bind("General", "Default Blue", 0, new ConfigDescription("New Nodes will have their Blue value set to this", new AcceptableValueRange<int>(0, 255)));
        }

        private void ApplyPatches() {
            Harmony.CreateAndPatchAll(typeof(FilterInserterUIPatch));
            Harmony.CreateAndPatchAll(typeof(InserterDefinitionPatch));
            Harmony.CreateAndPatchAll(typeof(InserterInstancePatch));
            Harmony.CreateAndPatchAll(typeof(TeamVisualsPatch));
        }

        private static void LoadPrefabs() {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssetBundle bundle = AssetBundle.LoadFromStream(assembly.GetManifestResourceStream("LaserLogistics.caspuinox"));
            foreach(string name in bundle.GetAllAssetNames()) {
                Debug.Log($"Asset Name: {name}");
            }

            LaserNode.prefab = bundle.LoadAsset<GameObject>("assets/lasercube.prefab");
        }
    }
}
