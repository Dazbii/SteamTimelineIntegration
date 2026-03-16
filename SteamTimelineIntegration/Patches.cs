using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using SteamTimelineIntegration;

[HarmonyPatch(typeof(Il2CppRUMBLE.Environment.MatchFlow.PedestalManager), nameof(Il2CppRUMBLE.Environment.MatchFlow.PedestalManager.TeleportPedestalsToOwners))]
public static class Pedestal_Patch
{
    public static void Prefix()
    {
        Il2CppStructArray<int> roundsWon = Il2CppRUMBLE.Networking.MatchFlow.MatchHandler.Instance.RoundsWonList;
        int currentRound = Il2CppRUMBLE.Networking.MatchFlow.MatchHandler.Instance.CurrentRound;

        Melon<Core>.Instance.RoundEnded(roundsWon, currentRound);
    }
}