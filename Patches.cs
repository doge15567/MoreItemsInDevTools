using HarmonyLib;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow.Data;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Pool;
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
                    Main.SetCheatMenuItems(Bonemenu._presetManager.presets[PresetManager.DEFAULT_PRESET_NAME].Barcodes.ToArray());
            }
            else
                Main.SetCheatMenuItems(Main.currentPresetArray);
        }
    }

    [HarmonyPatch(typeof(PopUpMenuView))]
    [HarmonyPatch(nameof(PopUpMenuView.AddDevMenu))]
    public static class AddDevMenuPatch
    {
        [HarmonyPrefix]
        [HarmonyAfter("BonelabSupport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")] // Fusion Bonelab Module
        public static void Prefix(PopUpMenuView __instance, ref Il2CppSystem.Action spawnDelegate)
        {
            // Completely override the spawn delegate with modified ver
            spawnDelegate = (Il2CppSystem.Action)(() => { OnSpawnDelegate(__instance); });
        }

        public static void OnSpawnDelegate(PopUpMenuView menuView)
        {
            if (Main.hasFusion)
                OnSpawnDelegateFusion(menuView);
            else
                SpawnDevTools(menuView);
        }

        public static void SpawnDevTools(PopUpMenuView menuView)
        {
            var ct = Main.currentInstance;
            var spawnPoint = ct.spawnLocation;

            foreach (var crateRef in ct.crates)
            {
                Spawnable spawnable = new() { crateRef = crateRef };
                AssetSpawner.Register(spawnable);
                var awaiter = AssetSpawner.SpawnAsync(spawnable, spawnPoint.position, spawnPoint.rotation, new(Vector3.zero){hasValue=false},null,false,new(0){hasValue=false})
                    .GetAwaiter(); // i'm assuming fusion does this bc the action might get GC'd with no references in the managed domain if the param on SpawnAsync is used
                awaiter.OnCompleted  // though that doesnt really make sense bc it would be the same here, right? if its not broke dont fix it ig :/
                    ((Il2CppSystem.Action)(() => 
                    {
                        var res = awaiter.GetResult();
                        if (res != null)
                        {
                            AlignVelocityIfPossible(res.gameObject, spawnPoint);
                        }
                    })); 
            }
        }
        public static void AlignVelocityIfPossible(GameObject gameObj, Transform wishpos)
        {
            var physrig = PlayerRefs.Instance.PlayerPhysicsRig;
            if (physrig == null) return;
            var ent = gameObj.GetComponent<MarrowEntity>();
            if (ent == null) return;
            foreach (var item in ent.Bodies)
                if (item)
                    if (item._rigidbody)
                    {
                        item._rigidbody.position = wishpos.position;
                        item._rigidbody.AddForce(physrig.marrowEntity.AnchorBody._rigidbody.velocity, ForceMode.VelocityChange);
                    }
        }

        public static void OnSpawnDelegateFusion(PopUpMenuView menu)
        {
            // If there is no server, leave method
            if (!NetworkSceneManager.IsLevelNetworked)
            {
                SpawnDevTools(menu);
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
                    SpawnCallback = (info) =>
                    {
#if DEBUG
                        Main.MelonLog.Msg($"Spawned {barcode} ({info.Spawned.name}).");
#endif
                        AlignVelocityIfPossible(info.Spawned, transform);
                    }
                };

                NetworkAssetSpawner.Spawn(spawnableFusionInfo);
            }
        }
    }

    //public static class PreventFusionPatch
    //{
    //    // This doesnt work for some reason as the module assembly isnt loaded at the time this is called (except that the exception is thrown after the module is loaded???
    //    // This however, doesnt matter as this patch happens later than the fusion patch, which with how the patch is written still completely overwrites it.
    //    public static void Patch()
    //    {
    //        var orig = typeof(MarrowFusion.Bonelab.Patching.PopUpMenuViewPatches).GetMethod("AddDevMenuPrefix", AccessTools.all);
    //        var prefix = new HarmonyMethod(typeof(PreventFusionPatch).GetMethod("Prefix", AccessTools.all));

    //        Main.Harmoney.Patch(orig, prefix);
    //    }
    //    public static bool Prefix()
    //    {
    //        return false;
    //    }
    //}
}