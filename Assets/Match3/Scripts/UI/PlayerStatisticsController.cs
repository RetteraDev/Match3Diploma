using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public class PlayerStatisticsController : MonoBehaviour
    {
        [System.Serializable]
        public struct ButtonPlayerPrefs
        {
            public GameObject gameObject;
            public string playerPrefKey;
        };
        [Header("Level States Buttons:")]
        public ButtonPlayerPrefs[] buttons;

        public bool debugMode;
        [Header("Player Statistics")]
        [Header("Player Points")]
        public int currentPoints;
        public Text pointsText;
        [Header("Player Health")]
        public int currentHP;
        public int maxHP;
        public Image hpImage;
        public Text hpText;
        [Header("Timer Settings")]
        public int defaultStartMinutes = 1;
        public int defaultStartSeconds = 0;
        private void Awake()
        {
            SetDefaults();
            GetDataFromMemory();
            ManageLevelButtons();
            LockLevelButtons();
        }

        private void SetDefaults()
        {
            PlayerPrefs.SetInt("MaxHealth", maxHP);
            PlayerPrefs.SetInt("defaultStartMinutes", defaultStartMinutes);
            PlayerPrefs.SetInt("defaultStartSeconds", defaultStartSeconds);
        }

        private void GetDataFromMemory()
        {
            if(debugMode) PlayerPrefs.DeleteAll();

            if(PlayerPrefs.HasKey("PlayerModel"))
            {
                currentHP = PlayerPrefs.GetInt("Health");
                currentPoints = PlayerPrefs.GetInt("Points");
            }
            else
            {
                PlayerPrefs.SetString("PlayerModel",  SystemInfo.deviceUniqueIdentifier);
                PlayerPrefs.SetInt("Health", 2);
                PlayerPrefs.SetInt("Points", 20000);
            }            
        }

        private void ManageLevelButtons()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                int score = PlayerPrefs.GetInt(buttons[i].playerPrefKey, 0);

                for (int starIndex = 1; starIndex <= 3; starIndex++)
                {
                    Transform star = buttons[i].gameObject.transform.Find("Stars").Find($"Star_0{starIndex}");
                    star.gameObject.SetActive(starIndex <= score);                
                }
            }
        }

        private void LockLevelButtons()
        {
            for (int i = 1; i < buttons.Length; i++)
            {
                buttons[i].gameObject.transform.GetChild(0).GetComponent<Button>().interactable = false;
                int score = PlayerPrefs.GetInt(buttons[i - 1].playerPrefKey);
                if (score >= 1) buttons[i].gameObject.transform.GetChild(0).GetComponent<Button>().interactable = true;
            }
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
            if(currentHP > 0) UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
        }
    }
}
