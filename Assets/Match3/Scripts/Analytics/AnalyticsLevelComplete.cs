using System.Collections.Generic;
using Unity.Services.Analytics;

namespace Match3
{
    public class AnalyticsLevelComplete : AnalyticsCore
    {
        public void Send(
            int level, bool isWin, int movesUsed, float timeUsed, int obstaclesLeft,
            int horisontalBonus, int verticalBonus, int rainbowBonus, int score, int stars
        )
        {
            AnalyticsService.Instance.CustomData(
                "levelComplete",
                new Dictionary<string, object> {
                    { "level", level },
                    { "isWin", isWin },
                    { "movesUsed", movesUsed },
                    { "timeUsed", timeUsed },
                    { "obstaclesLeft", obstaclesLeft },
                    { "horisontalBonus", horisontalBonus },
                    { "verticalBonus", verticalBonus },
                    { "rainbowBonus", rainbowBonus },
                    { "score", score },
                    { "stars", stars }
                }
            );

            AnalyticsService.Instance.Flush();
        }
    }
}