using System;
using SpaceMission;
using UnityEngine;

namespace RunningCube
{
    public class Spikes : MovingObject
    {
        public event Action GotPlayer;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Player player))
            {
                GotPlayer?.Invoke();
            }
        }
    }
}
