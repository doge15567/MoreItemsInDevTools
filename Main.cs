using MelonLoader;
using static MelonLoader.MelonLogger;
using BoneLib;

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

using Il2CppSLZ;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.UI;
using UnityEngine;
//using SLZ.Bonelab;

//using SLZ.Marrow.Warehouse;
using Harmony;
using HarmonyLib;
using BoneLib.BoneMenu.Elements;
using BoneLib.BoneMenu;
using static Il2CppSystem.Array;
using Il2CppSLZ.Rig;
using Il2CppSLZ.Props.Weapons;
using Il2CppSLZ.Marrow.SceneStreaming;
/*
using LabFusion.Data;
using LabFusion.Representation;
using LabFusion.Network;
using LabFusion.Patching;
using LabFusion.Utilities;
*/
//using SLZ.UI;


namespace MoreItemsInDevTools
{
    internal partial class Main : MelonMod
    {
        
        internal const string Name = "More Items in Dev Tools";
        internal const string Description = "Adds more items to list of items spawned by dev tools cheat";
        internal const string Author = "doge15567";
        internal const string Company = "";
        internal const string Version = "3.0.1";
        internal const string DownloadLink = "https://thunderstore.io/c/bonelab/p/doge15567/MoreItemsInDevTools/";
        internal static MelonLogger.Instance MelonLog;
        private static MenuCategory _mainCategory;
        internal static bool hasfusion; 
        internal static CheatTool playerCheatMenu;

        public static string[] currentPresetArray;
        public override void OnInitializeMelon()
        {
            MelonLog = LoggerInstance;
            MelonLog.Msg("Initalised Mod");
            Bonemenu.BonemenuSetup();

            _mainCategory = MenuManager.CreateCategory("MoreItemsInDevTools", "#f6f6f6");
            hasfusion = HelperMethods.CheckIfAssemblyLoaded("labfusion");


            BoneLib.Hooking.OnLevelInitialized += OnLevelInitHook;
            BoneLib.Hooking.OnMarrowGameStarted += OnMarrowGameStartedHook;



            //BoneLib.Hooking.OnGripAttached
        }

        

        public static void OnMarrowGameStartedHook() { Bonemenu.RebuildBonemenu(); }

        public static void ForceRadial(string add)
        {
            MelonLog.Msg("Fixed Radial (Hopefully)" + add);
            BoneLib.Player.rigManager.bodyVitals.quickmenuEnabled = true;
            BoneLib.Player.rigManager.openControllerRig.quickmenuEnabled = true;
        }

        private static System.Timers.Timer _timer;
        public static void OnLevelInitHook(LevelInfo info)
        {

            
#if DEBUG
            Main.MelonLog.Msg("OnLevelInitHook called.");
#endif
            Bonemenu.CheckForDefaultPreset();
            string[] Items = Bonemenu._presetManager.presets["DEFAULT"].Barcodes.ToArray();

            SetCheatMenuItems(Items);


            // Force enable radial menu
            // FIX!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            ForceRadial(" Level Init Hook");
            _timer = new System.Timers.Timer(1500);
            _timer.AutoReset = true;
            _timer.Elapsed += (sender, args) =>
            {
                ForceRadial(" Timer");
                if (Player.handsExist) _timer.Stop(); 
            };
            _timer.Start();
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
