using BepInEx;
using EquinoxsDebuggingTools;
using EquinoxsModUtils;
using EquinoxsModUtils.Additions;
using FIMSpace.Generating.Rules.Transforming.Legacy;
using FluffyUnderware.DevTools.Extensions;
using LaserLogistics.Modules;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using ToolBuddy.ThirdParty.VectorGraphics;
using Unity.Collections;
using UnityEngine;

namespace LaserLogistics
{
    internal class LaserNode {
        // Objects & Variables
        internal uint instanceId;
        internal int index;
        internal int powerConsumption;
        internal bool showRange;
        
        internal Vector3 center;
        internal byte strata;

        internal int numSpeedUpgrades;
        internal int numStackUpgrades;
        internal int numRangeUpgrades;
        internal bool infiniteRangeUpgrade;

        internal Module[] modules = new Module[8];
        internal ResourceStack buffer = ResourceStack.CreateEmptyStack();

        internal List<int> activeSlots = new List<int>();
        internal int currentModuleIndex = -1;

        internal static GameObject prefab;
        internal static Color nextLaserColour;

        private float sUntilNextTask;
        private float sSinceLastLaser;
        private bool lastTaskSuccessful = true;

        // Properties

        private int red = LaserLogisticsPlugin.defaultRed.Value;
        internal int Red {
            get => red;
            set {
                red = value;
                Colour = new Color(red / 255f, green / 255f, blue / 255f);
            }
        }

        private int green = LaserLogisticsPlugin.defaultGreen.Value;
        internal int Green {
            get => green;
            set {
                green = value;
                Colour = new Color(red / 255f, green / 255f, blue / 255f);
            }
        }
        
        private int blue = LaserLogisticsPlugin.defaultBlue.Value;
        internal int Blue {
            get => blue;
            set {
                blue = value;
                Colour = new Color(red / 255f, green / 255f, blue / 255f);
            }
        }

        private Color colour;
        internal Color Colour {
            get => colour;
            private set {
                colour = value;
                LaserNodeManager.UpdateNodeColour(instanceId, colour);
            }
        }

        // Internal Functions

        internal void Initialise() {
            for (int i = 0; i < modules.Length; i++) modules[i] = null;
        }

        internal void AddModule(string name, int slotIndex) {
            if (IsModuleInSlot(slotIndex)) return;
            activeSlots.Add(slotIndex);
            activeSlots.Sort();

            switch (name) {
                case Names.Items.pullerModule: modules[slotIndex] = new PullerModule(); break;
                case Names.Items.pusherModule: modules[slotIndex] = new PusherModule(); break;
                case Names.Items.collectorModule: modules[slotIndex] = new CollectorModule(); break;
                case Names.Items.distributorModule: modules[slotIndex] = new DistributorModule(); break;
                case Names.Items.voidModule: modules[slotIndex] = new VoidModule(); break;
                case Names.Items.compressorModule: modules[slotIndex] = new CompressorModule(); break;
                case Names.Items.expanderModule: modules[slotIndex] = new ExpanderModule(); break;
            }
        }

        internal void LoadModule(string serial, int slotIndex) {
            string name = serial.Split('/')[0];
            switch (name) {
                case Names.Items.pullerModule: modules[slotIndex] = new PullerModule(serial); break;
                case Names.Items.pusherModule: modules[slotIndex] = new PusherModule(serial); break;
                case Names.Items.collectorModule: modules[slotIndex] = new CollectorModule(serial); break;
                case Names.Items.distributorModule: modules[slotIndex] = new DistributorModule(serial); break;
                case Names.Items.voidModule: modules[slotIndex] = new VoidModule(serial); break;
                case Names.Items.compressorModule: modules[slotIndex] = new CompressorModule(serial); break;
                case Names.Items.expanderModule: modules[slotIndex] = new ExpanderModule(serial); break;
            }
        }

        internal bool CanAddUpgrade(string name, out int max) {
            switch (name) {
                case Names.Items.speedUpgrade:
                    max = 10 - numSpeedUpgrades;
                    return numSpeedUpgrades < 10;

                case Names.Items.stackUpgrade:
                    max = 10 - numStackUpgrades;
                    return numStackUpgrades < 10;

                case Names.Items.rangeUpgrade:
                    max = 10 - numRangeUpgrades;
                    return numRangeUpgrades < 10;

                case Names.Items.infiniteRangeUpgrade:
                    max = infiniteRangeUpgrade ? 0 : 1;
                    return !infiniteRangeUpgrade;
            }

            LaserLogisticsPlugin.Log.LogError($"Cannot add unknown upgrade {name}");
            max = 0;
            return false;
        }

        internal void AddUpgrade(string name, int count = 1) {
            switch (name) {
                case Names.Items.speedUpgrade: numSpeedUpgrades += count; break;
                case Names.Items.stackUpgrade: numStackUpgrades += count; break;
                case Names.Items.rangeUpgrade: numRangeUpgrades += count; break;
                case Names.Items.infiniteRangeUpgrade: infiniteRangeUpgrade = true; break;
            }
        }

        internal void RemoveUpgrade(string name, out int removed, int count = 1) {
            removed = 0;

            switch (name) {
                case Names.Items.speedUpgrade:
                    count = Mathf.Min(count, numSpeedUpgrades);
                    numSpeedUpgrades -= count;
                    removed = count;
                    break;

                case Names.Items.stackUpgrade:
                    count = Mathf.Min(count, numStackUpgrades);
                    numStackUpgrades -= count;
                    removed = count;
                    break;

                case Names.Items.rangeUpgrade:
                    count = Mathf.Min(count, numRangeUpgrades);
                    numRangeUpgrades -= count;
                    removed = count;
                    break;

                case Names.Items.infiniteRangeUpgrade:
                    infiniteRangeUpgrade = false;
                    removed = 1;
                    break;
            }
        }

        internal bool IsModuleInSlot(int slotIndex) {
            return modules[slotIndex] != null;
        }

        internal void ShowHideRange() {
            showRange = !showRange;
            // ToDo: Render Range Sphere
        }

        internal bool GetFirstEmptySlot(out int index) {
            for (int i = 0; i < modules.Length; i++) {
                if (modules[i] == null) {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }

        public InserterInstance GetInserterInstance() {
            IMachineInstanceRef reference = MachineManager.instance.idLookup[instanceId];
            return reference.GetTyped<InserterInstance>();
        }

        // Update Functions

        internal void DoUpdate(float dt) {
            Save();
            HideInserterVisuals();
            sSinceLastLaser += dt;
            dt = DoPowerUpdate(dt);
            DoModulesUpdate(dt);
        }

        private float DoPowerUpdate(float dt) {
            SetMaxPowerDraw();
            if (!MachineManager.instance.GetRefFromId(instanceId, out IMachineInstanceRef machineRef)) return 0;

            if (machineRef.GetPowerInfo().shouldRefreshPowerNetwork) {
                machineRef.GetPowerInfo().RefreshPowerNetwork(machineRef.gridInfo.neighbors, (MachineInstanceRef<InserterInstance>)machineRef);
            }
            
            machineRef.GetPowerInfo().UpdateSimplePowerUsage(true);
            dt *= machineRef.GetPowerInfo().powerSatisfaction;
            return dt;
        }

        private void DoModulesUpdate(float dt) {
            sUntilNextTask -= dt;
            if (sUntilNextTask > 0) return;
            if (activeSlots.Count == 0) return;

            // Uncomment for pause if task can't be completed
            //if (lastTaskSuccessful) {
            //    ++currentModuleIndex;
            //    if (currentModuleIndex >= activeSlots.Count) currentModuleIndex = 0;
            //}

            ++currentModuleIndex;
            if (currentModuleIndex >= activeSlots.Count) currentModuleIndex = 0;
            
            Module module = modules[activeSlots[currentModuleIndex]];
            if (module is PullerModule puller) lastTaskSuccessful = DoPullerModule(ref puller);
            else if (module is PusherModule pusher) lastTaskSuccessful = DoPusherUpdate(ref pusher);
            else if (module is CollectorModule collector) lastTaskSuccessful = DoCollectorUpdate(ref collector);
            else if (module is DistributorModule distributor) lastTaskSuccessful = DoDistributorUpdate(ref distributor);
            else if (module is VoidModule voidModule) lastTaskSuccessful = DoVoidUpdate(ref voidModule);
            else if (module is CompressorModule compressor) lastTaskSuccessful = DoCompressorUpdate(ref compressor);
            else if (module is ExpanderModule expander) lastTaskSuccessful = DoExpanderUpdate(ref expander);

            sUntilNextTask = GetTaskDelay();
        }

        // Data Functions

        internal void Save() {
            EMUAdditions.CustomData.Update(instanceId, "red", red);
            EMUAdditions.CustomData.Update(instanceId, "green", green);
            EMUAdditions.CustomData.Update(instanceId, "blue", blue);

            EMUAdditions.CustomData.Update(instanceId, "numSpeedUpgrades", numSpeedUpgrades);
            EMUAdditions.CustomData.Update(instanceId, "numStackUpgrades", numStackUpgrades);
            EMUAdditions.CustomData.Update(instanceId, "numRangeUpgrades", numRangeUpgrades);
            EMUAdditions.CustomData.Update(instanceId, "infiniteRangeUpgrade", infiniteRangeUpgrade);

            for(int i = 0; i < 8; i++) {
                string serialised = modules[i] == null ? "null" : modules[i].Serialise();
                EMUAdditions.CustomData.Update(instanceId, $"module#{i}", serialised);
            }

            EMUAdditions.CustomData.Update(instanceId, "buffer.id", buffer.id);
            EMUAdditions.CustomData.Update(instanceId, "buffer.count", buffer.count);
            EMUAdditions.CustomData.Update(instanceId, "buffer.maxStack", buffer.maxStack);

            EMUAdditions.CustomData.Update(instanceId, "activeSlots", string.Join(",", activeSlots));
            EMUAdditions.CustomData.Update(instanceId, "currentModuleIndex", currentModuleIndex);

            EMUAdditions.CustomData.Update(instanceId, "sUntilNextTask", sUntilNextTask);
            EMUAdditions.CustomData.Update(instanceId, "sSinceLastLaser", sSinceLastLaser);
            EMUAdditions.CustomData.Update(instanceId, "lastTaskSuccessfull", lastTaskSuccessful);
        }

        internal void Load() {
            Dictionary<string, object> savedFields = EMUAdditions.CustomData.GetAll(instanceId);

            Red = (int)savedFields["red"];
            Green = (int)savedFields["green"];
            Blue = (int)savedFields["blue"];

            numSpeedUpgrades = (int)savedFields["numSpeedUpgrades"];
            numStackUpgrades = (int)savedFields["numStackUpgrades"];
            numRangeUpgrades = (int)savedFields["numRangeUpgrades"];
            infiniteRangeUpgrade = (bool)savedFields["infiniteRangeUpgrade"];

            for(int i = 0; i < 8; i++) {
                string serial = savedFields[$"module#{i}"].ToString();
                if (serial == "null") continue;

                string type = serial.Split('/')[0];
                switch (type) {
                    case Names.Items.pullerModule: modules[i] = new PullerModule(serial); break;
                    case Names.Items.pusherModule: modules[i] = new PusherModule(serial); break;
                    case Names.Items.collectorModule: modules[i] = new CollectorModule(serial); break;
                    case Names.Items.distributorModule: modules[i] = new DistributorModule(serial); break;
                    case Names.Items.voidModule: modules[i] = new VoidModule(serial); break;
                    case Names.Items.compressorModule: modules[i] = new CompressorModule(serial); break;
                    case Names.Items.expanderModule: modules[i] = new ExpanderModule(serial); break;
                }
            }

            int bufferId = (int)savedFields["buffer.id"];
            int bufferCount = (int)savedFields["buffer.count"];
            int bufferMaxStack = (int)savedFields["buffer.maxStack"];
            buffer = ResourceStack.CreateSimpleStack(bufferId, bufferCount);
            buffer.maxStack = bufferMaxStack;

            string activeSlotsJoined = savedFields["activeSlots"].ToString();
            if (!string.IsNullOrEmpty(activeSlotsJoined)) {
                foreach (string activeSlotString in activeSlotsJoined.Split(',')) {
                    activeSlots.Add(int.Parse(activeSlotString));
                }
            }

            currentModuleIndex = (int)savedFields["currentModuleIndex"];
            sUntilNextTask = (float)savedFields["sUntilNextTask"];
            sSinceLastLaser = (float)savedFields["sSinceLastLaser"];
            lastTaskSuccessful = (bool)savedFields["lastTaskSuccessfull"];
        }

        // Module Functions

        private bool DoPullerModule(ref PullerModule puller) {
            bool success = PullFromInventory(ref puller.targetInventory, puller, puller.machinePos, out bool didLaser);
            if(didLaser) sSinceLastLaser = 0f;
            return success;
        }

        private bool DoPusherUpdate(ref PusherModule pusher) {
            bool success = PushToInventory(ref pusher.targetInventory, pusher, pusher.machinePos, out bool didLaser);
            if (didLaser) sSinceLastLaser = 0f;
            return success;
        }

        private bool DoCollectorUpdate(ref CollectorModule collector) {
            bool laserDoneThisUpdate = false;
            bool anySuccess = false;
            
            for(int i = 0; i < collector.targetInventories.Count; i++) {
                Inventory inventory = collector.targetInventories[collector.nextInventoryIndex];
                if(PullFromInventory(ref inventory, collector, collector.machinePositions[collector.nextInventoryIndex], out bool didLaser)) {
                    anySuccess = true;
                    if(!laserDoneThisUpdate) laserDoneThisUpdate = didLaser;
                    ++collector.nextInventoryIndex;
                    if (collector.nextInventoryIndex >= collector.targetInventories.Count) {
                        collector.nextInventoryIndex = 0;
                    }
                    EDT.Log("Modules", $"collector.nextInventoryIndex: {collector.nextInventoryIndex}");
                }
            }

            if (laserDoneThisUpdate) sSinceLastLaser = 0f;

            return anySuccess;
        }

        private bool DoDistributorUpdate(ref DistributorModule distributor) {
            bool laserDoneThisUpdate = false;
            bool anySuccess = false;

            for(int i = 0; i < distributor.targetInventories.Count; i++) {
                if (buffer.isEmpty) return anySuccess;
                Inventory inventory = distributor.targetInventories[distributor.nextInventoryIndex];
                if(PushToInventory(ref inventory, distributor, distributor.machinePositions[distributor.nextInventoryIndex], out bool didLaser)) {
                    anySuccess = true;
                }

                if (!laserDoneThisUpdate) laserDoneThisUpdate = didLaser;
                
                ++distributor.nextInventoryIndex;
                if(distributor.nextInventoryIndex >= distributor.targetInventories.Count) {
                    distributor.nextInventoryIndex = 0;
                }
            }

            if (laserDoneThisUpdate) sSinceLastLaser = 0f;
            return anySuccess;
        }

        private bool DoVoidUpdate(ref VoidModule voidModule) {
            if (buffer.isEmpty) return false;
            if (!voidModule.DoFilterTest(buffer.id)) return false;

            buffer = ResourceStack.CreateEmptyStack();
            return true;
        }

        private bool DoCompressorUpdate(ref CompressorModule compressor) {
            if (buffer.isEmpty) return false;
            if (!compressor.DoFilterTest(buffer.id)) return false;

            int toCompress = Mathf.Min(buffer.count, GetStackSize());
            QuantumStorageNetwork.AddItems(buffer.id, toCompress);
            if (toCompress == buffer.count) {
                buffer = ResourceStack.CreateEmptyStack();
            }
            else {
                buffer.count -= toCompress;
            }

            return true;
        }

        private bool DoExpanderUpdate(ref ExpanderModule expander) {
            if (expander.filters.Count == 0) return false;
            
            ResourceInfo filteredItem = EMU.Resources.GetResourceInfoByName(expander.filters[0]);
            if (!QuantumStorageNetwork.IsItemInNetwork(filteredItem)) return false;
            if (!buffer.isEmpty && buffer.id != filteredItem.uniqueId) return false;

            int toRemove = GetStackSize();
            if(!buffer.isEmpty) toRemove = Mathf.Min(toRemove, buffer.maxStack - buffer.count);

            int removed = QuantumStorageNetwork.RemoveItems(filteredItem, toRemove);
            if (buffer.isEmpty) {
                buffer.id = filteredItem.uniqueId;
                buffer.count = removed;
                buffer.maxStack = filteredItem.maxStackCount;
            }
            else {
                buffer.count += removed;
            }

            return true;
        }

        // Upgrade Functions

        internal float GetTaskDelay() {
            return 1f - numSpeedUpgrades / 10f;
        }

        internal int GetStackSize() {
            if (numStackUpgrades == 0) return 1;
            else return 50 * numStackUpgrades;
        }

        internal int GetRange() {
            if (infiniteRangeUpgrade) return int.MaxValue;
            return Settings.rangeUpgradeAmount * (numRangeUpgrades + 1);
        }

        internal bool IsMachineInRange(uint instanceId) {
            if (MachineManager.instance.GetRefFromId(instanceId, out IMachineInstanceRef machineRef)) {
                float distance = Vector3.Distance(machineRef.gridInfo.Center, center);
                return distance <= GetRange();
            }

            return false;
        }

        // Private Functions

        private void HideInserterVisuals() {
            InserterMachineList list = (InserterMachineList)MachineManager.instance.GetMachineList(MachineTypeEnum.Inserter);
            if(list.TryGetVisualIndex(index, out int visualsIndex)) {
                list.visualsList[visualsIndex].commonData.showGPUIMeshes = false;
            }
        }

        private void SetMaxPowerDraw() {
            int maxPower = 0;

            foreach (int moduleIndex in activeSlots) {
                maxPower += Settings.powerMap[modules[moduleIndex].name];
            }

            maxPower += numSpeedUpgrades * Settings.powerMap[Names.Items.speedUpgrade];
            maxPower += numStackUpgrades * Settings.powerMap[Names.Items.stackUpgrade];
            maxPower += numRangeUpgrades * Settings.powerMap[Names.Items.rangeUpgrade];
            if (infiniteRangeUpgrade) maxPower += Settings.powerMap[Names.Items.infiniteRangeUpgrade];

            if (!MachineManager.instance.GetRefFromId(instanceId, out IMachineInstanceRef machineRef)) return;

            powerConsumption = maxPower;
            machineRef.GetPowerInfo().maxPowerConsumption = maxPower;
        }
        
        private bool PullFromInventory(ref Inventory inventory, Module module, Vector3 position, out bool didLaser) {
            didLaser = false;
            if (inventory.myStacks == null) return false;

            if (!InventoryContainsFilteredItem(ref inventory, module, out int index)) return false;
            ResourceStack targetResource = inventory.myStacks[index];
            
            if (!buffer.isEmpty && buffer.id != targetResource.id) return false;
            if (!module.DoFilterTest(targetResource.id)) return false;

            int toTransfer = Mathf.Min(targetResource.count, GetStackSize());
            toTransfer = Mathf.Min(toTransfer, targetResource.maxStack - buffer.count);

            if(module is CollectorModule collector) {
                toTransfer = Mathf.Min(targetResource.count, Mathf.CeilToInt(GetStackSize() / (float)collector.targetInventories.Count));
            }

            if (toTransfer == 0) return false;

            inventory.TryRemoveResources(targetResource.id, toTransfer);

            if (buffer.isEmpty) {
                buffer.id = targetResource.id;
                buffer.count = toTransfer;
                buffer.maxStack = targetResource.maxStack;
            }
            else {
                buffer.count += toTransfer;
            }

            if (ShootLaser(position)) didLaser = true;

            EDT.Log("LaserNode.Transfer", $"Pulled {toTransfer} res#{buffer.id} from inventory");
            return true;
        }

        private bool PushToInventory(ref Inventory inventory, Module module, Vector3 position, out bool didLaser) {
            didLaser = false;
            if (inventory.myStacks == null) return false;
            if (buffer.isEmpty) return false;
            if (!inventory.CanAddResources(buffer.id, buffer.count)) return false;
            if (!module.DoFilterTest(buffer.id)) return false;

            int toTransfer = Mathf.Min(buffer.count, GetStackSize());
            
            if(module is DistributorModule distributor) {
                toTransfer = Mathf.Min(buffer.count, Mathf.CeilToInt(GetStackSize() / (float)distributor.targetInventories.Count));
            }

            inventory.AddResources(buffer.id, out int remaining, toTransfer);
            int sent = toTransfer - remaining;
            if (sent >= buffer.count) {
                buffer = ResourceStack.CreateEmptyStack();
            }
            else {
                buffer.count -= sent;
            }

            if (ShootLaser(position)) didLaser = true;

            EDT.Log("LaserNode.Transfer", $"Pushed {sent} res#{buffer.id} to inventory");
            return true;
        }

        private bool InventoryContainsFilteredItem(ref Inventory inventory, Module module, out int index) {
            foreach(ResourceStack stack in inventory.myStacks.Where(stack => !stack.isEmpty)) {
                if (module.DoFilterTest(stack.id)){
                    index = inventory.myStacks.IndexOf(stack);
                    return true;
                }
            }

            index = -1;
            return false;
        }

        private bool ShootLaser(Vector3 inventoryPos) {
            if (!LaserLogisticsPlugin.showLasers.Value) return false;
            
            if (sSinceLastLaser > LaserLogisticsPlugin.laserCooldown.Value) {
                nextLaserColour = GetLaserColour();
                LaserGunTool.ShowShot(new ShootInfo() {
                    origin = inventoryPos,
                    target = center,
                    strata = strata
                });
                
                return true;
            }

            return false;
        }

        private Color GetLaserColour() {
            if (!LaserLogisticsPlugin.discoMode.Value) return colour;
            else {
                float r = UnityEngine.Random.Range(0, 256) / 255f;
                float g = UnityEngine.Random.Range(0, 256) / 255f;
                float b = UnityEngine.Random.Range(0, 256) / 255f;
                return new Color(r, g, b);
            }
        }
    }
}
