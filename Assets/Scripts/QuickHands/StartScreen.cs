using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QuickHands
{
    public class StartScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text _bestBarelsText;

        public event Action PlayClicked;

        private void Start()
        {
            Enable();
        }

        public void Enable()
        {
            gameObject.SetActive(true);
            _bestBarelsText.text = StatisticsDataHolder.StatisticsDatas[4].BestTime.ToString();
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
