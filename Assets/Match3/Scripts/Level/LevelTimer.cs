using UnityEngine;

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
                if (currentScore >= targetScore)
                {
                    GameWin();
                    _sender.Send(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, "Win", $"{_movesUsed}",
                    $"0", $"{Mathf.Round(_timer)}", $"{Mathf.Round(timeInSeconds)}", $"0", $"0", $"{horisontalBonus}",
                    $"{verticalBonus}", $"{rainbowBonus}", $"{currentScore}", $"{currentScore/10}", $"{hud._starIndex}");
                }
                else
                {
                    GameLose();
                    _sender.Send(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, "Lose", $"{_movesUsed}",
                    $"0", $"{Mathf.Round(_timer)}", $"{Mathf.Round(timeInSeconds)}", $"0", $"0", $"{horisontalBonus}",
                    $"{verticalBonus}", $"{rainbowBonus}", $"{currentScore}", $"0", $"0");
                }
                levelCompleted = true;
            }
        }
	
    }
}
