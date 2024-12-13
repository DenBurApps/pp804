using System;
using UnityEngine;

namespace RunningCube
{
    public class Spikes : MovingObject
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Player player))
            {
                OnGotPlayer();
            }
        }
    }
}
