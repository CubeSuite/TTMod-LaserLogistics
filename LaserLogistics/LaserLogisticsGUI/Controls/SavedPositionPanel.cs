using CasperEquinoxGUI;
using CasperEquinoxGUI.Controls;
using CasperEquinoxGUI.Layouts;
using CasperEquinoxGUI.Utilities;
using FIMSpace.Generating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LaserLogistics.Controls
{
    public class SavedPositionPanel : Panel
    {
        // Members
        public uint instanceId;
        public event EventHandler MoveClicked;
        public event EventHandler DeleteClicked;

        // Properties
        private static PositionMemoryTablet pmt => PositionMemoryTablet.instance;

        // Constructors

        public SavedPositionPanel(uint instanceId, bool showMove = true) {
            this.instanceId = instanceId;
            Grid grid = new Grid(3, 1, new string[] { "equal", "40", "40" }, "equal");

            if (!MachineManager.instance.GetRefFromId(instanceId, out IMachineInstanceRef machineRef)) return;

            string name = machineRef.builderInfo.displayName;
            Vector3 machinePos = machineRef.gridInfo.Center;
            Vector3Int machinePosRounded = new Vector3Int(
                Mathf.RoundToInt(machinePos.x),
                Mathf.RoundToInt(machinePos.y),
                Mathf.RoundToInt(machinePos.z)
            );

            grid.AddControl(new TextBlock() {
                Text = $"{name} @ {machinePosRounded}",
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Center
            });

            if(NewLaserNodeGUI.currentModule != null && showMove) {
                Button moveButton = new Button(Images.LaserNodeGUI.Icons.move.texture2d) {
                    ColumnIndex = 1,
                    Margin = new Thickness(5),
                    Padding = new Thickness(5)
                };
                moveButton.LeftClicked += OnMoveClicked;
                grid.AddControl(moveButton);
            }

            Button deleteButton = new Button(Images.LaserNodeGUI.Icons.delete.texture2d) {
                ColumnIndex = 2,
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            deleteButton.LeftClicked += OnDeleteClicked;
            grid.AddControl(deleteButton);

            Layout = grid;
        }

        // Events

        private void OnMoveClicked(object sender, EventArgs e) {
            MoveClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnDeleteClicked(object sender, EventArgs e) {
            pmt.savedMachines.Remove(instanceId);
            DeleteClicked?.Invoke(this, EventArgs.Empty);
        }

        // Overrides

        protected override int CalculateContentHeight() {
            return 40;
        }
    }
}
