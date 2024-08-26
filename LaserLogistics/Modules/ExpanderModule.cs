using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserLogistics.Modules
{
    internal class ExpanderModule : Module
    {
        internal ExpanderModule() : base(Names.Items.expanderModule) { }

        internal ExpanderModule(string serial) : base(Names.Items.expanderModule) {
            LoadFilters(serial);
        }
    }
}
