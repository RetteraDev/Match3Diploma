using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public class HPBonus : MonoBehaviour
    {
        public AdsController adsController;
        public int hpCost;
        public Text hpCostText;

        public void WatchAds()
        {
            if(PlayerPrefs.GetInt("Health") < PlayerPrefs.GetInt("MaxHealth"))
            {
                adsController.ShowAds();
                AddHP();
                print("Ads watched");
            }
        }

        public void BuyHP()
        {
            if(PlayerPrefs.GetInt("Health") < PlayerPrefs.GetInt("MaxHealth"))
            {
                int points = PlayerPrefs.GetInt("Points");
                if(points > hpCost)
                {
                    PlayerPrefs.SetInt("Points", points - hpCost);
                    AddHP();
                    print("HP Bought");
                }
            }
        }

        private void AddHP()
        {
            int currentHP = PlayerPrefs.GetInt("Health");
            PlayerPrefs.SetInt("Health", currentHP += 1);
        }
    }
}
