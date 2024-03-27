using MelonLoader;
using BoneLib;
using System.Runtime.InteropServices;
using SLZ.Bonelab;
using System;
using SLZ.Marrow.Warehouse;
using Harmony;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using BoneLib.BoneMenu.Elements;
using BoneLib.BoneMenu;
using UnityEngine;
using LabFusion.Data;
using LabFusion.Representation;
using LabFusion.Network;
using LabFusion.Utilities;
using static MelonLoader.MelonLogger;
using SLZ.UI;


namespace MoreItemsInDevTools
{
    internal partial class Main : MelonMod
    {
        
        internal const string Name = "More Items in Dev Tools";
        internal const string Description = "Adds more items to list of items spawned by dev tools cheat";
        internal const string Author = "doge15567";
        internal const string Company = "";
        internal const string Version = "1.0.0";
        internal const string DownloadLink = "https://thunderstore.io/c/bonelab/p/doge15567/MoreItemsInDevTools/";
        internal static MelonLogger.Instance MelonLog;
        private static MenuCategory _mainCategory;
        internal static bool hasfusion; 
        internal static CheatTool playerCheatMenu;
        private static FunctionElement GenerateButtonBM;
        private static FunctionElement Preset1;
        private static FunctionElement Preset2;
        private static FunctionElement Preset3;
        private static FunctionElement Preset4;
        private static FunctionElement Preset5;

        internal static string[] currentPresetArray;
        public override void OnInitializeMelon()
        {
            MelonLog = LoggerInstance;
            MelonLog.Msg("Initalised Mod");
            Prefs.SetupMelonPrefs();

            _mainCategory = MenuManager.CreateCategory("MoreItemsInDevTools", "#f6f6f6");
            GenerateButtonBM = _mainCategory.CreateFunctionElement("Automatic Preset", Color.cyan, () => { SetCheatMenuItems(Prefs.Barcodes.Value); });
            Preset1 = _mainCategory.CreateFunctionElement(Prefs.Preset1Name.Value, Color.white, () => { SetCheatMenuItems(Prefs.Preset1.Value); });
            Preset2 = _mainCategory.CreateFunctionElement(Prefs.Preset2Name.Value, Color.white, () => { SetCheatMenuItems(Prefs.Preset2.Value); });
            Preset3 = _mainCategory.CreateFunctionElement(Prefs.Preset3Name.Value, Color.white, () => { SetCheatMenuItems(Prefs.Preset3.Value); });
            Preset4 = _mainCategory.CreateFunctionElement(Prefs.Preset4Name.Value, Color.white, () => { SetCheatMenuItems(Prefs.Preset4.Value); });
            Preset5 = _mainCategory.CreateFunctionElement(Prefs.Preset5Name.Value, Color.white, () => { SetCheatMenuItems(Prefs.Preset5.Value); });


            hasfusion = HelperMethods.CheckIfAssemblyLoaded("labfusion");


            BoneLib.Hooking.OnLevelInitialized += OnLevelInitHook;
        }
        

        public static void OnLevelInitHook(LevelInfo info)
        { 
            SetCheatMenuItems(Prefs.Barcodes.Value); 
        }
        public static void SetCheatMenuItems(string[] BarcodeStrArray)
        {
            MelonLog.Msg("AITCM Called");
            currentPresetArray = BarcodeStrArray;

            playerCheatMenu = Player.rigManager.GetComponent<CheatTool>();

            List<SpawnableCrateReference> newCrateList = new List<SpawnableCrateReference>();
            foreach (var crateCode in BarcodeStrArray)
            {
                MelonLog.Msg("Adding Barcode " + crateCode + " to Array");
                newCrateList.Add(new SpawnableCrateReference
                {
                    _barcode = AssetWarehouse.Instance.GetCrate<GameObjectCrate>(crateCode)._barcode
                });
            }
            playerCheatMenu.crates = newCrateList.ToArray(); // Convert List to Array if necessary
            MelonLog.Msg("AITCM Ended");
        }
    }
}
