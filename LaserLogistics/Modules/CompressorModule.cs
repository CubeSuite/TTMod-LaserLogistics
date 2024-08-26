using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserLogistics.Modules
{
    internal class CompressorModule : Module
    {
        internal CompressorModule() : base(Names.Items.compressorModule) { }

        internal CompressorModule(string serial) : base(Names.Items.compressorModule) {
            LoadFilters(serial);
        }
    }
}
