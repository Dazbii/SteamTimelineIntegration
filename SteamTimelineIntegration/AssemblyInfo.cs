using MelonLoader;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: MelonInfo(typeof(SteamTimelineIntegration.Core), SteamTimelineIntegration.BuildInfo.ModName, SteamTimelineIntegration.BuildInfo.ModVersion, SteamTimelineIntegration.BuildInfo.Author)]
[assembly: VerifyLoaderVersion(0,7,2)]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]

[assembly: AssemblyTitle(SteamTimelineIntegration.BuildInfo.ModName)]
[assembly: AssemblyDescription(SteamTimelineIntegration.BuildInfo.Description)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(SteamTimelineIntegration.BuildInfo.Company)]
[assembly: AssemblyProduct(SteamTimelineIntegration.BuildInfo.ModName)]
[assembly: AssemblyCopyright("Copyright ©  2026")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("621d30a5-8fa1-4d87-9826-92c0149b033e")]

[assembly: AssemblyVersion(SteamTimelineIntegration.BuildInfo.ModVersion)]
[assembly: AssemblyFileVersion(SteamTimelineIntegration.BuildInfo.ModVersion)]