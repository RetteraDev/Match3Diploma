using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public class PlayerStatistics : MonoBehaviour
    {
        [Header("Player Statistics")]
        [Header("Player Points")]
        public int currentPoints;
        public Text pointsText;
        [Header("Player Health")]
        public int currentHP;
        public int maxHP;
        public Image hpImage;
        public Text hpText;
        private void Start()
        {
            GetDataFromMemory();
        }

        private void GetDataFromMemory()
        {
            currentHP = PlayerPrefs.GetInt("Health");
            currentPoints = PlayerPrefs.GetInt("Points");
            maxHP = PlayerPrefs.GetInt("MaxHealth");
        }

        private void Update()
        {
            UpdateHP();
            UpdatePoints();
        }

        private void UpdateHP()
        {
            currentHP = PlayerPrefs.GetInt("Health");
            hpImage.fillAmount = 1f/maxHP * currentHP;
            hpText.text = $"{currentHP}/{maxHP}";
        }

        private void UpdatePoints()
        {
            currentPoints = PlayerPrefs.GetInt("Points");
            pointsText.text = currentPoints.ToString();
        }

        public void OnButtonPress(string levelName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
        }
    }
}
