using EquinoxsDebuggingTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LaserLogistics
{
    internal static class PositionMemoryTabletGUI
    {
        // Objects & Variables
        private static PositionMemoryTablet pmt => PositionMemoryTablet.instance;
        private static float xPosition;
        private static float yPosition;
        private static float guiStartX;
        private static float guiStartY;

        private static GUIStyle labelStyle;

        // Internal Functions

        internal static void Draw() {
            if (labelStyle == null) InitialiseStyles();
            if (!IsHeld()) return;
            if (UIManager.instance.anyMenuOpen) return;

            xPosition = Screen.width - Images.PMTGUI.tablet.width;
            yPosition = Screen.height - Images.PMTGUI.tablet.height;
            guiStartX = xPosition + 29;
            guiStartY = yPosition + 29;

            DrawBackground();
            DrawEntries();
        }

        // Draw Functions

        private static void DrawBackground() {
            Images.PMTGUI.tablet.Draw(xPosition, yPosition);
            Images.PMTGUI.background.Draw(guiStartX, guiStartY);
            Images.PMTGUI.thumb.Draw(xPosition, yPosition);
        }

        private static void DrawEntries() {
            for (int i = 0; i < pmt.savedMachines.Count; i++) {
                uint instanceId = pmt.savedMachines[i];

                if (!MachineManager.instance.GetRefFromId(instanceId, out IMachineInstanceRef machineRef)) return;

                string name = machineRef.builderInfo.displayName;
                Vector3 machinePos = machineRef.gridInfo.Center;
                Vector3Int machinePosRounded = new Vector3Int(
                    Mathf.RoundToInt(machinePos.x),
                    Mathf.RoundToInt(machinePos.y),
                    Mathf.RoundToInt(machinePos.z)
                );

                float yPos = yPosition + (i * (Images.PMTGUI.entry.height + 5)) + 77;
                Images.PMTGUI.entry.Draw(guiStartX + 10, yPos, 428, 25);
                EDT.PacedLog("GUI.PMT", $"Drawing entry #{i} at {Images.PMTGUI.entry.rect}");
                GUI.Label(new Rect(guiStartX + 20, yPos, 418, 25), $"{name} @ {machinePosRounded}", labelStyle);
            }
        }

        // Private Functions

        private static void InitialiseStyles() {
            labelStyle = new GUIStyle() {
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Color.yellow }
            };
        }

        private static bool IsHeld() {
            if (Player.instance.toolbar.selectedInfo == null) return false;
            return Player.instance.toolbar.selectedInfo.displayName == Names.Items.pmt;
        }
    }
}
