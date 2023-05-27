using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace Match3
{
    public class LevelMoves : Level
    {

        [HideInInspector] public float _timer;
        public int targetScore;
        private int _movesUsed = 0;

        private void Start()
        {
            type = LevelType.Moves;
            targetScore = score1Star;
            hud.SetLevelType(type);
            hud.SetScore(currentScore);
            hud.SetTarget(targetScore);
            hud.SetRemaining(numMoves);
        }

        private void Update()
        {
            _timer += Time.deltaTime;
        }

        public override void OnMove()
        {
            _movesUsed++;

            hud.SetRemaining(numMoves - _movesUsed);

            if (numMoves - _movesUsed != 0) return;

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
                    { "movesLeft", numMoves },
                    { "timeUsed", Mathf.Round(_timer) },
                    { "timeLeft", 0 },
                    { "obstaclesDestroyed", 0 },
                    { "obstaclesLeft", 0 },
                    { "horisontalBonus", horisontalBonus },
                    { "verticalBonus", verticalBonus },
                    { "rainbowBonus", rainbowBonus },
                    { "score", currentScore },
                    { "stars", hud._starIndex }
                }
            );
        }
    }
}
