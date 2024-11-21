using CasperEquinoxGUI.Controls;
using CasperEquinoxGUI;
using CasperEquinoxGUI.Layouts;
using CasperEquinoxGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaserLogistics.Controls;
using LaserLogistics.Modules;
using System.ComponentModel;
using FluffyUnderware.Curvy;

namespace LaserLogistics.LaserLogisticsGUI.LaserNodeWindow.Panels
{
    public class SavedPositionsPanel : Panel
    {
        // Members
        private StackPanel savedPositionsStackPanel;
        
        // Properties
        private string titleRowHeights => NewLaserNodeGUI.titleRowHeights;
        private static PositionMemoryTablet pmt => PositionMemoryTablet.instance;

        // Constructors

        public SavedPositionsPanel(ref Grid parentGrid)
        {
            Grid subGrid = new Grid(1, 2, "equal", new string[] { titleRowHeights, "equal" }) {
                Margin = new Thickness(5)
            };

            subGrid.AddControl(new TitleLabel("Saved Positions"));
            
            savedPositionsStackPanel = new StackPanel() {
                RowIndex = 1,
                Margin = new Thickness(5),
                Orientation = Orientation.Vertical
            };

            subGrid.AddControl(savedPositionsStackPanel);

            Layout = subGrid;
            parentGrid.AddControl(this);
        }

        // Events

        private void OnPMTEntryMoveClicked(object sender, EventArgs e) {
            SavedPositionPanel panel = sender as SavedPositionPanel;
            uint instanceId = panel.instanceId;

            if (!NewLaserNodeGUI.currentNode.IsMachineInRange(instanceId)) {
                NewLaserNodeGUI.PlayAudio("event:/SFX/UI SFX/Building UI SFX/Build Error");
                NewLaserNodeGUI.sSinceRangeError = 0f;
                return;
            }

            if (NewLaserNodeGUI.currentModule is PullerModule puller) puller.SetTarget(instanceId);
            else if (NewLaserNodeGUI.currentModule is PusherModule pusher) pusher.SetTarget(instanceId);
            else if (NewLaserNodeGUI.currentModule is CollectorModule collector) collector.AddTarget(instanceId);
            else if (NewLaserNodeGUI.currentModule is DistributorModule distributor) distributor.AddTarget(instanceId);

            PositionMemoryTablet.instance.savedMachines.Remove(instanceId);
            NewLaserNodeGUI.PlayAudio("event:/SFX/UI SFX/Main Menu_Click Option");
            Refresh();
            NewLaserNodeGUI.currentNode.Save();
            NewLaserNodeGUI.moduleSettingsPanel.Refresh();
        }

        private void OnPMTEntryDeleteClicked(object sender, EventArgs e) {
            SavedPositionPanel panel = sender as SavedPositionPanel;
            PositionMemoryTablet.instance.savedMachines.Remove(panel.instanceId);
            NewLaserNodeGUI.PlayAudio("event:/SFX/UI SFX/Main Menu_Click Option");
            Refresh();
        }

        // Public Functions

        public void Refresh() {
            savedPositionsStackPanel.ClearChildren();
            for (int i = 0; i < pmt.savedMachines.Count; i++) {
                uint instanceId = pmt.savedMachines[i];
                SavedPositionPanel panel = new SavedPositionPanel(instanceId) {
                    Margin = new Thickness(10, 5)
                };
                panel.MoveClicked += OnPMTEntryMoveClicked;
                panel.DeleteClicked += OnPMTEntryDeleteClicked;
                savedPositionsStackPanel.AddControl(panel);
            }
        }
    }
}
