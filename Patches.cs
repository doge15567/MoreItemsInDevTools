using Il2CppSystem;
using HarmonyLib;

using Il2CppSLZ.UI;

using LabFusion.Data;
using LabFusion.Network;
using LabFusion.Utilities;

using LabFusion.Representation;
using MelonLoader;
using LabFusion.SDK.Gamemodes;
using Il2CppSLZ.Bonelab;
using LabFusion.Player;
using Action = System.Action;
using Il2CppSLZ.Marrow.Data;
using LabFusion.RPC;
using BoneLib;
using Il2CppSLZ.Marrow.Warehouse;

namespace MoreItemsInDevTools.Patches
{
    [HarmonyPatch(typeof(LabFusion.Patching.AddDevMenuPatch), "OnSpawnDelegate")]
    public static class PreventFusionPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(PopUpMenuView menu, Action originalDelegate)
        {
            // If there is no server, we can just spawn the original items as normal
            if (!NetworkInfo.HasServer)
            {
                return true;
            }

            var playerCheatMenu = Player.RigManager.GetComponent<CheatTool>();
            var transform = menu.radialPageView.transform;

            foreach (SpawnableCrateReference crateRef in playerCheatMenu.crates)
            {
                var spawnable = new Spawnable() { crateRef = new(crateRef.Crate.Barcode) };
                var spawnableFusionInfo = new NetworkAssetSpawner.SpawnRequestInfo()
                {
                    spawnable = spawnable,
                    position = transform.position,
                    rotation = transform.rotation
                };

                NetworkAssetSpawner.Spawn(spawnableFusionInfo);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(CheatTool))]
    [HarmonyPatch(nameof(CheatTool.Start))]

    public class CheatToolPatch
    {
        static void Prefix(CheatTool __instance)
        {
            Main.currentInstance = __instance;

#if DEBUG
            Main.MelonLog.Msg("CheatTool Prefix called.");
#endif
            Main._presetManager.CheckForDefaultPreset();
            Main._presetManager.LoadPresets();

            string[] Items = Main._presetManager.presets["DEFAULT"].Barcodes.ToArray();
            Main.SetCheatMenuItems(Items);
        }

    }

}