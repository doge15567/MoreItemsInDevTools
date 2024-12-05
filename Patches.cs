using Il2CppSystem;
using HarmonyLib;

using Il2CppSLZ.UI;


using MelonLoader;
using Action = System.Action;
using Il2CppSLZ.Marrow.Data;
using BoneLib;
using Il2CppSLZ.Marrow.Warehouse;

using LabFusion.Data;
using LabFusion.Network;
using Il2CppSLZ.Bonelab;
using LabFusion.Player;
using LabFusion.RPC;



namespace MoreItemsInDevTools.Patches
{
    [HarmonyPatch(typeof(CheatTool))]
    [HarmonyPatch(nameof(CheatTool.Start))]

    public class CheatToolPatch
    {
        static bool Prefix(CheatTool __instance)
        {
            Main.currentInstance = __instance;

#if DEBUG
            Main.MelonLog.Msg("CheatTool Prefix called.");
#endif
            Main._presetManager.CheckForDefaultPreset();
            Main._presetManager.LoadPresets();

            string[] Items = Main._presetManager.presets["DEFAULT"].Barcodes.ToArray();
            Main.SetCheatMenuItems(Items);

            return true;
        }

    }




    [HarmonyPatch(typeof(LabFusion.Patching.AddDevMenuPatch), nameof(LabFusion.Patching.AddDevMenuPatch.OnSpawnDelegate))]
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

            CheatTool playerCheatMenu = Main.currentInstance;
            var transform = menu.radialPageView.transform;

            // Sanitization, prevent skid crashing by only allowing 5 unique spawnables to be spawned at a time.
            string[] sanitizedStringArray = Main.RemoveDuplicateBarcodes(Main.currentPresetArray);
            System.Array.Resize(ref sanitizedStringArray, 5);
            
            foreach (string crateRef in sanitizedStringArray)
            {
                var spawnable = new Spawnable() { crateRef = new(crateRef) };
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

   
}