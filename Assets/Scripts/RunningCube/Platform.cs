using System;
using UnityEngine;

namespace RunningCube
{
    public class Platform : MovingObject
    {
        [SerializeField] private Coin _coin;
        
        private void OnEnable()
        {
            _coin = GetComponentInChildren<Coin>();
            _coin.gameObject.SetActive(true);
            _coin.GotPlayer += OnGotPlayer;
        }

        private void OnDisable()
        {
            _coin.GotPlayer -= OnGotPlayer;
        }
    }
}
