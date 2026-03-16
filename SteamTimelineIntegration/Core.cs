using Il2CppSteamworks;
using MelonLoader;
using RumbleModUI;
using RumbleModdingAPI.RMAPI;
using Il2CppRUMBLE.Players;

namespace SteamTimelineIntegration
{
    public static class BuildInfo
    {
        public const string ModName = "SteamTimelineIntegration";
        public const string ModVersion = "1.0.1";
        public const string Description = "Rumble mod to enable integrations with the steam timeline ";
        public const string Author = "Dazbii";
        public const string Company = "";
    }

    enum Gamemode
    {
        Playing = ETimelineGameMode.k_ETimelineGameMode_Playing,
        Park = ETimelineGameMode.k_ETimelineGameMode_Staging,
        Gym = ETimelineGameMode.k_ETimelineGameMode_Menus,
        BetweenMatches = ETimelineGameMode.k_ETimelineGameMode_LoadingScreen
    }

    public class Core : MelonMod
    {
        Mod Mod = new Mod();
        private ModSetting<bool> incomingDamage;
        private ModSetting<bool> outgoingDamage;

        private string opponentName;
        private PlayerData player;
        private PlayerData opponent;
        private int clientRoundWins;

        private string previousOpponentName;

        private int cumulativeWins;
        private int cumulativeClientWins;
        private int cumulativeLosses;
        private int cumulativeHostLosses;

        private TimelineEventHandle_t roundEvent;

        public override void OnLateInitializeMelon()
        {
            base.OnLateInitializeMelon();

            UI.instance.UI_Initialized += OnUIInit;

            Actions.onRoundStarted += RoundStarted;

            Actions.onMatchStarted += MatchStarted;
            Actions.onMatchEnded += MatchEnded;

            Actions.onPlayerHealthChanged += PlayerHealthChange;
        }

        public void OnUIInit()
        {
            Mod.ModName = BuildInfo.ModName;
            Mod.ModVersion = BuildInfo.ModVersion;
            Mod.SetFolder(BuildInfo.ModName);
            Mod.AddDescription("Description", "", BuildInfo.Description, new Tags { IsSummary = true });

            incomingDamage = Mod.AddToList("Incoming", false, 0, "!!WARNING: This will clutter your timeline!!\n" +
                "If true, this will set add a marker each time you take damage", new Tags());
            outgoingDamage = Mod.AddToList("Outgoing", false, 0, "!!WARNING: This will clutter your timeline!!\n" +
                "If true, this will set add a marker each time your opponent takes damage", new Tags());

            Mod.GetFromFile();

            UI.instance.AddMod(Mod);

            LoggerInstance.Msg("Added Mod");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
            switch (sceneName)
            {
                case "Gym":
                    SteamTimeline.SetTimelineGameMode((ETimelineGameMode) Gamemode.Gym);
                    break;
                case "Park":
                    SteamTimeline.SetTimelineGameMode((ETimelineGameMode) Gamemode.Park);
                    break;
                case "Map0":
                case "Map1":
                    SteamTimeline.SetTimelineGameMode((ETimelineGameMode) Gamemode.BetweenMatches);
                    break;
            }
        }

        private void PlayerHealthChange(Player damagedPlayer, int damage)
        {
            bool isDamageIncoming = damagedPlayer == player.Player;
            if ((isDamageIncoming && (bool) incomingDamage.Value)
                || (!isDamageIncoming && (bool) outgoingDamage.Value))
            {
                SteamTimeline.AddInstantaneousTimelineEvent(
                    $"{damage}",
                    "",
                    isDamageIncoming ? "steam_defend" : "steam_attack",
                    0,
                    0f,
                    ETimelineEventClipPriority.k_ETimelineEventClipPriority_None);         
            }
        }

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

        public void RoundEnded(int[] roundsWonList, int currentRound)
        {
            WinsLosses score = Util.CalcWinsLosses(roundsWonList, currentRound);

            if (!Calls.Players.IsHost() && player.HealthPoints > 0
                || Calls.Players.IsHost() && player.HealthPoints <= 0)
            {
                clientRoundWins += 1;
                if (!Calls.Players.IsHost())
                {
                    cumulativeClientWins += 1;
                } else
                {
                    cumulativeHostLosses += 1;
                }
            }

            SteamTimeline.UpdateRangeTimelineEvent(
                roundEvent,
                $"Round {currentRound}",
                $"Versus {opponentName}",
                player.HealthPoints > 0 ? "steam_crown" : "steam_death",
                1000,
                ETimelineEventClipPriority.k_ETimelineEventClipPriority_Standard);
            SteamTimeline.EndRangeTimelineEvent(roundEvent, 0);
            SteamTimeline.AddInstantaneousTimelineEvent(
                $"Round {currentRound} {(player.HealthPoints > 0 ? "Win" : "Loss")}",
                $"Versus {opponentName}",
                player.HealthPoints > 0 ? "steam_crown" : "steam_death",
                900,
                0f,
                ETimelineEventClipPriority.k_ETimelineEventClipPriority_None);
            SteamTimeline.SetGamePhaseAttribute("Round Score", $"{score.wins}/{score.losses}", 2);
        }

        private void MatchStarted()
        {
            opponent = Calls.Players.GetEnemyPlayers().First().Data;
            player = Calls.Players.GetLocalPlayer().Data;
            opponentName = Util.GetPlayerName(opponent);

            if (opponentName != previousOpponentName)
            {
                cumulativeClientWins = 0;
                cumulativeHostLosses = 0;
                cumulativeLosses = 0;
                cumulativeWins = 0;
            }
            previousOpponentName = opponentName;
            

            clientRoundWins = 0;

            SteamTimeline.SetTimelineGameMode((ETimelineGameMode) Gamemode.Playing);
            SteamTimeline.StartGamePhase();
            SteamTimeline.SetGamePhaseID($"Match against {opponentName}");
            SteamTimeline.SetGamePhaseAttribute("Round Score", "0/0", 10);
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
            SteamTimeline.SetTimelineGameMode((ETimelineGameMode) Gamemode.BetweenMatches);
            if (player.MatchData.MatchPoints >= 2)
            {
                SteamTimeline.AddGamePhaseTag("Win", "steam_crown", "Result", 10);
                cumulativeWins += 1;
                if (clientRoundWins == 2)
                {
                    cumulativeClientWins -= 2;
                }
            }
            else
            {
                SteamTimeline.AddGamePhaseTag("Loss", "steam_death", "Result", 10);
                cumulativeLosses += 1;
                if (clientRoundWins == 2)
                {
                    cumulativeHostLosses -= 2;
                }
            }
            SteamTimeline.SetGamePhaseAttribute("Set Score", $"Set: {cumulativeWins}({cumulativeClientWins}) - {cumulativeLosses}({cumulativeHostLosses})", 10);
            SteamTimeline.SetGamePhaseAttribute("Client Round Wins", $"Client round wins: {clientRoundWins}", 5);
            SteamTimeline.EndGamePhase();
        }
    }

}