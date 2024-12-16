using System;
using RunningCube;
using UnityEngine;

namespace BarelDestroyer
{
    public class Cannon : MonoBehaviour
    {
        [SerializeField] private float _movingSpeed;
        [SerializeField] private float _maxXposition;
        [SerializeField] private float _minXposition;
        [SerializeField] private Transform _shotPoint;
        [SerializeField] private BulletSpawner _bulletSpawner;

        private Vector2 _defaultPosition;
        private Vector2 _previousTouchPosition;
        private Transform _transform;
        private int _currentDirection = 0;

        public Transform ShotPoint => _shotPoint; 

        private void Awake()
        {
            _transform = transform;
            _defaultPosition = _transform.position;
        }

        private void Start()
        {
            _bulletSpawner.SetSpawnPosition(_shotPoint);
            _transform.position = _defaultPosition;
        }

        public void ReturnToDefaultPosition()
        {
            _transform.position = _defaultPosition;
        }

        public void Shoot()
        {
            _bulletSpawner.Spawn();
        }

        private void Update()
        {
            if (_currentDirection != 0)
            {
                Move(_currentDirection);
            }
        }

        private void Move(int direction)
        {
            Vector2 newPosition = _transform.position;
            newPosition.x += direction * _movingSpeed * Time.deltaTime;

            newPosition.x = Mathf.Clamp(newPosition.x, _minXposition, _maxXposition);

            _transform.position = newPosition;
        }

        public void SetDirection(int direction)
        {
            _currentDirection = direction;
        }
    }
}