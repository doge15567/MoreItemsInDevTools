using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreItemsInDevTools
{
    internal static class Prefs
    {
        public static readonly MelonPreferences_Category GlobalCategory = MelonPreferences.CreateCategory("MoreItemsInDevTools");

        public static MelonPreferences_Entry<string[]> Barcodes;

        public static MelonPreferences_Entry<string> Preset1Name;
        public static MelonPreferences_Entry<string[]> Preset1;
        public static MelonPreferences_Entry<string> Preset2Name;
        public static MelonPreferences_Entry<string[]> Preset2;
        public static MelonPreferences_Entry<string> Preset3Name;
        public static MelonPreferences_Entry<string[]> Preset3;
        public static MelonPreferences_Entry<string> Preset4Name;
        public static MelonPreferences_Entry<string[]> Preset4;
        public static MelonPreferences_Entry<string> Preset5Name;
        public static MelonPreferences_Entry<string[]> Preset5;

        public static void SetupMelonPrefs()
        {
            Barcodes = GlobalCategory.GetEntry<string[]>("Barcodes") ?? GlobalCategory.CreateEntry("Barcodes", new string[] { "c1534c5a-5747-42a2-bd08-ab3b47616467", "c1534c5a-6b38-438a-a324-d7e147616467", "c1534c5a-3813-49d6-a98c-f595436f6e73", "c1534c5a-c6a8-45d0-aaa2-2c954465764d" }, null,  "Array of Barcodes of items to be added to the Dev Tools option automatically.");
            Preset1Name = GlobalCategory.GetEntry<string>("Preset1Name") ?? GlobalCategory.CreateEntry("Preset1Name", "Zombie Fighter", null, "Preset 1's Display Name in Bonemenu.");
            Preset1 = GlobalCategory.GetEntry<string[]>("Preset1") ?? GlobalCategory.CreateEntry("Preset1", new string[] { "c1534c5a-e777-4d15-b0c1-3195426f6172", "c1534c5a-3e35-4aeb-b1ec-4a95534d474d", "c1534c5a-1fb8-477c-afbe-2a95436f6d62", "c1534c5a-aade-4fa1-8f4b-d4c547756e4d", "c1534c5a-d30c-4c18-9f5f-7cfe54726173" },null, "Preset 1's Array of Barcodes of items.");
            Preset2Name = GlobalCategory.GetEntry<string>("Preset2Name") ?? GlobalCategory.CreateEntry("Preset2Name", "The Forde", null, "Preset 2's Display Name in Bonemenu.");
            Preset2 = GlobalCategory.GetEntry<string[]>("Preset2") ?? GlobalCategory.CreateEntry("Preset2", new string[] { "c1534c5a-3fd8-4d50-9eaf-0695466f7264", "c1534c5a-3fd8-4d50-9eaf-0695466f7264", "c1534c5a-3fd8-4d50-9eaf-0695466f7264", "c1534c5a-3fd8-4d50-9eaf-0695466f7264", "c1534c5a-3fd8-4d50-9eaf-0695466f7264", "c1534c5a-3fd8-4d50-9eaf-0695466f7264", "c1534c5a-3fd8-4d50-9eaf-0695466f7264", "c1534c5a-3fd8-4d50-9eaf-0695466f7264", }, null, "Preset 2's Array of Barcodes of items.");
            Preset3Name = GlobalCategory.GetEntry<string>("Preset3Name") ?? GlobalCategory.CreateEntry("Preset3Name", "Preset 3", null, "Preset 3's Display Name in Bonemenu.");
            Preset3 = GlobalCategory.GetEntry<string[]>("Preset3") ?? GlobalCategory.CreateEntry("Preset3", new string[] { "c1534c5a-5747-42a2-bd08-ab3b47616467", "c1534c5a-6b38-438a-a324-d7e147616467" }, null, "Preset 3's Array of Barcodes of items.");
            Preset4Name = GlobalCategory.GetEntry<string>("Preset4Name") ?? GlobalCategory.CreateEntry("Preset4Name", "Preset 4", null, "Preset 4's Display Name in Bonemenu.");
            Preset4 = GlobalCategory.GetEntry<string[]>("Preset4") ?? GlobalCategory.CreateEntry("Preset4", new string[] { "c1534c5a-5747-42a2-bd08-ab3b47616467", "c1534c5a-6b38-438a-a324-d7e147616467" }, null, "Preset 4's Array of Barcodes of items.");
            Preset5Name = GlobalCategory.GetEntry<string>("Preset5Name") ?? GlobalCategory.CreateEntry("Preset5Name", "Preset 5", null, "Preset 5's Display Name in Bonemenu.");
            Preset5 = GlobalCategory.GetEntry<string[]>("Preset5") ?? GlobalCategory.CreateEntry("Preset5", new string[] { "c1534c5a-5747-42a2-bd08-ab3b47616467", "c1534c5a-6b38-438a-a324-d7e147616467" }, null, "Preset 5's Array of Barcodes of items.");

            GlobalCategory.SetFilePath(MelonUtils.UserDataDirectory + "/MoreItemsInDevTools.cfg");
            GlobalCategory.SaveToFile(false);
            Main.MelonLog.Msg("Initalised prefs");

        }
    }
}
