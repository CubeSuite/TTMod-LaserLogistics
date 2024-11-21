using EquinoxsModUtils;
using EquinoxsModUtils.Additions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace LaserLogistics
{
    internal static class LaserNodeManager
    {
        // Objects & Variables
        internal static Dictionary<uint, LaserNode> nodes = new Dictionary<uint, LaserNode>();
        internal static Dictionary<uint, GameObject> visualsMap = new Dictionary<uint, GameObject>();

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

            if(instRef.gridInfo.strata == GameState.instance.GetStrata()) {
                GameObject visuals = GameObject.Instantiate(LaserNode.prefab, instRef.gridInfo.BottomCenter, Quaternion.Euler(0, instRef.gridInfo.yawRot, 0));
                if (visualsMap.ContainsKey(instRef.instanceId)) {
                    GameObject.Destroy(visualsMap[instRef.instanceId]);
                }

                visualsMap[instRef.instanceId] = visuals;
            }
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

        internal static void UpdateNodeColour(uint instanceId, Color colour) {
            if (!visualsMap.ContainsKey(instanceId)) return;
            
            GameObject visuals = visualsMap[instanceId];
            Transform sphereTransform = visuals.transform.Find("Sphere");
            Renderer sphereRenderer = sphereTransform.GetComponent<Renderer>();

            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            sphereRenderer.GetPropertyBlock(propertyBlock);

            propertyBlock.SetColor("_Color", colour);
            propertyBlock.SetColor("_EmissionColor", colour * Mathf.LinearToGammaSpace(20f)); // ToDo: Test emission strengths
            sphereRenderer.SetPropertyBlock(propertyBlock); 
            DynamicGI.SetEmissive(sphereRenderer, colour * 20f);
        }

        internal static void RedoVisualsOnStrataChange() {
            if (!EMU.LoadingStates.hasGameLoaded) return;
            ClearVisuals();
            foreach(uint id in nodes.Keys) {
                InserterInstance inserter = nodes[id].GetInserterInstance();
                if (inserter.gridInfo.strata == GameState.instance.GetStrata()) {
                    GameObject visuals = GameObject.Instantiate(LaserNode.prefab, inserter.gridInfo.BottomCenter, Quaternion.Euler(0, inserter.gridInfo.yawRot, 0));
                    if (visualsMap.ContainsKey(inserter.commonInfo.instanceId)) {
                        GameObject.Destroy(visualsMap[inserter.commonInfo.instanceId]);
                    }

                    visualsMap[inserter.commonInfo.instanceId] = visuals;
                }
            }
        }

        internal static void DestroyNode(InserterInstance instance) {
            nodes.Remove(instance.commonInfo.instanceId);
            GameObject.Destroy(visualsMap[instance.commonInfo.instanceId]);
            visualsMap.Remove(instance.commonInfo.instanceId);
        }

        // Events

        internal static void OnGameUnloaded() {
            nodes.Clear();
            ClearVisuals();
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

        private static void ClearVisuals() {
            foreach (GameObject visuals in visualsMap.Values) {
                GameObject.Destroy(visuals);
            }

            visualsMap.Clear();
        }
    }
}
