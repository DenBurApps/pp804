using System;
using UnityEngine;

namespace RunningCube
{
    public class PlayerBalance : MonoBehaviour
    {
        private const string BalanceSaveKey = "RunningCubeBalance";

        public int CurrentBalance { get; private set; }

        private void Start()
        {
            LoadBalance();
        }
        
        public void IncreaseBalance(int value)
        {
            CurrentBalance += value;
            PlayerPrefs.SetInt(BalanceSaveKey, CurrentBalance);
        }

        public void DecreaseBalance(int value)
        {
            CurrentBalance = (int)Mathf.Clamp(CurrentBalance - value, 0, Mathf.Infinity);
            PlayerPrefs.SetInt(BalanceSaveKey, CurrentBalance);
        }

        private void LoadBalance()
        {
            if (!PlayerPrefs.HasKey(BalanceSaveKey))
            {
                CurrentBalance = 0;
                return;
            }

            CurrentBalance = PlayerPrefs.GetInt(BalanceSaveKey);
        }
    }
}