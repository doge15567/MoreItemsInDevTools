using MelonLoader;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow.Warehouse;
using System.Xml.Linq;

namespace MoreItemsInDevTools
{
    internal partial class Main : MelonMod
    {
        
        internal const string Name = "More Items in Dev Tools";
        internal const string Description = "Adds more items to list of items spawned by dev tools cheat";
        internal const string Author = "doge15567";
        internal const string Company = "";
        internal const string Version = "4.0.0";
        internal const string DownloadLink = "https://thunderstore.io/c/bonelab/p/doge15567/MoreItemsInDevTools/";
        internal static MelonLogger.Instance MelonLog;
        internal static HarmonyLib.Harmony Harmoney;
        internal static CheatTool playerCheatMenu;

        public static string[] currentPresetArray;
        public static CheatTool currentInstance;
        public static bool oldAssemblyDetected = false;
        public const string oadString = "An older version of the mod is installed alongside the current version, delete MoreItemsInDevToolsML6.dll.";
        public static bool hasFusion = false;

        public override void OnInitializeMelon()
        {
            MelonLog = LoggerInstance;
            Harmoney = HarmonyInstance;
            MelonLog.Msg("Initalising Mod");
            if (CheckIfAssemblyLoaded("MoreItemsInDevToolsML6")) oldAssemblyDetected = true;
            if (oldAssemblyDetected) MelonLog.Warning(oadString);
            if (CheckIfAssemblyLoaded("BoneLib")) BonelibSetup();
            if (CheckIfAssemblyLoaded("LabFusion")) hasFusion = true;

            if (hasFusion)
            {
                Patches.AddDevMenuPatch.Patch();
                //Patches.PreventFusionPatch.Patch();
            }
        }

        private static void BonelibSetup()
        {
            if (oldAssemblyDetected) Bonemenu.BoneMenuNotif(BoneLib.Notifications.NotificationType.Warning, oadString);
            Bonemenu.BonemenuSetup();
            BoneLib.Hooking.OnMarrowGameStarted += Bonemenu.RebuildBonemenu;
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

        public static string[] RemoveDuplicateBarcodes(string[] s) // https://stackoverflow.com/a/9833
        {
            HashSet<string> set = new HashSet<string>(s);
            string[] result = new string[set.Count];
            set.CopyTo(result);
            return result;
        }
        public static bool CheckIfAssemblyLoaded(string name)  // Check for bonelib with bonelib method lol
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                string asmName = assemblies[i].GetName().Name;
                if (asmName.ToLower() == name.ToLower())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
