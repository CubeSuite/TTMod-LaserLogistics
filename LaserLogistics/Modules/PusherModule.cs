using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LaserLogistics.Modules
{
    internal class PusherModule : Module
    {
        internal uint machineId;
        internal string machineName;
        internal Vector3Int machinePos;
        internal Inventory targetInventory;
        internal string targetString => $"{machineName} @ {machinePos}";

        internal PusherModule() : base(Names.Items.pusherModule) { }

        internal PusherModule(string serial) : base(Names.Items.pusherModule) {
            LoadFilters(serial);
            
            string[] parts = serial.Split('/');
            machineId = uint.Parse(parts[3]);
            SetTarget(machineId);
        }

        internal void SetTarget(uint id) {
            machineId = id;

            if (MachineManager.instance.GetRefFromId(machineId, out IMachineInstanceRef machineRef)) {
                targetInventory = machineRef.GetInventoriesList().Last();
                if(machineRef.typeIndex == MachineTypeEnum.Assembler) {
                    Debug.Log($"Assembler has: {machineRef.GetInventoriesList().Count()} inventories");
                }

                machineName = machineRef.builderInfo.displayName;
                machinePos = new Vector3Int(
                        Mathf.RoundToInt(machineRef.gridInfo.Center.x),
                        Mathf.RoundToInt(machineRef.gridInfo.Center.y),
                        Mathf.RoundToInt(machineRef.gridInfo.Center.z)
                );
            }
        }

        internal override void RemoveTarget(uint id) {
            machineId = 0;
            targetInventory = default;
            machinePos = default;
            machineName = "";
        }

        internal override string Serialise() {
            return $"{base.Serialise()}/{machineId}";
        }
    }
}
