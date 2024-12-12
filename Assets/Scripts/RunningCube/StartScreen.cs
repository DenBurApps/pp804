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

        public event Action PlayClicked;
        
        public void Enable()
        {
            gameObject.SetActive(true);
            _coinsText.text = "<sprite name=\"Fra1me 8 2\">  " + 0;
            /*int minutes = Mathf.FloorToInt(0 / 60);
            int seconds = Mathf.FloorToInt(0 % 60);*/

           // _bestTimeText.text = $"{minutes:00}:{seconds:00}";
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
