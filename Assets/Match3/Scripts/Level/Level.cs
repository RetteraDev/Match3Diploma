using System.Collections;
using UnityEngine;

namespace Match3
{
    public class Level : MonoBehaviour
    {
        public GameGrid gameGrid;
        public Hud hud;
        public DataSender _sender;

        public int score1Star;
        public int score2Star;
        public int score3Star;
        public int numMoves;
        public int timeInSeconds;
        [HideInInspector] public int horisontalBonus, verticalBonus, rainbowBonus;

        protected LevelType type;

        protected int currentScore;

        private bool _didWin;

        private void Awake()
        {
            gameGrid = this.GetComponent<GameGrid>();
            hud = this.GetComponent<Hud>();
            _sender = this.GetComponent<DataSender>();
        }
        private void Start()
        {
            hud.SetScore(currentScore);
        }

        public LevelType Type => type;

        protected virtual void GameWin()
        {
            gameGrid.GameOver();
            _didWin = true;
            StartCoroutine(WaitForGridFill());
        }

        protected virtual void GameLose()
        {        
            gameGrid.GameOver();
            _didWin = false;
            StartCoroutine(WaitForGridFill());
        }
    
        public virtual void OnMove()
        {
        }

        public virtual void OnPieceCleared(GamePiece piece)
        {
            currentScore += piece.score;
            hud.SetScore(currentScore);
        }

        protected virtual IEnumerator WaitForGridFill()
        {
            while (gameGrid.IsFilling)
            {
                yield return null;
            }

            if (_didWin) hud.OnGameWin(currentScore);
            else hud.OnGameLose(currentScore);
        }
    }
}
