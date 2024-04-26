using BoneLib.BoneMenu;
using BoneLib.BoneMenu.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SLZ.Marrow.Pool;
using SLZ.Marrow.Warehouse;
using MoreItemsInDevTools;
using BoneLib.Notifications;

namespace MoreItemsInDevTools
{
    internal class Bonemenu
    {
        private static MenuCategory _mainCategory;
        private static MenuCategory _presetCategory;
        public static PresetManager _presetManager = new PresetManager();
        public static void BonemenuSetup() 
        {
            _presetManager.OnStart();
            _mainCategory = MenuManager.CreateCategory("MoreItemsInDevTools", "#f6f6f6");
            var hotloadButton = _mainCategory.CreateFunctionElement("Reload Presets from file", Color.yellow, () => { _presetManager.LoadPresets(); RebuildBonemenu(); });
            #if DEBUG
            Main.MelonLog.Msg("Setting up Bonemenu Category");
            #endif
        }

        public static void BoneMenuNotif(BoneLib.Notifications.NotificationType type, string content)
        {
            var notif = new BoneLib.Notifications.Notification
            {
                Title = "MoreItemsInDevTools",
                Message = content,
                Type = type,
                PopupLength = 3,
                ShowTitleOnPopup = true
            };
            BoneLib.Notifications.Notifier.Send(notif);

            #if DEBUG
            Main.MelonLog.Msg("Sent notification \"" + content +"\"");
            #endif

        }

        public static void RebuildBonemenu()
        {
            if (_presetCategory != null) 
            {
                _mainCategory.RemoveElement(_presetCategory);
            }

            CheckForDefaultPreset();

            _presetCategory = _mainCategory.CreateCategory("Presets", Color.white);

            var createNewPresetButton = _presetCategory.CreateFunctionElement("Create New Preset", Color.green, () => 
            {
                int presetAppend = _presetManager.GetNumberOfPresets();
                string presetName = "Preset " + presetAppend;
               
                _presetManager.LoadPresets();
                var success = _presetManager.CreateNewPreset(presetName);
                if (success)
                {
#if DEBUG
                    Main.MelonLog.Msg("Created New Preset (button press) with name " + presetName);
#endif
                    CreatePresetCatagory(_presetCategory, presetName);

                }
            });

            foreach (KeyValuePair<string, PresetData> preset in _presetManager.presets)
            {
                CreatePresetCatagory(_presetCategory, preset.Key);
            }

        }
        public static void CreatePresetCatagory(MenuCategory category, string presetName)
        {
#if DEBUG
            Main.MelonLog.Msg("Creating new Preset Catagory with name " + presetName);
#endif
            var _category = category.CreateCategory(presetName, Color.white);

            var presetData = _presetManager.GetPresetData(presetName);

            var nameElement = _category.CreateFunctionElement(presetName, Color.white, () => { });
            var applyButton = _category.CreateFunctionElement("Apply Preset", Color.cyan, () => 
            {
                _presetManager.LoadPresets();
                var Items = _presetManager.GetPresetData(presetName).Barcodes.ToArray();
                Main.SetCheatMenuItems(Items);
            });
            var addItemButton = _category.CreateFunctionElement("Add Item", Color.green, () => 
            {
                string errortext = "";
                try 
                {
                    if (BoneLib.Player.GetObjectInHand(BoneLib.Player.leftHand) == null)
                    { errortext = "Error: Nothing in left hand."; throw new Exception();  }
                    if (BoneLib.Player.GetComponentInHand<AssetPoolee>(BoneLib.Player.leftHand) == null)
                    { errortext = "Error: Object is not a spawnable, or is a prefab."; throw new Exception(); }
                    
                    string barcode = BoneLib.Player.GetComponentInHand<AssetPoolee>(BoneLib.Player.leftHand).spawnableCrate.Barcode;

                    _presetManager.AddBarcodeToPreset(presetName, barcode);
                    CreatePBarcodeCatagory(_category, barcode);
                    _presetManager.SavePresets();
                }
                catch (Exception)
                {
                    BoneMenuNotif(NotificationType.Error, errortext);
                }
            }   
            );
            FunctionElement removePresetButton;
            if (presetName != "DEFAULT")
            {
                removePresetButton = _category.CreateFunctionElement("Remove Preset", Color.red, () =>
                {
                    _presetManager.RemovePreset(presetName);
                    category.RemoveElement(_category);
                    _presetManager.SavePresets();
                    MenuManager.SelectCategory(category);

                });
            }
            // Create pre-existing preset buttons
            var barcodes = _presetManager.GetPresetData(presetName).Barcodes;
            foreach (string barcode in barcodes)
            { 
                CreatePBarcodeCatagory(_category, barcode);
            }


        }

        public static void CreatePBarcodeCatagory(MenuCategory category, string Barcode)
        {
            var Title = AssetWarehouse.Instance.GetCrate<GameObjectCrate>(Barcode).Title;
            var _category = category.CreateCategory(Title, Color.white);

#if DEBUG
            Main.MelonLog.Msg("Creating new Preset Item element with Title "+Title+" and barcode "+Barcode);
#endif

            var titleElement = _category.CreateFunctionElement(Title, Color.white, () => { });
            var barcodeElement = _category.CreateFunctionElement(Barcode, Color.white, () => { });
            var removeButtom = _category.CreateFunctionElement("Remove Item", Color.red, () => 
            {
                _presetManager.RemoveBarcodeFromPreset(category.Name, Barcode);
                category.RemoveElement(_category);
                MenuManager.SelectCategory(category);
            });
        }

        public static void CheckForDefaultPreset()
        {
            if (!_presetManager.presets.ContainsKey("DEFAULT"))
            {
                BoneMenuNotif(BoneLib.Notifications.NotificationType.Error, "Error: No DEFAULT preset detected! Attempting to create a new one \n Organization of the JSON file might be mangled!");
                _presetManager.CreateNewPreset("DEFAULT");
            }
        }
    }
}
