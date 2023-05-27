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

            bool isWin = false;
            if (currentScore >= targetScore)
            {
                GameWin();
                isWin = true;
            }
            else
            {
                GameLose();
            }

            analyticSender.Send(
                1,
                isWin,
                _movesUsed,
                Mathf.Round(_timer),
                0,
                horisontalBonus,
                verticalBonus,
                rainbowBonus,
                currentScore,
                hud._starIndex

            );
        }
    }
}
