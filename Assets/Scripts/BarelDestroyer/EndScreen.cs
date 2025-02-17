using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BarelDestroyer
{
    public class EndScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text _barrelsText;
        //[SerializeField] private TMP_Text _currentTimeText;
        [SerializeField] private TMP_Text _bestText;

        private ScreenVisabilityHandler _screenVisabilityHandler;

        public event Action RestartClicked;
        public event Action MainMenuClicked;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void Start()
        {
            Disable();
        }

        public void Enable(int barrels, string time, float bestBarrels)
        {
            _screenVisabilityHandler.EnableScreen();
            _barrelsText.text = barrels.ToString();

           // _currentTimeText.text = time;

            _bestText.text = bestBarrels.ToString();
        }

        public void Disable()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        public void OnRestartClicked()
        {
            RestartClicked?.Invoke();
            _screenVisabilityHandler.DisableScreen();
        }

        public void OnMainMenuClicked()
        {
            MainMenuClicked?.Invoke();
            Disable();
        }
    }
}