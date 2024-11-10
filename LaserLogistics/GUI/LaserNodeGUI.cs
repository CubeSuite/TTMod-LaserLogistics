using BepInEx;
using EquinoxsDebuggingTools;
using EquinoxsModUtils;
using FMODUnity;
using FMOD.Studio;
using Gamekit3D;
using Rewired.Demos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RayFire.RayfireBomb;
using LaserLogistics.Modules;

namespace LaserLogistics
{
    internal static class LaserNodeGUI
    {
        // Objects & Variables
        internal static float sSinceOpen;
        internal static float sSinceGuiInteract;
        internal static float sSinceClose;
        internal static float sSinceRangeError;

        private static bool shouldShow;
        private static string itemInHand;
        private static Dictionary<string, List<ResourceInfo>> inventoryCache = new Dictionary<string, List<ResourceInfo>>();

        private static PlayerInventory inventory => Player.instance.inventory;
        private static PositionMemoryTablet pmt => PositionMemoryTablet.instance;
        
        private static LaserNode currentNode;
        internal static Module currentModule;
        private static EventInstance audioPlayer;

        private static float guiStartX;
        private static float guiStartY;
        private static Vector2 targetsScrollPos;
        private static Vector2 inventoryScrollPos;
        private static string redString;
        private static string greenString;
        private static string blueString;

        private static GUIStyle countLabelStyle;
        private static GUIStyle statLabelStyle;
        private static GUIStyle labelStyle;
        private static GUIStyle redStatLabelStyle;
        private static GUIStyle buttonTextStyle;

        // Internal Functions

        internal static void Draw() {
            if (!shouldShow) return;
            if (CheckForExitKeys()) return;

            if(countLabelStyle == null) InitialiseStyles();

            DrawTablet();
            DrawConfigButtons();
            DrawUpgrades();
            DrawModules();
            DrawBuffer();
            DrawStats();
            DrawColourSettings();
            DrawInventory();
            DrawItemInHand();
            DrawPMTEntries();

            if(currentModule != null) {
                DrawModuleTargets();
                DrawFilterModeButton();
                DrawModuleFilters();
            }
            else {
                Images.LaserNodeGUI.configOverlay.Draw(guiStartX + 1387, guiStartY + 169);
            }
        }

        internal static void Open(uint instanceId) {
            if (sSinceClose < 0.2f) return;

            currentNode = LaserNodeManager.GetNode(instanceId);

            inventoryScrollPos = Vector2.zero;
            redString = currentNode.red.ToString();
            greenString = currentNode.green.ToString();
            blueString = currentNode.blue.ToString(); 
            shouldShow = true;
            EMU.FreeCursor(true);
            sSinceOpen = 0;

            PlayAudio("event:/SFX/Machine SFX/Machine Open");

            InserterMachineList list = (InserterMachineList)MachineManager.instance.GetMachineList(MachineTypeEnum.Inserter);
            Debug.Log($"list.visualsPools.count: {list.visualsList.Length}");
            foreach(StreamedMachineData<InserterInstance, InserterDefinition> data in list.visualsList) {
                data.ToggleOnOff(false);
            }
        }

        internal static void Close() {
            if (sSinceOpen < 0.2f) return;

            shouldShow = false;
            EMU.FreeCursor(false);
            sSinceClose = 0;

            currentNode = null;
            currentModule = null;

            if(!string.IsNullOrEmpty(itemInHand)) {
                inventory.AddResources(EMU.Resources.GetResourceIDByName(itemInHand), 1);
                itemInHand = "";
            }

            PlayAudio("event:/SFX/Machine SFX/Machine Close");
        }

        internal static void LoadResourceInfosToCache() {
            foreach (SchematicsHeader header in GameDefines.instance.schematicsHeaderEntries) {
                List<ResourceInfo> resources = GameDefines.instance.resources.Where(resource => resource.headerType.filterTag == header).ToList();
                inventoryCache.Add($"{header.title}", resources);
            }
        }

        internal static void TrackTime(float dt) {
            sSinceOpen += dt;
            sSinceClose += dt;
            sSinceGuiInteract += dt;
            sSinceRangeError += dt;
        }

        // Draw Functions

        private static void DrawTablet() {
            guiStartX = (Screen.width - Images.LaserNodeGUI.background.width) / 2f;
            guiStartY = Screen.height - 1157;

            Images.LaserNodeGUI.shaderTile.Draw(0, 0, Screen.width, Screen.height);
            Images.LaserNodeGUI.background.Draw(guiStartX, guiStartY);
        }

        private static void DrawConfigButtons() {
            for(int i = 0; i < 8; i++) {
                float xPos = guiStartX + (i * 45) + 1016;
                float yPos = guiStartY + 334;
                if(Images.LaserNodeGUI.configButton.DrawAsButton(xPos, yPos)) {
                    HandleConfigButtonClicked(i);
                }

                if (currentNode.IsModuleInSlot(i)) continue;
                Images.LaserNodeGUI.shader.Draw(xPos, yPos, 30, 30);
            }
        }

        private static void DrawUpgrades() {
            float yPos = guiStartY + 204;

            if(currentNode.numSpeedUpgrades != 0) {
                if(Images.Upgrades.speedUpgrade.DrawAsButton(guiStartX + 1196, yPos, 30, 30) && sSinceGuiInteract > 0.2f) {
                    sSinceGuiInteract = 0f;
                    PlayAudio("event:/SFX/UI SFX/Building UI SFX/Build Error");
                    if (UnityInput.Current.GetKey(KeyCode.LeftShift)) {
                        int count = Event.current.button == 1 ? 5 : 1;
                        currentNode.RemoveUpgrade(Names.Items.speedUpgrade, out int removed, count);
                        inventory.AddResources(ResIDs.speedUpgrade, removed);
                    }
                    else {
                        currentNode.RemoveUpgrade(Names.Items.speedUpgrade, out _, 1);
                        itemInHand = Names.Items.speedUpgrade;
                    }
                }
                GUI.Label(Images.Upgrades.speedUpgrade.rect, currentNode.numSpeedUpgrades.ToString(), countLabelStyle);
            }

            if(currentNode.numStackUpgrades != 0) {
                if (Images.Upgrades.stackUpgrade.DrawAsButton(guiStartX + 1241, yPos, 30, 30) && sSinceGuiInteract > 0.2f) {
                    sSinceGuiInteract = 0f;
                    PlayAudio("event:/SFX/UI SFX/Building UI SFX/Build Error");
                    if (UnityInput.Current.GetKey(KeyCode.LeftShift)) {
                        int count = Event.current.button == 1 ? 5 : 1;
                        currentNode.RemoveUpgrade(Names.Items.stackUpgrade, out int removed, count);
                        inventory.AddResources(ResIDs.stackUpgrade, removed);
                    }
                    else {
                        currentNode.RemoveUpgrade(Names.Items.stackUpgrade, out _, 1);
                        itemInHand = Names.Items.stackUpgrade;
                    }
                }
                GUI.Label(Images.Upgrades.stackUpgrade.rect, currentNode.numStackUpgrades.ToString(), countLabelStyle);
            }

            if(currentNode.numRangeUpgrades != 0) {
                if (Images.Upgrades.rangeUpgrade.DrawAsButton(guiStartX + 1286, yPos, 30, 30) && sSinceGuiInteract > 0.2f) {
                    sSinceGuiInteract = 0f;
                    PlayAudio("event:/SFX/UI SFX/Building UI SFX/Build Error");
                    if (UnityInput.Current.GetKey(KeyCode.LeftShift)) {
                        int count = Event.current.button == 1 ? 5 : 1;
                        currentNode.RemoveUpgrade(Names.Items.rangeUpgrade, out int removed, count);
                        inventory.AddResources(ResIDs.rangeUpgrade, removed);
                    }
                    else {
                        currentNode.RemoveUpgrade(Names.Items.rangeUpgrade, out _, 1);
                        itemInHand = Names.Items.rangeUpgrade;
                    }
                }
                GUI.Label(Images.Upgrades.rangeUpgrade.rect, currentNode.numRangeUpgrades.ToString(), countLabelStyle);
            }

            if (currentNode.infiniteRangeUpgrade) {
                if (Images.Upgrades.infiniteRangeUpgrade.DrawAsButton(guiStartX + 1331, yPos, 30, 30) && sSinceGuiInteract > 0.2f) {
                    sSinceGuiInteract = 0f;
                    PlayAudio("event:/SFX/UI SFX/Building UI SFX/Build Error");
                    currentNode.RemoveUpgrade(Names.Items.infiniteRangeUpgrade, out _, 1);
                    if (UnityInput.Current.GetKey(KeyCode.LeftShift)) {
                        inventory.AddResources(ResIDs.infiniteRangeUpgrade, 1);
                    }
                    else {
                        itemInHand = Names.Items.infiniteRangeUpgrade;
                    }
                }
            }
        }

        private static void DrawModules() {
            for(int moduleIndex = 0; moduleIndex < 8; moduleIndex++) {
                if (!currentNode.IsModuleInSlot(moduleIndex) || string.IsNullOrEmpty(currentNode.modules[moduleIndex].name)) continue;

                float xPos = guiStartX + (moduleIndex * 45) + 1016;
                float yPos = guiStartY + 289;

                Module module = currentNode.modules[moduleIndex];
                ModImage image = null;
                switch (module.name) {
                    case Names.Items.pullerModule: image = Images.Modules.pullerModule; break;
                    case Names.Items.pusherModule: image = Images.Modules.pusherModule; break;
                    case Names.Items.collectorModule: image = Images.Modules.collectorModule; break;
                    case Names.Items.distributorModule: image = Images.Modules.distributorModule; break;
                    case Names.Items.voidModule: image = Images.Modules.voidModule; break;
                    case Names.Items.compressorModule: image = Images.Modules.compressorModule; break;
                    case Names.Items.expanderModule: image = Images.Modules.expanderModule; break;
                }

                if (image.DrawAsButton(xPos, yPos, 30, 30) && sSinceGuiInteract > 0.2f && string.IsNullOrEmpty(itemInHand)) {
                    currentNode.activeSlots.Remove(moduleIndex);
                    currentNode.modules[moduleIndex] = null;
                    currentModule = null;
                    sSinceGuiInteract = 0f;
                    PlayAudio("event:/SFX/UI SFX/Building UI SFX/Build Error");

                    if (UnityInput.Current.GetKey(KeyCode.LeftShift)) {
                        inventory.AddResources(EMU.Resources.GetResourceIDByName(module.name), 1);
                    }
                    else {
                        itemInHand = module.name;
                    }
                }
            }
        }

        private static void DrawBuffer() {
            if (currentNode.buffer.isEmpty) return;

            Texture2D itemImage = EMU.Images.GetImageForResource(SaveState.GetResInfoFromId(currentNode.buffer.id).displayName);
            Rect rect = new Rect(guiStartX + 1016, guiStartY + 204, 30, 30);
            GUI.Box(rect, "", new GUIStyle() { normal = { background = itemImage } });
            GUI.Label(rect, currentNode.buffer.count.ToString(), countLabelStyle);
        }

        private static void DrawStats() {
            GUIStyle rangeStyle = sSinceRangeError < 1 ? redStatLabelStyle : statLabelStyle;

            float xPos = guiStartX + 1010;
            GUI.Label(new Rect(xPos, guiStartY + 430, 231, 15), $"{currentNode.powerConsumption} kW", statLabelStyle);
            GUI.Label(new Rect(xPos, guiStartY + 455, 231, 15), $"{currentNode.GetTaskDelay()} s", statLabelStyle);
            GUI.Label(new Rect(xPos, guiStartY + 482, 231, 15), currentNode.GetStackSize().ToString(), statLabelStyle);

            int range = currentNode.GetRange();
            string rangeLabelText = range == int.MaxValue ? "Inf." : $"{range} m";
            GUI.Label(new Rect(xPos, guiStartY + 506, 231, 15), rangeLabelText, rangeStyle);

            if (Images.LaserNodeGUI.rangeButton.DrawAsButton(guiStartX + 1142, guiStartY + 389)) {
                currentNode.ShowHideRange();
            }

            GUI.Label(Images.LaserNodeGUI.rangeButton.rect, currentNode.showRange ? "Hide Range" : "Show Range", buttonTextStyle);
        }

        private static void DrawColourSettings() {
            float xPos = guiStartX + 1322;
            redString = GUI.TextField(new Rect(xPos, guiStartY + 426, 42, 20), redString, labelStyle);
            greenString = GUI.TextField(new Rect(xPos, guiStartY + 464, 42, 20), greenString, labelStyle);
            blueString = GUI.TextField(new Rect(xPos, guiStartY + 502, 42, 20), blueString, labelStyle);

            if(int.TryParse(redString, out int red)) {
                red = Mathf.Clamp(red, 0, 255);
                redString = red.ToString();
                currentNode.red = red;
            }

            if(int.TryParse(greenString, out int green)) {
                green = Mathf.Clamp(green, 0, 255);
                greenString = green.ToString();
                currentNode.green = green;
            }

            if(int.TryParse(blueString, out int blue)) {
                blue = Mathf.Clamp(blue, 0, 255);
                blueString = blue.ToString();
                currentNode.blue = blue;
            }
        }

        private static void DrawInventory() {
            DrawInventoryLLItem(ResIDs.pullerModule);
            DrawInventoryLLItem(ResIDs.pusherModule);
            DrawInventoryLLItem(ResIDs.collectorModule);
            DrawInventoryLLItem(ResIDs.distributorModule);
            DrawInventoryLLItem(ResIDs.voidModule);
            DrawInventoryLLItem(ResIDs.compressorModule);
            DrawInventoryLLItem(ResIDs.expanderModule);
            
            DrawInventoryLLItem(ResIDs.speedUpgrade);
            DrawInventoryLLItem(ResIDs.stackUpgrade);
            DrawInventoryLLItem(ResIDs.rangeUpgrade);
            DrawInventoryLLItem(ResIDs.infiniteRangeUpgrade);

            DrawNonLLItems();
            HandleInventoryClicked();
        }

        private static void DrawInventoryLLItem(int resID) {
            int count = inventory.GetResourceCount(resID);
            string name = SaveState.GetResInfoFromId(resID).displayName;

            float xOffset = 0;
            float yOffset = 618f;
            ModImage image = null;

            switch (name) {
                case Names.Items.pullerModule:
                    xOffset = 631;
                    image = count == 0 ? Images.Modules.Dim.pullerModule : Images.Modules.pullerModule;
                    break;

                case Names.Items.pusherModule:
                    xOffset = 676;
                    image = count == 0 ? Images.Modules.Dim.pusherModule : Images.Modules.pusherModule;
                    break;

                case Names.Items.collectorModule:
                    xOffset = 721;
                    image = count == 0 ? Images.Modules.Dim.collector : Images.Modules.collectorModule;
                    break;

                case Names.Items.distributorModule:
                    xOffset = 766;
                    image = count == 0 ? Images.Modules.Dim.distributor : Images.Modules.distributorModule;
                    break;

                case Names.Items.voidModule:
                    xOffset = 811;
                    image = count == 0 ? Images.Modules.Dim.voidModule : Images.Modules.voidModule;
                    break;

                case Names.Items.compressorModule:
                    xOffset = 856;
                    image = count == 0 ? Images.Modules.Dim.compressor : Images.Modules.compressorModule;
                    break;

                case Names.Items.expanderModule:
                    xOffset = 901;
                    image = count == 0 ? Images.Modules.Dim.expander : Images.Modules.expanderModule;
                    break;

                case Names.Items.speedUpgrade:
                    xOffset = 991;
                    image = count == 0 ? Images.Upgrades.Dim.speedUpgrade : Images.Upgrades.speedUpgrade;
                    break;

                case Names.Items.stackUpgrade:
                    xOffset = 1036;
                    image = count == 0 ? Images.Upgrades.Dim.stack : Images.Upgrades.stackUpgrade;
                    break;

                case Names.Items.rangeUpgrade:
                    xOffset = 1081;
                    image = count == 0 ? Images.Upgrades.Dim.range : Images.Upgrades.rangeUpgrade;
                    break;

                case Names.Items.infiniteRangeUpgrade:
                    xOffset = 1126;
                    image = count == 0 ? Images.Upgrades.Dim.infiniteRange : Images.Upgrades.infiniteRangeUpgrade;
                    break;
            }

            float xPos = guiStartX + xOffset;
            float yPos = guiStartY + yOffset;

            if (image.DrawAsButton(xPos, yPos, 30, 30) && sSinceGuiInteract > 0.2f) {
                sSinceGuiInteract = 0f;
                if(string.IsNullOrEmpty(itemInHand)) {
                    if (UnityInput.Current.GetKey(KeyCode.LeftShift)) {
                        HandleShiftClickItemIntoNode(name);
                    }
                    else {
                        itemInHand = name;
                        inventory.TryRemoveResources(EMU.Resources.GetResourceIDByName(name), 1);
                        EDT.Log("GUI.InventoryModules", $"Moving {name} into hand");
                    }
                }
                else {
                    itemInHand = "";
                    inventory.AddResources(EMU.Resources.GetResourceIDByName(name), 1);
                    EDT.Log("GUI.InventoryModules", $"Moving {name} into inventory");
                }
            }
            
            GUI.Label(new Rect(xPos, yPos, 30, 30), count.ToString(), countLabelStyle);

            EDT.PacedLog("GUI.InventoryModules", $"Drawing {count} {name} at ({xPos},{yPos})");
        }

        private static void DrawNonLLItems() {
            Dictionary<string, List<ResourceInfo>> resourcesToDraw = new Dictionary<string, List<ResourceInfo>>();
            
            int itemsPerRow = Mathf.FloorToInt(1123 / 45f);
            float scrollerHeight = 0;
            
            foreach(KeyValuePair<string, List<ResourceInfo>> pair in inventoryCache) {
                List<ResourceInfo> toDraw = pair.Value.Where(resource => 
                    inventory.HasAnyOfResource(resource.uniqueId) &&
                    !resource.headerType.title.Contains("Laser")
                ).ToList();

                if (toDraw.Count == 0) continue;
                resourcesToDraw.Add(pair.Key, toDraw);
                scrollerHeight += 45 * (1 + Mathf.CeilToInt(toDraw.Count / (float)itemsPerRow));
            }

            inventoryScrollPos = GUI.BeginScrollView(new Rect(guiStartX + 626, guiStartY + 663, 1123, 238), inventoryScrollPos, new Rect(guiStartX + 626, guiStartY + 663, 1103, scrollerHeight), false, false);
            float currentYPos = guiStartY + 663;
            foreach (KeyValuePair<string, List<ResourceInfo>> pair in resourcesToDraw) {
                GUI.Label(new Rect(guiStartX + 626, currentYPos, 1103, 40), pair.Key, labelStyle);
                currentYPos += 45;

                for (int i = 0; i < pair.Value.Count; i++) {
                    ResourceInfo resource = pair.Value[i];
                    
                    float xPos = guiStartX + (i % itemsPerRow * 45) + 626;
                    float xEnd = guiStartX + 626 + 1083;

                    if(i != 0 && i % itemsPerRow == 0) {
                        currentYPos += 45;
                    }

                    if (Images.LaserNodeGUI.inventoryItem.DrawAsButton(xPos, currentYPos)) {
                        sSinceGuiInteract = 0f;
                        if (string.IsNullOrEmpty(itemInHand)) {
                            if (UnityInput.Current.GetKey(KeyCode.LeftShift)) {
                                HandleShiftClickItemIntoNode(resource.displayName);
                            }
                            else {
                                itemInHand = resource.displayName;
                                inventory.TryRemoveResources(resource.uniqueId, 1);
                                EDT.Log("GUI.InventoryModules", $"Moving {resource.displayName} into hand");
                            }
                        }
                        else {
                            itemInHand = "";
                            inventory.AddResources(resource.uniqueId, 1);
                            EDT.Log("GUI.InventoryModules", $"Moving {resource.displayName} into inventory");
                        }
                    }

                    GUI.Box(new Rect(xPos + 5, currentYPos + 5, 30, 30), "", GetItemImage(resource.displayName));
                }
                
                currentYPos += 45;
            }

            GUI.EndScrollView();
        }

        private static void DrawItemInHand() {
            if (string.IsNullOrEmpty(itemInHand)) return;
            EDT.PacedLog("GUI.ItemInHand", $"Drawing {itemInHand} in hand");

            Vector3 mousePos = UnityInput.Current.mousePosition;
            Rect position = new Rect(mousePos.x - 15, Screen.height - mousePos.y - 15, 30, 30);
            GUI.Box(position, "", GetItemImage(itemInHand));
            HandlePlaceItemInNode();
            HandleRightClick();
        }

        private static void DrawPMTEntries() {
            for (int i = 0; i < pmt.savedMachines.Count; i++) {
                uint instanceId = pmt.savedMachines[i];

                if (!MachineManager.instance.GetRefFromId(instanceId, out IMachineInstanceRef machineRef)) continue;

                string name = machineRef.builderInfo.displayName;
                Vector3 machinePos = machineRef.gridInfo.Center;
                Vector3Int machinePosRounded = new Vector3Int(
                    Mathf.RoundToInt(machinePos.x),
                    Mathf.RoundToInt(machinePos.y),
                    Mathf.RoundToInt(machinePos.z)
                );

                float xPos = guiStartX + 626;
                float yPos = guiStartY + (i * 39) + 213;
                Images.PMTGUI.entry.Draw(xPos, yPos, 355, 34);
                GUI.Label(new Rect(xPos, yPos, 355, 34), $"{name} @ {machinePosRounded}", labelStyle);

                if (Images.LaserNodeGUI.delete.DrawAsButton(xPos + 321, yPos)) {
                    PositionMemoryTablet.instance.savedMachines.Remove(instanceId);
                    PlayAudio("event:/SFX/UI SFX/Main Menu_Click Option");
                }

                if (currentModule == null ||
                    currentModule is VoidModule ||
                    currentModule is CompressorModule ||
                    currentModule is ExpanderModule) {
                    continue;
                }

                if (Images.LaserNodeGUI.move.DrawAsButton(xPos + 282, yPos)) {
                    if (!currentNode.IsMachineInRange(instanceId)) {
                        PlayAudio("event:/SFX/UI SFX/Building UI SFX/Build Error");
                        sSinceRangeError = 0f;
                        continue;
                    }

                    if (currentModule is PullerModule puller) puller.SetTarget(instanceId);
                    else if (currentModule is PusherModule pusher) pusher.SetTarget(instanceId);
                    else if (currentModule is CollectorModule collector) collector.AddTarget(instanceId);
                    else if (currentModule is DistributorModule distributor) distributor.AddTarget(instanceId);

                    PositionMemoryTablet.instance.savedMachines.Remove(instanceId);
                    PlayAudio("event:/SFX/UI SFX/Main Menu_Click Option");
                }
            }
        }

        private static void DrawModuleTargets() {
            if (currentModule == null ||
                currentModule is VoidModule ||
                currentModule is CompressorModule ||
                currentModule is ExpanderModule) {
                return;
            }

            List<uint> moduleTargets = new List<uint>();
            if (currentModule is PullerModule puller) moduleTargets.Add(puller.machineId);
            else if (currentModule is PusherModule pusher) moduleTargets.Add(pusher.machineId);
            else if (currentModule is CollectorModule collector) moduleTargets.AddRange(collector.machineIds);
            else if (currentModule is DistributorModule distributor) moduleTargets.AddRange(distributor.machineIds);

            if (moduleTargets.Count == 0) return;

            float xPos = guiStartX + 1396;
            float yPos = guiStartY + 333;
            Rect scrollViewRect = new Rect(xPos, yPos, 360, 186);
            Rect entriesArea = new Rect(xPos, yPos, 344, 8 * 39 - 5);
            targetsScrollPos = GUI.BeginScrollView(scrollViewRect, targetsScrollPos, entriesArea, false, false);

            for(int i = 0; i < moduleTargets.Count; i++) {
                uint instanceId = moduleTargets[i];

                if (!MachineManager.instance.GetRefFromId(instanceId, out IMachineInstanceRef machineRef)) continue;

                string name = machineRef.builderInfo.displayName;
                Vector3 machinePos = machineRef.gridInfo.Center;
                Vector3Int machinePosRounded = new Vector3Int(
                    Mathf.RoundToInt(machinePos.x),
                    Mathf.RoundToInt(machinePos.y),
                    Mathf.RoundToInt(machinePos.z)
                );

                float entryPosY = guiStartY + (i * 39) + 333;
                Images.PMTGUI.entry.Draw(xPos, entryPosY, 345, 34);
                GUI.Label(new Rect(xPos + 20, entryPosY, 325, 34), $"{name} @ {machinePosRounded}", labelStyle);

                if(Images.LaserNodeGUI.delete.DrawAsButton(xPos + 310, entryPosY)) {
                    currentModule.RemoveTarget(instanceId);
                    PlayAudio("event:/SFX/UI SFX/Main Menu_Click Option");
                }
            }

            GUI.EndScrollView();
        }

        private static void DrawFilterModeButton() {
            if (currentModule == null) return;

            if(Images.LaserNodeGUI.filterButton.DrawAsButton(guiStartX + 1681, guiStartY + 203)) {
                currentModule.ToggleFilterMode();
                PlayAudio("event:/SFX/UI SFX/Main Menu_Click Option");
            }

            string mode = currentModule.filterMode == Modules.FilterMode.Whitelist ? "Allow" : "Block";
            GUI.Label(Images.LaserNodeGUI.filterButton.rect, mode, buttonTextStyle);
        }

        private static void DrawModuleFilters() {
            if (currentModule == null) return;

            for(int i = 0; i < currentModule.filters.Count; i++) {
                float xPos = guiStartX + (i * 45) + 1401;
                float yPos = guiStartY + 256;
                if (Images.LaserNodeGUI.inventoryItem.DrawAsButton(xPos - 5, yPos - 5) &&
                    i < currentModule.filters.Count && sSinceGuiInteract > 0.2f) 
                {
                    currentModule.filters.RemoveAt(i);
                    sSinceGuiInteract = 0f;
                    PlayAudio("event:/SFX/UI SFX/Building UI SFX/Build Error");
                }
                else {
                    GUI.Box(new Rect(xPos, yPos, 30, 30), "", GetItemImage(currentModule.filters[i]));
                }
            }

            if (currentModule.name != Names.Items.expanderModule) return;

            for(int i = 1; i < 8; i++) {
                float xPos = guiStartX + (i * 45) + 1396;
                float yPos = guiStartY + 251;
                Images.LaserNodeGUI.shader.Draw(xPos, yPos, 40, 40);
            }
        }

        // Private Functions

        private static bool CheckForExitKeys() {
            if (UnityInput.Current.GetKeyDown(KeyCode.Escape) ||
                UnityInput.Current.GetKeyDown(KeyCode.Tab) ||
                UnityInput.Current.GetKeyDown(KeyCode.Q) ||
                UnityInput.Current.GetKeyDown(KeyCode.E)) {
                Close();
                return true;
            }

            return false;
        }

        private static void InitialiseStyles() {
            countLabelStyle = new GUIStyle() {
                alignment = TextAnchor.LowerRight,
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.yellow }
            };

            statLabelStyle = new GUIStyle() {
                alignment = TextAnchor.MiddleRight,
                fontSize = 16,
                normal = { textColor = Color.yellow }
            };

            redStatLabelStyle = new GUIStyle() {
                alignment = TextAnchor.MiddleRight,
                fontSize = 16,
                normal = { textColor = Color.red }
            };

            labelStyle = new GUIStyle() {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 16,
                normal = { textColor = Color.yellow }
            };

            buttonTextStyle = new GUIStyle() {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16,
                normal = { textColor = Color.yellow }
            };
        }

        private static GUIStyle GetItemImage(string item) {
            switch (item) {
                case Names.Items.pullerModule: return Images.Modules.pullerModule.style;
                case Names.Items.pusherModule: return Images.Modules.pusherModule.style;
                case Names.Items.collectorModule: return Images.Modules.collectorModule.style;
                case Names.Items.distributorModule: return Images.Modules.distributorModule.style;
                case Names.Items.voidModule: return Images.Modules.voidModule.style;
                case Names.Items.compressorModule: return Images.Modules.compressorModule.style;
                case Names.Items.expanderModule: return Images.Modules.expanderModule.style;

                case Names.Items.speedUpgrade: return Images.Upgrades.speedUpgrade.style;
                case Names.Items.stackUpgrade: return Images.Upgrades.stackUpgrade.style;
                case Names.Items.rangeUpgrade: return Images.Upgrades.rangeUpgrade.style;
                case Names.Items.infiniteRangeUpgrade: return Images.Upgrades.infiniteRangeUpgrade.style;

                default: return new GUIStyle() { normal = { background = EMU.Images.GetImageForResource(item) } };
            }
        }

        private static void PlayAudio(string eventName) {
            audioPlayer.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            audioPlayer = RuntimeManager.CreateInstance(eventName);
            audioPlayer.setParameterByName("Volume", 0.2f);
            audioPlayer.start();
        }

        // Events

        private static void HandleConfigButtonClicked(int index) {
            if(!currentNode.IsModuleInSlot(index)) return;
            currentModule = currentNode.modules[index];
            targetsScrollPos = Vector2.zero;
            PlayAudio("event:/SFX/UI SFX/Main Menu_Click Option");
        }

        private static void HandleInventoryClicked() {
            if (string.IsNullOrEmpty(itemInHand)) return;
            if (!UnityInput.Current.GetMouseButtonDown(0)) return;
            if (sSinceGuiInteract < 0.2f) return;

            float mouseX = UnityInput.Current.mousePosition.x;
            float mouseY = Screen.height - UnityInput.Current.mousePosition.y;

            if (mouseX < 616 || mouseX > 1759) return;
            if (mouseY < 541 || mouseY > 901) return;

            inventory.AddResources(EMU.Resources.GetResourceIDByName(itemInHand), 1);
            itemInHand = "";
            
            sSinceGuiInteract = 0f;
        }

        private static void HandlePlaceItemInNode() {
            if (string.IsNullOrEmpty(itemInHand)) return;
            if (!UnityInput.Current.GetMouseButtonDown(0)) return;
            if (sSinceGuiInteract < 0.2f) {
                EDT.Log("GUI.InventoryModules", $"Too soon after item placed: {sSinceGuiInteract}");
                return;
            }

            if (itemInHand.Contains("Module")) HandlePlaceModuleInNode();
            else if (itemInHand.Contains("Upgrade")) HandlePlaceUpgradeInNode();
            else HandlePlaceItemInFilter();
        }

        private static void HandlePlaceModuleInNode() {
            EDT.Log("GUI.InventoryModules", $"Placing {itemInHand} in node");

            float mouseX = UnityInput.Current.mousePosition.x;
            float mouseY = Screen.height - UnityInput.Current.mousePosition.y;

            if (mouseY < 284 || mouseY > 324) {
                EDT.Log("GUI", $"Click out of Y bounds: {mouseY} < {guiStartY + 115} || {mouseY} > {guiStartY + 155}");
                return;
            }

            mouseX -= guiStartX;
            if (mouseX > 1366) {
                EDT.Log("GUI", $"Click out of X bounds: {mouseX} > 1366");
                return;
            }

            if (mouseX < 1011) {
                EDT.Log("GUI", $"Click out of X bounds: {mouseX} < 1011");
                return;
            }

            int slotIndex = 0;

            if (mouseX > 1326) slotIndex = 7;
            else if (mouseX > 1281) slotIndex = 6;
            else if (mouseX > 1236) slotIndex = 5;
            else if (mouseX > 1191) slotIndex = 4;
            else if (mouseX > 1146) slotIndex = 3;
            else if (mouseX > 1101) slotIndex = 2;
            else if (mouseX > 1056) slotIndex = 1;

            // ToDo: Test slotIndex = Mathf.FloorToInt((mouseX - 10) / 45.0);

            if (currentNode.IsModuleInSlot(slotIndex)) return;
            currentNode.AddModule(itemInHand, slotIndex);
            itemInHand = "";
            sSinceGuiInteract = 0f;
            EDT.Log("GUI", $"Placed {itemInHand} in slot #{slotIndex}");

            PlayAudio("event:/SFX/UI SFX/Building UI SFX/Build Click");
        }

        private static void HandlePlaceUpgradeInNode() {
            float mouseX = UnityInput.Current.mousePosition.x;
            float mouseY = Screen.height - UnityInput.Current.mousePosition.y;

            if (mouseX < guiStartX + 1002 || mouseX > guiStartX + 1375) return;
            if (mouseY < guiStartY + 169 || mouseY > guiStartX + 371) return;

            if(!currentNode.CanAddUpgrade(itemInHand, out int max)) return;
            EDT.Log("GUI", $"Placing {itemInHand}");
            currentNode.AddUpgrade(itemInHand);
            itemInHand = "";
            sSinceGuiInteract = 0f;

            PlayAudio("event:/SFX/UI SFX/Building UI SFX/Build Click");
        }

        private static void HandlePlaceItemInFilter() {
            if (currentModule == null) return;

            float mouseX = UnityInput.Current.mousePosition.x;
            float mouseY = Screen.height - UnityInput.Current.mousePosition.y;

            if (mouseX < guiStartX + 1387 || mouseX > guiStartX + 1387 + 372) {
                EDT.Log("GUI.InventoryModules", $"Click out of filter xBounds: {mouseX}");
                return;
            }
            if (mouseY < guiStartY + 169 || mouseY > guiStartY + 169 + 360) {
                EDT.Log("GUI.InventoryModules", $"Click out of filter yBounds: {mouseY}");
                return;
            }

            if(currentModule.name != Names.Items.expanderModule || currentModule.filters.Count == 0) {
                currentModule.filters.Add(itemInHand);
                inventory.AddResources(EMU.Resources.GetResourceIDByName(itemInHand), 1);
                EDT.Log("GUI.InventoryModules", $"Placed {itemInHand} into filters");
                itemInHand = "";
                sSinceGuiInteract = 0f;
                PlayAudio("event:/SFX/UI SFX/Building UI SFX/Build Click");
            }
        }

        private static void HandleShiftClickItemIntoNode(string item) {
            if (item.Contains("Module")) HandleShiftClickModuleIntoNode(item);
            else if (item.Contains("Upgrade")) HandleShiftClickUpgradeIntoNode(item);
            else HandleShiftClickFilterIntoNode(item);
        }

        private static void HandleShiftClickModuleIntoNode(string item) {
            if (!currentNode.GetFirstEmptySlot(out int index)) return;
            if (!inventory.HasAnyOfResource(EMU.Resources.GetResourceIDByName(item))) return;

            currentNode.AddModule(item, index);
            inventory.TryRemoveResources(EMU.Resources.GetResourceIDByName(item), 1);
            PlayAudio("event:/SFX/UI SFX/Building UI SFX/Build Click");
        }

        private static void HandleShiftClickUpgradeIntoNode(string item) {
            if (!currentNode.CanAddUpgrade(item, out int max)) return;

            int count = 1;
            if (Event.current.button == 1 && item != Names.Items.infiniteRangeUpgrade) {
                count = 5;
            }

            count = Mathf.Min(count, max);
            count = Mathf.Min(count, inventory.GetResourceCount(EMU.Resources.GetResourceIDByName(item)));

            if (count == 0) return;

            currentNode.AddUpgrade(item, count);
            inventory.TryRemoveResources(EMU.Resources.GetResourceIDByName(item), count);

            PlayAudio("event:/SFX/UI SFX/Building UI SFX/Build Click");
        }

        private static void HandleShiftClickFilterIntoNode(string item) {
            if (currentModule == null) return;
            if (!currentModule.CanAddFilter(item)) return;

            currentModule.filters.Add(item);
            PlayAudio("event:/SFX/UI SFX/Building UI SFX/Build Click");
        }

        private static void HandleRightClick() {
            if(UnityInput.Current.GetMouseButtonDown(1) && !string.IsNullOrEmpty(itemInHand)) {
                inventory.AddResources(EMU.Resources.GetResourceIDByName(itemInHand), 1);
                itemInHand = "";
                PlayAudio("event:/SFX/UI SFX/Building UI SFX/Build Error");
            }
        }
    }
}
