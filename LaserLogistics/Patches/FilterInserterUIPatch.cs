using EquinoxsDebuggingTools;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
 
namespace LaserLogistics.Patches
{
    internal class FilterInserterUIPatch
    {
        [HarmonyPatch(typeof(FilterInserterUI), "Open")]
        [HarmonyPrefix]
        static bool OpenLaserNodeUI(in MachineInstanceRef<InserterInstance> newInserterRef) {
            if(newInserterRef.Get().myDef.displayName != Names.Items.laserNode) return true;

            LaserNodeGUI.Open(newInserterRef.instanceId);
            return false;
        }
    }
}
