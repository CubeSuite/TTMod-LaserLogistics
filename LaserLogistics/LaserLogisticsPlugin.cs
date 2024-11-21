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
using CasperEquinoxGUI;

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

            EMU.Events.SaveStateLoaded += OnSaveStateLoaded;
            EMU.Events.GameSaved += OnGameSaved;
            EMU.Events.GameUnloaded += LaserNodeManager.OnGameUnloaded;
            CaspuinoxGUI.ReadyForGUI += OnReadyForGUI;

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

            ContentAdder.AddSpeedUpgrade();
            ContentAdder.AddStackUpgrade();
            ContentAdder.AddRangeUpgrade();
            ContentAdder.AddInfiniteRangeUpgrade();

            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
            Log = Logger;
        }

        private void Update() {
            NewLaserNodeGUI.HandleKeyPresses();
            NewLaserNodeGUI.TrackTime(Time.deltaTime);
            PositionMemoryTablet.instance.sSincePositionAdded += Time.deltaTime;
            QuantumStorageNetwork.Download();
        }

        private void LateUpdate() {
            //EDT.SetPacedLogDelay(1f);
        }

        private void OnGUI() {
            if (!Images.initialised) Images.InitialiseStyles();
            if (!EMU.LoadingStates.hasGameLoaded) return;
            PositionMemoryTabletGUI.Draw();
            NewLaserNodeGUI.DrawItemInHand();
        }

        // Events

        private void OnReadyForGUI() {
            NewLaserNodeGUI.CreateWindow();
        }

        private void OnSaveStateLoaded(object sender, EventArgs e) {
            ResIDs.pullerModule = EMU.Resources.GetResourceIDByName(Names.Items.pullerModule);
            ResIDs.pusherModule = EMU.Resources.GetResourceIDByName(Names.Items.pusherModule);
            ResIDs.collectorModule = EMU.Resources.GetResourceIDByName(Names.Items.collectorModule);
            ResIDs.distributorModule = EMU.Resources.GetResourceIDByName(Names.Items.distributorModule);
            ResIDs.voidModule = EMU.Resources.GetResourceIDByName(Names.Items.voidModule);
            ResIDs.compressorModule = EMU.Resources.GetResourceIDByName(Names.Items.compressorModule);
            ResIDs.expanderModule = EMU.Resources.GetResourceIDByName(Names.Items.expanderModule);

            ResIDs.rangeUpgrade = EMU.Resources.GetResourceIDByName(Names.Items.rangeUpgrade);
            ResIDs.infiniteRangeUpgrade = EMU.Resources.GetResourceIDByName(Names.Items.infiniteRangeUpgrade);
            ResIDs.speedUpgrade = EMU.Resources.GetResourceIDByName(Names.Items.speedUpgrade);
            ResIDs.stackUpgrade = EMU.Resources.GetResourceIDByName(Names.Items.stackUpgrade);

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
            laserCooldown = Config.Bind("General", "Laser Cooldown", 0.1f, new ConfigDescription("The amount of seconds that must between lasers firing (only affects visuals / audio)", new AcceptableValueRange<float>(0f, 120f)));
            discoMode = Config.Bind("General", "Disco Mode", false, new ConfigDescription("When enabled, each laser is fired with a random colour"));
            defaultRed = Config.Bind("General", "Default Red", 255, new ConfigDescription("New Nodes will have their Red value set to this", new AcceptableValueRange<int>(0, 255)));
            defaultGreen = Config.Bind("General", "Default Green", 0, new ConfigDescription("New Nodes will have their Green value set to this", new AcceptableValueRange<int>(0, 255)));
            defaultBlue = Config.Bind("General", "Default Blue", 0, new ConfigDescription("New Nodes will have their Blue value set to this", new AcceptableValueRange<int>(0, 255)));
        }

        private void ApplyPatches() {
            Harmony.CreateAndPatchAll(typeof(FlowManagerPatch));
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
