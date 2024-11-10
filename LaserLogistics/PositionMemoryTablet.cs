using BepInEx;
using EquinoxsDebuggingTools;
using EquinoxsModUtils;
using EquinoxsModUtils.Additions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LaserLogistics
{
    internal class PositionMemoryTablet : Scanner
    {
        internal static PositionMemoryTablet instance = new PositionMemoryTablet() {
            savedMachines = new List<uint>(),
            sSincePositionAdded = 0f
        };

        internal List<uint> savedMachines;
        internal float sSincePositionAdded;
        private const float cooldown = 0.3f;

        internal void Use() {
            if (IsOnCooldown()) return;
            sSincePositionAdded = 0;

            FieldInfo targetMachineInfo = typeof(PlayerInteraction).GetField("targetMachineRef", BindingFlags.Instance | BindingFlags.NonPublic);
            GenericMachineInstanceRef machine = (GenericMachineInstanceRef)targetMachineInfo.GetValue(Player.instance.interaction);

            if (!machine.IsValid()) {
                EMU.Notify("Not a machine");
                return;
            }

            if(machine.GetInventoriesList().Count() == 0) {
                EMU.Notify("Machine has no inventories");
                return;
            }

            if (savedMachines.Contains(machine.instanceId)) {
                if (UnityInput.Current.GetKey(KeyCode.LeftShift)) {
                    savedMachines.Remove(machine.instanceId);
                    return;
                }

                EMU.Notify("Already saved this location");
                return;
            }
            
            if(savedMachines.Count == 8) {
                EMU.Notify("Can't save > 8 locations");
                return;
            }

            savedMachines.Add(machine.instanceId);
            EDT.PacedLog($"PMT", $"Added machine #{machine.instanceId} to PMT");
        }

        internal bool IsOnCooldown() {
            return sSincePositionAdded < cooldown;
        }

        internal void Save() {
            string savedMachinesString = string.Join("|", savedMachines);
            EMUAdditions.CustomData.Update(0, "pmt", savedMachinesString);
        }

        internal void Load() {
            string savedMachinesString = EMUAdditions.CustomData.Get<string>(0, "pmt");
            if(string.IsNullOrEmpty(savedMachinesString)) return;

            string[] idStrings = savedMachinesString.Split('|');
            foreach(string idString in idStrings) {
                savedMachines.Add(uint.Parse(idString));
            }
        }
    }

    internal class PositionMemoryTabletInfo : ScannerInfo 
    {
        private PositionMemoryTablet tablet => Player.instance.equipment.GetEquipment<PositionMemoryTablet>();

        public override void OnHold() {
            tablet.Use();
        }

        public override void OnUpdate(bool isSelected) {
            GameObject scanner = (GameObject)EMU.GetPrivateField("mainObj", Player.instance.equipment.GetEquipment<Scanner>());
            if (scanner == null) return;
            scanner.SetActive(isSelected);
            
            if (isSelected) {
                bool shouldShowScanningAnim = PositionMemoryTablet.instance.sSincePositionAdded < 0.5f;
                Player.instance.fpcontroller.armsAnimator.SetBool("scannerOn", shouldShowScanningAnim);
            }
        }
    }
}
