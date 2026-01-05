
using BoneLib.BoneMenu;
using BoneLib.Notifications;
using Il2CppSLZ.Marrow.Pool;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;
using MoreItemsInDevTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MoreItemsInDevTools
{
    internal class Bonemenu
    {
        private static BoneLib.BoneMenu.Page _mainCategory;
        private static BoneLib.BoneMenu.Page _presetCategory;
        private static BoneLib.BoneMenu.FunctionElement _presetCategoryLink;
        public static PresetManager _presetManager = new PresetManager();
        public static readonly MelonPreferences_Category GlobalCategory = MelonPreferences.CreateCategory("MoreItemsInDevTools");
        public static MelonPreferences_Entry<bool> ApplyDefaultOnLevelLoad { get; private set; }
        public static void BonemenuSetup()
        {
#if DEBUG
            Main.MelonLog.Msg("Setting up Bonemenu Category");
#endif
            _presetManager.OnStart();
            _mainCategory = BoneLib.BoneMenu.Page.Root.CreatePage("MoreItemsInDevTools", Color.white);
            var hotloadButton = _mainCategory.CreateFunction("Reload Presets from File", Color.yellow, () => { _presetManager.LoadPresets(); RebuildBonemenu(); });
            ApplyDefaultOnLevelLoad = GlobalCategory.GetEntry<bool>("ApplyDefaultOnLevelLoad") ?? GlobalCategory.CreateEntry("ApplyDefaultOnLevelLoad", true);

            var defaultBehaviourToggler = _mainCategory.CreateBool("Apply Default On Level Load:", Color.white, ApplyDefaultOnLevelLoad.Value, 
                (b) => 
                { 
                    ApplyDefaultOnLevelLoad.Value = b; 
                    GlobalCategory.SaveToFile(false); 
                }
            );
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
                Menu.DestroyPage(_presetCategory);
                _mainCategory.Remove(_presetCategoryLink);
            }

            _presetCategory = _mainCategory.CreatePage("Presets", Color.white, createLink: false);
            _presetCategoryLink = _mainCategory.CreatePageLink(_presetCategory);

            var createNewPresetButton = _presetCategory.CreateFunction("Create New Preset", Color.green, () =>
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
        public static void CreatePresetCatagory(Page category, string presetName)
        {
#if DEBUG
            Main.MelonLog.Msg("Creating new Preset Catagory with name " + presetName);
#endif
            var _category = category.CreatePage(presetName, Color.white, createLink: false);
            var _categoryLink = category.CreatePageLink(_category);

            var presetData = _presetManager.GetPresetData(presetName);

            var nameStringElement = _category.CreateFunction(presetName, Color.white, () => { });
            var newnamestring = presetName;
            _category.CreateString("Rename Preset", Color.white, presetName, (s) => { newnamestring = s; });
            _category.CreateFunction("Apply New Name", Color.white, () =>
            {
                string s = newnamestring;
                if (!_presetManager.presets.ContainsKey(s))
                {
                    _presetManager.RemovePreset(presetName);
                    _presetManager.presets.Add(s, presetData);
                    _presetManager.SavePresets();
                    RebuildBonemenu();
                    BoneLib.BoneMenu.Menu.OpenPage(_presetCategory);
                }
                else BoneMenuNotif(NotificationType.Error, $"Error renaming preset {presetName} to {s}, as a preset of name {s} already exists!");
            });
            var applyButton = _category.CreateFunction("Apply Preset", Color.cyan, () =>
            {
                _presetManager.LoadPresets();
                var Items = _presetManager.GetPresetData(presetName).Barcodes.ToArray();
                Main.SetCheatMenuItems(Items);
            });
            var addItemButton = _category.CreateFunction("Add Item", Color.green, () =>
            {
                string errortext = "";
                try
                {
                    if (BoneLib.Player.GetObjectInHand(BoneLib.Player.LeftHand) == null)
                    { errortext = "Error: Nothing in left hand."; throw new Exception(); }
                    if (BoneLib.Player.GetComponentInHand<Poolee>(BoneLib.Player.LeftHand).SpawnableCrate == null)
                    { errortext = "Error: Object is not a spawnable, or is a prefab."; throw new Exception(); }

                    string barcode = BoneLib.Player.GetComponentInHand<Poolee>(BoneLib.Player.LeftHand).SpawnableCrate.Barcode.ID;

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
            if (presetName != PresetManager.DEFAULT_PRESET_NAME)
            {
                removePresetButton = _category.CreateFunction("Remove Preset", Color.red, () =>
                {
                    Menu.DisplayDialog(
                    "Remove Preset",
                    "Are you sure you want to delete this preset?",
                    confirmAction: () =>
                    {
                        _presetManager.RemovePreset(presetName);
                        Menu.DestroyPage(_category);
                        category.Remove(_categoryLink);
                        _presetManager.SavePresets();
                        Menu.OpenPage(category);
                    });
                });
            }
            // Create pre-existing preset buttons
            var barcodes = _presetManager.GetPresetData(presetName).Barcodes;
            foreach (string barcode in barcodes)
            {
                CreatePBarcodeCatagory(_category, barcode);
            }


        }

        public static void CreatePBarcodeCatagory(Page category, string Barcode)
        {
            var crateRef = new SpawnableCrateReference(Barcode);
            var isValid = crateRef.Crate != null;
            var title = "Invalid Crate";

            if (isValid)
                title = bandage + crateRef.Crate.Title;
            
            var _category = category.CreatePage(title, (isValid ? Color.white : Color.red), createLink: false);
            var _categoryLink = category.CreatePageLink(_category);

#if DEBUG
            Main.MelonLog.Msg("Creating new Preset Item element with Title "+title+" and barcode "+Barcode);
#endif

            var titleElement = _category.CreateFunction(title, Color.white, null);
            var barcodeElement = _category.CreateFunction(Barcode, Color.white, null);
            var removeButtom = _category.CreateFunction("Remove Item", Color.red, () =>
            {
                _presetManager.RemoveBarcodeFromPreset(category.Name, Barcode);
                Menu.DestroyPage(_category);
                category.Remove(_categoryLink);
                Menu.OpenPage(category);
            });
        }
        // CreateBoneMenuInvisDifferentiator
        public static string bandage => $"<size=0>{UnityEngine.Random.RandomRangeInt(int.MinValue, int.MaxValue):D10}</size>";
    }
}
