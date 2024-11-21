using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserLogistics.Patches
{
    internal class FlowManagerPatch
    {
        [HarmonyPatch(typeof(FlowManager), nameof(FlowManager.SwitchStrataTrampoline))]
        [HarmonyPostfix]
        static void RedoLaserNodeVisuals() {
            LaserNodeManager.RedoVisualsOnStrataChange();
        }
    }
}
