using LabFusion.Data;
using LabFusion.Network;
using LabFusion.Representation;
using SLZ.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using LabFusion.Patching;
using UnityEngine;
using SLZ.Marrow.Data;
using SLZ.Marrow.Pool;
using BoneLib.Nullables;

namespace MoreItemsInDevTools
{


    [HarmonyPatch(typeof(PopUpMenuView), nameof(PopUpMenuView.AddDevMenu))]
    internal class Patches
    {

        public static void Prefix(PopUpMenuView __instance, ref Action spawnDelegate)
        {
            spawnDelegate += (Action)(() => { SpawnListFusion(__instance); });
        }

        public static void SpawnListFusion(PopUpMenuView __instance)
        {
            if (Main.hasfusion)
            { if (LabFusion.SDK.Gamemodes.Gamemode.IsGamemodeRunning /*&& LabFusion.SDK.Gamemodes.Gamemode.DisableDevTools*/) // find fix
                {
                    if (NetworkInfo.HasServer && !NetworkInfo.IsServer && RigData.RigReferences.RigManager && RigData.RigReferences.RigManager.uiRig.popUpMenu == __instance)
                    {
                        var transform = new SerializedTransform(__instance.radialPageView.transform);
                        foreach (var CrateRef in Main.playerCheatMenu.crates)
                        {
                            string barcode = CrateRef.Barcode;
                            LabFusion.Utilities.PooleeUtilities.RequestSpawn(barcode, transform, PlayerIdManager.LocalSmallId);
                        }
                    }
                }
            }
            else // Re-implement spawning
            {
                var transform = __instance.radialPageView.transform;
                foreach (var CrateRef in Main.playerCheatMenu.crates)
                {
                    Spawnable spawnable = new Spawnable
                    {
                        crateRef = CrateRef
                    };
                    AssetSpawner.Register(spawnable);
                    AssetSpawner.Spawn(spawnable, transform.position, default(Quaternion), new BoxedNullable<Vector3>(Vector3.one), ignorePolicy: false, new BoxedNullable<int>(null));
                    
                }
            }
        }
    }
}
