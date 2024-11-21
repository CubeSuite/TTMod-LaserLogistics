using CasperEquinoxGUI.Controls;
using CasperEquinoxGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserLogistics.Controls
{
    public class ModuleButton : ResourceButton
    {
        // Members
        public int slotIndex;

        // Constructors

        public ModuleButton(int slotIndex) : base(""){
            this.slotIndex = slotIndex;
            ColumnIndex = (uint)slotIndex;
            Padding = new Thickness(0);
        }

        public ModuleButton(string moduleName, int slotIndex) : base(moduleName) {
            this.slotIndex = slotIndex;
            ColumnIndex = (uint)slotIndex;
            Padding = new Thickness(0);
        }

        // Overrides

        protected override int CalculateContentWidth() {
            return 40;
        }

        protected override int CalculateContentHeight() {
            return 40;
        }
    }
}
