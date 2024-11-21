using BepInEx;
using CasperEquinoxGUI;
using CasperEquinoxGUI.Controls;
using CasperEquinoxGUI.Layouts;
using CasperEquinoxGUI.Utilities;
using EquinoxsDebuggingTools;
using EquinoxsModUtils;
using LaserLogistics.Controls;
using LaserLogistics.LaserLogisticsGUI.Controls;
using LaserLogistics.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RootMotion.FinalIK.Grounding;

namespace LaserLogistics.LaserLogisticsGUI.LaserNodeWindow.Panels
{
    public class MainPanel : Panel
    {
        // Members
        private ModuleButton[] moduleButtons = new ModuleButton[8];

        // Constructors

        public MainPanel(ref Grid parentGrid)
        {
            Margin = new Thickness(10, 0, 10, 10);
            Grid grid = new Grid(1, 6, "equal", new string[] { "equal", "40", "equal", "10", "40", "40" });

            // Titles Row

            grid.AddControl(new TextBlock() {
                Text = "Buffer",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(5, 2, 0, 0)
            });

            grid.AddControl(new TextBlock() {
                Text = "Upgrades",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 2, 110, 0)
            });

            // Buffer And Upgrades Row

            Grid bufferAndUpgrades = new Grid(8, 1, "44", "equal") {
                RowIndex = 1
            };

            bufferAndUpgrades.AddControl(new BufferIcon() { Margin = new Thickness(5, 0, 0, 0) });

            UpgradeButton speedUpgradeButton = new UpgradeButton(Names.Items.speedUpgrade) { ColumnIndex = 4, Margin = new Thickness(2, 0)};
            UpgradeButton stackUpgradeButton = new UpgradeButton(Names.Items.stackUpgrade) { ColumnIndex = 5, Margin = new Thickness(2, 0)};
            UpgradeButton rangeUpgradeButton = new UpgradeButton(Names.Items.rangeUpgrade) { ColumnIndex = 6, Margin = new Thickness(2, 0)};
            UpgradeButton infiniteRangeUpgradeButton = new UpgradeButton(Names.Items.infiniteRangeUpgrade) { ColumnIndex = 7, Margin = new Thickness(2, 0)};

            speedUpgradeButton.LeftClicked += OnUpgradeButtonClicked;
            stackUpgradeButton.LeftClicked += OnUpgradeButtonClicked;
            rangeUpgradeButton.LeftClicked += OnUpgradeButtonClicked;
            infiniteRangeUpgradeButton.LeftClicked += OnUpgradeButtonClicked;

            bufferAndUpgrades.AddControl(speedUpgradeButton);
            bufferAndUpgrades.AddControl(stackUpgradeButton);
            bufferAndUpgrades.AddControl(rangeUpgradeButton);
            bufferAndUpgrades.AddControl(infiniteRangeUpgradeButton);

            grid.AddControl(bufferAndUpgrades);

            // Modules Title And Arrow

            grid.AddControl(new TextBlock() {
                RowIndex = 2,
                Text = "Modules",
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            });

            grid.AddControl(new Image() {
                RowIndex = 3,
                ImageToRender = Images.LaserNodeGUI.bigArrow.texture2d,
                Width = 340,
                Height = 8,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(5, 0, 0, 2)
            });

            // Modules

            Grid modulesGrid = new Grid(8, 1, "44", "equal") {
                RowIndex = 4
            };

            for (int i = 0; i < 8; i++) {
                ModuleButton button = new ModuleButton(i) { Margin = new Thickness(2, 0) };
                button.LeftClicked += OnModuleButtonClicked;
                modulesGrid.AddControl(button);
                moduleButtons[i] = button;
            }

            grid.AddControl(modulesGrid);

            // Config Buttons

            Grid configGrid = new Grid(8, 1, "44", "equal") { RowIndex = 5 };
            for (int i = 0; i < 8; i++) {
                ConfigButton button = new ConfigButton(i);
                button.LeftClicked += OnConfigButtonClicked;
                configGrid.AddControl(button);
            }

            grid.AddControl(configGrid);

            Layout = grid;
            parentGrid.AddControl(this);
        }

        // Events

        private void OnUpgradeButtonClicked(object sender, EventArgs e) {
            if (NewLaserNodeGUI.IsItemInHand) {
                if(NewLaserNodeGUI.currentNode.CanAddUpgrade(NewLaserNodeGUI.itemInHand.displayName, out _)) {
                    NewLaserNodeGUI.currentNode.AddUpgrade(NewLaserNodeGUI.itemInHand.displayName);
                    NewLaserNodeGUI.itemInHand = null;
                }
            }
            else {
                UpgradeButton clickedButton = sender as UpgradeButton;
                NewLaserNodeGUI.currentNode.RemoveUpgrade(clickedButton.ResourceName, out _);
                NewLaserNodeGUI.itemInHand = clickedButton.Resource;
            }

            NewLaserNodeGUI.mainPanel.Refresh();
            NewLaserNodeGUI.statsColourPanel.Refresh();
        }

        private void OnModuleButtonClicked(object sender, EventArgs e) {
            if (NewLaserNodeGUI.sSinceGuiInteract < 0.2f) return;
            NewLaserNodeGUI.sSinceGuiInteract = 0f;

            int slotIndex = (sender as ModuleButton).slotIndex;
            bool isModuleInSlot = NewLaserNodeGUI.currentNode.IsModuleInSlot(slotIndex);

            if (NewLaserNodeGUI.IsItemInHand && !isModuleInSlot) {
                NewLaserNodeGUI.currentNode.AddModule(NewLaserNodeGUI.itemInHand.displayName, slotIndex);
                NewLaserNodeGUI.itemInHand = null;
                EDT.Log("GUI", $"Placed {NewLaserNodeGUI.itemInHand} in slot #{slotIndex}");
                NewLaserNodeGUI.PlayAudio("event:/SFX/UI SFX/Building UI SFX/Build Click");
            }

            else if (!NewLaserNodeGUI.IsItemInHand && isModuleInSlot) {
                Module module = NewLaserNodeGUI.currentNode.modules[slotIndex];

                NewLaserNodeGUI.currentNode.activeSlots.Remove(slotIndex);
                NewLaserNodeGUI.currentNode.modules[slotIndex] = null;
                NewLaserNodeGUI.currentModule = null;

                EDT.Log("GUI", $"Removed {module.name} in slot #{slotIndex}");

                NewLaserNodeGUI.PlayAudio("event:/SFX/UI SFX/Building UI SFX/Build Error");

                if (UnityInput.Current.GetKeyDown(KeyCode.LeftShift)) {
                    Player.instance.inventory.AddResources(EMU.Resources.GetResourceIDByName(module.name), 1);
                }
                else {
                    NewLaserNodeGUI.itemInHand = EMU.Resources.GetResourceInfoByName(module.name);
                }
            }

            NewLaserNodeGUI.currentNode.Save();
            Refresh();
        }

        private void OnConfigButtonClicked(object sender, EventArgs e) {
            int slotIndex = (int)(sender as ConfigButton).ColumnIndex;
            NewLaserNodeGUI.currentModule = NewLaserNodeGUI.currentNode.modules[slotIndex];
            NewLaserNodeGUI.PlayAudio("event:/SFX/UI SFX/Main Menu_Click Option");
            NewLaserNodeGUI.savedPositionsPanel.Refresh();
            NewLaserNodeGUI.moduleSettingsPanel.Refresh();
        }

        // Private Functions

        public void Refresh() {
            for (int i = 0; i < 8; i++) {
                if (!NewLaserNodeGUI.currentNode.IsModuleInSlot(i)) {
                    moduleButtons[i].Resource = null;
                }
                else {
                    moduleButtons[i].Resource = EMU.Resources.GetResourceInfoByName(NewLaserNodeGUI.currentNode.modules[i].name);
                }
            }
        }
    }
}
