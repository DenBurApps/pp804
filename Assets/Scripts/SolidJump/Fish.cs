using System;
using UnityEngine;

namespace SolidJump
{
    public class Fish : MonoBehaviour
    {
        public event Action GotPlayer;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerMover player))
            {
                GotPlayer?.Invoke();
                gameObject.SetActive(false);
            }
        }
    }
}
