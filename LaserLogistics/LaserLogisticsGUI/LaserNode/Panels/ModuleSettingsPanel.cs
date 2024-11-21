using CasperEquinoxGUI;
using CasperEquinoxGUI.Controls;
using CasperEquinoxGUI.Layouts;
using CasperEquinoxGUI.Utilities;
using EquinoxsModUtils;
using FluffyUnderware.Curvy;
using LaserLogistics.Controls;
using LaserLogistics.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LaserLogistics.LaserLogisticsGUI.LaserNode.Panels
{
    public class ModuleSettingsPanel : Panel
    {
        // Members
        private TextBlock filterModeLabel;
        private Button switchModeButton;
        private FilterButton[] filterButtons = new FilterButton[8];
        private TextBlock noModuleWarningLabel;
        private Grid mainGrid;
        private StackPanel targetsPanel;
        private Grid noModuleGrid;

        // Properties
        private string titleRowHeights => NewLaserNodeGUI.titleRowHeights;

        private bool _showNoModuleGrid = true;
        private bool ShowNoModuleGrid {
            get => _showNoModuleGrid;
            set {
                if(_showNoModuleGrid != value) {
                    _showNoModuleGrid = value;
                    Layout = value ? noModuleGrid : mainGrid;
                }
            }
        }

        // Constructors

        public ModuleSettingsPanel(ref Grid parentGrid)
        {
            ColumnIndex = 2;
            mainGrid = new Grid(1, 5, "equal", new string[] { titleRowHeights, "40", "45", titleRowHeights, "equal" }) { Margin = new Thickness(5) };

            mainGrid.AddControl(new TitleLabel("Module Filters"));

            filterModeLabel = new TextBlock() {
                Text = "Current Mode:",
                WrapText = false,
                RowIndex = 1,
                VerticalAlignment = VerticalAlignment.Center
            };

            mainGrid.AddControl(filterModeLabel);

            switchModeButton = new Button("Switch Mode") {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(5),
                RowIndex = 1
            };

            switchModeButton.LeftClicked += OnSwitchFilterModeClicked;
            mainGrid.AddControl(switchModeButton);

            Grid filtersGrid = new Grid(8, 1, "equal", "equal") { RowIndex = 2 };
            for (int i = 0; i < 8; i++) {
                FilterButton button = new FilterButton(i);
                button.LeftClicked += OnFilterButtonClicked;
                filterButtons[i] = button;
                filtersGrid.AddControl(button);
            }

            mainGrid.AddControl(filtersGrid);

            mainGrid.AddControl(new TitleLabel("Module Targets") { RowIndex = 3 });

            targetsPanel = new StackPanel() { RowIndex = 4, Orientation = Orientation.Vertical };
            mainGrid.AddControl(targetsPanel);

            noModuleGrid = new Grid(1, 2, "equal", new string[] { titleRowHeights, "equal" });
            noModuleGrid.AddControl(new TitleLabel("Module Settings"));
            noModuleGrid.AddControl(new TextBlock() {
                Text = "Click a Module's configure button to see the settings for that module",
                RowIndex = 1,
                Margin = new Thickness(5, 10, 10, 10)
            });

            Layout = noModuleGrid;
            parentGrid.AddControl(this);
        }

        // Events

        private void OnSwitchFilterModeClicked(object sender, EventArgs e) {
            NewLaserNodeGUI.currentModule.ToggleFilterMode();
            NewLaserNodeGUI.PlayAudio("event:/SFX/UI SFX/Main Menu_Click Option");
            Refresh();
        }

        private void OnFilterButtonClicked(object sender, EventArgs e) {
            FilterButton clickedButton = sender as FilterButton;
            
            if (!NewLaserNodeGUI.IsItemInHand) {
                Debug.Log("No item in hand");
                if (clickedButton.Resource != null) {
                    Debug.Log($"{clickedButton.ResourceName} in slot");
                    NewLaserNodeGUI.currentModule.filters.Remove(clickedButton.ResourceName);
                    Refresh();
                    Debug.Log("Removed item from slot");
                    Debug.Log($"module now has {NewLaserNodeGUI.currentModule.filters.Count} modules: {string.Join(",", NewLaserNodeGUI.currentModule.filters)}");
                }

                return;
            }

            string name = NewLaserNodeGUI.currentModule.name;
            int numFilters = NewLaserNodeGUI.currentModule.filters.Count;

            if (name == Names.Items.expanderModule && numFilters > 0) return;
            if (numFilters == 8) return;

            NewLaserNodeGUI.currentModule.filters.Add(NewLaserNodeGUI.itemInHand.displayName);
            NewLaserNodeGUI.itemInHand = null;
            NewLaserNodeGUI.PlayAudio("event:/SFX/UI SFX/Building UI SFX/Build Click");
            Refresh();
        }

        private void OnRemoveTargetClicked(object sender, EventArgs e) {
            SavedPositionPanel panel = sender as SavedPositionPanel;
            uint target = panel.instanceId;

            if (NewLaserNodeGUI.currentModule is PullerModule puller) puller.RemoveTarget(target);
            if (NewLaserNodeGUI.currentModule is PusherModule pusher) pusher.RemoveTarget(target);
            if (NewLaserNodeGUI.currentModule is CollectorModule collector) collector.RemoveTarget(target);
            if (NewLaserNodeGUI.currentModule is DistributorModule distributor) distributor.RemoveTarget(target);

            Refresh();
        }

        // Public Functions

        public void Refresh() {
            Module module = NewLaserNodeGUI.currentModule;
            if (module == null) return;

            filterModeLabel.Text = $"Current Mode: {module.filterMode}";

            switchModeButton.Visible = module.name != Names.Items.expanderModule;

            for(int i = 0; i < 8; i++) {
                if(i >= NewLaserNodeGUI.currentModule.filters.Count) {
                    filterButtons[i].Resource = null;
                }
                else {
                    filterButtons[i].Resource = EMU.Resources.GetResourceInfoByName(module.filters[i]);
                }
            }

            targetsPanel.ClearChildren();
            if (NewLaserNodeGUI.currentModule is PullerModule puller && puller.machineId != 0) ShowTarget(puller.machineId);
            if (NewLaserNodeGUI.currentModule is PusherModule pusher && pusher.machineId != 0) ShowTarget(pusher.machineId);
            if (NewLaserNodeGUI.currentModule is CollectorModule collector) collector.machineIds.ForEach(ShowTarget);
            if (NewLaserNodeGUI.currentModule is DistributorModule distributor) distributor.machineIds.ForEach(ShowTarget);
        }

        // Overrides

        public override void Draw() {
            ShowNoModuleGrid = NewLaserNodeGUI.currentModule == null;
            base.Draw();
        }

        // Private Functions

        private void ShowTarget(uint instanceId) {
            SavedPositionPanel panel = new SavedPositionPanel(instanceId, false) { Margin = new Thickness(0, 5, 0, 0)};
            panel.DeleteClicked += OnRemoveTargetClicked;
            targetsPanel.AddControl(panel);
        }
    }
}
