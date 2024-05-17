using System.Reflection;
using MoreItemsInDevTools;
using MelonLoader;

[assembly: AssemblyDescription(MoreItemsInDevTools.Main.Description)]
[assembly: AssemblyCopyright("Developed by " + MoreItemsInDevTools.Main.Author)]
[assembly: AssemblyTrademark(MoreItemsInDevTools.Main.Company)]
[assembly: MelonInfo(typeof(MoreItemsInDevTools.Main), MoreItemsInDevTools.Main.Name, MoreItemsInDevTools.Main.Version, MoreItemsInDevTools.Main.Author, MoreItemsInDevTools.Main.DownloadLink)]
[assembly: MelonColor(255,255,255,255)]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONELAB")]