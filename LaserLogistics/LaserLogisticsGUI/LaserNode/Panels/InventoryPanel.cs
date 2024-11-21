using CasperEquinoxGUI.Controls;
using CasperEquinoxGUI;
using CasperEquinoxGUI.Layouts;
using CasperEquinoxGUI.Utilities;
using LaserLogistics.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserLogistics.LaserLogisticsGUI.LaserNode.Panels
{
    public class InventoryPanel : Panel
    {
        // Members

        // Properties
        private string titleRowHeights => NewLaserNodeGUI.titleRowHeights;

        // Constructors

        public InventoryPanel(ref Grid parentGrid)
        {
            Margin = new Thickness(0, 10, 0, 0);
            Grid topGrid = new Grid(1, 5, "equal", new string[] { titleRowHeights, "40", "60", "40", "70" }) { Margin = new Thickness(5) };

            topGrid.AddControl(new TitleLabel("Inventory"));
            topGrid.AddControl(new TextBlock() {
                Text = "Modules",
                VerticalAlignment = VerticalAlignment.Center,
                RowIndex = 1
            });

            CreateModulesInInventory(ref topGrid);

            topGrid.AddControl(new TextBlock() {
                Text = "Upgrades",
                VerticalAlignment = VerticalAlignment.Center,
                RowIndex = 3
            });

            CreateUpgradesInInventroy(ref topGrid);

            Layout = topGrid;
            parentGrid.AddControl(this);
        }

        // Events

        private void OnInventoryItemClicked(object sender, EventArgs e) {
            InventoryItemButton clickedButton = sender as InventoryItemButton;

            if (NewLaserNodeGUI.IsItemInHand) {
                Player.instance.inventory.AddResources(NewLaserNodeGUI.itemInHand, 1);
                NewLaserNodeGUI.itemInHand = null;
            }
            else if (clickedButton.lastQuantity > 0) {
                Player.instance.inventory.TryRemoveResources(clickedButton.Resource, 1);
                NewLaserNodeGUI.itemInHand = clickedButton.Resource;
            }
        }

        // Private Functions

        private void CreateModulesInInventory(ref Grid parentGrid) {
            StackPanel modulesPanel = new StackPanel() { Orientation = Orientation.Horizontal, RowIndex = 2 };

            InventoryItemButton pullerButton = new InventoryItemButton(Names.Items.pullerModule) { Margin = new Thickness(0, 0, 10, 0) };
            pullerButton.LeftClicked += OnInventoryItemClicked;
            modulesPanel.AddControl(pullerButton);

            InventoryItemButton pusherButton = new InventoryItemButton(Names.Items.pusherModule) { Margin = new Thickness(0, 0, 10, 0) };
            pusherButton.LeftClicked += OnInventoryItemClicked;
            modulesPanel.AddControl(pusherButton);

            InventoryItemButton collectorButton = new InventoryItemButton(Names.Items.collectorModule) { Margin = new Thickness(0, 0, 10, 0) };
            collectorButton.LeftClicked += OnInventoryItemClicked;
            modulesPanel.AddControl(collectorButton);

            InventoryItemButton distributorButton = new InventoryItemButton(Names.Items.distributorModule) { Margin = new Thickness(0, 0, 10, 0) };
            distributorButton.LeftClicked += OnInventoryItemClicked;
            modulesPanel.AddControl(distributorButton);

            InventoryItemButton voidButton = new InventoryItemButton(Names.Items.voidModule) { Margin = new Thickness(0, 0, 10, 0) };
            voidButton.LeftClicked += OnInventoryItemClicked;
            modulesPanel.AddControl(voidButton);

            InventoryItemButton compressorButton = new InventoryItemButton(Names.Items.compressorModule) { Margin = new Thickness(0, 0, 10, 0) };
            compressorButton.LeftClicked += OnInventoryItemClicked;
            modulesPanel.AddControl(compressorButton);

            InventoryItemButton expanderButton = new InventoryItemButton(Names.Items.expanderModule) { Margin = new Thickness(0, 0, 10, 0) };
            expanderButton.LeftClicked += OnInventoryItemClicked;
            modulesPanel.AddControl(expanderButton);

            parentGrid.AddControl(modulesPanel);
        }

        private void CreateUpgradesInInventroy(ref Grid parentGrid) {
            StackPanel upgradesPanel = new StackPanel() { Orientation = Orientation.Horizontal, RowIndex = 4 };

            InventoryItemButton speedUpgradeButton = new InventoryItemButton(Names.Items.speedUpgrade){ Margin = new Thickness(0, 0, 10, 0) };
            speedUpgradeButton.LeftClicked += OnInventoryItemClicked;
            upgradesPanel.AddControl(speedUpgradeButton);

            InventoryItemButton stackUpgradeButton = new InventoryItemButton(Names.Items.stackUpgrade) { Margin = new Thickness(0, 0, 10, 0) };
            stackUpgradeButton.LeftClicked += OnInventoryItemClicked;
            upgradesPanel.AddControl(stackUpgradeButton);

            InventoryItemButton rangeUpgradeButton = new InventoryItemButton(Names.Items.rangeUpgrade) { Margin = new Thickness(0, 0, 10, 0) };
            rangeUpgradeButton.LeftClicked += OnInventoryItemClicked;
            upgradesPanel.AddControl(rangeUpgradeButton);

            InventoryItemButton infiniteRangeUpgradeButton = new InventoryItemButton(Names.Items.infiniteRangeUpgrade) { Margin = new Thickness(0, 0, 10, 0) };
            infiniteRangeUpgradeButton.LeftClicked += OnInventoryItemClicked;
            upgradesPanel.AddControl(infiniteRangeUpgradeButton);

            parentGrid.AddControl(upgradesPanel);
        }
    }
}
