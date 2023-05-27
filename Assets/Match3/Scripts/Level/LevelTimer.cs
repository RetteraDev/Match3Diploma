using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace Match3
{
    public class LevelTimer : Level
    {
        public int targetScore;
        private float _timer;
        private int _movesUsed = 0;
        private bool levelCompleted;

        private void Start ()
        {
            type = LevelType.Timer;
            targetScore = score1Star;
            hud.SetLevelType(type);
            hud.SetScore(currentScore);
            hud.SetTarget(targetScore);
            hud.SetRemaining($"{timeInSeconds / 60}:{timeInSeconds % 60:00}");
        }

        public override void OnMove() => _movesUsed++;

        private void Update()
        {
            _timer += Time.deltaTime;
            hud.SetRemaining(
                $"{(int) Mathf.Max((timeInSeconds - _timer) / 60, 0)}:{(int) Mathf.Max((timeInSeconds - _timer) % 60, 0):00}");

            if (timeInSeconds - _timer <= 0 && !levelCompleted)
            {
                bool playerWon = false;
                if (currentScore >= targetScore)
                {
                    GameWin();
                    playerWon = true;
                }
                else
                {
                    GameLose();
                }
                Analytics.CustomEvent(
                    "levelComplete",
                    new Dictionary<string, object> {
                        { "levelName", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name },
                        { "playerWon", playerWon },
                        { "movesUsed", _movesUsed },
                        { "movesLeft", 0 },
                        { "timeUsed", Mathf.Round(_timer) },
                        { "timeLeft", Mathf.Round(timeInSeconds) },
                        { "obstaclesDestroyed", 0 },
                        { "obstaclesLeft", 0 },
                        { "horisontalBonus", horisontalBonus },
                        { "verticalBonus", verticalBonus },
                        { "rainbowBonus", rainbowBonus },
                        { "score", currentScore },
                        { "stars", hud._starIndex }
                    }
                );
                levelCompleted = true;
            }
        }
	
    }
}
