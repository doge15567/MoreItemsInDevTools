using System.Reflection;
using MoreItemsInDevTools;
using MelonLoader;

[assembly: System.Reflection.AssemblyCompanyAttribute(MoreItemsInDevTools.Main.Name)]
[assembly: System.Reflection.AssemblyProductAttribute(MoreItemsInDevTools.Main.Name)]
[assembly: System.Reflection.AssemblyTitleAttribute(MoreItemsInDevTools.Main.Name)]

[assembly: AssemblyDescription(MoreItemsInDevTools.Main.Description)]
[assembly: AssemblyVersion(MoreItemsInDevTools.Main.Version)]
[assembly: AssemblyFileVersion(MoreItemsInDevTools.Main.Version)]
[assembly: AssemblyInformationalVersionAttribute(MoreItemsInDevTools.Main.Version)]
[assembly: AssemblyCopyright("Developed by " + MoreItemsInDevTools.Main.Author)]
[assembly: AssemblyTrademark(MoreItemsInDevTools.Main.Company)]
[assembly: MelonInfo(typeof(MoreItemsInDevTools.Main), MoreItemsInDevTools.Main.Name, MoreItemsInDevTools.Main.Version, MoreItemsInDevTools.Main.Author, MoreItemsInDevTools.Main.DownloadLink)]
[assembly: MelonColor(255,255,255,255)]
[assembly: MelonOptionalDependencies("LabFusion", "BoneLib")]
//LabFusion

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONELAB")]