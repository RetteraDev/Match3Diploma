using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public class Timer : MonoBehaviour
    {
        [Header("PlayerStatistics")]
        public PlayerStatisticsController PlayerStatisticsController;
        public PlayerStatistics PlayerStatistics;
        [Header("Timer Settings")]
        public int timeLeft;
        public Text timerText;
        int currentHP, maxHP;
        int defaultStartMinutes = 0;
        int defaultStartSeconds = 0;

        public void Start()
        {
            if(PlayerStatisticsController == null) currentHP = PlayerStatistics.currentHP;
            currentHP = PlayerPrefs.GetInt("Health");
            maxHP = PlayerPrefs.GetInt("MaxHealth");

            defaultStartMinutes = PlayerPrefs.GetInt("defaultStartMinutes");
            defaultStartSeconds = PlayerPrefs.GetInt("defaultStartSeconds");

            timeLeft = defaultStartMinutes * 60 + defaultStartSeconds;
            
            if (PlayerPrefs.HasKey("TimeOnExit"))
            { 
                timeLeft = PlayerPrefs.GetInt("TimeOnExit");
                PlayerPrefs.DeleteKey("TimeOnExit");
            }
            if(currentHP < PlayerPrefs.GetInt("MaxHealth"))
            {
                StartCoroutine("LoseTime");
                print("TimeLeft: " + timeLeft);
                Time.timeScale = 1;
            }
        }
        
        public void Update()
        {
            currentHP = PlayerPrefs.GetInt("Health");
            maxHP = PlayerPrefs.GetInt("MaxHealth");
            if(currentHP < maxHP) timerText.text = ("" + (timeLeft > 0 ? $"{timeLeft/60}:{(timeLeft % 60):D2}" : "").ToString());
            else timerText.text = ("");
        }

        IEnumerator LoseTime()
        {
            while (timeLeft > 0) {
                yield return new WaitForSeconds (1);
                timeLeft--;
                if(timeLeft == 0) PlayerPrefs.SetInt("Health", currentHP += 1);
            }
        }

        private void OnApplicationQuit()
        {
            SaveTime();
        }

        public void SaveTime(){
            if (timeLeft > 0) PlayerPrefs.SetInt("TimeOnExit", timeLeft);
        }
    }
}
