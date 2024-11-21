using EquinoxsDebuggingTools;
using EquinoxsModUtils.Additions;
using FIMSpace.Generating.Planning.PlannerNodes.Logic;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LaserLogistics.Patches
{
    internal class InserterDefinitionPatch
    {
        [HarmonyPatch(typeof(InserterDefinition), "OnBuild")]
        [HarmonyPrefix]
        static void CreateNode(InserterDefinition __instance, MachineInstanceRef<InserterInstance> instRef) {
            if (__instance.displayName != Names.Items.laserNode) return;
            LaserNodeManager.CreateNewNode(instRef);
        }

        [HarmonyPatch(typeof(MachineDefinition<InserterInstance, InserterDefinition>), "OnDeconstruct")]
        [HarmonyPrefix]
        static void RemoveVisuals(ref InserterInstance erasedInstance) {
            if (erasedInstance.myDef.displayName != Names.Items.laserNode) return;
            LaserNodeManager.DestroyNode(erasedInstance);
        }
    }
}
