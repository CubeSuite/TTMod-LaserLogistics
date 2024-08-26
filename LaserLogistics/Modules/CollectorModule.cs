using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LaserLogistics.Modules
{
    internal class CollectorModule : Module
    {
        // Objects & Variables
        internal List<uint> machineIds = new List<uint>();
        internal List<string> machineNames = new List<string>();
        internal List<Vector3Int> machinePositions = new List<Vector3Int>();
        internal List<Inventory> targetInventories = new List<Inventory>();
        internal int nextInventoryIndex = 0;

        // Constructors

        internal CollectorModule() : base(Names.Items.collectorModule) { }

        internal CollectorModule(string serial) : base(Names.Items.collectorModule) {
            LoadFilters(serial);
            
            string[] parts = serial.Split('/');
            string[] idStrings = parts[3].Split(',');
            foreach (string idString in idStrings) {
                uint id = uint.Parse(idString);
                AddTarget(id);
            }

            nextInventoryIndex = int.Parse(parts[4]);
        }

        // Internal Functions

        internal string GetTargetString(int index) {
            return $"{machineNames[index]} @ {machinePositions[index]}";
        }

        internal void AddTarget(uint id) {
            machineIds.Add(id);

            if (MachineManager.instance.GetRefFromId(id, out IMachineInstanceRef machineRef)) {
                targetInventories.Add(machineRef.GetInventory(0));

                machineNames.Add(machineRef.builderInfo.displayName);
                machinePositions.Add(new Vector3Int() {
                    x = Mathf.RoundToInt(machineRef.gridInfo.Center.x),
                    y = Mathf.RoundToInt(machineRef.gridInfo.Center.y),
                    z = Mathf.RoundToInt(machineRef.gridInfo.Center.z)
                });
            }
            else {
                LaserLogisticsPlugin.Log.LogError($"Could not get machine #{id}");
            }
        }

        internal override void RemoveTarget(uint id) {
            int index = machineIds.IndexOf(id);
            machineIds.RemoveAt(index);
            targetInventories.RemoveAt(index);
            machineNames.RemoveAt(index);
            machinePositions.RemoveAt(index);
        }

        internal override string Serialise() {
            return $"{base.Serialise()}/{string.Join(",", machineIds)}/{nextInventoryIndex}";
        }
    }
}
