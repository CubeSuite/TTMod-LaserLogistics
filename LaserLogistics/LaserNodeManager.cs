using EquinoxsModUtils;
using EquinoxsModUtils.Additions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LaserLogistics
{
    internal static class LaserNodeManager
    {
        // Objects & Variables
        internal static Dictionary<uint, LaserNode> nodes = new Dictionary<uint, LaserNode>();
        internal static Dictionary<Vector3, GameObject> visualsMap = new Dictionary<Vector3, GameObject>();

        // Public Functions

        internal static void CreateNewNode(MachineInstanceRef<InserterInstance> instRef) {
            LaserNode node = new LaserNode() { 
                instanceId = instRef.instanceId, 
                index = instRef.index, 
                center = instRef.gridInfo.Center,
                strata = instRef.gridInfo.strata
            };

            node.Initialise();
            nodes.Add(instRef.instanceId, node);

            GameObject visuals = GameObject.Instantiate(LaserNode.prefab, instRef.gridInfo.BottomCenter, Quaternion.Euler(0, instRef.gridInfo.yawRot, 0));
            visualsMap.Add(instRef.gridInfo.BottomCenter, visuals);
        }

        internal static void LoadNode(uint instanceId) {
            LaserNode node = new LaserNode() { instanceId = instanceId };
            node.Load();
            nodes.Add(instanceId, node);
        }

        internal static LaserNode GetNode(uint instanceId) {
            if (!nodes.ContainsKey(instanceId)) {
                LaserLogisticsPlugin.Log.LogError($"Could not get node #{instanceId}");
                return null;
            }

            return nodes[instanceId];
        }

        internal static void DestroyNode(InserterInstance instance) {
            nodes.Remove(instance.commonInfo.instanceId);
            GameObject.Destroy(visualsMap[instance.gridInfo.BottomCenter]);
            visualsMap.Remove(instance.gridInfo.BottomCenter);
        }

        // Private Functions

        internal static void Save() {
            foreach(LaserNode node in nodes.Values) {
                node.Save();
            }
        }

        internal static void Load() {
            foreach(uint id in nodes.Keys) {
                UnityEngine.Debug.Log($"Data exists for #{id}: {EMUAdditions.CustomData.AnyExists(id)}");
                if (!EMUAdditions.CustomData.AnyExists(id)) continue;
                UnityEngine.Debug.Log($"Loading data for node #{id} - World {SaveState.instance.metadata.worldName}");
                nodes[id].Load();
            }
        }
    }
}
