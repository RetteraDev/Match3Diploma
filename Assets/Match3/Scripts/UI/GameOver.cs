using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public class GameOver : MonoBehaviour
    {
        public GameObject screenParent;
        public GameObject scoreParent;
        public Text messageText;
        public Text scoreText;
        public Image[] stars;
        private bool gameOver;

        private void Start ()
        {
            screenParent.SetActive(false);

            for (int i = 0; i < stars.Length; i++)
                stars[i].enabled = false;
        }

        public void ShowLose(int score)
        {
            screenParent.SetActive(true);
            scoreParent.SetActive(false);
            messageText.text = "You Lose!";

            scoreText.text = score.ToString();
            scoreText.enabled = false;

            Animator animator = GetComponent<Animator>();

            if (animator)
                animator.Play("GameOverShow");
            
            gameOver = true;
        }

        public void WatchAds()
        {
            PlayerPrefs.SetInt("Health", PlayerPrefs.GetInt("Health") + 1);
            print("Ads has been watched");
        }

        public void ShowWin(int score, int starCount)
        {
            screenParent.SetActive(true);
            messageText.text = "You win!";

            scoreText.text = score.ToString();
            scoreText.enabled = false;

            Animator animator = GetComponent<Animator>();

            if (animator)
                animator.Play("GameOverShow");

            PlayerPrefs.SetInt("Points", PlayerPrefs.GetInt("Points") + score/10);
            gameOver = false;
            StartCoroutine(ShowWinCoroutine(starCount));
        }

        private IEnumerator ShowWinCoroutine(int starCount)
        {
            yield return new WaitForSeconds(0.5f);

            if (starCount < stars.Length)
            {
                for (int i = 0; i <= starCount; i++)
                {
                    stars[i].enabled = true;

                    if (i > 0)
                        stars[i - 1].enabled = false;

                    yield return new WaitForSeconds(0.5f);
                }
            }

            scoreText.enabled = true;
        }

        public void OnReplayClicked()
        {
            PlayerPrefs.SetInt("Health", PlayerPrefs.GetInt("Health") - 1);
            if(PlayerPrefs.GetInt("Health") == 0)
            {
                WatchAds();
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            }
            else UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        public void OnDoneClicked()
        {
            if(gameOver) PlayerPrefs.SetInt("Health", PlayerPrefs.GetInt("Health") - 1);
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        }

    }
}
