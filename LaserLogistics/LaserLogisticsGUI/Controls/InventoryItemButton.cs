using CasperEquinoxGUI.Controls;
using CasperEquinoxGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LaserLogistics.Controls
{
    public class InventoryItemButton : ResourceButton
    {
        public InventoryItemButton(string resourceName) : base(resourceName) {
            Padding = new Thickness(10);
        }

        public int lastQuantity = -1;
        private GUIContent quantityContent;
        private Rect quantityLabelRect;

        private GUIStyle style;

        public override void Draw() {
            base.Draw();

            int quantity = Player.instance.inventory.GetResourceCount(ResourceID);
            if(quantity == 0) {
                Images.LaserNodeGUI.shader.Draw(PaddingRect.x - 1, PaddingRect.y - 1, 62, 62);
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
                float yPos = PaddingRect.yMax - size.y - 5;
                quantityLabelRect = new Rect(xPos, yPos, size.x, size.y);
            }

            GUI.Label(quantityLabelRect, quantityContent, style);
            lastQuantity = quantity;
        }

        protected override int CalculateContentWidth() {
            return 40;
        }

        protected override int CalculateContentHeight() {
            return 40;
        }
    }
}
