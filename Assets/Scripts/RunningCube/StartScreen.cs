using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RunningCube
{
    public class StartScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text _coinsText;
        [SerializeField] private TMP_Text _bestTimeText;
        [SerializeField] private PlayerBalance _playerBalance;

        public event Action PlayClicked;

        private void Start()
        {
            Disable();
        }

        public void Enable()
        {
            gameObject.SetActive(true);
            _coinsText.text = "<sprite name=\"Fra1me 8 2\">  " + _playerBalance.CurrentBalance;

            var bestTime = StatisticsDataHolder.StatisticsDatas[0].BestTime;
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
