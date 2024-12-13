using RunningCube;
using UnityEngine;

namespace SolidJump
{
    public class Platform : MovingObject
    {
        [SerializeField] private Fish _fish;

        private void OnEnable()
        {
            if (_fish != null)
            {
                _fish.gameObject.SetActive(true);
                _fish.GotPlayer += OnGotPlayer;
            }
        }

        private void OnDisable()
        {
            if (_fish != null)
                _fish.GotPlayer -= OnGotPlayer;
        }
    }
}