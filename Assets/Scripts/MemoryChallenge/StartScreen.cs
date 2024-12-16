using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MemoryChallenge
{
    public class StartScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text _bestTimeText;

        public event Action PlayClicked;

        private void Start()
        {
            Enable();
        }

        public void Enable()
        {
            gameObject.SetActive(true);

            var bestTime = StatisticsDataHolder.StatisticsDatas[2].BestTime;
            Debug.Log(StatisticsDataHolder.StatisticsDatas[2].BestTime);
            int minutes = Mathf.FloorToInt(bestTime / 60);
            int seconds = Mathf.FloorToInt(bestTime % 60);

            _bestTimeText.text = $"{minutes:00}:{seconds:00}";
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void OnExitClicked()
        {
            SceneManager.LoadScene("MainScene");
        }

        public void OnPlayClicked()
        {
            PlayClicked?.Invoke();
            Disable();
        }
    }
}