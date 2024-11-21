using CasperEquinoxGUI.Controls;
using CasperEquinoxGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LaserLogistics.LaserLogisticsGUI.Controls
{
    public class UpgradeButton : ResourceButton
    {
        public UpgradeButton(string resourceName) : base(resourceName) {
            ImageWidth = 40;
            ImageHeight = 40;
        }

        private int lastQuantity = -1;
        private GUIContent quantityContent;
        private Rect quantityLabelRect;

        private GUIStyle style;

        public override void Draw() {
            base.Draw();

            int quantity = GetQuantity();
            if (quantity == 0) {
                Images.LaserNodeGUI.shader.Draw(PaddingRect.x, PaddingRect.y, 40, 40);
            }

            if (style == null) {
                style = new GUIStyle() {
                    fontStyle = FontStyle.Bold,
                    fontSize = 20,
                    normal = { textColor = Color.yellow }
                };
            }

            if (quantity != lastQuantity) {
                quantityContent = new GUIContent(quantity.ToString());
                Vector2 size = style.CalcSize(quantityContent);
                float xPos = PaddingRect.xMax - size.x - 5;
                float yPos = PaddingRect.yMax - size.y;
                quantityLabelRect = new Rect(xPos, yPos, size.x, size.y);
            }

            GUI.Label(quantityLabelRect, quantityContent, style);
            lastQuantity = quantity;
        }

        // Private Functions

        private int GetQuantity() {
            switch (ResourceName) {
                case Names.Items.speedUpgrade: return NewLaserNodeGUI.currentNode.numSpeedUpgrades;
                case Names.Items.stackUpgrade: return NewLaserNodeGUI.currentNode.numStackUpgrades;
                case Names.Items.rangeUpgrade: return NewLaserNodeGUI.currentNode.numRangeUpgrades;
                case Names.Items.infiniteRangeUpgrade: return NewLaserNodeGUI.currentNode.infiniteRangeUpgrade ? 1 : 0;
                default: return 0;
            }
        }
    }
}
