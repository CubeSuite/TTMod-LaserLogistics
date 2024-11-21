using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserLogistics.Patches
{
    internal class TeamVisualsPatch
    {
        [HarmonyPatch(typeof(NexusDefinition.TeamVisuals), "GetTeamVisualsForNetID")]
        [HarmonyPostfix]
        static void OverwriteColour(ref NexusDefinition.TeamVisuals __result) {
            __result.PrimaryColour = LaserNode.nextLaserColour;
            __result.SecondaryColour = LaserNode.nextLaserColour;
        }
    }
}
