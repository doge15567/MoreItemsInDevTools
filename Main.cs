using MelonLoader;
using static MelonLoader.MelonLogger;

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
using static Il2CppSystem.Array;
using Il2CppSLZ.Rig;
using Il2CppSLZ.Marrow.SceneStreaming;
using BoneLib;
using Il2CppSLZ.VRMK;
using Il2CppSLZ.Marrow;
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
        
        internal const string Name = "More Items in Dev Tools (Rework BoneMenu)";
        internal const string Description = "Adds more items to list of items spawned by dev tools cheat";
        internal const string Author = "doge15567";
        internal const string Company = "";
        internal const string Version = "3.0.1";
        internal const string DownloadLink = "https://thunderstore.io/c/bonelab/p/doge15567/MoreItemsInDevTools/";
        internal static MelonLogger.Instance MelonLog;
        internal static HarmonyLib.Harmony Harmoney;
        internal static bool hasfusion; 
        internal static CheatTool playerCheatMenu;

        public static PresetManager _presetManager = new PresetManager();
        public static string[] currentPresetArray;
        public static CheatTool currentInstance;


        // TODO: Make player name Preset when creating it
        // Add Preset Rename Option


        private static System.Timers.Timer _timer;
        public override void OnInitializeMelon()
        {
            MelonLog = LoggerInstance;
            Harmoney = HarmonyInstance;
            MelonLog.Msg("Initalised Mod");
            Bonemenu.BonemenuSetup();

            hasfusion = false;
            hasfusion = HelperMethods.CheckIfAssemblyLoaded("labfusion");
            if (hasfusion)
            {
            }
              
            

            //Hooking.OnSwitchAvatarPostfix += OnPostAvatarSwap;
            Hooking.OnMarrowGameStarted += OnMarrowGameStartedHook;
            _presetManager.OnStart();


            //BoneLib.Hooking.OnGripAttached
        }

        public static void OnPostAvatarSwap(Il2CppSLZ.VRMK.Avatar unused) 
        {
            ForceRadial("Avatar Swap");
            _timer = new System.Timers.Timer(500);
            _timer.AutoReset = false;
            _timer.Elapsed += (sender, args) =>
            {
                ForceRadial("Avatar Timer");
            };
            _timer.Start();
        }


        public static void OnMarrowGameStartedHook() { Bonemenu.RebuildBonemenu(); }

        public static void ForceRadial(string add)
        {
#if DEBUG
            MelonLog.Msg("Fixed Radial (Hopefully)" + add);

#endif
            Player.RigManager.GetComponentInChildren<BodyVitals>().quickmenuEnabled = true;
            Player.RigManager.ControllerRig.TryCast<OpenControllerRig>().quickmenuEnabled = true;
        }

        
        


        




        public static void SetCheatMenuItems(string[] BarcodeStrArray)
        {
            #if DEBUG            
            MelonLog.Msg("SetCheatMenuItems Called");
            MelonLog.Msg("BarcodeStrArray is :" + BarcodeStrArray);
            #endif            
            currentPresetArray = BarcodeStrArray;

            

            playerCheatMenu = currentInstance;

            List<SpawnableCrateReference> newCrateList = new List<SpawnableCrateReference>();
            foreach (var crateCode in BarcodeStrArray)
            {
                #if  DEBUG
                MelonLog.Msg("Adding Barcode " + crateCode + " to Array");
             #endif
                newCrateList.Add(new SpawnableCrateReference
                {
                    _barcode = new Barcode() { ID = crateCode }
                });
            }
            playerCheatMenu.crates = newCrateList.ToArray();
            #if DEBUG            
            MelonLog.Msg("SetCheatMenuItems Ended");
            #endif
        }
    }
}
