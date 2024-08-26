using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserLogistics.Patches
{
    internal class InserterInstancePatch
    {
        [HarmonyPatch(typeof(InserterInstance), "SimUpdate")]
        [HarmonyPrefix]
        static bool DoLaserNodeUpdate(InserterInstance __instance, float dt) {
            if (__instance.myDef.displayName != Names.Items.laserNode) return true;

            LaserNodeManager.GetNode(__instance.commonInfo.instanceId).DoUpdate(dt);
            return false;
        }
    }
}
