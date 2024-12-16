using System;
using UnityEngine;

namespace BarelDestroyer
{
    public class BulletReturner : MonoBehaviour
    {
        [SerializeField] private BulletSpawner _bulletSpawner;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Bullet bullet))
            {
                _bulletSpawner.ReturnToPool(bullet);
            }
        }
    }
}
