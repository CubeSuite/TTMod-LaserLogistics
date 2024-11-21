using CasperEquinoxGUI.Controls;
using CasperEquinoxGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserLogistics.Controls
{
    public class FilterButton : ResourceButton
    {
        public FilterButton(int columnIndex) : base("") {
            ColumnIndex = (uint)columnIndex;
            Padding = new Thickness(2);
            Margin = new Thickness(2, 0, 2, 5);
        }

        // Overrides

        public override void Draw() {
            base.Draw();

            if (NewLaserNodeGUI.currentModule == null) return;
            if (NewLaserNodeGUI.currentModule.name != Names.Items.expanderModule) return;
            if (ColumnIndex == 0) return;

            Images.LaserNodeGUI.shader.Draw(PaddingRect.x, PaddingRect.y, 42, 40);
        }

        protected override int CalculateContentWidth() {
            return 36;
        }

        protected override int CalculateContentHeight() {
            return 36;
        }
    }
}
