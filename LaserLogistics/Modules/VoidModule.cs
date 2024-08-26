using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserLogistics.Modules
{
    internal class VoidModule : Module
    {
        internal VoidModule() : base(Names.Items.voidModule) { }

        internal VoidModule(string serial) : base(Names.Items.voidModule) {
            LoadFilters(serial);
        }
    }
}
