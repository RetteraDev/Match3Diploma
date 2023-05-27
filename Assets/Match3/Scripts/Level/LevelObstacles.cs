using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace Match3
{
    public class LevelObstacles : Level
    {
        [HideInInspector] public float _timer;
        public PieceType[] obstacleTypes;
        private const int ScorePerPieceCleared = 1000;
        private int _movesUsed = 0;
        private int _numObstaclesLeft;
        private int _numObstacles;

        private void Start ()
        {
            type = LevelType.Obstacle;

            for (int i = 0; i < obstacleTypes.Length; i++) _numObstaclesLeft += gameGrid.GetPiecesOfType(obstacleTypes[i]).Count;
            _numObstacles = _numObstaclesLeft;

            hud.SetLevelType(type);
            hud.SetScore(currentScore);
            hud.SetTarget(_numObstaclesLeft);
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

            if (numMoves - _movesUsed == 0 && _numObstaclesLeft > 0)
            {
                GameLose();
                Analytics.CustomEvent(
                    "gameOver",
                    new Dictionary<string, object> {
                        { "levelName", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name },
                        { "playerWon", false },
                        { "movesUsed", _movesUsed },
                        { "movesLeft", numMoves },
                        { "timeUsed", Mathf.Round(_timer) },
                        { "timeLeft", 0 },
                        { "obstaclesDestroyed", _numObstacles - _numObstaclesLeft },
                        { "obstaclesLeft", _numObstaclesLeft },
                        { "horisontalBonus", horisontalBonus },
                        { "verticalBonus", verticalBonus },
                        { "rainbowBonus", rainbowBonus },
                        { "score", currentScore },
                        { "stars", 0 }
                    }
                );
            }
        }

        public override void OnPieceCleared(GamePiece piece)
        {
            base.OnPieceCleared(piece);

            for (int i = 0; i < obstacleTypes.Length; i++)
            {
                if (obstacleTypes[i] != piece.Type) continue;
            
                _numObstaclesLeft--;
                hud.SetTarget(_numObstaclesLeft);
                if (_numObstaclesLeft != 0) continue;
            
                currentScore += ScorePerPieceCleared * (numMoves - _movesUsed);
                hud.SetScore(currentScore);
                GameWin();
                Analytics.CustomEvent(
                    "levelComplete",
                    new Dictionary<string, object> {
                        { "levelName", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name },
                        { "playerWon", true },
                        { "movesUsed", _movesUsed },
                        { "movesLeft", numMoves },
                        { "timeUsed", Mathf.Round(_timer) },
                        { "timeLeft", 0 },
                        { "obstaclesDestroyed", _numObstacles - _numObstaclesLeft },
                        { "obstaclesLeft", _numObstaclesLeft },
                        { "horisontalBonus", horisontalBonus },
                        { "verticalBonus", verticalBonus },
                        { "rainbowBonus", rainbowBonus },
                        { "score", currentScore },
                        { "stars", 0 }
                    }
                );
            }
        }
    }
}
