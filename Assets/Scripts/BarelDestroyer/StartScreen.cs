using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BarelDestroyer
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
            _bestBarelsText.text = StatisticsDataHolder.StatisticsDatas[3].CollectedBonuses.ToString();
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