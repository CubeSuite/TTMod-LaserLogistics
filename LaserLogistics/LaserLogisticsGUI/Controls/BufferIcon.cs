using CasperEquinoxGUI.Controls;
using CasperEquinoxGUI.Layouts;
using EquinoxsModUtils;
using EquinoxsModUtils.Additions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LaserLogistics.LaserLogisticsGUI.Controls
{
    public class BufferIcon : Panel
    {
        // Overrides
        public override void Draw() {
            base.Draw();
            if (NewLaserNodeGUI.currentNode.buffer.isEmpty) return;

            string resName = SaveState.GetResInfoFromId(NewLaserNodeGUI.currentNode.buffer.id).displayName;
            GUI.Box(ContentRectFloat, "", new GUIStyle() { normal = { background = EMU.Images.GetImageForResource(resName) } });
            GUI.Label(new Rect(ContentRectFloat.x - 5, ContentRectFloat.y - 5, 40, 40), NewLaserNodeGUI.currentNode.buffer.count.ToString(), new GUIStyle() {
                alignment = TextAnchor.LowerRight,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.yellow }
            });
        }

        // ToDo: replace this shitstack with cached data
    }
}
