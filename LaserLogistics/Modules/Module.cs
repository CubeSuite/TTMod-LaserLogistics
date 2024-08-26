using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LaserLogistics.Modules
{
    internal class Module
    {
        internal string name;
        internal List<string> filters = new List<string>();
        internal FilterMode filterMode = FilterMode.Blacklist;

        internal Module(string _name) {
            name = _name;

            if (name == Names.Items.expanderModule) {
                filterMode = FilterMode.Whitelist;
            }
        }

        internal void ToggleFilterMode() {
            if (name == Names.Items.expanderModule) return;

            if (filterMode == FilterMode.Blacklist) filterMode = FilterMode.Whitelist;
            else filterMode = FilterMode.Blacklist;
        }

        internal bool DoFilterTest(string name) {
            if (filterMode == FilterMode.Blacklist && filters.Contains(name)) return false;
            if (filterMode == FilterMode.Whitelist && !filters.Contains(name)) return false;

            return true;
        }

        internal bool DoFilterTest(int uniqueId) {
            return DoFilterTest(SaveState.GetResInfoFromId(uniqueId).displayName);
        }

        internal virtual string Serialise() {
            string filterList = string.Join(",", filters);
            return $"{name}/{filterList}/{filterMode}"; 
        }

        internal void LoadFilters(string serial) {
            string[] parts = serial.Split('/');

            if (!string.IsNullOrEmpty(parts[1])) {
                filters = parts[1].Split(',').ToList();
            }

            filterMode = (FilterMode)Enum.Parse(typeof(FilterMode), parts[2]);
        }

        internal bool CanAddFilter(string item) {
            if(filters.Contains(item)) return false;

            int limit = name == Names.Items.expanderModule ? 1 : 8;
            return filters.Count < limit;
        }

        internal virtual void RemoveTarget(uint id){ }
    }
    
    internal enum FilterMode
    {
        Whitelist,
        Blacklist
    }
}
