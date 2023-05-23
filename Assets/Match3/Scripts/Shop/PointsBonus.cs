using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public class PointsBonus : MonoBehaviour
    {
        public RewardedAds rewardedAds;

        public void WatchAds()
        {
            rewardedAds.ShowAd();
            AddPoints();
        }

        private void AddPoints()
        {
            int currentPoints = PlayerPrefs.GetInt("Points");
            PlayerPrefs.SetInt("Points", currentPoints += 1000);
        }
    }
}
