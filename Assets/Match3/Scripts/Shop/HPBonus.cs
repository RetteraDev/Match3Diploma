using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public class HPBonus : MonoBehaviour
    {
        public RewardedAds rewardedAds;

        public void WatchAds()
        {   
            if(PlayerPrefs.GetInt("Health") < PlayerPrefs.GetInt("MaxHealth"))
            {
                rewardedAds.ShowAd();
                AddHP();
            }
        }

        private void AddHP()
        {
            int currentHP = PlayerPrefs.GetInt("Health");
            PlayerPrefs.SetInt("Health", currentHP += 1);
        }
    }
}
