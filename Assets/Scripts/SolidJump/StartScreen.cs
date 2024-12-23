using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SolidJump
{
    public class StartScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text _fishText;
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
            _fishText.text = _playerBalance.CurrentBalance.ToString();

            var bestTime = StatisticsDataHolder.StatisticsDatas[1].BestTime;
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
