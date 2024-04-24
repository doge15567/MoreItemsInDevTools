using Il2CppSystem;
using HarmonyLib;

using SLZ.UI;

using LabFusion.Data;
using LabFusion.Network;
using LabFusion.Utilities;

using LabFusion.Representation;
using MelonLoader;

namespace MoreItemsInDevTools.Patches
{
    [HarmonyPatch(typeof(LabFusion.Patching.AddDevMenuPatch), "OnSpawnDelegate")]
    public static class PreventFusionPatch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(PopUpMenuView), nameof(PopUpMenuView.AddDevMenu))]
    public static class AddDevMenuPatch
    {
        [HarmonyPrefix]
        public static void Prefix(PopUpMenuView __instance, ref Action spawnDelegate)
        {
            spawnDelegate += (Action)(() => { SpawnListFusion(__instance); });
        }
        
        [HarmonyPostfix]
        public static void Postfix(PopUpMenuView __instance)
        {
            SpawnListFusion(__instance);
        }

        public static void SpawnListFusion(PopUpMenuView __instance)
        {
            if (NetworkInfo.HasServer && !NetworkInfo.IsServer && RigData.RigReferences.RigManager && RigData.RigReferences.RigManager.uiRig.popUpMenu == __instance)
            {
                var transform = new SerializedTransform(__instance.radialPageView.transform);
                foreach (var CrateRef in MoreItemsInDevTools.Main.playerCheatMenu.crates)
                {
                    string barcode = CrateRef.Barcode;
                    #if DEBUG
                    MelonLogger.Msg("Spawning " + barcode);
                    #endif
                    PooleeUtilities.RequestSpawn(barcode, transform, PlayerIdManager.LocalSmallId);
                }
            }
        }
    }
}