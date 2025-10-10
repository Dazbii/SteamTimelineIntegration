using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppRUMBLE.MoveSystem;
using MelonLoader;
using RumbleModdingAPI;
using Il2CppSteamworks;
using static UnityEngine.Rendering.ProbeReferenceVolume;
using System.Text.RegularExpressions;

[assembly: MelonInfo(typeof(SteamTimelineIntegration.Core), "SteamTimelineIntegration", "1.0.0", "Dazbii", null)]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]

namespace SteamTimelineIntegration
{
    public class Core : MelonMod
    {
        private string opponentName;
        private Il2CppRUMBLE.Players.PlayerData player;
        //private int previousPlayerHealth;
        private Il2CppRUMBLE.Players.PlayerData opponent;
        //private int previousOpponentHealth;
        private int clientRoundWins;

        private TimelineEventHandle_t roundEvent;

        public override void OnInitializeMelon()
        {
            base.OnInitializeMelon();

            Calls.onRoundStarted += RoundStarted;
            Calls.onRoundEnded += RoundEnded;

            Calls.onMatchStarted += MatchStarted;
            Calls.onMatchEnded += MatchEnded;

            //Calls.onLocalPlayerHealthChanged += PlayerHealthChange;
            //Calls.onRemotePlayerHealthChanged += OpponentHealthChange;
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
            MelonLogger.Msg($"{Priority}");
            switch (sceneName)
            {
                case "Gym":
                    break;
                case "Park":
                    break;
                case "Map0":
                case "Map1":
                    break;
            }
            if (sceneName == "Gym")
            {
                SteamTimeline.SetTimelineGameMode(ETimelineGameMode.k_ETimelineGameMode_Menus);
            }

            if (sceneName == "Map0" || sceneName == "Map1")
            {
                SteamTimeline.SetTimelineGameMode(ETimelineGameMode.k_ETimelineGameMode_Staging);
            }

            if (sceneName == "Park")
            {
                SteamTimeline.SetTimelineGameMode(ETimelineGameMode.k_ETimelineGameMode_Staging);
            }
        }

        //private void PlayerHealthChange()
        //{
        //    SteamTimeline.AddInstantaneousTimelineEvent(
        //        $"{previousPlayerHealth - player.HealthPoints}",
        //        $"Combat Event Count: {Il2CppRUMBLE.MoveSystem.CombatManager.Instance.runningCombatEvents.Count}",
        //        "steam_defend",
        //        0,
        //        0f,
        //        ETimelineEventClipPriority.k_ETimelineEventClipPriority_None);
        //    previousPlayerHealth = player.HealthPoints;
        //}

        //private void OpponentHealthChange()
        //{
        //    SteamTimeline.AddInstantaneousTimelineEvent(
        //        $"{previousOpponentHealth - opponent.HealthPoints}",
        //        $"Combat Event Count: {Il2CppRUMBLE.MoveSystem.CombatManager.Instance.runningCombatEvents.Count}",
        //        "steam_attack",
        //        0,
        //        0f,
        //        ETimelineEventClipPriority.k_ETimelineEventClipPriority_None);
        //    previousOpponentHealth = opponent.HealthPoints;
        //}

        private void RoundStarted()
        {
            int currentRound = Il2CppRUMBLE.Networking.MatchFlow.MatchHandler.Instance.CurrentRound + 1;

            roundEvent = SteamTimeline.StartRangeTimelineEvent(
                $"Round {currentRound}",
                $"Versus {opponentName}",
                "steam_pair",
                1000,
                0f,
                ETimelineEventClipPriority.k_ETimelineEventClipPriority_Standard);
        }
        private void RoundEnded()
        {
            int currentRound = Il2CppRUMBLE.Networking.MatchFlow.MatchHandler.Instance.CurrentRound + 1;
            Il2CppStructArray<int> roundsWon = Il2CppRUMBLE.Networking.MatchFlow.MatchHandler.Instance.RoundsWonList;

            SteamTimeline.UpdateRangeTimelineEvent(
                roundEvent,
                $"Round {currentRound}",
                $"Versus {opponentName}",
                player.HealthPoints > 0 ? "steam_crown" : "steam_death",
                1000,
                ETimelineEventClipPriority.k_ETimelineEventClipPriority_Standard);
            SteamTimeline.EndRangeTimelineEvent(roundEvent, 0);
            SteamTimeline.AddInstantaneousTimelineEvent(
                $"Round {currentRound} Win",
                $"Versus {opponentName}",
                player.HealthPoints > 0 ? "steam_crown" : "steam_death",
                900,
                0f,
                ETimelineEventClipPriority.k_ETimelineEventClipPriority_None);
            if (!Calls.Players.IsHost() && player.HealthPoints > 0
                || Calls.Players.IsHost() && player.HealthPoints <= 0)
            {
                clientRoundWins += 1;
            }
            //SteamTimeline.SetGamePhaseAttribute("Round Score", roundsWon.ToArray().ToString(), 10);
        }
        private void MatchStarted()
        {
            player = Calls.Players.GetLocalPlayer().Data;
            //previousPlayerHealth = 20;
            opponent = Calls.Players.GetEnemyPlayers().First().Data;
            //previousOpponentHealth = 20;
            opponentName = opponent.GeneralData.PublicUsername ?? "Unknown Opponent";
            opponentName = Regex.Replace(opponentName, "<.*?>|\\(.*?\\)|[^a-zA-Z0-9_ ]", "");

            clientRoundWins = 0;

            SteamTimeline.SetTimelineGameMode(ETimelineGameMode.k_ETimelineGameMode_Playing);
            SteamTimeline.StartGamePhase();
            SteamTimeline.SetGamePhaseID($"Match against {opponentName}");
            //SteamTimeline.SetGamePhaseAttribute("Round Score", "?/?", 10);
            SteamTimeline.SetGamePhaseAttribute("Opponent", opponentName, 9);
            if (Calls.Players.IsHost() == true)
            {
                SteamTimeline.SetGamePhaseAttribute("Host/Client", "Host", 10);
            } else
            {
                SteamTimeline.SetGamePhaseAttribute("Host/Client", "Client", 10);
            }
        }
        private void MatchEnded()
        {
            SteamTimeline.SetTimelineGameMode(ETimelineGameMode.k_ETimelineGameMode_Staging);
            if (player.MatchData.MatchPoints >= 2)
            {
                SteamTimeline.AddGamePhaseTag("Win", "steam_crown", "Result", 10);
            }
            else
            {
                SteamTimeline.AddGamePhaseTag("Loss", "steam_death", "Result", 10);
            }
            SteamTimeline.SetGamePhaseAttribute("Client Round Wins", $"Client round wins: {clientRoundWins}", 5);
            SteamTimeline.EndGamePhase();
        }
    }

}