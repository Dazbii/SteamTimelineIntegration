using Il2CppRUMBLE.Players;
using System.Text.RegularExpressions;

namespace SteamTimelineIntegration
{
    internal class WinsLosses
    {
        public int wins = 0;
        public int losses = 0;
    }

    internal static class Util
    {
        public static WinsLosses CalcWinsLosses(int[] roundsWonList, int currentRound)
        {
            WinsLosses score = new();

            for (int i = 0; i <= currentRound; i++)
            {
                if (roundsWonList[i] == 1)
                {
                    score.wins++;
                }
                else
                {
                    score.losses++;
                }
            }

            return score;
        }

        public static string GetPlayerName(PlayerData player)
        {
            return Regex.Replace(player.GeneralData.PublicUsername, "<.*?>|\\(.*?\\)|[^a-zA-Z0-9_ ]", "");
        }
    }
}
