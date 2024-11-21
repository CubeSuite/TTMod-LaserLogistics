using CasperEquinoxGUI;
using EquinoxsModUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LaserLogistics.Controls
{
    public class LaserNodeWindow : Window
    {
        public override void Draw() {
            if (!Visible) return;

            if (FreeCursor) EMU.FreeCursor(true);
            if (ShowShader) DrawShader();

            float xPos = (Screen.width - Images.LaserNodeGUI.background.width) / 2.0f;
            float yPos = Screen.height - Images.LaserNodeGUI.background.height;

            Images.LaserNodeGUI.background.Draw(xPos, yPos);

            if (ShowBackground) DrawBackground();
            if (!ShowTitle && !ShowCloseButton) {
                RootLayout.Margin.Top = 10;
            }

            rootGrid.Draw();
        }
    }
}
