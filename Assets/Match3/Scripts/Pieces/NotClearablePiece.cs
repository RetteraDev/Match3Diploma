using System.Collections;
using UnityEngine;

namespace Match3
{
    public class NotClearablePiece : MonoBehaviour
    {
        protected GamePiece piece;

        private void Awake()
        {
            piece = GetComponent<GamePiece>();
        }
    }
}
