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
    public class ItemsPanel : Panel
    {
        // Members
        public static readonly Dictionary<string, string[]> itemCategories = new Dictionary<string, string[]>() {
            {"Intermediates", new string[]{
                "Raw Materials",
                "Ingots",
                "Parts",
                "Threshed Resources",
                "Advanced Parts",
                "Refined Powders",
                "Materials",
                "Utility"}
            },
            {"Logistics", new string[] {
                "Conveyors",
                "Inserters",
                "Power",
                "Utility"}
            },
            {"Production", new string[]{
                "Assemblers",
                "Resource Extraction",
                "Plants",
                "Power",
                "Research",
                "Smelting"}
            },
            {"Base Building", new string[] {
                "Floors",
                "Walls",
                "Stairs",
                "Lighting",
                "Barriers",
                "Decoration",
                "Supports",
                "Utility"}
            }
        };

        // Properties
        private string titleRowHeights => NewLaserNodeGUI.titleRowHeights;

        // Constructors

        public ItemsPanel(ref Grid parentGrid)
        {
            Margin = new Thickness(10, 10, 0, 0);
            ColumnIndex = 1;

            Grid topGrid = new Grid(1, 2, "equal", new string[] { titleRowHeights, "equal" }) { Margin = new Thickness(5) };

            topGrid.AddControl(new TitleLabel("Items"));

            StackPanel stack = new StackPanel() {
                RowIndex = 1,
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0, 10, 0, 0)
            };

            foreach (KeyValuePair<string, string[]> pair in itemCategories) {
                foreach (string subHeader in pair.Value) {
                    stack.AddControl(new ItemsSubPanel(pair.Key, subHeader) { Margin = new Thickness(0, 0, 0, 10) });
                }
            }

            topGrid.AddControl(stack);

            Layout = topGrid;
            parentGrid.AddControl(this);
        }
    }
}
