using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TLCGen.Helpers
{
    public class VlogConfig
    {
        public string SystemName { get; set; }
        public Dictionary<int, string> Detectors { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, string> DummyDetectors { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, string> Inputs { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, string> SignalGroups { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, string> Outputs { get; set; } = new Dictionary<int, string>();
    }

    public static class VlogConfigParser
    {
        public static VlogConfig Parse(string filePath)
        {
            var config = new VlogConfig();
            
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("VLOGCFG file not found.", filePath);
            }

            var lines = File.ReadAllLines(filePath);
            
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                    continue;

                var parts = line.Split(',');
                if (parts.Length < 2) continue;

                var type = parts[0].Trim();
                
                if (type == "SYS")
                {
                    config.SystemName = parts[1].Trim('"');
                    continue;
                }

                if (parts.Length < 3) continue;

                if (int.TryParse(parts[1], out int index))
                {
                    string name = parts[2].Trim('"');
                    
                    switch (type)
                    {
                        case "DP":
                            config.Detectors[index] = name;
                            break;
                        case "DS":
                            config.DummyDetectors[index] = name;
                            break;
                        case "IS":
                            config.Inputs[index] = name;
                            break;
                        case "FC":
                            config.SignalGroups[index] = name;
                            break;
                        case "US":
                            config.Outputs[index] = name;
                            break;
                    }
                }
            }
            
            return config;
        }

        /// <summary>
        /// Reorders an IList based on the parsed VLOGCFG order.
        /// Existing items found in the config keep their index, and any newly added items are placed at the end.
        /// </summary>
        public static void ApplyVlogOrder<T>(IList<T> tlcGenItems, Dictionary<int, string> vlogOrder, Func<T, string> nameSelector)
        {
            var orderedList = new List<T>();
            var remainingItems = new List<T>(tlcGenItems);

            var orderedKeys = vlogOrder.Keys.OrderBy(k => k).ToList();

            // Pick items that are documented in the VLOG config and maintain their correct order
            foreach (var key in orderedKeys)
            {
                var expectedName = vlogOrder[key];
                var item = remainingItems.Find(x => nameSelector(x) == expectedName);
                if (item != null)
                {
                    orderedList.Add(item);
                    remainingItems.Remove(item);
                }
            }

            // Append any new or remaining items behind the sorted elements
            orderedList.AddRange(remainingItems);

            // Update the original collection (maintains ObservableCollection data binding properly)
            tlcGenItems.Clear();
            foreach (var item in orderedList)
            {
                tlcGenItems.Add(item);
            }
        }
    }
}