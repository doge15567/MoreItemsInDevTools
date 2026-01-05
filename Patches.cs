using HarmonyLib;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow.Data;
using Il2CppSLZ.Marrow.Warehouse;
using LabFusion.RPC;
using LabFusion.Scene;
using UnityEngine;


namespace MoreItemsInDevTools.Patches
{
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
            Bonemenu._presetManager.LoadPresets();

            if (Main.currentPresetArray == null || Main.currentPresetArray is null || Bonemenu.ApplyDefaultOnLevelLoad.Value) // brah
            {
                if (Bonemenu._presetManager.presets.ContainsKey(PresetManager.DEFAULT_PRESET_NAME))
                {
                    Main.SetCheatMenuItems(Bonemenu._presetManager.presets[PresetManager.DEFAULT_PRESET_NAME].Barcodes.ToArray());
                }
            }
            else
                Main.SetCheatMenuItems(Main.currentPresetArray);
        }
    }

    public static class AddDevMenuPatch
    {
        public static void Patch()
        {
            var orig = typeof(PopUpMenuView).GetMethod(nameof(PopUpMenuView.AddDevMenu), AccessTools.all);
            var prefix = new HarmonyMethod(typeof(AddDevMenuPatch).GetMethod("Prefix", AccessTools.all));

            Main.Harmoney.Patch(orig, prefix);
        }
        public static void Prefix(PopUpMenuView __instance, ref Il2CppSystem.Action spawnDelegate)
        {
            // Completely override the spawn delegate with modified networked version
            var originalDelegate = Il2CppSystem.Action.Combine(spawnDelegate).TryCast<Il2CppSystem.Action>();

            spawnDelegate = (Il2CppSystem.Action)(() => { OnSpawnDelegate(__instance, originalDelegate); });
        }
        public static void OnSpawnDelegate(PopUpMenuView menu, Il2CppSystem.Action originalDelegate)
        {
            // If there is no server, leave method
            if (!NetworkSceneManager.IsLevelNetworked)
            {
                originalDelegate?.Invoke();
                return;
            }

            var transform = menu.radialPageView.transform;

            // Sanitization, prevent skid crashing by only allowing 5 unique spawnables to be spawned at a time.

            var gameList = new List<string>();
            foreach (var item in Main.currentInstance.crates)
                gameList.Add(item.Barcode.ID);
            
            string[] sanitizedStringArray = Main.RemoveDuplicateBarcodes(gameList.ToArray());
            if (sanitizedStringArray.Length > 5) 
                Array.Resize(ref sanitizedStringArray, 5);

            foreach (string barcode in sanitizedStringArray)
            {
#if DEBUG
                Main.MelonLog.Msg($"Attempting to spawn {barcode}.");
#endif
                SpawnableCrateReference crateRef = new(barcode);
                if (!crateRef.IsValid()) continue;
                var spawnable = new Spawnable() { crateRef = crateRef };
                var spawnableFusionInfo = new NetworkAssetSpawner.SpawnRequestInfo()
                {
                    Spawnable = spawnable,
                    Position = transform.position,
                    Rotation = transform.rotation,
#if DEBUG
                    SpawnCallback = (info) => Main.MelonLog.Msg($"Spawned {barcode} ({info.Spawned.name}).")
#endif
                };

                NetworkAssetSpawner.Spawn(spawnableFusionInfo);
            }
        }
    }

    public static class PreventFusionPatch
    {
        public static void Patch()
        {
            var orig = typeof(MarrowFusion.Bonelab.Patching.PopUpMenuViewPatches).GetMethod("AddDevMenuPrefix", AccessTools.all);
            var prefix = new HarmonyMethod(typeof(PreventFusionPatch).GetMethod("Prefix", AccessTools.all));

            Main.Harmoney.Patch(orig, prefix);
        }
        public static bool Prefix()
        {
            return false;
        }
    }
}