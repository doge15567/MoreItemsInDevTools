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
using LabFusion.Patching;
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
        internal const string Version = "3.0.0";
        internal const string DownloadLink = "https://thunderstore.io/c/bonelab/p/doge15567/MoreItemsInDevTools/";
        internal static MelonLogger.Instance MelonLog;
        private static MenuCategory _mainCategory;
        internal static bool hasfusion; 
        internal static CheatTool playerCheatMenu;

        internal static string[] currentPresetArray;
        public override void OnInitializeMelon()
        {
            MelonLog = LoggerInstance;
            MelonLog.Msg("Initalised Mod");
            Bonemenu.BonemenuSetup();

            _mainCategory = MenuManager.CreateCategory("MoreItemsInDevTools", "#f6f6f6");
            hasfusion = HelperMethods.CheckIfAssemblyLoaded("labfusion");


            BoneLib.Hooking.OnLevelInitialized += OnLevelInitHook;
            BoneLib.Hooking.OnMarrowGameStarted += OnMarrowGameStartedHook;
        }

        public static void OnMarrowGameStartedHook() { Bonemenu.RebuildBonemenu(); }

        public static void OnLevelInitHook(LevelInfo info)
        {
#if DEBUG
            Main.MelonLog.Msg("OnLevelInitHook called.");
#endif
            Bonemenu.CheckForDefaultPreset();
            string[] Items = Bonemenu._presetManager.presets["DEFAULT"].Barcodes.ToArray();

            SetCheatMenuItems(Items);
        }

        public static void SetCheatMenuItems(string[] BarcodeStrArray)
        {
            #if DEBUG            
            MelonLog.Msg("SetCheatMenuItems Called");
            MelonLog.Msg("BarcodeStrArray is :" + BarcodeStrArray);
            #endif            
            currentPresetArray = BarcodeStrArray;

            playerCheatMenu = Player.rigManager.GetComponent<CheatTool>();

            List<SpawnableCrateReference> newCrateList = new List<SpawnableCrateReference>();
            foreach (var crateCode in BarcodeStrArray)
            {
            #if  DEBUG
                MelonLog.Msg("Adding Barcode " + crateCode + " to Array");
            #endif
                newCrateList.Add(new SpawnableCrateReference
                {
                    _barcode = AssetWarehouse.Instance.GetCrate<GameObjectCrate>(crateCode)._barcode
                });
            }
            playerCheatMenu.crates = newCrateList.ToArray(); // Convert List to Array if necessary
            #if DEBUG            
            MelonLog.Msg("SetCheatMenuItems Ended");
            #endif            
        }
    }
}
