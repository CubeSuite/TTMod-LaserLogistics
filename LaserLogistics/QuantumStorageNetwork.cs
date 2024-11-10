using EquinoxsDebuggingTools;
using EquinoxsModUtils;
using EquinoxsModUtils.Additions;
using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserLogistics
{
    internal static class QuantumStorageNetwork
    {
        // Objects & Variables
        private static Dictionary<int, int> items = new Dictionary<int, int>();

        // Internal Functions

        internal static void AddItems(int id, int count) {
            if (items.ContainsKey(id)) {
                items[id] += count;
            }
            else {
                items.Add(id, count);
            }
        }

        internal static int GetCountOfItem(int id) {
            if (!items.ContainsKey(id)) {
                LaserLogisticsPlugin.Log.LogError($"Cannot get count of item #{id} as it is not in QSN");
                return 0;
            }

            return items[id];
        }
        
        internal static bool IsItemInNetwork(int id) {
            return items.ContainsKey(id);
        }

        internal static int RemoveItems(int id, int requestedAmount) {
            if (!items.ContainsKey(id)) {
                LaserLogisticsPlugin.Log.LogError($"Cannot remove item #{id} as it is not in QSN");
                return 0;
            }

            int countInNetwork = GetCountOfItem(id);
            if(requestedAmount > countInNetwork) {
                items.Remove(id);
                return countInNetwork;
            }

            items[id] -= requestedAmount;
            return requestedAmount;
        }

        // Data Functions

        internal static void Save() {
            List<string> itemSerials = new List<string>();
            foreach(KeyValuePair<int, int> pair in items) {
                itemSerials.Add($"{pair.Key},{pair.Value}");
            }

            string serialised = string.Join("|", itemSerials);
            EMUAdditions.CustomData.Update(0, "quantumStorageNetwork", serialised);
        }

        internal static void Load() {
            string serialised = EMUAdditions.CustomData.Get<string>(0, "quantumStorageNetwork");
            if (string.IsNullOrEmpty(serialised)) {
                EDT.Log("Data", "serialised is null or empty, aborting attempt to load");
                return;
            }

            string[] itemSerials = serialised.Split('|');
            foreach(string serial in itemSerials) {
                string[] parts = serial.Split(',');
                int id = int.Parse(parts[0]);
                int count = int.Parse(parts[1]);
                AddItems(id, count);
            }
        }

        #region Overloads

        internal static void AddItems(ResourceStack stack) {
            AddItems(stack.id, stack.count);
        }

        internal static void AddItems(ResourceInfo info, int count) {
            AddItems(info.uniqueId, count);
        }

        internal static bool IsItemInNetwork(ResourceInfo info) {
            return IsItemInNetwork(info.uniqueId);
        }

        internal static int RemoveItems(ResourceStack stack) {
            return RemoveItems(stack.id, stack.count);
        }

        internal static int RemoveItems(ResourceInfo info, int count) { 
            return RemoveItems(info.uniqueId, count);
        }

        #endregion
    }
}
