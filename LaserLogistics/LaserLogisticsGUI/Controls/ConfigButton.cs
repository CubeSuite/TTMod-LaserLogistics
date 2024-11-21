using CasperEquinoxGUI;
using CasperEquinoxGUI.Controls;
using CasperEquinoxGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserLogistics.Controls
{
    public class ConfigButton : Button
    {
        // Members
        public int slotIndex;

        public ConfigButton(int slotIndex) : base(Images.LaserNodeGUI.Icons.settings.texture2d) {
            this.slotIndex = slotIndex;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;
            Padding = new Thickness(5);
            ColumnIndex = (uint)slotIndex;
        }

        public override void Draw() {
            base.Draw();
            if(!NewLaserNodeGUI.currentNode.IsModuleInSlot(slotIndex)) {
                Images.LaserNodeGUI.shader.Draw(PaddingRect.x, PaddingRect.y, 30, 30);
            }
        }
    }
}
