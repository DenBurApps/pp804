using System;
using System.Collections;
using RunningCube;
using UnityEngine;

namespace BarelDestroyer
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _speed;
        
        private Transform _transform;
        private IEnumerator _movingCoroutine;
        private Vector2 _defaultPosition;

        public event Action<Bullet> CollidedWithBarrel;

        private void Awake()
        {
            _transform = transform;
            _defaultPosition = _transform.position;
        }
        
        public void EnableMovement()
        {
            if (_movingCoroutine == null)
                _movingCoroutine = StartMoving();

            StartCoroutine(_movingCoroutine);
        }
    
        public void DisableMovement()
        {
            if (_movingCoroutine != null)
            {
                StopCoroutine(_movingCoroutine);
                _movingCoroutine = null;
            }
        }

        public void SetSpeed(float speed)
        {
            _speed = speed;
        }

        private IEnumerator StartMoving()
        {
            while (true)
            {
                _transform.position += Vector3.up * _speed * Time.deltaTime;
            
                yield return null;
            }
        }

        public void Collided()
        {
            CollidedWithBarrel?.Invoke(this);
        }
    }
}
