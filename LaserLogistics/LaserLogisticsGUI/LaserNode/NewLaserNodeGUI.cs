using BepInEx;
using CasperEquinoxGUI;
using CasperEquinoxGUI.Controls;
using CasperEquinoxGUI.Layouts;
using CasperEquinoxGUI.Utilities;
using EquinoxsDebuggingTools;
using EquinoxsModUtils;
using FluffyUnderware.DevTools.Extensions;
using FMOD.Studio;
using FMODUnity;
using LaserLogistics.Controls;
using LaserLogistics.LaserLogisticsGUI.LaserNode.Panels;
using LaserLogistics.LaserLogisticsGUI.LaserNodeWindow.Panels;
using LaserLogistics.Modules;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolBuddy.ThirdParty.VectorGraphics;
using UnityEngine;
using static DestructibleConfig;

namespace LaserLogistics
{
    public static class NewLaserNodeGUI
    {
        // Components
        public static LaserNodeWindow window;
        public static Grid bottomGrid;
        public static SavedPositionsPanel savedPositionsPanel;
        public static MainPanel mainPanel;
        public static StatsColourPanel statsColourPanel;
        public static ModuleSettingsPanel moduleSettingsPanel;
        public static InventoryPanel inventoryPanel;
        public static ItemsPanel itemsPanel;
        
        // Time Tracking
        internal static float sSinceOpen;
        internal static float sSinceClose;
        internal static float sSinceGuiInteract;
        internal static float sSinceRangeError;

        // Members
        private static EventInstance audioPlayer;
        internal static LaserNode currentNode;
        internal static Module currentModule;
        internal const string titleRowHeights = "30";
        public static ResourceInfo itemInHand;
        private static GUIStyle itemInHandleStyle;
        
        // Properties
        private static PlayerInventory inventory => Player.instance.inventory;
        private static PositionMemoryTablet pmt => PositionMemoryTablet.instance;
        public static bool IsItemInHand => itemInHand != null;

        // Public Functions

        public static void CreateWindow() {
            Grid topMostGrid = new Grid(1, 2, "equal", "equal");
            Grid topGrid = new Grid(3, 1, "equal", "equal");
            bottomGrid = new Grid(2, 1, new string[]{ "500", "equal" }, "equal") { RowIndex = 1 };

            // Create Panels
            savedPositionsPanel = new SavedPositionsPanel(ref topGrid);
            CreateTopMiddlePanels(ref topGrid);
            moduleSettingsPanel = new ModuleSettingsPanel(ref topGrid);

            inventoryPanel = new InventoryPanel(ref bottomGrid);
            //CreateInventoryPanel(ref bottomGrid);
            EMU.Events.GameDefinesLoaded += OnGameDefinesLoaded; // Items Panel

            topMostGrid.AddControl(topGrid);
            topMostGrid.AddControl(bottomGrid);
            window = new LaserNodeWindow() {
                ShowTitle = false,
                ShowCloseButton = false,
                ShowShader = true,
                FreeCursor = true,
                Width = 1165,
                Height = 754,
                Visible = false,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 0, 246),
                RootLayout = topMostGrid
            };

            Window normalWindow = (Window)window;
            CaspuinoxGUI.RegisterWindow(ref normalWindow );
        }

        private static void CreateTopMiddlePanels(ref Grid parentGrid) {
            Grid topMiddle = new Grid(1, 2, "equal", "equal") {
                ColumnIndex = 1
            };

            mainPanel = new MainPanel(ref topMiddle);
            statsColourPanel = new StatsColourPanel(ref topMiddle);

            parentGrid.AddControl(topMiddle);
        }

        public static void ShowForLaserNode(uint instanceId) {
            if (sSinceClose < 0.2f) return;

            currentNode = LaserNodeManager.GetNode(instanceId);

            window.Show();
            savedPositionsPanel.Refresh();
            mainPanel.Refresh();
            statsColourPanel.Refresh();

            sSinceClose = 0f;
        }

        public static void Hide() {
            window.Hide();
            sSinceClose = 0f;

            currentNode = null;
            currentModule = null;
        }

        public static void HandleKeyPresses() {
            if (window == null || !window.Visible || sSinceOpen < 0.2f) return;
            if (UnityInput.Current.GetKey(KeyCode.Escape)) Hide();
            if (UnityInput.Current.GetKey(KeyCode.Tab)) Hide();
            if (UnityInput.Current.GetKey(KeyCode.Q)) Hide();
            if (UnityInput.Current.GetKey(KeyCode.E)) Hide();
        }

        public static void TrackTime(float dt) {
            sSinceOpen += dt;
            sSinceClose += dt;
            sSinceGuiInteract += dt;
            sSinceRangeError += dt;
        }

        public static void PlayAudio(string eventName) {
            audioPlayer.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            audioPlayer = RuntimeManager.CreateInstance(eventName);
            audioPlayer.setParameterByName("Volume", 0.2f);
            audioPlayer.start();
        }

        public static void DrawItemInHand() {
            if(itemInHandleStyle == null) itemInHandleStyle = new GUIStyle() { normal = { background = null } };
            if (!IsItemInHand) return;

            Texture2D item = EMU.Images.GetImageForResource(itemInHand.displayName);
            UnityEngine.GUI.depth = 0;
            UnityEngine.GUI.Box(new Rect(UnityInput.Current.mousePosition.x - 25, Screen.height - (UnityInput.Current.mousePosition.y + 25), 50, 50), item, itemInHandleStyle);
        }

        // Events

        private static void OnGameDefinesLoaded() {
            itemsPanel = new ItemsPanel(ref bottomGrid);
        }
    }
}
