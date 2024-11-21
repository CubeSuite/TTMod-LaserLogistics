using CasperEquinoxGUI.Controls;
using CasperEquinoxGUI.Layouts;
using CasperEquinoxGUI.Utilities;
using EquinoxsModUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LaserLogistics.Controls
{
    public class ItemsSubPanel : Panel
    {
        private WrapPanel wrap;
        private static int[] blockedItems = new int[] { 255, 63, 116, 117, 118, 269, 192, 158, 159, 160, 163, 82, 20 };

        public ItemsSubPanel(string headerName, string subHeaderName)
        {
            SchematicsHeader header = EMU.Recipes.GetSchematicsHeaderByTitle(headerName);
            SchematicsSubHeader subHeader = EMU.Recipes.GetSchematicsSubHeaderByTitle(headerName, subHeaderName);

            Grid mainGrid = new Grid(1, 2, "equal", new string[] { "30", "60" }) { Margin = new Thickness(10) };
            mainGrid.AddControl(new TextBlock() { Text = $"{headerName} / {subHeaderName}" });

            List<ResourceInfo> items = GameDefines.instance.resources.Where(resource =>
                resource.headerType == subHeader && !resource.redacted && !blockedItems.Contains(resource.uniqueId)
            ).ToList();
            
            int height = 60 + (70 * Mathf.FloorToInt((items.Count - 1) / 8f));
            mainGrid.RowHeights = new string[] { "30", height.ToString() };

            wrap = new WrapPanel() { RowIndex = 1 };
            foreach(ResourceInfo item in items) {
                if (item.redacted || string.IsNullOrEmpty(item.displayName)) continue;
                ResourceButton button = new ResourceButton(item) {
                    Margin = new Thickness(5),
                    ImageHeight = 40,
                    ImageWidth = 40,
                    Padding = new Thickness(10)
                };

                button.LeftClicked += OnResourceButtonClicked;
                wrap.AddControl(button);
            }

            mainGrid.AddControl(wrap);
            Layout = mainGrid;
        }

        private void OnResourceButtonClicked(object sender, EventArgs e) {
            ResourceButton button = sender as ResourceButton;
            if (NewLaserNodeGUI.IsItemInHand) {
                NewLaserNodeGUI.itemInHand = null;
            }
            else {
                NewLaserNodeGUI.itemInHand = button.Resource;
            }
        }

        protected override int CalculateContentWidth() {
            return ParentRect.width;
        }

        protected override int CalculateContentHeight() {
            return 60 + wrap?.ContentRect.height ?? 0;
        }
    }
}
