using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace MoreItemsInDevTools
{
    using BoneLib;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;

    // public static string SavePath = MelonUtils.UserDataDirectory;
    public class PresetData
    {
        public List<string> Barcodes { get; set; }
    }

    public class PresetManager
    {
        public Dictionary<string, PresetData> presets;
        private string filePath = $"{MelonUtils.UserDataDirectory}/MoreItemsInDevTools.json";
        public readonly Dictionary<string, PresetData> DefaultJsonDict = new Dictionary<string, PresetData>() { { "DEFAULT", new PresetData
                {
                    Barcodes = new List<string>
                    {
                        "c1534c5a-5747-42a2-bd08-ab3b47616467", "c1534c5a-6b38-438a-a324-d7e147616467", "c1534c5a-3813-49d6-a98c-f595436f6e73", "c1534c5a-c6a8-45d0-aaa2-2c954465764d"
                    }
                } 
            } 
        };


        public void OnStart() 
        {
            LoadPresets();
            SavePresets();
        }

        public void LoadPresets()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                presets = JsonConvert.DeserializeObject<Dictionary<string, PresetData>>(json);
            }
            else
            {
                presets = DefaultJsonDict;
            }
        }

        public void SavePresets()
        {
            string json = JsonConvert.SerializeObject(presets, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public void AddBarcodeToPreset(string presetName, string barcode)
        {
            if (presets.ContainsKey(presetName))
            {
                presets[presetName].Barcodes.Add(barcode);
                SavePresets();
            }
            else
            {
                Main.MelonLog.Error("Function AddBarcodeToPreset: Attempted to add a barcode to a preset that doesnt exist. (?????????)");
            }
        }

        public void RemoveBarcodeFromPreset(string presetName, string barcode)
        {
            if (presets.ContainsKey(presetName))
            {
                presets[presetName].Barcodes.Remove(barcode);
                SavePresets();
            }
            else
            {
                Main.MelonLog.Error("Function RemoveBarcodeToPreset: Attempted to remove a barcode to a preset that doesnt exist. (???).");
            }
        }

        public void CreateNewPreset(string presetName, List<string> barcodes)
        {
            if (!presets.ContainsKey(presetName))
            {
                PresetData newPreset = new PresetData
                {
                    Barcodes = barcodes
                };
                presets.Add(presetName, newPreset);
                SavePresets();
            }
            else
            {
                Main.MelonLog.Msg("Function CreateNewPreset: Attempted to create a new preset with a pre-existing name.");
                Bonemenu.BoneMenuNotif(BoneLib.Notifications.NotificationType.Error, "Attempted to create a new preset with a pre-existing name \n Please rename presets in a text editor before creating more.");
            }
        }

        public void RemovePreset(string presetName)
        {
            if (presets.ContainsKey(presetName))
            {
                presets.Remove(presetName);
                SavePresets();
            }
            else
            {
                Main.MelonLog.Msg("Preset not found.");
            }
        }

        public PresetData GetPresetData(string presetName)
        {
            if (presets.ContainsKey(presetName))
            {
                return presets[presetName];
            }
            else
            {
                Console.WriteLine("Preset not found.");
                return null;
            }
        }


        public int GetNumberOfPresets()
        {
            return presets.Count;
        }


    }

}
