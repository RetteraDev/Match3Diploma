using UnityEngine;

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
                _sender.Send(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, "Lose", $"{_movesUsed}",
                $"{numMoves}", $"{Mathf.Round(_timer)}", $"0", $"{_numObstacles - _numObstaclesLeft}", $"{_numObstaclesLeft}",
                $"{horisontalBonus}", $"{verticalBonus}", $"{rainbowBonus}", $"{currentScore}", $"0", $"0");
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
                _sender.Send(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, "Win", $"{_movesUsed}",
                $"{numMoves}", $"{Mathf.Round(_timer)}", $"0", $"{_numObstacles - _numObstaclesLeft}", $"{_numObstaclesLeft}",
                $"{horisontalBonus}", $"{verticalBonus}", $"{rainbowBonus}", $"{currentScore}", $"{currentScore/10}", $"{hud._starIndex}");
            }
        }
    }
}
