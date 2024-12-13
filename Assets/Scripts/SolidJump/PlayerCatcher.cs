using System;
using UnityEngine;

namespace SolidJump
{
    public class PlayerCatcher : MonoBehaviour
    {
        public event Action PlayerCathced;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerMover playerMover))
            {
                PlayerCathced?.Invoke();
            }
        }
    }
}
