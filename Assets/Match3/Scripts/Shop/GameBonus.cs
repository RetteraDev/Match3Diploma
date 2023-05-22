using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public class GameBonus : MonoBehaviour
    {
        public PieceType type;
        public Level level;
        public GameGrid grid;
        public AdsController adsController;

        public int price;
        public int uses;
        public Text priceText;

        private void Update()
        {
            priceText.text = price + "Pt";
        }

        public void BuyBonus()
        {
            if(PlayerPrefs.GetInt("Points") >= price)
            {
                int points = PlayerPrefs.GetInt("Points");
                PlayerPrefs.SetInt("Points", points - price);   
            }
            else
            {
                adsController.ShowAds();
            }
            uses++;
            switch(type)
            {
                case PieceType.RowClear:
                    level.horisontalBonus = uses;
                    break;
                case PieceType.ColumnClear:
                    level.verticalBonus = uses;
                    break;
                case PieceType.Rainbow:
                    level.rainbowBonus = uses;
                    break;
            }
            InstantiateBonus();
        }

        private void InstantiateBonus()
        {
            bool isAvailable = false;
            while(!isAvailable)
            {
                int x = UnityEngine.Random.Range(0, grid.xDim);
                int y = UnityEngine.Random.Range(0, grid.yDim);
                if(grid._pieces[x, y].Type == PieceType.Normal)
                {
                    grid.ChangePiece(x, y, type);
                    isAvailable = true;
                }
            }
        }
    }
}
