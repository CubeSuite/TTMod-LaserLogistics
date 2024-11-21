using CasperEquinoxGUI;
using CasperEquinoxGUI.Controls;
using CasperEquinoxGUI.Layouts;
using CasperEquinoxGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LaserLogistics.LaserLogisticsGUI.LaserNode.Panels
{
    public class StatsColourPanel : Panel
    {
        // Members
        
        private TextBlock powerLabel;
        private TextBlock taskDelayLabel;
        private TextBlock stackSizeLabel;
        private TextBlock rangeLabel;
        
        private Textbox redBox;
        private Textbox greenBox;
        private Textbox blueBox;

        // Properties
        private string titleRowHeights => NewLaserNodeGUI.titleRowHeights;

        // Constructors

        public StatsColourPanel(ref Grid parentGrid)
        {
            ShowBackground = false;
            RowIndex = 1;
            Grid grid = new Grid(2, 1, new string[] { "250", "equal" }, "equal");

            // Stats Panel

            Panel statsPanel = new Panel() { Margin = new Thickness(10, 0) };
            Grid statsTopGrid = new Grid(1, 2, "equal", new string[] { titleRowHeights, "equal" }) { Margin = new Thickness(5) };
            statsTopGrid.AddControl(new TitleLabel("Stats"));

            Grid statsBottomGrid = new Grid(2, 4, new string[] { "equal", "50" }, "equal") { RowIndex = 1 };
            statsBottomGrid.AddControl(new TextBlock() {
                Text = "Power Consumption:",
                RowIndex = 0,
                VerticalAlignment = VerticalAlignment.Center
            });
            statsBottomGrid.AddControl(new TextBlock() {
                Text = "Task Delay:",
                RowIndex = 1,
                VerticalAlignment = VerticalAlignment.Center
            });
            statsBottomGrid.AddControl(new TextBlock() {
                Text = "Stack Size:",
                RowIndex = 2,
                VerticalAlignment = VerticalAlignment.Center
            });
            statsBottomGrid.AddControl(new TextBlock() {
                Text = "Range:",
                RowIndex = 3,
                VerticalAlignment = VerticalAlignment.Center
            });

            powerLabel = new TextBlock() { RowIndex = 0, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
            taskDelayLabel = new TextBlock() { RowIndex = 1, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
            stackSizeLabel = new TextBlock() { RowIndex = 2, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
            rangeLabel = new TextBlock() { RowIndex = 3, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };

            statsBottomGrid.AddControl(powerLabel);
            statsBottomGrid.AddControl(taskDelayLabel);
            statsBottomGrid.AddControl(stackSizeLabel);
            statsBottomGrid.AddControl(rangeLabel);

            statsTopGrid.AddControl(statsBottomGrid);

            statsPanel.Layout = statsTopGrid;
            grid.AddControl(statsPanel);

            // Colour Panel

            Panel colourPanel = new Panel() { Margin = new Thickness(0, 0, 10, 0), ColumnIndex = 1 };
            Grid colourTopGrid = new Grid(1, 2, "equal", new string[] { titleRowHeights, "equal" }) { Margin = new Thickness(5) };
            colourTopGrid.AddControl(new TitleLabel("Colour"));

            Grid colourBottomGrid = new Grid(2, 3, new string[] { "equal", "50" }, "equal") { RowIndex = 1 };
            colourBottomGrid.AddControl(new TextBlock() {
                Text = "Red:",
                RowIndex = 0,
                VerticalAlignment = VerticalAlignment.Center
            });
            colourBottomGrid.AddControl(new TextBlock() {
                Text = "Green:",
                RowIndex = 1,
                VerticalAlignment = VerticalAlignment.Center
            });
            colourBottomGrid.AddControl(new TextBlock() {
                Text = "Blue:",
                RowIndex = 2,
                VerticalAlignment = VerticalAlignment.Center
            });

            redBox = new Textbox() { ColumnIndex = 1, RowIndex = 0, Margin = new Thickness(0, 2), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            greenBox = new Textbox() { ColumnIndex = 1, RowIndex = 1, Margin = new Thickness(0, 2), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            blueBox = new Textbox() { ColumnIndex = 1, RowIndex = 2, Margin = new Thickness(0, 2), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };

            redBox.TextChanged += OnColourBoxTextChanged;
            greenBox.TextChanged += OnColourBoxTextChanged;
            blueBox.TextChanged += OnColourBoxTextChanged;

            colourBottomGrid.AddControl(redBox);
            colourBottomGrid.AddControl(greenBox);
            colourBottomGrid.AddControl(blueBox);

            colourTopGrid.AddControl(colourBottomGrid);
            colourPanel.Layout = colourTopGrid;
            grid.AddControl(colourPanel);

            Layout = grid;
            parentGrid.AddControl(this);
        }

        // Events

        private void OnColourBoxTextChanged(object sender, EventArgs e) {
            if (int.TryParse(redBox.Input, out int red)) {
                red = Mathf.Clamp(red, 0, 255);
                redBox.Input = red.ToString();
                NewLaserNodeGUI.currentNode.Red = red;
            }

            if (int.TryParse(greenBox.Input, out int green)) {
                green = Mathf.Clamp(green, 0, 255);
                greenBox.Input = green.ToString();
                NewLaserNodeGUI.currentNode.Green = green;
            }

            if (int.TryParse(blueBox.Input, out int blue)) {
                blue = Mathf.Clamp(blue, 0, 255);
                blueBox.Input = blue.ToString();
                NewLaserNodeGUI.currentNode.Blue = blue;
            }
        }

        // Public Functions

        public void Refresh() {
            RefreshStats();
            RefreshColour();
        }

        // Private Functions

        public void RefreshStats() {
            powerLabel.Text = $"{NewLaserNodeGUI.currentNode.powerConsumption} kW";
            taskDelayLabel.Text = $"{NewLaserNodeGUI.currentNode.GetTaskDelay()} s";
            stackSizeLabel.Text = $"{NewLaserNodeGUI.currentNode.GetStackSize()}";

            int range = NewLaserNodeGUI.currentNode.GetRange();
            rangeLabel.Text = range == int.MaxValue ? "Inf." : $"{range} m";
        }

        public void RefreshColour() {
            redBox.Input = NewLaserNodeGUI.currentNode.Red.ToString();
            greenBox.Input = NewLaserNodeGUI.currentNode.Green.ToString();
            blueBox.Input = NewLaserNodeGUI.currentNode.Blue.ToString();
        }
    }
}
