using MelonLoader;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace MoreItemsInDevTools
{
    using BoneLib;
    using Il2CppInterop.Runtime.Runtime;
    using Newtonsoft.Json;
    using MelonLoader.Utils;
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
        private string filePath = $"{MelonEnvironment.UserDataDirectory}/MoreItemsInDevTools.json";
        public readonly Dictionary<string, PresetData> DefaultJsonDict = new Dictionary<string, PresetData>() { { "DEFAULT", new PresetData
                {
                    Barcodes = new List<string>
                    {
                        "c1534c5a-5747-42a2-bd08-ab3b47616467", "c1534c5a-6b38-438a-a324-d7e147616467", "c1534c5a-3813-49d6-a98c-f595436f6e73", "c1534c5a-c6a8-45d0-aaa2-2c954465764d"
                    }
                } 
            } 
        };


        /*
        public Il2CppSystem.Collections.Generic.Dictionary<string,PresetData> ConvertDictToIL2CppDict(Dictionary<string,PresetData> originDict)
        {
            var newDict = new Il2CppSystem.Collections.Generic.Dictionary<string, PresetData> { };
            foreach (KeyValuePair<string, PresetData> entry in originDict)
            {
                newDict.Add(entry.Key, entry.Value);
            }
            return newDict;
        }
        */
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
                presets = JsonConvert.DeserializeObject<Dictionary<string, PresetData>>(json); // This angers the compiler for some reason but just compile again and you'l be good.
#if DEBUG
                Main.MelonLog.Msg("Loaded JSON file.");
#endif
            }
            else
            {
                presets = DefaultJsonDict;
#if DEBUG
                Main.MelonLog.Msg("MoreItemsInDevTools.json didnt exist, creating it");
#endif
                SavePresets();
            }
        }

        public void SavePresets()
        {
            string json = JsonConvert.SerializeObject(presets, Formatting.Indented);
            File.WriteAllText(filePath, json);
#if DEBUG
            Main.MelonLog.Msg("Saved presets to file.");
#endif
        }

        public void AddBarcodeToPreset(string presetName, string barcode)
        {
            if (presets.ContainsKey(presetName))
            {
#if DEBUG
                Main.MelonLog.Msg("Saving " +barcode + " to preset "+ presetName);
#endif
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
#if DEBUG
                Main.MelonLog.Msg("Removing barcode "+barcode+ " from preset "+presetName);
#endif
                presets[presetName].Barcodes.Remove(barcode);
                SavePresets();
            }
            else
            {
                Main.MelonLog.Error("Function RemoveBarcodeToPreset: Attempted to remove a barcode to a preset that doesnt exist. (???).");
            }
        }

        public bool CreateNewPreset(string presetName)
        {
            if (!presets.ContainsKey(presetName))
            {
#if DEBUG
                Main.MelonLog.Msg("Creating new preset named "+presetName);
#endif
                
                presets.Add(presetName, new PresetData
                {
                    Barcodes = new List<string>
                    {
                        "c1534c5a-5747-42a2-bd08-ab3b47616467", "c1534c5a-6b38-438a-a324-d7e147616467", "c1534c5a-3813-49d6-a98c-f595436f6e73", "c1534c5a-c6a8-45d0-aaa2-2c954465764d"
                    }
                });
                SavePresets();
                return true;
            }
            else
            {
                Main.MelonLog.Msg("Function CreateNewPreset: Attempted to create a new preset with a pre-existing name.");
                Bonemenu.BoneMenuNotif(BoneLib.Notifications.NotificationType.Error, "Attempted to create a new preset with a pre-existing name \n Please rename presets in a text editor before creating more.");
                return false;
            }
        }

        public void RemovePreset(string presetName)
        {
            if (presets.ContainsKey(presetName))
            {
#if DEBUG
                Main.MelonLog.Msg("Removing preset" + presetName);
#endif
                presets.Remove(presetName);
                SavePresets();
            }
            else
            {
                Main.MelonLog.Msg("RemovePreset: Preset not found.");
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
                Main.MelonLog.Msg("GetPresetData: Preset not found.");
                return null;
            }
        }


        public int GetNumberOfPresets()
        {
            return presets.Count;
        }


    }

}
