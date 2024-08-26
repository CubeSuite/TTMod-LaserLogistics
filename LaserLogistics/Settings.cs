using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserLogistics
{
    internal static class Settings
    {
        internal static int rangeUpgradeAmount = 16;
        internal static float speedUpgradeAmount = 0.1f;
        internal static int stackUpgradeAmount = 10;

        internal static Dictionary<string, int> powerMap = new Dictionary<string, int>() {
            {Names.Items.pullerModule, 100 },
            {Names.Items.pusherModule, 100 },
            {Names.Items.collectorModule, 800 },
            {Names.Items.distributorModule, 800 },
            {Names.Items.voidModule, 500 },
            {Names.Items.compressorModule, 1000 },
            {Names.Items.expanderModule, 1000 },

            {Names.Items.speedUpgrade, 50 },
            {Names.Items.stackUpgrade, 100 },
            {Names.Items.rangeUpgrade, 50 },
            {Names.Items.infiniteRangeUpgrade, 5000 },
        };
    }
}
