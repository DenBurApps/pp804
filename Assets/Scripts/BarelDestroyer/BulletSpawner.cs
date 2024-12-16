using System.Collections.Generic;
using UnityEngine;

namespace BarelDestroyer
{
    public class BulletSpawner : ObjectPool<Bullet>
    {
        [SerializeField] private Bullet _prefab;
        [SerializeField] private Transform _spawnPosition;
        [SerializeField] private int _poolCapacity;
        [SerializeField] private float _objMovingSpeed;

        private List<Bullet> _spawnedObjects = new List<Bullet>();

        private void Awake()
        {
            for (int i = 0; i <= _poolCapacity; i++)
            {
                Initalize(_prefab);
            }
        }

        public void SetSpawnPosition(Transform position)
        {
            _spawnPosition = position;
        }
        
        public void Spawn()
        {
            if (ActiveObjects.Count >= _poolCapacity)
                return;
            
            if (TryGetObject(out Bullet @object, _prefab))
            {
                _spawnedObjects.Add(@object);
                @object.transform.position = _spawnPosition.position;
                @object.CollidedWithBarrel += PutObject;
                @object.EnableMovement();
                @object.SetSpeed(_objMovingSpeed);
            }
        }
        
        public void ReturnToPool(Bullet @object)
        {
            if (@object == null)
                return;

            @object.DisableMovement();
            @object.CollidedWithBarrel -= PutObject;
            PutObject(@object);

            if (_spawnedObjects.Contains(@object))
                _spawnedObjects.Remove(@object);
        }
        
        public void ReturnAllObjectsToPool()
        {
            if (_spawnedObjects.Count <= 0)
                return;

            List<Bullet> objectsToReturn = new List<Bullet>(_spawnedObjects);
            foreach (var @object in objectsToReturn)
            {
                @object.DisableMovement();
                @object.CollidedWithBarrel -= PutObject;
                ReturnToPool(@object);
            }
        }
    }
}